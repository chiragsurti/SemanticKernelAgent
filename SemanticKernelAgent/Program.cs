using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using System.Net;
using System;

namespace rest_client
{
    public class Program
    {
        private static string modelId;
        private static string endpoint;
        private static string apiKey;
        private static bool isOpenApiPlugin;
        private static string lightsOnlinePluginURL;
        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();


                IConfigurationRoot configuration = configBuilder.Build();
                modelId = configuration["ModelId"];
                endpoint = configuration["Endpoint"];
                apiKey = configuration["ApiKey"];
                lightsOnlinePluginURL = configuration["LightsOnlinePluginURL"];
                isOpenApiPlugin = configuration["IsOpenApiPlugin"] != null ? Convert.ToBoolean(configuration["IsOpenApiPlugin"]) : false;


                // Create a kernel with Azure OpenAI chat completion
                var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

                // Build the kernel
                Kernel kernel = builder.Build();

                var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();


                if (isOpenApiPlugin)
                {
                    //Open api spec
#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                    await kernel.ImportPluginFromOpenApiAsync(
                       pluginName: "lightsOnline",
                       uri: new Uri(lightsOnlinePluginURL),
                       executionParameters: new OpenApiFunctionExecutionParameters()
                       {
                           // Determines whether payload parameter names are augmented with namespaces.
                           // Namespaces prevent naming conflicts by adding the parent parameter name
                           // as a prefix, separated by dots
                           EnablePayloadNamespacing = true
                       }
                    );
#pragma warning restore SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

                }
                else
                {
                    // Add a plugin (the LightsPlugin class is defined below)
                    kernel.Plugins.AddFromType<LightsPlugin>("Lights");
                }
                kernel.Plugins.AddFromType<TimeInformationPlugin>();

                
                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    //ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions // Auto invoke kernel functions
                    ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions // Function callling need to use when you are looking for planner
                };

                // Create a history store the conversation
                var history = new ChatHistory();
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("---- Planner with Functional calling ------");
                //Planner message with Functional calling
                history.AddUserMessage("Fetch current time then Convert time into Local time format. If current time is AM format then switch on lamp id = 1, Get status of all the lamps");

                while (true)
                {
                    // Start or continue chat based on the chat history
                    ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(history, openAIPromptExecutionSettings, kernel);
                    if (result.Content is not null)
                    {
                        Console.Write(result.Content);
                    }

                    // Get function calls from the chat message content and quit the chat loop if no function calls are found.
                    IEnumerable<FunctionCallContent> functionCalls = FunctionCallContent.GetFunctionCalls(result);
                    if (!functionCalls.Any())
                    {
                        break;
                    }

                    // Preserving the original chat message content with function calls in the chat history.
                    history.Add(result);

                    // Iterating over the requested function calls and invoking them
                    foreach (FunctionCallContent functionCall in functionCalls)
                    {
                        try
                        {
                            // Invoking the function
                            FunctionResultContent resultContent = await functionCall.InvokeAsync(kernel);

                            // Adding the function result to the chat history
                            history.Add(resultContent.ToChatMessage());
                        }
                        catch (Exception ex)
                        {
                            // Adding function exception to the chat history.
                            history.Add(new FunctionResultContent(functionCall, ex).ToChatMessage());
                            // or
                            //chatHistory.Add(new FunctionResultContent(functionCall, "Error details that LLM can reason about.").ToChatMessage());
                        }
                    }

                    Console.WriteLine();
                }


                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("---- AutoInvoke Kernel function ------");

                openAIPromptExecutionSettings = new()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions // Auto invoke kernel functions
                    //ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions // Function callling need to use when you are looking for planner
                };

                Console.WriteLine("Ask questions to use the Time Plugin such as:\n" +
                  "- What time is it?");

                Console.WriteLine("Ask questions to use the Light Plugin such as:\n" +
                  "- Provide command to switch on/off lamp or get status of lamps");
                string? input = null;
                while (true)
                {
                    Console.Write("\nUser > ");
                    input = Console.ReadLine();
                    if (input.ToLower() == "quit")
                    {
                        break;
                    }
                    history.AddUserMessage(input);
                    var chatResult = await chatCompletionService.GetChatMessageContentAsync(history, openAIPromptExecutionSettings, kernel);
                    Console.Write($"\nAssistant > {chatResult}\n");
                    await SwitchonOffLights(kernel, chatCompletionService, openAIPromptExecutionSettings, history);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task SwitchonOffLights(Kernel kernel, IChatCompletionService chatCompletionService, OpenAIPromptExecutionSettings openAIPromptExecutionSettings, ChatHistory history)
        {
            history.AddUserMessage("Please turn on the lamp");

            // Get the response from the AI
            var result = await chatCompletionService.GetChatMessageContentAsync(
               history,
               executionSettings: openAIPromptExecutionSettings,
               kernel: kernel);

            // Print the results
            Console.WriteLine("Assistant > " + result);

            // Add the message from the agent to the chat history
            history.AddMessage(result.Role, result.Content ?? string.Empty);
        }
    }

   
}
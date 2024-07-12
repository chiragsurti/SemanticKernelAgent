using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using System.Net;

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

                // Enable planning
                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                };

                // Create a history store the conversation
                var history = new ChatHistory();

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
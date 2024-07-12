# Semantic kernel Agent
### Simple chat completion
### Native plugin - Time Plugins folder (TimeInformationPlugin.cs)
### Native plugin - Light Plugins in folder (LightsPlugin.cs)
### Open api specification plugin  (Azure function app swagger url- SemanticKernelAgent.OpenAPI.Plugins project)
### Planner - planner will fetch the current time and according to if time is PM then it will switch on all lights.
### Persona - User 

```
dotnet add package Microsoft.SemanticKernel
```

# User secret configuration
## add below entry to secret.json file.
```
{
  "ModelId": "gpt-4",
  "Endpoint": "https://{*}.openai.azure.com/",
  "ApiKey": "",
  "IsOpenApiPlugin": "true",
  "LightsOnlinePluginURL": "http://localhost:7168/api/swagger.json" //Azure function app swagger url- SemanticKernelAgent.OpenAPI.Plugins 
}
```

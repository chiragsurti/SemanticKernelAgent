using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SemanticKernalAgent.Domain;
using SemanticKernelAgent.Domain.DTO;
using System.Net;

namespace SemanticKernelAgent.OpenAPI.Plugins
{
    public class LightsPlugin
    {
        private readonly ILogger<LightsPlugin> _logger;

        // Mock data for the lights
        private readonly List<LightModel> lights = new()
       {
          new LightModel { Id = 1, Name = "Table Lamp", IsOn = false },
          new LightModel { Id = 2, Name = "Porch light", IsOn = false },
          new LightModel { Id = 3, Name = "Chandelier", IsOn = true }
       };
        public LightsPlugin(ILogger<LightsPlugin> logger)
        {
            _logger = logger;
        }

        [Function("get_lights")]
        [OpenApiOperation(operationId: "get_lights")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LightModel>),
            Description = "Gets a list of lights and their current state.")]
        public IActionResult GetLights([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("HTTP trigger function get_lights processed a request.");
            return  new OkObjectResult(lights);
        }

        [Function("change_state")]
        [OpenApiOperation(operationId: "change_state")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(ChangeStateRequestDto),
            Description = "JSON request body containing { id, isOn  light status}")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LightModel),
            Description = "The updated state of the light; will return null if the light does not exist")]
        public async Task<IActionResult> ChangeState([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("HTTP trigger function change_state processed a request.");

            var content = await new StreamReader(req.Body).ReadToEndAsync();

            ChangeStateRequestDto changeStateRequestDto = JsonConvert.DeserializeObject<ChangeStateRequestDto>(content);
            var light = lights.FirstOrDefault(light => light.Id == changeStateRequestDto?.Id);

            if (light == null)
            {
                return null;
            }

            // Update the light with the new state
            light.IsOn = changeStateRequestDto?.IsOn;

            return new OkObjectResult(light);

            }

        
    }
}

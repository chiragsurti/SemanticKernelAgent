using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;

    /// A plugin that returns the current time.
    /// </summary>
    public class TimeInformationPlugin
    {
        /// <summary>
        /// Retrieves the current time in UTC.
        /// </summary>
        /// <returns>The current time in UTC. </returns>
        [KernelFunction, Description("Retrieves the current time in UTC.")]
        public string GetCurrentUtcTime()
            => DateTime.UtcNow.ToString("R");
    }


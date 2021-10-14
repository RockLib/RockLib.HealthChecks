using RockLib.HealthChecks.Client;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Example.HealthChecks.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();

            Console.Write("Press any key when ready to send health request . . .");
            Console.ReadKey(true);
            Console.WriteLine();
            Console.WriteLine();

            // Note: This example is designed to make a request to the
            // Example.HealthChecks.AspNetCore example.
            var httpResponse = await httpClient.GetAsync("https://localhost:5001/health");

            // Deserialize the request content into a HealthResponse object. Note that the
            // ReadFromJsonAsync extension method comes from the System.Net.Http.Json package.
            var healthResponse = await httpResponse.Content.ReadFromJsonAsync<HealthResponse>();

            // Use the HealthResponse object.
            Console.WriteLine($"Overall health check status: {healthResponse.Status}");
            Console.WriteLine($"Number of health checks performed: {healthResponse.Checks.Count}");
            foreach (var healthCheck in healthResponse.Checks)
                foreach (var healthCheckResult in healthCheck.Value)
                    Console.WriteLine($"Health check '{healthCheck.Key}': {healthCheckResult.ObservedValue} {healthCheckResult.ObservedUnit}");

            Console.WriteLine();
            Console.Write("Press any key to exit . . .");
            Console.ReadKey(true);
        }
    }
}

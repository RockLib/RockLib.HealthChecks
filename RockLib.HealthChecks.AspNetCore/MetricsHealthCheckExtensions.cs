using Microsoft.Extensions.DependencyInjection;
using RockLib.HealthChecks.AspNetCore.Collector;

namespace RockLib.HealthChecks.AspNetCore;

/// <summary>
/// 
/// </summary>
public static class MetricsHealthCheckExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureHttpHealthChecks(this IServiceCollection services)
    {
        services.AddSingleton<IHealthMetricCollectorFactory, HealthMetricCollectorFactory>();

        services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.ConfigureAdditionalHttpMessageHandlers((handlers, sp) =>
            {
                var factory = sp.GetRequiredService<IHealthMetricCollectorFactory>();
                handlers.Add(new HttpResponseCollector(factory));
            });
        });
    }
}
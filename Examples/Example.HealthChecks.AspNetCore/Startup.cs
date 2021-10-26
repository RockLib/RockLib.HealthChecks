using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RockLib.HealthChecks.AspNetCore;
using RockLib.HealthChecks.DependencyInjection;
using RockLib.HealthChecks.System;

namespace Example.HealthChecks.AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // This application defines its health check runner in its appsettings.json, so
            // a health check runner should *not* be added to the service collection. To define an
            // equivalent health check runner programmatically:
            /*
            services.AddHealthCheckRunner(options =>
                {
                    options.Version = "1";
                    options.ServiceId = "c0c4b71f-d540-4515-8a87-5a4ae5ca4c55";
                    options.Description = "My Application";
                })
                .AddProcessUptimeHealthCheck()
                .AddDiskDriveHealthCheck(warnGigabytes: 10, failGigabytes: 1, driveName: "C:\\");
            */
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Add health checks endpoint to the pipeline before the call to UseEndpoints.
            app.UseRockLibHealthChecks(formatter: new NewtonsoftJsonResponseFormatter(Formatting.Indented));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RockLib.HealthChecks.AspNetCore.ResponseWriter;

namespace Example.HealthChecks.AspNetCore.ResponseWriter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Set health checks options.
            RockLibHealthChecks.Indent = true;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Add health check service to the service collection.
            services.AddHealthChecks()
                .AddDiskStorageHealthCheck(options => options.AddDrive("C:\\", 309646), "disk:space");
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
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                // To write health check responses in the rfc format, set the response writer of
                // the health check options.
                ResponseWriter = RockLibHealthChecks.ResponseWriter
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RockLib.HealthChecks.AspNetCore.ResponseWriter;
using HealthChecks.System;

namespace NetCoreResponseWriter
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("disk:space", new DiskStorageHealthCheck(new DiskStorageOptions().AddDrive("C:\\", 309646)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            RockLibHealthChecks.Indent = true;

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = RockLibHealthChecks.ResponseWriter
            });
        }
    }
}

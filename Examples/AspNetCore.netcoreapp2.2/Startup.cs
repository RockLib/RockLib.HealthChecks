using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using RockLib.HealthChecks.AspNetCore;

namespace AspNetCore.netcoreapp2._2
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRockLibHealthChecks(formatter: new NewtonsoftJsonResponseFormatter(Formatting.Indented));
        }
    }
}

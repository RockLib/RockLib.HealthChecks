using RockLib.HealthChecks;
using System;
using System.Threading.Tasks;
using System.Web.UI;

namespace NetFramework45WebForms
{
    public partial class Health : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(RunHealthCheck));
        }

        private async Task RunHealthCheck()
        {
            var healthCheckResponse = await HealthCheck.GetRunner().RunAsync();

            Response.Clear();
            Response.StatusCode = healthCheckResponse.StatusCode;
            Response.ContentType = healthCheckResponse.ContentType;
            Response.Write(healthCheckResponse.Serialize(true));
            Response.End();
        }
    }
}

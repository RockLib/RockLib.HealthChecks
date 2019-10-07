using RockLib.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace NetFramework47Api.HealthChecks
{
    public class MyCustomHealthCheck : SingleResultHealthCheck
    {
        public MyCustomHealthCheck()
            : base(null, null, null, null)
        {
        }

        protected override Task CheckAsync(HealthCheckResult result, CancellationToken cancellationToken)
        {
            result.Status = HealthStatus.Pass;
            return Task.FromResult(0);
        }
    }
}

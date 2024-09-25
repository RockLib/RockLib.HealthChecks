using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.HealthChecks.AspNetCore.Collector;

/// <summary>
/// 
/// </summary>
public class HttpResponseCollector : DelegatingHandler
{
    private readonly IHealthMetricCollectorFactory _factory;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factory"></param>
    public HttpResponseCollector(IHealthMetricCollectorFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(request);
#else
            if (request is null) { throw new ArgumentNullException(nameof(request)); }
#endif
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        _factory.LeaseCollector(request.RequestUri?.Host).Collect((int)response.StatusCode);
        return response;
    }
}
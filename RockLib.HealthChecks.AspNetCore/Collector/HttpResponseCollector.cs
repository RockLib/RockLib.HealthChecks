using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.HealthChecks.AspNetCore.Collector;

/// <summary>
/// A delegating handler that collects the status code of the response from an HTTP request.
/// </summary>
/// <remarks>Added to the HttpClient pipeline to collect response status codes.</remarks>
public class HttpResponseCollector : DelegatingHandler
{
    private readonly IHealthMetricCollectorFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpResponseCollector"/> class.
    /// </summary>
    /// <param name="factory"></param>
    public HttpResponseCollector(IHealthMetricCollectorFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
    /// Records the status code of the response in the <see cref="IHealthMetricCollector"/> for the host of the request.
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
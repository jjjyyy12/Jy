using Jy.ILog;
using Jy.Resilience.Http;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;

namespace Jy.Resilience.Http
{
    public class ResilientHttpClientFactory : IResilientHttpClientFactory
    {
        private readonly ILogger _logger;

        public ResilientHttpClientFactory(ILogger logger)
            => _logger = logger;

        public ResilientHttpClient CreateResilientHttpClient()
            => new ResilientHttpClient((origin) => CreatePolicies(), _logger);

        private AsyncPolicy[] CreatePolicies()
            => new AsyncPolicy[]
            {
                Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    // number of retries
                    6,
                    // exponential backofff
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    // on retry
                    (exception, timeSpan, retryCount, context) =>
                    {
                        var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                            $"of {context.PolicyKey} " +
                            $"at {context.OperationKey}, " +
                            $"due to: {exception}.";
                        _logger.LogError(msg,exception);
                        
                    }),
                Policy.Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                   // number of exceptions before breaking circuit
                   5,
                   // time circuit opened before retry
                   TimeSpan.FromMinutes(1),
                   (exception, duration) =>
                   {
                        // on circuit opened
                        _logger.LogError("Circuit breaker opened",exception);
                   },
                   () =>
                   {
                        // on circuit closed
                        _logger.LogWarning("Circuit breaker reset");
                   })
            };
    }
}

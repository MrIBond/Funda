using System.Net;
using Funda.Application.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.RateLimit;

namespace Funda.Infrastructure.Factories;

public static class PolicyFactory
{
    //parameters can be taken from configuration, not from static constants. simplified for test assessment
    public static IAsyncPolicy CreateRateLimiterPolicy(ILogger logger)
    {
        var waitAndRetry = Policy
            .Handle<RateLimitRejectedException>()
            .WaitAndRetryForeverAsync(attempt => TimeSpan.FromSeconds(attempt), (_, timeSpan) =>
            {
                logger.LogInformation($"RateLimiter. Delay = {timeSpan.TotalMilliseconds} ms.");
            });

        var rateLimiterPolicy = Policy.RateLimitAsync(
            RateLimiterSettings.Threshold,
            TimeSpan.FromSeconds(RateLimiterSettings.TimeFrameInSeconds),
            RateLimiterSettings.MaxBurst
            );

        return waitAndRetry.WrapAsync(rateLimiterPolicy);
    }

    public static IAsyncPolicy<HttpResponseMessage> CreateHttpClientPolicy(ILogger logger)
    {
        var waitAndRetryPolicy = Policy
            .HandleResult<HttpResponseMessage>(responseMessage =>
                responseMessage.StatusCode is 
                    HttpStatusCode.ServiceUnavailable or 
                    HttpStatusCode.Unauthorized or 
                    HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(FundaHttpClientPolicy.RetryTime,
                attempt => TimeSpan.FromSeconds(attempt),
                (exception, calculatedWaitDuration) =>
                {
                    logger.LogError(
                        $"Retry. Delay = {calculatedWaitDuration.TotalMilliseconds} ms. Error status code = {exception.Result.StatusCode}");
                }
            );

        var circuitBreakerPolicyForRecoverable = Policy
            .HandleResult<HttpResponseMessage>(responseMessage =>
                responseMessage.StatusCode is 
                    HttpStatusCode.InternalServerError or
                    HttpStatusCode.BadGateway or 
                    HttpStatusCode.GatewayTimeout
            )
            .CircuitBreakerAsync(
                FundaHttpClientPolicy.BeforeBreakCount,
                TimeSpan.FromSeconds(FundaHttpClientPolicy.CircuitBreakerDurationOnBreakInSeconds),
                (outcome, breakDelay) =>
                {
                    logger.LogInformation(
                        $"Circuit breaker. Delay = {breakDelay.TotalMilliseconds} ms. Error status code = {outcome.Result.StatusCode}");
                },
                () => logger.LogInformation("Circuit breaker reset.")
            );

        return Policy.WrapAsync(waitAndRetryPolicy, circuitBreakerPolicyForRecoverable);
    }
}
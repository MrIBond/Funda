namespace Funda.Application.Models;

public class FundaApiSettings
{
    public string AmsterdamObjectsSalesUrl { get; set; }
    public string AmsterdamObjectsWithGardenSalesUrl { get; set; }
}

public static class RateLimiterSettings
{
    public static int Threshold => 100;
    public static int TimeFrameInSeconds => 60;
    public static int MaxBurst => 12;
}

public static class FundaHttpClientPolicy
{
    public static int RetryTime => 7;
    public static int BeforeBreakCount => 7;
    public static int CircuitBreakerDurationOnBreakInSeconds => 5;
}
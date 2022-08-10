using Funda.Application.Interfaces;
using Funda.Application.Models;
using Funda.Application.Query;
using Funda.Application.Services;
using Funda.Domain.Interfaces;
using Funda.Domain.Services;
using Funda.Infrastructure.Factories;
using Funda.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Funda;

internal static class Program
{
    //Should come from UI or sit in settings. Skipped for test assessment.
    private const int Top = 10;

    private static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        var brokerService = host.Services.GetService<IBrokerService>();
        var getTopBrokersQuery = new GetTopBrokersQuery(false, Top);
        var getTopBrokersObjectsHasGardenQuery = new GetTopBrokersQuery(true, Top);

        Console.WriteLine("Dear user! System calculates top brokers for you. Please wait...");
        Console.WriteLine();

        var topBrokersInAmsterdam = 
            await brokerService!.GetTopBrokersInAmsterdamAsync(getTopBrokersQuery);
        PrintBrokers(topBrokersInAmsterdam, "Top brokers Amsterdam");

        var topBrokersInAmsterdamObjectsWithGarden =
            await brokerService.GetTopBrokersInAmsterdamAsync(getTopBrokersObjectsHasGardenQuery);
        PrintBrokers(topBrokersInAmsterdamObjectsWithGarden, "Top brokers Amsterdam. Objects with garden.");

        Console.WriteLine("Top brokers calculations are finished. Enjoy!");

        await host.RunAsync();
    }

    private static void PrintBrokers(IEnumerable<BrokerRatingDto> brokerRatings, string title)
    {
        Console.WriteLine(title);

        foreach (var brokerRating in brokerRatings)
        {
            Console.WriteLine($"{brokerRating.BrokerName} - {brokerRating.SalesQuantity}");
        }

        Console.WriteLine();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();

        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                var logger = services.BuildServiceProvider().GetService<ILogger<FundaApiClient>>();

                //here we can make named policies or more elegant solution. skipped for test project
                services.AddTransient<IBrokerService, BrokerService>();
                services.AddTransient<IBrokersDomainService, BrokersDomainService>();
                services.AddHttpClient<IFundaApiClient, FundaApiClient>()
                    .AddPolicyHandler(PolicyFactory.CreateHttpClientPolicy(logger));
                services.AddSingleton(PolicyFactory.CreateRateLimiterPolicy(logger));
                services.Configure<FundaApiSettings>(configuration.GetSection("ApiSettings"));
            })
            .ConfigureLogging((_, logging) =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options => options.IncludeScopes = true);
            });
    }
}
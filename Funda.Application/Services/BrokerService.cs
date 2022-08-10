using Funda.Application.Exceptions;
using Funda.Application.Interfaces;
using Funda.Application.Models;
using Funda.Application.Query;
using Funda.Domain.Entites;
using Funda.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Funda.Application.Services;

public class BrokerService : IBrokerService
{
    private readonly IBrokersDomainService _brokersDomainService;
    private readonly IFundaApiClient _fundaApiClient;
    private readonly ILogger<BrokerService> _logger;

    public BrokerService(
        IFundaApiClient fundaApiClient,
        IBrokersDomainService brokersDomainService,
        ILogger<BrokerService> logger
        )
    {
        _fundaApiClient = fundaApiClient;
        _brokersDomainService = brokersDomainService;
        _logger = logger;
    }

    public async Task<IEnumerable<BrokerRatingDto>> GetTopBrokersInAmsterdamAsync(
        GetTopBrokersQuery query,
        CancellationToken cancellationToken = default)
    {
        //auto mapper or custom mappers will make code cleaner. skipped for test project
        try
        {
            var salesDtos = await _fundaApiClient
                .GetBrokerObjectsSalesInAmsterdamAsync(query.ObjectsHasGarden, cancellationToken)
                .ConfigureAwait(false);
            var sales = salesDtos.Select(x => new Sale(x.ObjectId, new Broker(x.BrokerId, x.BrokerName)));
            var topBrokers = _brokersDomainService.GetTopBrokers(sales, query.Top);

            return topBrokers.Select(x => new BrokerRatingDto(x.Broker.BrokerName, x.SalesQuantity));
        }
        catch (ArgumentOutOfRangeException argumentOutOfRangeException)
        {
            _logger.LogError(argumentOutOfRangeException.Message, argumentOutOfRangeException);
            throw;
        }
        catch (ArgumentNullException argumentNullException)
        {
            _logger.LogError(argumentNullException.Message, argumentNullException);
            throw;
        }
        catch (FundaClientException fundaClientException)
        {
            _logger.LogError(fundaClientException.Message, fundaClientException);
            throw;
        }
    }
}
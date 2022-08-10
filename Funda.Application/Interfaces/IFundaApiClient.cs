using Funda.Application.Models;

namespace Funda.Application.Interfaces;

public interface IFundaApiClient
{
    Task<IEnumerable<BrokerSaleDto>> GetBrokerObjectsSalesInAmsterdamAsync(
        bool objectsHasGarden,
        CancellationToken cancellationToken = default);
}
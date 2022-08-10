using Funda.Application.Models;
using Funda.Application.Query;

namespace Funda.Application.Interfaces;

public interface IBrokerService
{
    Task<IEnumerable<BrokerRatingDto>> GetTopBrokersInAmsterdamAsync(
        GetTopBrokersQuery query,
        CancellationToken cancellationToken = default);
}
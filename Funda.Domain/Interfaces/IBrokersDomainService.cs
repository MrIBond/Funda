using Funda.Domain.Entites;

namespace Funda.Domain.Interfaces;

public interface IBrokersDomainService
{
    IEnumerable<BrokerSales> GetTopBrokers(IEnumerable<Sale> sales, int top);
}
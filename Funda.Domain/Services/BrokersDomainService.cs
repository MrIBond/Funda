using Funda.Domain.Entites;
using Funda.Domain.Interfaces;

namespace Funda.Domain.Services;

public class BrokersDomainService : IBrokersDomainService
{
    public IEnumerable<BrokerSales> GetTopBrokers(IEnumerable<Sale> sales, int top)
    {
        if (sales == null) throw new ArgumentNullException(nameof(sales));
        if (top <= 0) throw new ArgumentOutOfRangeException(nameof(top));

        return sales
            .GroupBy(x => x.Broker.BrokerId)
            .Select(x => new BrokerSales(
                x.First().Broker,
                x.Count()
            ))
            .OrderByDescending(x => x.SalesQuantity)
            .Take(top);
    }
}
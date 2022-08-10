namespace Funda.Domain.Entites;

public class BrokerSales
{
    public BrokerSales(Broker broker, int salesQuantity)
    {
        Broker = broker;
        SalesQuantity = salesQuantity;
    }

    public Broker Broker { get; }
    public int SalesQuantity { get; }
}
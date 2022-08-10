namespace Funda.Domain.Entites;

public class Broker
{
    public Broker(int brokerId, string brokerName)
    {
        BrokerId = brokerId;
        BrokerName = brokerName;
    }

    public int BrokerId { get; }
    public string BrokerName { get; }
}

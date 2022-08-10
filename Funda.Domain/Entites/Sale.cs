namespace Funda.Domain.Entites;

public class Sale
{
    public Sale(string objectId, Broker broker)
    {
        ObjectId = objectId;
        Broker = broker;
    }

    public string ObjectId { get; }
    public Broker Broker { get; }
}
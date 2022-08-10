using FluentAssertions;
using Funda.Domain.Entites;
using Funda.Domain.Services;

namespace Funda.Tests.UnitTests
{
    public class BrokerDomainServiceTests
    {
        [Fact]
        public void GetTopBrokers_CorrectInputParams_ReturnsCorrectTopBrokers()
        {
            //Assess
            var sut = new BrokersDomainService();
            var sales = CreateSales();
            const int top = 5;

            //Act
            var result = sut.GetTopBrokers(sales, top);


            //Assert
            var expected = CreateExpectedBrokerSales();

            result
                .Should()
                .Equal(expected,
                    (b1, b2) => b1.Broker.BrokerId == b2.Broker.BrokerId &&
                                b1.SalesQuantity == b2.SalesQuantity);
        }

        [Fact]
        public void GetTopBrokers_EmptySales_ReturnsEmptyTop()
        {
            //Assess
            var sut = new BrokersDomainService();
            var sales = new List<Sale>();
            const int topCount = 5;

            //Act
            var result = sut.GetTopBrokers(sales, topCount).ToList();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetTopBrokers_NullSales_ThrowsArgumentNullException()
        {
            //Assess
            var sut = new BrokersDomainService();
            IEnumerable<Sale> sales = null;
            const int topCount = 5;

            //Act
            Action act = () => sut.GetTopBrokers(sales, topCount);

            //Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetTopBrokers_NegativeTop_ThrowsArgumentOutOfRangeException()
        {
            //Assess
            var sut = new BrokersDomainService();
            var sales = CreateSales();
            const int topCount = -5;

            //Act
            Action act = () => sut.GetTopBrokers(sales, topCount);

            //Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        //can be done more elegant. quick solution for test project
        private static IEnumerable<Sale> CreateSales()
        {
            var sales = new List<Sale>();
            var brokers = CreateBrokers();

            var brokerA = brokers.Single(x => x.BrokerName == "A");
            var brokerB = brokers.Single(x => x.BrokerName =="B");
            var brokerC = brokers.Single(x => x.BrokerName =="C");
            var brokerD = brokers.Single(x => x.BrokerName =="D");
            var brokerE = brokers.Single(x => x.BrokerName =="E");
            var brokerF = brokers.Single(x => x.BrokerName =="F");
            var brokerG = brokers.Single(x => x.BrokerName == "G");

            sales.AddRange(Enumerable.Range(1, 10).Select(x => new Sale(x.ToString(), brokerA)));
            sales.AddRange(Enumerable.Range(11, 20).Select(x => new Sale(x.ToString(), brokerB)));
            sales.AddRange(Enumerable.Range(31, 30).Select(x => new Sale(x.ToString(), brokerD)));
            sales.AddRange(Enumerable.Range(61, 40).Select(x => new Sale(x.ToString(), brokerF)));
            sales.AddRange(Enumerable.Range(101, 100).Select(x => new Sale(x.ToString(), brokerG)));

            sales.AddRange(Enumerable.Range(201, 4).Select(x => new Sale(x.ToString(), brokerC)));
            sales.AddRange(Enumerable.Range(206, 1).Select(x => new Sale(x.ToString(), brokerE)));

            return sales;
        }

        private static IEnumerable<BrokerSales> CreateExpectedBrokerSales()
        {
            var brokers = CreateBrokers();
            var brokerA = brokers.Single(x => x.BrokerName == "A");
            var brokerB = brokers.Single(x => x.BrokerName == "B");
            var brokerD = brokers.Single(x => x.BrokerName == "D");
            var brokerF = brokers.Single(x => x.BrokerName == "F");
            var brokerG = brokers.Single(x => x.BrokerName == "G");

            var sales = new List<BrokerSales>
            {
                new(brokerG, 100),
                new(brokerF, 40),
                new(brokerD, 30),
                new(brokerB, 20),
                new(brokerA, 10)
            };

            return sales;
        }

        private static IEnumerable<Broker> CreateBrokers()
        {
            var brokers = new List<Broker>
            {
                new(1, "A"),
                new(2, "B"),
                new(3, "C"),
                new(4, "D"),
                new(5, "E"),
                new(6, "F"),
                new(7, "G")
            };

            return brokers;
        }
    }
}
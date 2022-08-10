namespace Funda.Application.Exceptions;

public class FundaClientException : Exception
{
    public FundaClientException()
    {
    }

    public FundaClientException(string message)
        : base(message)
    {
    }

    public FundaClientException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
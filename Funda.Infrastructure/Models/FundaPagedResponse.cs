using Funda.Application.Exceptions;
using Newtonsoft.Json;

namespace Funda.Infrastructure.Models;

public record FundaPagedResponse(
    [JsonProperty("EmailNotConfirmed")] bool EmailNotConfirmed,
    [JsonProperty("ValidationFailed")] bool ValidationFailed,
    [JsonProperty("Objects")] List<PropertyObject> PropertyObjects,
    [JsonProperty("Paging")] PaginationInfo PaginationInfo)
{
    public void EnsureSuccessResponse()
    {
        if (EmailNotConfirmed || ValidationFailed)
        {
            throw
                new FundaClientException(
                    $"Funda Api exception for page {PaginationInfo.CurrentPage}"
                );
        }
    }
}

public record PropertyObject(
    [JsonProperty("Id")] string Id,
    [JsonProperty("MakelaarId")] int BrokerId,
    [JsonProperty("MakelaarNaam")] string BrokerName);


public record PaginationInfo(
    [JsonProperty("AantalPaginas")] int PageCount,
    [JsonProperty("HuidigePagina")] int CurrentPage
    );

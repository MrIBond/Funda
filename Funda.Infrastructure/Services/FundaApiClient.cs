using Funda.Application.Interfaces;
using Funda.Application.Models;
using Funda.Infrastructure.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;

namespace Funda.Infrastructure.Services;

public class FundaApiClient : IFundaApiClient
{
    private const int FirstPage = 1;
    private const string PageUrlParamPlaceholder = "{page}";

    private readonly FundaApiSettings _apiSettings;
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy _rateLimiter;

    public FundaApiClient(
        IOptions<FundaApiSettings> apiSettings,
        HttpClient httpClient,
        IAsyncPolicy rateLimiter
    )
    {
        _apiSettings = apiSettings.Value;
        _httpClient = httpClient;
        _rateLimiter = rateLimiter;
    }

    public async Task<IEnumerable<BrokerSaleDto>> GetBrokerObjectsSalesInAmsterdamAsync(
        bool objectsHasGarden,
        CancellationToken cancellationToken = default)
    {
        var uri = GetUrl(objectsHasGarden);
        var fundaPagedResponses = await CrawlAsync(uri, cancellationToken).ConfigureAwait(false);

        return fundaPagedResponses
            .SelectMany(x => x.PropertyObjects)
            .Select(x => new BrokerSaleDto(x.Id, x.BrokerId, x.BrokerName));
    }

    private async Task<IEnumerable<FundaPagedResponse>> CrawlAsync(
        string url,
        CancellationToken cancellationToken = default
        )
    {
        var totalPageCount = await GetTotalPageCount(url, _rateLimiter, cancellationToken);
        var getAsyncTasks = GeneratePageUrls(totalPageCount, url)
            .Select(pageUrl => _rateLimiter.ExecuteAsync(
                    () => GetAsync(pageUrl, cancellationToken)
                )
            );

        var fundaPagedResponses = await Task.WhenAll(getAsyncTasks);

        return fundaPagedResponses;
    }

    private async Task<int> GetTotalPageCount(
        string url,
        IAsyncPolicy policy,
        CancellationToken cancellationToken = default
        )
    {
        var firstPageUrl = GetUrlForPage(url, FirstPage);
        var fundaPagedResponse = await policy.ExecuteAsync(() => GetAsync(firstPageUrl, cancellationToken));

        return fundaPagedResponse.PaginationInfo.PageCount;
    }

    private async Task<FundaPagedResponse> GetAsync(
        string url,
        CancellationToken cancellationToken = default
        )
    {
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var fundaPagedResponse = JsonConvert.DeserializeObject<FundaPagedResponse>(responseContent);
        fundaPagedResponse!.EnsureSuccessResponse();

        return fundaPagedResponse;
    }

    private string GetUrl(bool objectsWithGarden)
    {
        //here we can use url builder or some custom builder to construct a right url. skipped for test project
        return objectsWithGarden
            ? _apiSettings.AmsterdamObjectsWithGardenSalesUrl
            : _apiSettings.AmsterdamObjectsSalesUrl;
    }

    private static IEnumerable<string> GeneratePageUrls(int pagesCont, string url)
    {
        return Enumerable.Range(FirstPage, pagesCont).Select(page => GetUrlForPage(url, page));
    }

    private static string GetUrlForPage(string url, int page)
    {
        return url.Replace(PageUrlParamPlaceholder, $"{page}");
    }
}
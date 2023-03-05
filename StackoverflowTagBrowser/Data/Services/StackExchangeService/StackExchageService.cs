using Newtonsoft.Json;
using StackoverflowTagBrowser.Data.Enums;
using StackoverflowTagBrowser.Exceptions;
using StackoverflowTagBrowser.Models;
using StackoverflowTagBrowser.ViewModels;
using System.IO.Compression;

namespace StackoverflowTagBrowser.Data.Services.StackExchangeService
{
    public class StackExchageService : IStackExchangeService
    {
        private readonly HttpClient _httpClient;
        private const int maximumPageSize = 100;
        public StackExchageService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<TagViewModel> GetTagsAsync(string site, int quantity, SortType sortType, SortOrder sortOrder)
        {
            List<Tag> downloadedTags = new();
            int downloadedTagsCount = 0;
            int currentPage = 1;
            while (downloadedTagsCount < quantity)
            {
                int pageSize = quantity - downloadedTagsCount > maximumPageSize? maximumPageSize : quantity - downloadedTagsCount;
                string url = $"https://api.stackexchange.com/2.3/tags?order={sortOrder}&sort={sortType}&site={site}&page={currentPage}&pagesize={pageSize}";
                var response = await GetAsync<TagResponse>(url);
                downloadedTags.AddRange(response.items);

                if (!response.has_more)
                {
                    break;
                }

                if(response.quota_remaining == 0)
                {
                    throw new RateLimitException("You have reached your daily request limit! Try again in 24 hours.");
                }

                currentPage++;
                downloadedTagsCount += response.items.Length;
            }

            TagViewModel tagVM = new()
            {
                Tags = downloadedTags,
                TotalCount = downloadedTags.Sum(c => c.count)
            };
            return tagVM;
        }

        private async Task<T> GetAsync<T>(string url) 
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                throw new HttpRequestException("Executed request returned errors!");
            }
            string responseString;
            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                var decompressedStream = new GZipStream(contentStream, CompressionMode.Decompress);

                using var streamReader = new StreamReader(decompressedStream);
                responseString = await streamReader.ReadToEndAsync();
            }
            else
            {
                responseString = await response.Content.ReadAsStringAsync();
            }

            var TResponse = JsonConvert.DeserializeObject<T>(responseString);
            return TResponse;
        }

    }
}

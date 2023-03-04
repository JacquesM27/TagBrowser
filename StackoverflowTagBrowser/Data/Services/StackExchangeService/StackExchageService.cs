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
        private readonly int maximumPageSize = 100;
        public StackExchageService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<TagViewModel> GetMostPopularTagsAsync(string site, int quantity, SortType sortType, SortOrder sortOrder)
        {
            List<Tag> tags = new();
            int tagsTotal = 0;
            int page = 1;
            while (tagsTotal < quantity)
            {
                int pageSize = quantity - tagsTotal > maximumPageSize? maximumPageSize : quantity - tagsTotal;
                string url = $"https://api.stackexchange.com/2.3/tags?order={sortOrder}&sort={sortType}&site={site}&page={page}&pagesize={pageSize}";
                var downloadedTags = await GetAsync<TagResponse>(url);
                tags.AddRange(downloadedTags.items);

                if (!downloadedTags.has_more)
                {
                    break;
                }

                if(downloadedTags.quota_remaining == 0)
                {
                    throw new RateLimitException("You have reached your daily request limit! Try again in 24 hours.");
                }

                page++;
                tagsTotal += downloadedTags.items.Length;
            }

            TagViewModel tagVM = new()
            {
                Tags = tags,
                TotalCount = tags.Sum(c => c.count)
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
            var contentStream = await response.Content.ReadAsStreamAsync();
            var decompressedStream = new GZipStream(contentStream, CompressionMode.Decompress);

            using var streamReader = new StreamReader(decompressedStream);
            var responseString = await streamReader.ReadToEndAsync();

            var TResponse = JsonConvert.DeserializeObject<T>(responseString);
            return TResponse;
        }

    }
}

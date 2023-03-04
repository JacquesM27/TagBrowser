using StackoverflowTagBrowser.Data.Enums;
using StackoverflowTagBrowser.ViewModels;

namespace StackoverflowTagBrowser.Data.Services.StackExchangeService
{
    public interface IStackExchangeService
    {
        public Task<TagViewModel> GetTagsAsync(string site, int quantity, SortType sortType, SortOrder sortOrder);
    }
}

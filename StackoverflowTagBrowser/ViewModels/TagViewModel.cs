using StackoverflowTagBrowser.Models;

namespace StackoverflowTagBrowser.ViewModels
{
    public class TagViewModel
    {
        public IList<Tag> Tags { get; set; }
        public int TotalCount { get; set; }
    }
}

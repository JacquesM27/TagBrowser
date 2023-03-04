using StackoverflowTagBrowser.Models;
using System.ComponentModel.DataAnnotations;

namespace StackoverflowTagBrowser.ViewModels
{
    public class TagViewModel
    {
        public IList<Tag> Tags { get; set; }
        [Display(Name = "Popularity %")]
        public int TotalCount { get; set; }
    }
}

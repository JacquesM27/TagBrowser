using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace StackoverflowTagBrowser.Models
{
    public class TagResponse
    {
        public Tag[] items { get; set; }
        public bool has_more { get; set; }
        public int quota_max { get; set; }
        public int quota_remaining { get; set; }
    }

    public class Tag
    {
        public bool has_synonyms { get; set; }
        public bool is_moderator_only { get; set; }
        public bool is_required { get; set; }
        [Display(Name = "Count")]
        public int count { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; }
    }

}

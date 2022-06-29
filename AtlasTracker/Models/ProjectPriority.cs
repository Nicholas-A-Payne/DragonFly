using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class ProjectPriority
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Priority name")]
        public string? Name { get; set; }
    }
}

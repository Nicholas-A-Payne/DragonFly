using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketType
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Type name")]
        public string? Name { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketHistory
    {
//Primary Key---------------------------------------------------------------------------------------------
        public int Id { get; set; }
//History Properties--------------------------------------------------------------------------------------

        [DisplayName("Updated Ticket Item")]
        public string? PropertyName { get; set; }

        [DisplayName("Description Change")]
        public string? Description { get; set; }

        [DisplayName("Date modified")]
        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }
        
        [DisplayName("Previous")]
        public string? OldValue { get; set; }
        
        [DisplayName("Current")]
        public string? NewValue { get; set; }

        public int TicketId { get; set; }

        [Required]
        public string? UserId { get; set; }

 //Navigation---------------------------------------------------------------------------------------------

        [DisplayName("Ticket")]
        public virtual Ticket? Tickets { get; set; }

        [DisplayName("Team Member")]
        public virtual AppUser? User { get; set; }
    }
}

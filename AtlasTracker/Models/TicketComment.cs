using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketComment
    {
//Primary Key---------------------------------------------------------------------------------------------
        public int Id { get; set; }

//Comment Properties--------------------------------------------------------------------------------------
        [Required]
        [DisplayName("Member Comment")]
        [StringLength(100)]
        public string? Comment { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        public int TicketId { get; set; }

        [Required]
        public string? UserId { get; set; }

//Navigation----------------------------------------------------------------------------------------------
        [DisplayName("Ticket")]
        public virtual Ticket? Tickets { get; set; }

        [DisplayName("Team Member")]
        public virtual AppUser? User { get; set; }

    }
}

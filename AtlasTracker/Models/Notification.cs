using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class Notification
    {
//Primary Key---------------------------------------------------------------------------------------------

        public int Id { get; set; }

//Notification Properties---------------------------------------------------------------------------------

        [DisplayName("Ticket")]
        public int? TicketId { get; set; }

        [Required]
        [DisplayName("Title")]
        public string? Title { get; set; }

        [Required]
        [DisplayName("Message")]
        public string? Message { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        [Required]
        public string? RecipentId { get; set; }

        [Required]
        public string? SenderId { get; set; }

        [DisplayName("Has Been Viewed")]
        public bool Viewed { get; set; }

        [Required]
        public int NotificationTypeId { get; set; }

//Navigation----------------------------------------------------------------------------------------------

        public virtual Ticket? Tickets { get; set; }

        public virtual NotificationType? NotificationType { get; set; }

        public virtual AppUser? Recipient { get; set; }

        public virtual AppUser? Sender { get; set; }

    }
}

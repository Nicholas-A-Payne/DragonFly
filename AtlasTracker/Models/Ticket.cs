using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class Ticket
    {

        //primary Key
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Ticket Title")]
        public string? Title { get; set; }

        [Required]
        [StringLength(1000)]
        [DisplayName("Ticket Description")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created Date")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Updated Date")]
        public DateTimeOffset? Updated { get; set; }

        [DisplayName("Archvied")]
        public bool Archived { get; set; }

        [DisplayName("Archvied by Project")]
        public bool ArchivedByProject { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        public int TicketTypeId { get; set; }

        [DisplayName("Priority")]
        public int TicketPriorityId { get; set; }

        [DisplayName("Status")]
        public int TicketStatusId { get; set; }

        [Required]
        public string? OwnerUserId { get; set; }
        public string? DeveloperUserId { get; set; }



        //Navigation--------------------------------------------------------------------------------------
        [DisplayName("Project")]
        public virtual Project? Projects { get; set; }

        public virtual TicketPriority? TicketPriority { get; set; }

        public virtual TicketType? TicketTypes { get; set; }

        public virtual TicketStatus? TicketStatus { get; set; }

        public virtual AppUser? OwnerUser { get; set; }
        public virtual AppUser? DeveloperUser { get; set; }

        public virtual ICollection<TicketComment>? Comments { get; set; } = new HashSet<TicketComment>();

        public virtual ICollection<TicketAttatchment>? Attatchments { get; set; } = new HashSet<TicketAttatchment>();
        public virtual ICollection<TicketHistory>? History { get; set; } = new HashSet<TicketHistory>();
        public virtual ICollection<Notification>? Notifications { get; set; } = new HashSet<Notification>();


    }
}

using AtlasTracker.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtlasTracker.Models
{
    public class TicketAttatchment
    {
//Primary Key---------------------------------------------------------------------------------------------

        public int Id { get; set; }

//Attatchment Properties----------------------------------------------------------------------------------

        [DisplayName("File Description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [DisplayName("Created")]
        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }

        [NotMapped]
        [DisplayName("Select a file")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" })]
        public IFormFile? FormFile { get; set; }

        [DisplayName("Logo")]
        public byte[]? FileData { get; set; }
        public string? FileName { get; set; }

        [Display(Name = "File Extension")]
        public string? FileType { get; set; }


        public int TicketId { get; set; }

        [Required]
        public string? UserId { get; set; }



//Navigation----------------------------------------------------------------------------------------------

        public virtual Ticket? Tickets { get; set; }

        public virtual AppUser? User { get; set; }

    }
}

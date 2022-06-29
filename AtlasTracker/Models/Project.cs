using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtlasTracker.Models
{
    public class Project
    {

        //Primary Key
        public int Id { get; set; }

        //Basic Info--------------------------------------------------------------------------------------------------
        [Required]
        [StringLength(240, ErrorMessage = "The {0} must be {2} and at most {1} characters long", MinimumLength = 2)]
        [DisplayName("Project name")]
        public string? Name { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be {2} and at most {1} characters long", MinimumLength = 10)]
        public string? Description { get; set; }

        [Required]
        [DisplayName("Created Date")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        //Id's-----------------------------------------------------------------------------------------
        public int CompanyId { get; set; }
        public int ProjectPriorityId { get; set; }

        //ImageData------------------------------------------------------------------------------------
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? LogoFormFile { get; set; }

        [DisplayName("Logo")]
        public byte[]? LogoData { get; set; }
        public string? LogoName { get; set; }

        [Display(Name = "File Extension")]
        public string? LogoType { get; set; }

        //Bools
        public bool Archived { get; set; }

        //Navigation-----------------------------------------------------------------------------------
        public virtual Company? Company { get; set; }
        public virtual ICollection<AppUser>? Members { get; set; } = new HashSet<AppUser>();
        public virtual ICollection<Ticket>? Tickets { get; set; } = new HashSet<Ticket>();
        public virtual ProjectPriority? ProjectPriority { get; set; }

    }
}

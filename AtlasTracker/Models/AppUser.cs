using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtlasTracker.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [DisplayName("First Name")]
        [StringLength(15, ErrorMessage = "The {0} must be {2} and at most {1} characters long", MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(15, ErrorMessage = "The {0} must be {2} and at most {1} characters long", MinimumLength = 2)]
        public string? LastName { get; set; }

        [NotMapped]
        [DisplayName("Full Name")]
        public string? FullName { get { return $"{FirstName} {LastName}"; } }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? AvatarFormFile { get; set; }

        [DisplayName("Avatar")]
        public byte[]? AvatarData { get; set; }
        public string? AvatarName { get; set; }

        [Display(Name = "File Extension")]
        public string? AvatarType { get; set; }

        public int CompanyId { get; set; }

        public virtual Company? Company { get; set; }

        public virtual ICollection<Project>? Projects { get; set; } = new HashSet<Project>();



    }
}

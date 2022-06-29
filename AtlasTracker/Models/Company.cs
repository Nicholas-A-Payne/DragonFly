using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class Company
    {

        public Company()
        {

        }

        public Company(int id, string name)
        {
            Id = id;
            Name = name;
        }



        public int Id { get; set; }

        [Required]
        [DisplayName("Company Name")]
        public string? Name { get; set; }

        [DisplayName("Company Description")]
        public string? Descript { get; set; }


        public virtual ICollection<Invite>? Invites { get; set; } = new HashSet<Invite>();
        public virtual ICollection<Project>? Projects { get; set; } = new HashSet<Project>();
        public virtual ICollection<AppUser>? Members { get; set; } = new HashSet<AppUser>();

    }
}

namespace AtlasTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        public Company Company { get; set; }
        public List<Company> Companys { get; set; }
        public Project Project { get; set; }
        public List<Project> Projects { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<AppUser> Members { get; set; }

    }
}

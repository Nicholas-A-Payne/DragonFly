using AtlasTracker.Models;

namespace AtlasTracker.Services.Interfaces
{
    public interface IBTCompanyInfoService
    {
        public Task<Company> GetCompanyInfoByIdAsync(int? CompanyId);

        public Task<List<AppUser>> GetAllMembersAsync(int? CompanyId);

        public Task<List<Project>> GetAllProjectsAsync(int? companyId);

        public Task<List<Ticket>> GetAllTicketsAsync(int? companyId);

        public Task<List<Invite>> GetAllInvitesAsync(int companyId);

    }
}

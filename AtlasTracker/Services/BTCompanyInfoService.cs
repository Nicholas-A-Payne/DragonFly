using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtlasTracker.Services
{
    public class BTCompanyInfoService : IBTCompanyInfoService
    {
        private readonly ApplicationDbContext _context;
        public BTCompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {

            Company company = new();

            try
            {
                if (companyId != null)
                {
                    company = await _context.Companies.Include(c => c.Members)
                                                      .Include(c => c.Projects)
                                                      .Include(c => c.Invites)
                                                      .FirstOrDefaultAsync(c => c.Id == companyId);
                }
                return company!;
            }
            catch (Exception)
            {

                throw;
            }
        }



        public async Task<List<AppUser>> GetAllMembersAsync(int? companyId)
        {
            List<AppUser> members = new();

            try
            {
                members = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

                return members;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsAsync(int? companyId)
        {
            List<Project> project = new();

            try
            {
                project = await _context.Projects.Where(p => p.CompanyId == companyId)
                                        .Include(p => p.Members)!
                                        .Include(p => p.Tickets)!
                                            .ThenInclude(t => t.Comments)
                                        .Include(p => p.Tickets)!
                                            .ThenInclude(t => t.Attatchments)
                                        .Include(p => p.Tickets)!
                                            .ThenInclude(t => t.History)
                                        .Include(p => p.Tickets)!
                                            .ThenInclude(t => t.Notifications)
                                        .Include(p => p.Tickets)!
                                            .ThenInclude(t => t.DeveloperUser)
                                        .Include(p => p.Tickets)!
                                            .ThenInclude(t => t.OwnerUser)
                                        .Include(p => p.Tickets)!
                                            .ThenInclude(t => t.TicketStatus)
                                        .Include(p => p.Tickets)!
                                            .ThenInclude(t => t.TicketPriority)
                                        .Include(p => p.Tickets)!
                                            .ThenInclude(t => t.TicketTypes)
                                        .Include(p => p.ProjectPriority)
                                        .ToListAsync();

                return project;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsAsync(int? companyId)
        {
            List<Ticket> ticket = new();
            List<Project> projects = new();

            try
            {
                projects = await GetAllProjectsAsync(companyId);

                ticket = projects.SelectMany(p => p.Tickets!).ToList();

                return ticket;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Invite>> GetAllInvitesAsync(int companyId)
        {
            List<Invite>? invites = new();
            try
            {
                invites = (await _context.Companies.Include(c => c.Invites)!
                                                    .ThenInclude(i => i.Invitor)
                                                  .Include(c => c.Invites)!
                                                    .ThenInclude(i => i.Invitee)
                                                  .Include(c => c.Invites)!
                                                    .ThenInclude(i => i.Projects)
                                                  .FirstOrDefaultAsync(c => c.Id == companyId))?.Invites!.ToList();
                return invites!;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

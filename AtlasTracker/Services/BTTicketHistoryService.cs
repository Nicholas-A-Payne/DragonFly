using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtlasTracker.Services
{
    public class BTTicketHistoryService : IBTTicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            try
            {
                //New Ticket has been added
                if (oldTicket == null && newTicket != null)
                {

                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        PropertyName = "",
                        OldValue = "",
                        NewValue = "",
                        Created = DateTime.UtcNow,
                        UserId = userId,
                        Description = "New Ticket Created"
                    };

                    try
                    {
                        await _context.AddAsync(history);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    //Existing Ticket has been edited

                    if(oldTicket!.Title != newTicket!.Title)
                    {
                        TicketHistory history = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Title",
                            OldValue = oldTicket.Title,
                            NewValue = newTicket.Title,
                            Created = DateTime.UtcNow,
                            UserId = userId,
                            Description = $"New Ticket Title: {newTicket.Title}"
                        };
                        await _context.AddAsync(history);
                    }

                    if (oldTicket!.Description != newTicket!.Description)
                    {
                        TicketHistory history = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Description",
                            OldValue = oldTicket.Description,
                            NewValue = newTicket.Description,
                            Created = DateTime.UtcNow,
                            UserId = userId,
                            Description = $"New Ticket Description: {newTicket.Description}"
                        };
                        await _context.AddAsync(history);
                    }

                    if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                    {
                        TicketHistory history = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "TicketPriority",
                            OldValue = oldTicket.TicketPriority!.Name,
                            NewValue = newTicket.TicketPriority!.Name,
                            Created = DateTime.UtcNow,
                            UserId = userId,
                            Description = $"New Ticket Priority: {newTicket.TicketPriority.Name}"
                        };
                        await _context.AddAsync(history);
                    }

                    if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                    {
                        TicketHistory history = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "TicketStatus",
                            OldValue = oldTicket.TicketStatus!.Name,
                            NewValue = newTicket.TicketStatus!.Name,
                            Created = DateTime.UtcNow,
                            UserId = userId,
                            Description = $"New Ticket Status: {newTicket.TicketStatus!.Name}"
                        };
                        await _context.AddAsync(history);
                    }

                    if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                    {
                        TicketHistory history = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "TicketType",
                            OldValue = oldTicket.TicketTypes!.Name,
                            NewValue = newTicket.TicketTypes!.Name,
                            Created = DateTime.UtcNow,
                            UserId = userId,
                            Description = $"New Ticket Type: {newTicket.TicketTypes!.Name}"
                        };
                        await _context.AddAsync(history);
                    }
                    if (oldTicket.DeveloperUserId != newTicket!.DeveloperUserId)
                    {
                        TicketHistory history = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Developer",
                            OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                            NewValue = newTicket.DeveloperUser.FullName,
                            Created = DateTime.UtcNow,
                            UserId = userId,
                            Description = $"New Ticket Developer: {newTicket.DeveloperUser!.FullName}"
                        };
                        await _context.AddAsync(history);
                    }




                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddHistoryAsync(int ticketId, string model, string userId)
        {
            try
            {
                Ticket? ticket = await _context.Tickets.FindAsync(ticketId);
                string description = model.ToLower().Replace("ticket", "");
                description = $"New {description} added to ticket: {ticket!.Title}";

                TicketHistory history = new()
                {
                    TicketId = ticket.Id,
                    PropertyName = model,
                    OldValue = "",
                    NewValue = "" ,
                    Created = DateTime.UtcNow,
                    UserId = userId,
                    Description = description
                };
                await _context.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            try
            {
                List<Project> project = (await _context.Companies
                                                       .Include(c => c.Projects)
                                                            .ThenInclude(p => p.Tickets)
                                                                .ThenInclude(t => t.History)
                                                                    .ThenInclude(h => h.User)
                                                       .FirstOrDefaultAsync(c => c.Id == companyId))!.Projects.ToList();

                List<Ticket> tickets = project.SelectMany(p => p.Tickets).ToList();

                List<TicketHistory> ticketHistory = tickets.SelectMany(t => t.History).ToList();

                return ticketHistory;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                         .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.History)
                                                                .ThenInclude(h => h.User)
                                                         .FirstOrDefaultAsync(p => p.Id == projectId);

                List<TicketHistory> ticketHistory = project.Tickets.SelectMany(t => t.History).ToList();

                return ticketHistory;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

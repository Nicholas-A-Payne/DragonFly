﻿using AtlasTracker.Models;

namespace AtlasTracker.Services.Interfaces
{
    public interface IBTLookUpService
    {
        public Task<List<TicketPriority>> GetTicketPrioritiesAsync();
        public Task<List<TicketStatus>> GetTicketStatusesAsync();
        public Task<List<TicketType>> GetTicketTypesAsync();
        public Task<List<ProjectPriority>> GetProjectPrioritiesAsync();
        public Task<int?> LookupNotificationTypeIdAsync(string typeName);
    }
}
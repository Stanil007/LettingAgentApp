using LettingAgentApp.Core.Contracts;
using LettingAgentApp.Infrastructure.Data;
using LettingAgentApp.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LettingAgentApp.Core.Services
{
    public class AgentService : IAgentService
    {
        private readonly ApplicationDbContext context;

        public AgentService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task Create(string userId, string phoneNumber)
        {
            var agent = new Agent()
            {
                UserId = userId,
                PhoneNumber = phoneNumber
            };

            await context.Agents.AddAsync(agent);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsById(string userId)
        {
            return await context.Agents
                 .AnyAsync(a => a.UserId == userId);
        }

        public async Task<int> GetAgentId(string userId)
        {
            return (await context.Agents
                .FirstOrDefaultAsync(a => a.UserId == userId)).Id;
        }

        public async Task<bool> UserHasRents(string userId)
        {
            return await context.Houses
                .AnyAsync(h => h.RenterId == userId);
        }

        public async Task<bool> UserWithPhoneNumberExists(string phoneNumber)
        {
            return await context.Agents
                .AnyAsync(a => a.PhoneNumber == phoneNumber);
        }
    }
}

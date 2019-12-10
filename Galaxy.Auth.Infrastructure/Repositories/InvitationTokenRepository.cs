using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Galaxy.Auth.Infrastructure.Repositories
{
    public class InvitationTokenRepository : IInvitationTokenRepository
    {
        private readonly AuthDbContext _dbContext;

        public InvitationTokenRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(InvitationToken token)
        {
            await _dbContext.InvitationTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<InvitationToken> GetAsync(string token)
        {
            return await _dbContext.InvitationTokens.FirstOrDefaultAsync(t => t.Token == token);
        }
        public async Task RemoveAsync(InvitationToken token)
        {
            _dbContext.InvitationTokens.Remove(token);
            await _dbContext.SaveChangesAsync();
        }
    }
}
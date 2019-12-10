using System.Threading.Tasks;
using Galaxy.Auth.Core.Models;

namespace Galaxy.Auth.Core.Interfaces
{
    public interface IInvitationTokenRepository
    {
        Task AddAsync(InvitationToken token);
        Task<InvitationToken> GetAsync(string token);
        Task RemoveAsync(InvitationToken token);
    }
}
using Microsoft.AspNetCore.Identity;

namespace API_LIBRARY.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
        Task<string> GetTokenAsync();
    }
}

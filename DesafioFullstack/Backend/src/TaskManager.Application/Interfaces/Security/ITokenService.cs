using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces.Security
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
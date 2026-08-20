using InventoryApp.Identity.Domain.Entities;

namespace InventoryApp.Identity.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string token, DateTime expiresAt) GenerateToken(User user, IList<string> roles, IList<string> permissions);
    string GenerateRefreshToken();
}

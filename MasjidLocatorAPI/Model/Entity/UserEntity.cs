using Microsoft.AspNetCore.Identity;

namespace MasjidLocatorAPI.Model.Entity
{
    public class UserEntity : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public IEnumerable<RefreshTokenEntity>? RefreshToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

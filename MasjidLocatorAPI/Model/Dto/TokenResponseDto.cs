namespace MasjidLocatorAPI.Model.Dto
{
    public class TokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}

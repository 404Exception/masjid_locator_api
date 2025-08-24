using MasjidLocatorAPI.Data;
using MasjidLocatorAPI.Model.Dto;
using MasjidLocatorAPI.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MasjidLocatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(UserManager<UserEntity> userManager, IConfiguration configuration, AppDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new UserEntity { UserName = model.Email, Email = model.Email, FullName = model.FullName };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "User registered successfully" });
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginDto model)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        //        return Unauthorized();

        //    var token = GenerateJwtToken(user);
        //    return Ok(new { Token = token, UserId = user.Id, IsAdmin = user.IsAdmin });
        //}

        private string GenerateJwtToken(UserEntity user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            var accessToken = GenerateJwtToken(user);
            var refreshToken = await GenerateRefreshToken(user);

            return Ok(new TokenResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                RefreshTokenExpiry = refreshToken.Expires,
                UserId = user.Id,
                IsAdmin = user.IsAdmin
            });
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == model.RefreshToken);

            if (refreshToken == null || !refreshToken.IsActive)
                return Unauthorized(new { Message = "Invalid refresh token" });

            // Rotate refresh token
            var newRefreshToken = await GenerateRefreshToken(refreshToken.User);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            refreshToken.RevokedByIp = GetIpAddress();

            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();

            var accessToken = GenerateJwtToken(refreshToken.User);

            return Ok(new TokenResponseDto
            {
                Token = accessToken,
                RefreshToken = newRefreshToken.Token,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                RefreshTokenExpiry = newRefreshToken.Expires
            });
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto model)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == model.Token && rt.UserId == GetCurrentUserId());

            if (refreshToken == null || !refreshToken.IsActive)
                return BadRequest(new { Message = "Invalid token" });

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = GetIpAddress();

            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Token revoked" });
        }

        private async Task<RefreshTokenEntity> GenerateRefreshToken(UserEntity user)
        {
            var refreshToken = new RefreshTokenEntity
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddMinutes(Constants.Constants.RefreshTokenValidityMins),
                CreatedByIp = GetIpAddress(),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private string GetIpAddress()
        {
            return Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                   HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }
    }

    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RevokeTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}

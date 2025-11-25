using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using USR5_Login.Entities;

namespace USR5_Login.Repository
{
    public class TokenRepository
    {

        private readonly IConfiguration _configuration;

        private readonly Dictionary<string, (string Email, DateTime Expiration)> _refreshTokens = new();

        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Token GenerarToken(string email, int minutosExpiracion = 5, int minutosRefreshToken = 10)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

           
            var expiresUtc = DateTime.UtcNow.AddMinutes(minutosExpiracion);
            var refreshExpiresUtc = DateTime.UtcNow.AddMinutes(minutosRefreshToken);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresUtc,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.CreateToken(tokenDescriptor);
            string jwtToken = jwtHandler.WriteToken(token);

            string refreshToken = GenerarRefreshToken();
            _refreshTokens[refreshToken] = (email, refreshExpiresUtc);

            
            var expiresLocal = expiresUtc.ToLocalTime();
            var refreshLocal = refreshExpiresUtc.ToLocalTime();

            return new Token
            {
                Access_Token = jwtToken,
                Refresh_Token = refreshToken,
                Expires_In = expiresLocal,
                Email = email,
                RefreshTokenExpires = refreshLocal
            };
        }

        public bool ValidarToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var handler = new JwtSecurityTokenHandler();

            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Token? RefreshToken(string refreshToken)
        {
            if (_refreshTokens.TryGetValue(refreshToken, out var info))
            {
                if (DateTime.UtcNow > info.Expiration)
                {
                    _refreshTokens.Remove(refreshToken);
                    return null;
                }

                _refreshTokens.Remove(refreshToken);

                int minutosJwt = int.Parse(_configuration["Jwt:AccessTokenMinutes"] ?? "5");
                int minutosRefresh = int.Parse(_configuration["Jwt:RefreshTokenMinutes"] ?? "10");

                return GenerarToken(info.Email, minutosJwt, minutosRefresh);
            }

            return null;
        }

        private static string GenerarRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

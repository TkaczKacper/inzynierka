using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using server.Helpers;
using server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace server.Authorization
{
    public interface IJwtUtils
    {
        public string GetJwtToken(User user);
        public Guid? ValidateJwtToken(string token);
        public RefreshToken GetRefreshToken(string ipAddress);
    }
    public class JwtUtils : IJwtUtils
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;

        public JwtUtils(
            DataContext context,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        // generate token that is valid for 30 minutes
        public string GetJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.ID.ToString()) }),
                // Expires = DateTime.UtcNow.AddSeconds(30),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Guid? ValidateJwtToken(string token)
        {
            Console.WriteLine(token);
            if (token == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateActor = false,
                    // make sure that tokens expire exactly at token expiration time
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                Console.WriteLine(jwtToken);
                Console.WriteLine($"xd");
                return userId;
            }
            catch
            {
                // return null if validations fails somewhere
                return null;
            }
        }

        public RefreshToken GetRefreshToken(string ipAddress)
        {
            var refreshToken = new RefreshToken
            {
                Token = getUniqueToken(),

                // token is valid for 31 days
                Expires = DateTime.UtcNow.AddDays(31),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;

            string getUniqueToken()
            {
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                var tokenIsUnique = !_context.Users.Any(u => u.RefreshTokens.Any(t => t.Token == token));

                if (!tokenIsUnique)
                    return getUniqueToken();

                return token;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Authorization;
using server.Helpers;
using server.Models;
using server.Models.Authenticate;
using server.Utilities;

namespace server.Services
{
    public interface IUserService
    {
        AuthResponse Authenticate(LoginRequest model, string ipAddress);
        AuthResponse Register(RegisterRequest model, string ipAddress);
        AuthResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        AuthResponse RenewAccessToken(string token);
    }
    public class UserService : IUserService
    {
        private DataContext _context;
        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;
        private IPasswordHasher _passwordHasher;

        public UserService(
            DataContext context,
            IJwtUtils jwtUtils,
            IOptions<AppSettings> appSettings,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
            _passwordHasher = passwordHasher;
        }

        public AuthResponse Authenticate(LoginRequest model, string ipAddress)
        {
            var user = _context.Users.Include(u => u.StravaProfile).SingleOrDefault(x => x.Username == model.Username);

            // validate
            if (user == null || !_passwordHasher.Verify(user.Password, model.Password).Result)
                throw new AppException("Username or password is incorrect");

            // auth success, generate jwt and refresh tokens
            var jwtToken = _jwtUtils.GetJwtToken(user);
            var refreshToken = _jwtUtils.GetRefreshToken(ipAddress);
            user.RefreshTokens?.Add(refreshToken);

            // remove old refresh tokens from user
            removeOldRefreshTokens(user);

            _context.Update(user);
            _context.SaveChanges();
            var stravaRefreshToken = user.StravaProfile?.StravaRefreshToken;
            return new AuthResponse(user, jwtToken, refreshToken.Token, stravaRefreshToken);
        }

        public AuthResponse Register(RegisterRequest model, string ipAddress)
        {
            var possibleUser = _context.Users.SingleOrDefault(x => x.Username == model.Username || x.Email == model.Email);
            if (possibleUser != null) throw new AppException("Username or/and Email taken.");

            // if username/email are available register user
            User userRegister = new User
            {
                Email = model.Email,
                Username = model.Username,
                Password = _passwordHasher.Hash(model.Password).Result,
                RegisterDate = DateTime.UtcNow
            };
            _context.Add(userRegister);
            _context.SaveChanges();

            // generate jwt and refresh token
            var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);

            var jwtToken = _jwtUtils.GetJwtToken(user);
            var refreshToken = _jwtUtils.GetRefreshToken(ipAddress);
            Console.WriteLine(jwtToken, refreshToken.Token);
            user.RefreshTokens.Add(refreshToken);

            _context.Update(user);
            _context.SaveChanges();

            return new AuthResponse(userRegister, jwtToken, refreshToken.Token, "");
        }
        public AuthResponse RefreshToken(string token, string ipAddress)
        {
            var user = getUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case leak
                revokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked token: {token}");
                _context.Update(user);
                _context.SaveChanges();
            }

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token.");

            // update refresh token
            var newRefreshToken = updateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh token from user
            removeOldRefreshTokens(user);

            // save changes
            _context.Update(user);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = _jwtUtils.GetJwtToken(user);

            return new AuthResponse(user, jwtToken, newRefreshToken.Token, "");
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var user = getUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token.");

            // revoke token and save
            revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            _context.Update(user);
            _context.SaveChanges();
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(Guid id)
        {
            var user = _context.Users.Find(id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }

        public AuthResponse RenewAccessToken(string token) 
        {
            var user = getUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (refreshToken.IsExpired) {
                throw new AppException("TODO logout response");
            }
            var jwtToken = _jwtUtils.GetJwtToken(user);
            
            return new AuthResponse(user, jwtToken, token, "");
        }

        // helper methods
        private User getUserByRefreshToken(string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            return user == null ? throw new AppException("Invalid token.") : user;
        }

        private RefreshToken updateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GetRefreshToken(ipAddress);
            revokeRefreshToken(refreshToken, ipAddress, "Replaced by new Token", newRefreshToken.Token);

            return newRefreshToken;
        }

        private void removeOldRefreshTokens(User user)
        {
            // remove old inactive refresh token from user based on TTL
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private void revokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string ipAddress, string reason)
        {
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);

                if (childToken.IsActive)
                    revokeRefreshToken(childToken, ipAddress, reason);
                else
                    revokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
            }
        }

        private void revokeRefreshToken(RefreshToken token, string ipAddress, string? reason = null, string? replaceByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.RevokedReason = reason;
            token.ReplacedByToken = replaceByToken;
        }
    }
}

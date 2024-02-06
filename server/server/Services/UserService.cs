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
        Task<AuthResponse> Authenticate(LoginRequest model, string ipAddress);
        Task<AuthResponse> Register(RegisterRequest model, string ipAddress);
        Task<AuthResponse> RefreshToken(string token, string ipAddress);
        Task<AuthResponse> RenewAccessToken(string token);
        Task<string> ChangePassword(Guid? userId, ChangePassword password);
        Task<string> RevokeToken(string token, string ipAddress);
        //TODO usuwanie konta
        User GetById(Guid id);
    }
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            DataContext context,
            IJwtUtils jwtUtils,
            IOptions<AppSettings> appSettings,
            IPasswordHasher passwordHasher,
            IHelperService helperService)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponse> Authenticate(LoginRequest model, string ipAddress)
        {
            var user = await _context.Users.Include(u => u.StravaProfile).SingleOrDefaultAsync(x => x.Username == model.Username);
            
            // validate
            if (user == null || !_passwordHasher.Verify(user.Password, model.Password).Result)
                throw new AppException("Username or password are incorrect.");

            // auth success, generate jwt and refresh tokens
            var jwtToken = _jwtUtils.GetJwtToken(user);
            var refreshToken = _jwtUtils.GetRefreshToken(ipAddress);
            user.RefreshTokens?.Add(refreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);

            _context.Update(user);
            await _context.SaveChangesAsync();
            
            var stravaRefreshToken = user.StravaProfile?.StravaRefreshToken;
            return new AuthResponse(user, jwtToken, refreshToken.Token, stravaRefreshToken);
        }

        public async Task<string> ChangePassword(Guid? userId, ChangePassword model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.ID == userId);
            
            if (user == null || !_passwordHasher.Verify(user.Password, model.OldPassword).Result)
                throw new AppException("Provided password does not match with current password.");

            var passwordHashed = _passwordHasher.Hash(model.NewPassword).Result;

            user.Password = passwordHashed;

            _context.Update(user);
            await _context.SaveChangesAsync();
            
            return "Password updated.";
        }
        
        public async Task<AuthResponse> Register(RegisterRequest model, string ipAddress)
        {
            var possibleUser = await _context.Users.SingleOrDefaultAsync(x => x.Username == model.Username || x.Email == model.Email);
            
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
            await _context.SaveChangesAsync();

            // generate jwt and refresh token
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == model.Username);

            var jwtToken = _jwtUtils.GetJwtToken(user);
            var refreshToken = _jwtUtils.GetRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);

            _context.Update(user);
            await _context.SaveChangesAsync();

            return new AuthResponse(userRegister, jwtToken, refreshToken.Token, "");
        }

        public async Task<AuthResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case leak
                RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked token: {token}");
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token.");

            // update refresh token
            var newRefreshToken = UpdateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh token from user
            RemoveOldRefreshTokens(user);

            // save changes
            _context.Update(user);
            await _context.SaveChangesAsync();

            // generate new jwt
            var jwtToken = _jwtUtils.GetJwtToken(user);

            return new AuthResponse(user, jwtToken, newRefreshToken.Token, "");
        }

        public async Task<string> RevokeToken(string token, string ipAddress)
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token.");

            // revoke token and save
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            
            _context.Update(user);
            await _context.SaveChangesAsync();

            return "Token revoked.";
        }

        public async Task<AuthResponse> RenewAccessToken(string token) 
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            
            if (refreshToken.IsExpired) {
                throw new AppException("TODO logout response");
            }
            var jwtToken = _jwtUtils.GetJwtToken(user);
            
            return new AuthResponse(user, jwtToken, token, "");
        }

        public User GetById(Guid id)
        {
            var user = _context.Users.Find(id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }
        
        // helper methods
        private async Task<User> GetUserByRefreshToken(string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            return user == null ? throw new AppException("Invalid token.") : user;
        }

        private RefreshToken UpdateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GetRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new Token", newRefreshToken.Token);

            return newRefreshToken;
        }

        private void RemoveOldRefreshTokens(User user)
        {
            // remove old inactive refresh token from user based on TTL
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string ipAddress, string reason)
        {
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);

                if (childToken.IsActive)
                    RevokeRefreshToken(childToken, ipAddress, reason);
                else
                    RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
            }
        }

        private void RevokeRefreshToken(RefreshToken token, string ipAddress, string? reason = null, string? replaceByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.RevokedReason = reason;
            token.ReplacedByToken = replaceByToken;
        }
    }
}

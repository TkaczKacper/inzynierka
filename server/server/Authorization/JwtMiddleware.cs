using Microsoft.Extensions.Options;
using server.Helpers;
using server.Services;

namespace server.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _request;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate request, IOptions<AppSettings> appSettings)
        {
            _request = request;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Console.WriteLine(token);
            var userId = jwtUtils.ValidateJwtToken(token);

            if (userId != null)
            {
                context.Items["User"] = userService.GetById(userId.Value);
            }

            await _request(context);
        }
    }
}

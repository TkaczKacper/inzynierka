using Microsoft.AspNetCore.Mvc;
using server.Authorization;
using server.Helpers;
using server.Models;

namespace server.Services
{
    public interface IStravaService
    {
        StravaProfile ProfileUpdate(StravaProfile profileInfo, int? id);
    }

    public class StravaService : IStravaService
    {
        private DataContext _context;

        public StravaService(DataContext context)
        {
            _context = context;
        }

        public StravaProfile ProfileUpdate(StravaProfile profile, int? id)
        {
            Console.WriteLine("profile update");
            User? user = GetById(id);

            StravaProfile profileDetails = new StravaProfile
            {
                StravaRefreshToken = profile.StravaRefreshToken,
                ProfileID = profile.ProfileID,
                Username = profile.Username,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Sex = profile.Sex,
                Bio = profile.Bio,
                ProfileAvatar = profile.ProfileAvatar,
                Country = profile.Country,
                State = profile.State,
                City = profile.City,
                Weight = profile.Weight,
                ProfileCreatedAt = profile.ProfileCreatedAt
            };

            Console.WriteLine(profileDetails);

            user.StravaProfile = profileDetails;

            _context.Update(user);
            _context.SaveChanges();

            return profile;
        }


        //helper methods
        public User GetById(int? id)
        {
            var user = _context.Users.Find(id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }
    }
}

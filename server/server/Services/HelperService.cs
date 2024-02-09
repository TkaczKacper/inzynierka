using Microsoft.EntityFrameworkCore;
using server.Helpers;
using server.Models;
using server.Models.Activity;
using server.Models.Profile;
using server.Models.Profile.Summary;

namespace server.Services;

public interface IHelperService
{
    Task<User?> GetUserById(Guid userId);
    Task<User?> GetDetailedUserById(Guid userId);
    Task<List<long>> GetSyncedActivitiesId(Guid userId);
    Task<List<Activity>> GetAthleteActivities(Guid userId, DateTime date, int perPage);
    Task<List<Activity>> GetAthleteActivitiesInDateRange(Guid userId, DateTime first, DateTime last);
    Task<DateTime> GetLatestActivity(Guid userId);
    Task<List<ProfilePower>> GetAthletePower(Guid userId);
    Task<List<ProfileHeartRate>> GetAthleteHeartRate(Guid userId);
    Task<List<ProfileYearlySummary>> GetAthleteYearlySummary(Guid userId, int yearOffset);
    Task<List<ProfileYearlySummary>> GetAthleteYearlySummaries(Guid userId);
    Task<List<ProfileMonthlySummary>> GetAthleteMonthlySummary(Guid userId, int yearOffset);
    Task<List<ProfileMonthlySummary>> GetAthleteMonthlySummaries(Guid userId);
    Task<List<ProfileWeeklySummary>> GetAthleteWeeklySummary(Guid userId, int yearOffset);
    Task<List<ProfileWeeklySummary>> GetAthleteWeeklySummaries(Guid userId);
}

public class HelperService : IHelperService
{
    private readonly DataContext _context;

    public HelperService(DataContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetUserById(Guid id)
    {
        User? user = await _context.Users
            .FirstOrDefaultAsync(u => u.ID == id);
        return user == null ? throw new KeyNotFoundException("User not found.") : user;
    }
    
    public async Task<User?> GetDetailedUserById(Guid id)
    {
        User? user = await _context.Users
            .Include(u => u.StravaProfile)
            .FirstOrDefaultAsync(u => u.ID == id);
        return user == null ? throw new KeyNotFoundException("User not found.") : user;
    }
    
    public async Task<List<long>> GetSyncedActivitiesId(Guid id)
    {
        List<long> userActivitiesId = await _context.Activity
            .Where(sa => sa.UserId == id)
            .Select(sa => sa.StravaActivityID)
            .ToListAsync();

        return userActivitiesId;
    }
    
    public async Task<List<Activity>> GetAthleteActivities(Guid userId, DateTime date, int perPage)
    {
        var activities = await _context.Activity
            .Where(a => a.UserId == userId && a.StartDate < date) 
            .Include(a=> a.Laps)
            .OrderByDescending(a=> a.StartDate)
            .Take(perPage)
            .ToListAsync();

        return activities;
    }

    public async Task<List<Activity>> GetAthleteActivitiesInDateRange(Guid userId, DateTime first, DateTime last)
    {
        var activities = await _context.Activity
            .Where(a => a.UserId == userId && a.StartDate > first && a.StartDate < last)
            .Include(a => a.Laps)
            .OrderByDescending(a => a.StartDate)
            .ToListAsync();

        return activities;
    }

    public async Task<DateTime> GetLatestActivity(Guid userId)
    {
        var date = await _context.Activity
            .Where(sa => sa.UserId == userId)
            .OrderByDescending(sa => sa.StartDate)
            .Select(sa => sa.StartDate)
            .FirstOrDefaultAsync();
        return date;
    }

    public async Task<List<ProfilePower>> GetAthletePower(Guid userId)
    {
        var profilePower = await _context.ProfilePower
            .Where(pp => pp.UserID == userId)
            .ToListAsync();
        
        return profilePower;
    }

    public async Task<List<ProfileHeartRate>> GetAthleteHeartRate(Guid userId)
    {
        var profileHr = await _context.ProfileHeartRate
            .Where(phr => phr.UserID == userId)
            .ToListAsync();

        return profileHr;
    }

    public async Task<List<ProfileYearlySummary>> GetAthleteYearlySummary(Guid userId, int yearOffset)
    {
        var summary = await _context.ProfileYearlySummary
            .Where(x => x.UserId == userId && x.Year == DateTime.UtcNow.AddYears(-yearOffset).Year)
            .OrderBy(x => x.Year)
            .ToListAsync();

        return summary;
    }
    
    public async Task<List<ProfileYearlySummary>> GetAthleteYearlySummaries(Guid userId)
    {
        var summary = await _context.ProfileYearlySummary
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Year)
            .ToListAsync();

        return summary;
    }

    public async Task<List<ProfileMonthlySummary>> GetAthleteMonthlySummary(Guid userId, int yearOffset)
    {
        var summary = await _context.ProfileMonthlySummary
            .Where(x => x.UserId == userId && x.Year == DateTime.UtcNow.AddYears(-yearOffset).Year)
            .OrderBy(x => x.Month)
            .ToListAsync();

        return summary;
    }
    
    public async Task<List<ProfileMonthlySummary>> GetAthleteMonthlySummaries(Guid userId)
    {
        var summary = await _context.ProfileMonthlySummary
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        return summary;
    }

    public async Task<List<ProfileWeeklySummary>> GetAthleteWeeklySummary(Guid userId, int yearOffset)
    {
        var summary = await _context.ProfileWeeklySummary
            .Where(x => x.UserId == userId && x.Year == DateTime.UtcNow.AddYears(-yearOffset).Year)
            .OrderBy(x => x.Week)
            .ToListAsync();

        return summary;
    }
    public async Task<List<ProfileWeeklySummary>> GetAthleteWeeklySummaries(Guid userId)
    {
        var summary = await _context.ProfileWeeklySummary
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Week)
            .ToListAsync();

        return summary;
    }
}
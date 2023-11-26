﻿using server.Models;
using server.Models.Profile;
using server.Models.Profile.Summary;

namespace server.Responses;

public class AthleteData
{
    public StravaProfileStats AthleteStats { get; set; }
    public StravaProfile StravaProfileInfo { get; set; }
    
    public List<ProfileHeartRate>? HrZones { get; set; }
    public List<ProfilePower>? PowerZones { get; set; }
    public List<ProfileMonthlySummary>? MonthlySummaries { get; set; }
}
using server.Models;
using server.Models.Profile;

namespace server.Responses;

public class AthleteData
{
    public StravaProfileStats AthleteStats { get; set; }
    public StravaProfile StravaProfileInfo { get; set; }
}
using Microsoft.EntityFrameworkCore;
using server.Helpers;
using server.Models;
using server.Models.Strava;
using System.Globalization;
using server.Models.Profile;
using server.Models.Profile.Summary;

namespace server.Services
{
    public interface IActivityService
    {
        Task<string> GetActivityDetails(string accessToken, Guid? userId);
        StravaActivity GetActivityById(long activityId);
        List<TrainingLoads> GetUserTrainingLoad(Guid? userId);
    }

    public class ActivityService : IActivityService
    {
        private readonly DataContext _context;
        private IStravaApiService _stravaApi;

        public ActivityService(
            DataContext context,
            IStravaApiService stravaApi)
        {
            _context = context;
            _stravaApi = stravaApi;
        }

        private static HttpClient stravaClient = new()
        {
            BaseAddress = new Uri("https://www.strava.com/api/v3/"),
        };

        public async Task<string> GetActivityDetails(string accesstoken, Guid? userId)
        {
            User user = await GetUserByIdAsync(userId);

            ProfilePower? powerZones = _context.ProfilePower
                .FirstOrDefault(pp => pp.UserID == userId);

            ProfileHeartRate? hrZones = _context.ProfileHeartRate
                .FirstOrDefault(hr => hr.UserID == userId);

            List<ProfileWeeklySummary>? weeklySummary = _context.ProfileWeeklySummary
                .Where(sum => sum.UserId == userId)
                .ToList();
            List<int[]> existingWeeklySummary = new List<int[]>();
            foreach (var obj in weeklySummary)
            {
                existingWeeklySummary.Add(new[] { obj.Year, obj.Week });
            }

            List<ProfileWeeklySummary> weeklySummariesToAdd = new List<ProfileWeeklySummary>();
            List<int[]> existingWeeklySummaryToAdd = new List<int[]>();

            List<ProfileMonthlySummary>? monthlySummary = _context.ProfileMonthlySummary
                .Where(sum => sum.UserId == userId)
                .ToList();
            List<int[]> existingMonthlySummary = new List<int[]>();
            foreach (var obj in monthlySummary)
            {
                existingMonthlySummary.Add(new[] { obj.Year, obj.Month });
            }

            List<ProfileMonthlySummary> monthlySummariesToAdd = new List<ProfileMonthlySummary>();
            List<int[]> existingMonthlySummaryToAdd = new List<int[]>();

            List<TrainingLoads>? trainingLoads = _context.TrainingLoads
                .Where(tl => tl.UserId == userId)
                .ToList();

            List<TrainingLoads>? trainingLoadsToAdd = new List<TrainingLoads>();


            List<long> ids = user.ActivitiesToFetch;
            List<long> activitiesAdded = new List<long>();

            List<StravaActivity> activities = new List<StravaActivity>();
            Console.WriteLine(stravaClient.DefaultRequestHeaders);
            if (!stravaClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                stravaClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accesstoken}");
            }

            Console.WriteLine(stravaClient.DefaultRequestHeaders);
            int? HrRest = user.UserHeartRate?.LastOrDefault()?.HrRest;
            int? HrMax = user.UserHeartRate?.LastOrDefault()?.HrMax;
            int? userFtp = user.UserPower?.LastOrDefault()?.FTP;

            CultureInfo myCI = new CultureInfo("pl-PL");
            Calendar myCal = myCI.Calendar;

            foreach (long id in ids)
            {
                var details = await _stravaApi.GetDetailsById(id, stravaClient);
                if (details.Manual)
                {
                    activitiesAdded.Add(id);
                    continue;
                }

                ;

                StravaActivityStreams streams = await _stravaApi.GetStreamsById(id, stravaClient);

                if (details is null || streams is null) break;

                Console.WriteLine($"creating activity {id}");

                List<StravaActivityLap> activityLaps = new List<StravaActivityLap>();

                List<int> activityPowerCurve = new List<int>();
                if (streams.Watts.Count > 0)
                {
                    for (int i = 1; i < streams.Watts.Count; i++)
                    {
                        activityPowerCurve.Add(CalculateActivityPowerCurve(streams.Watts, i));
                    }
                }

                foreach (var lap in details.Laps)
                {
                    StravaActivityLap activityLap = new StravaActivityLap()
                    {
                        ElapsedTime = lap.elapsed_time,
                        MovingTime = lap.moving_time,
                        StartDate = lap.start_date,
                        Distance = lap.distance,
                        StartIdx = lap.start_index,
                        EndIdx = lap.end_index,
                        LapIndex = lap.lap_index,
                        TotalElevationGain = lap.total_elevation_gain,
                        AvgSpeed = lap.average_speed,
                        MaxSpeed = lap.max_speed,
                        AvgWatts = lap.average_watts,
                        AvgCadence = lap.average_cadence,
                        AvgHeartRate = lap.average_heartrate,
                        MaxHeartRate = lap.max_heartrate
                    };
                    activityLaps.Add(activityLap);
                }

                try
                {
                    StravaActivity activity = new StravaActivity
                    {
                        StravaActivityID = details.Id,
                        Title = details.Name,
                        Type = details.Type,
                        SportType = details.Sport_type,
                        TotalDistance = details.Distance,
                        MovingTime = details.Moving_time,
                        ElapsedTime = details.Elapsed_time,
                        TotalElevationGain = details.Total_elevation_gain,
                        Calories = details.Calories,
                        StartDate = details.Start_date,
                        StartLatLng = details.Start_latlng,
                        EndLatLng = details.End_latlng,
                        AvgSpeed = details.Average_speed,
                        MaxSpeed = details.Max_speed,
                        AvgHeartRate = details.Average_heartrate,
                        MaxHeartRate = (int)details.Max_heartrate,
                        Trainer = details.Trainer,
                        HasPowerMeter = details.Device_watts,
                        AvgWatts = details.Average_watts,
                        MaxWatts = details.Max_watts,
                        WeightedAvgWatts = details.Weighted_average_watts,
                        AvgCadence = details.Average_cadence,
                        AvgTemp = details.Average_temp,
                        ElevationHigh = details.Elev_high,
                        ElevationLow = details.Elev_low,
                        Gear = details.Gear?.name,
                        DeviceName = details.Device_name,
                        SummaryPolyline = details.Map?.summary_polyline,
                        DetailedPolyline = details.Map?.polyline,
                        Achievements = details.Achievement_count,
                        ActivityStreams = streams,
                        Laps = activityLaps,
                        PowerCurve = activityPowerCurve,
                        UserProfile = user
                    };
                    if (streams.Temperature?.Count > 0)
                    {
                        activity.MaxTemp = streams.Temperature.Max();
                    }
                    if (streams.Cadence?.Count > 0)
                    {
                        activity.MaxCadence = streams.Cadence.Max();
                    }
                    
                    double activityTrainingLoad = 0;
                    if (details.Average_heartrate > 0 && HrMax is not null && HrRest is not null &&
                        streams.HeartRate?.Count > 0)
                    {
                        TimeInHrZone timeInHrZones = CalculateTimeInHrZones(streams.HeartRate, userId, hrZones);
                        activity.HrTimeInZone = timeInHrZones;

                        double multiplier = user.StravaProfile.Sex == "M" ? 1.92 : 1.67;
                        double trimp =
                            details.Moving_time / 60
                            * (details.Average_heartrate - (int)HrRest) / ((int)HrMax - (int)HrRest)
                            * 0.64
                            * Math.Exp(multiplier * (details.Average_heartrate - (int)HrRest) /
                                       ((int)HrMax - (int)HrRest));
                        activity.Trimp = trimp;
                        activityTrainingLoad = trimp;
                    }

                    if (details.Device_watts && streams.Watts?.Count > 0)
                    {
                        TimeInPowerZone timeInPowerZone = CalculateTimeInPowerZones(streams.Watts, userId, powerZones);
                        activity.PowerTimeInZone = timeInPowerZone;

                        int FTP = userFtp is null ? 250 : (int)userFtp;
                        List<double> avg = Enumerable.Range(0, streams.Watts
                            .Count - 29).Select(i => Math.Pow(streams.Watts.Skip(i).Take(30).Average(), 4)).ToList();

                        double EffectivePower = Math.Pow(avg.Average(), 0.25);
                        double Intensity = EffectivePower / FTP;
                        double VariabilityIndex = EffectivePower / details.Average_watts;
                        double TrainingLoad = (details.Moving_time * EffectivePower * Intensity) / (FTP * 36);


                        activity.EffectivePower = EffectivePower;
                        activity.Intensity = Intensity;
                        activity.VariabilityIndex = VariabilityIndex;
                        activity.TrainingLoad = TrainingLoad;
                        activityTrainingLoad = TrainingLoad;
                    }

                    //weekly summary
                    int weekNumber = myCal.GetWeekOfYear(activity.StartDate, CalendarWeekRule.FirstFourDayWeek,
                        DayOfWeek.Monday);
                    int weeklySummaryId = existingWeeklySummary.IndexOf(
                        existingWeeklySummary.Find(arr => arr.SequenceEqual(new[]
                        {
                            activity.StartDate.Year,
                            weekNumber
                        })));
                    if (weeklySummaryId >= 0)
                    {
                        weeklySummary[weeklySummaryId].TotalDistance += activity.TotalDistance;
                        weeklySummary[weeklySummaryId].TotalElevationGain += activity.TotalElevationGain;
                        weeklySummary[weeklySummaryId].TotalCalories += activity.Calories;
                        weeklySummary[weeklySummaryId].TotalMovingTime += activity.MovingTime;
                        weeklySummary[weeklySummaryId].TotalElapsedTime += activity.ElapsedTime;
                        weeklySummary[weeklySummaryId].TrainingLoad += activityTrainingLoad;
                    }
                    else
                    {
                        int weeklySummaryToAddId = existingWeeklySummaryToAdd.IndexOf(
                            existingWeeklySummaryToAdd.Find(arr => arr.SequenceEqual(new[]
                            {
                                activity.StartDate.Year,
                                weekNumber
                            })));
                        if (weeklySummaryToAddId >= 0)
                        {
                            weeklySummariesToAdd[weeklySummaryToAddId].TotalDistance += activity.TotalDistance;
                            weeklySummariesToAdd[weeklySummaryToAddId].TotalElevationGain +=
                                activity.TotalElevationGain;
                            weeklySummariesToAdd[weeklySummaryToAddId].TotalCalories += activity.Calories;
                            weeklySummariesToAdd[weeklySummaryToAddId].TotalMovingTime += activity.MovingTime;
                            weeklySummariesToAdd[weeklySummaryToAddId].TotalElapsedTime += activity.ElapsedTime;
                            weeklySummariesToAdd[weeklySummaryToAddId].TrainingLoad += activityTrainingLoad;
                        }
                        else
                        {
                            DateTime firstDay = DateTime.Now;
                            DateTime lastDay = DateTime.Now;
                            if ((int)activity.StartDate.DayOfWeek == 0)
                            {
                                firstDay = activity.StartDate.AddDays(-6);
                                lastDay = activity.StartDate;
                            }
                            else
                            {
                                firstDay = activity.StartDate.AddDays(1 - (int)activity.StartDate.DayOfWeek);
                                lastDay = activity.StartDate.AddDays(7 - (int)activity.StartDate.DayOfWeek);
                            }

                            ProfileWeeklySummary newWeeklySummary = new ProfileWeeklySummary
                            {
                                Year = activity.StartDate.Year,
                                Week = weekNumber,
                                WeekStartDate = firstDay,
                                WeekEndDate = lastDay,
                                TotalDistance = activity.TotalDistance,
                                TotalElevationGain = activity.TotalElevationGain,
                                TotalCalories = activity.Calories,
                                TotalMovingTime = activity.MovingTime,
                                TotalElapsedTime = activity.ElapsedTime,
                                TrainingLoad = activityTrainingLoad,
                                UserId = (Guid)userId
                            };

                            weeklySummariesToAdd.Add(newWeeklySummary);
                            existingWeeklySummaryToAdd.Add(new[] { activity.StartDate.Year, weekNumber });
                        }
                    }

                    //monthly summary
                    int monthlySummaryId = existingMonthlySummary.IndexOf(
                        existingMonthlySummary.Find(arr =>
                            arr.SequenceEqual(new[] { activity.StartDate.Year, activity.StartDate.Month })));

                    if (monthlySummaryId >= 0)
                    {
                        monthlySummary[monthlySummaryId].TotalDistance += activity.TotalDistance;
                        monthlySummary[monthlySummaryId].TotalElevationGain += activity.TotalElevationGain;
                        monthlySummary[monthlySummaryId].TotalCalories += activity.Calories;
                        monthlySummary[monthlySummaryId].TotalMovingTime += activity.MovingTime;
                        monthlySummary[monthlySummaryId].TotalElapsedTime += activity.ElapsedTime;
                        monthlySummary[monthlySummaryId].TrainingLoad += activityTrainingLoad;
                    }
                    else
                    {
                        int monthlySummaryToAddId = existingMonthlySummaryToAdd.IndexOf(
                            existingMonthlySummaryToAdd.Find(arr =>
                                arr.SequenceEqual(new[] { activity.StartDate.Year, activity.StartDate.Month })));
                        if (monthlySummaryToAddId >= 0)
                        {
                            monthlySummariesToAdd[monthlySummaryToAddId].TotalDistance += activity.TotalDistance;
                            monthlySummariesToAdd[monthlySummaryToAddId].TotalElevationGain +=
                                activity.TotalElevationGain;
                            monthlySummariesToAdd[monthlySummaryToAddId].TotalCalories += activity.Calories;
                            monthlySummariesToAdd[monthlySummaryToAddId].TotalMovingTime += activity.MovingTime;
                            monthlySummariesToAdd[monthlySummaryToAddId].TotalElapsedTime += activity.ElapsedTime;
                            monthlySummariesToAdd[monthlySummaryToAddId].TrainingLoad += activityTrainingLoad;
                        }
                        else
                        {
                            ProfileMonthlySummary newMonthlySummary = new ProfileMonthlySummary
                            {
                                Year = activity.StartDate.Year,
                                Month = activity.StartDate.Month,
                                TotalDistance = activity.TotalDistance,
                                TotalElevationGain = activity.TotalElevationGain,
                                TotalMovingTime = activity.MovingTime,
                                TotalElapsedTime = activity.ElapsedTime,
                                TrainingLoad = activityTrainingLoad,
                                TotalCalories = activity.Calories,
                                UserId = (Guid)userId
                            };
                            monthlySummariesToAdd.Add(newMonthlySummary);
                            existingMonthlySummaryToAdd.Add(new[]
                                { activity.StartDate.Year, activity.StartDate.Month });
                        }
                    }

                    // training load
                    int tlIndex = trainingLoads.IndexOf(trainingLoads
                        .Find(trainingLoad =>
                            trainingLoad.Date == DateOnly.FromDateTime(activity.StartDate)));
                    int tlToAddIndex = trainingLoadsToAdd.IndexOf(trainingLoadsToAdd
                        .Find(trainingLoadToAdd =>
                            trainingLoadToAdd.Date == DateOnly.FromDateTime(activity.StartDate)));

                    if (tlIndex >= 0)
                    {
                        trainingLoads[tlIndex].TrainingImpulse += activity.Trimp > 0 ? (int)activity.Trimp : 0;
                        trainingLoads[tlIndex].TrainingLoad += activity.TrainingLoad > 0 ? (int)activity.TrainingLoad : 0;
                    }
                    else if (tlToAddIndex >= 0)
                    {
                        trainingLoadsToAdd[tlToAddIndex].TrainingImpulse +=
                            activity.Trimp > 0 ? (int)activity.Trimp : 0;
                        trainingLoadsToAdd[tlToAddIndex].TrainingLoad +=
                            activity.TrainingLoad > 0 ? (int)activity.TrainingLoad : 0;
                    }
                    else
                    {
                        trainingLoadsToAdd.Add(new TrainingLoads
                        {
                            Date = DateOnly.FromDateTime(activity.StartDate),
                            TrainingLoad = activity.TrainingLoad > 0 ? (int)activity.TrainingLoad : 0,
                            TrainingImpulse = activity.Trimp > 0 ? (int)activity.Trimp : 0,
                            UserId = (Guid)userId
                        });
                    }

                    activities.Add(activity);
                    activitiesAdded.Add(id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR" + ex.Message);
                }
            }

            foreach (var trainingLoad in trainingLoads)
            {
                _context.TrainingLoads.Update(trainingLoad);
            }

            _context.TrainingLoads.AddRange(trainingLoadsToAdd);
            _context.StravaActivity.AddRange(activities);
            _context.ProfileWeeklySummary.AddRange(weeklySummariesToAdd);
            _context.ProfileMonthlySummary.AddRange(monthlySummariesToAdd);
            _context.SaveChanges();

            List<long> remainingActivities = ids.Where(id => !activitiesAdded.Contains(id)).ToList();
            user.ActivitiesToFetch = remainingActivities;
            _context.SaveChanges();

            UpdateTrainingLoad(userId);

            return $"{activities.Count} synced.";
        }

        public StravaActivity GetActivityById(long activityId)
        {
            var activity = _context.StravaActivity
                .Include(sa => sa.HrTimeInZone)
                .Include(sa => sa.PowerTimeInZone)
                .FirstOrDefault(sa => sa.ID == activityId);

            return activity == null ? throw new KeyNotFoundException("Activity not found.") : activity;
        }

        public List<TrainingLoads> GetUserTrainingLoad(Guid? userId)
        {
            List<TrainingLoads> trainingLoads = _context.TrainingLoads
                .Where(tl => tl.UserId == userId)
                .OrderBy(tl => tl.Date)
                .ToList();

            if (trainingLoads.Last().Date < DateOnly.FromDateTime(DateTime.Today))
            {
                return CalculateTrainingLoadToday(userId, trainingLoads);
            }
            
            return trainingLoads;
        }
        
        private async Task<User> GetUserByIdAsync(Guid? id)
        {
            User? user = await _context.Users.Include(u => u.StravaProfile).Include(u => u.UserHeartRate)
                .Include(u => u.UserPower).FirstOrDefaultAsync(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }

        private int CalculateActivityPowerCurve(List<int> watts, int k)
        {
            int max = 0, curr_sum = 0;
            //int curr_start = 0, start = 0, end = 0;

            for (int i = 0; i < watts.Count; i++)
            {
                curr_sum += watts[i];

                if (i >= k)
                    curr_sum -= watts[i - k];

                if (i >= k - 1)
                {
                    if (curr_sum > max)
                    {
                        max = curr_sum;
                        // end = curr_start;
                        // start = curr_start - k + 1;
                    }
                }

                //curr_start++;
            }

            return max / k;
        }

        private TimeInHrZone CalculateTimeInHrZones(List<int> hr, Guid? userId, ProfileHeartRate? hrZones)
        {
            int Zone1 = 0;
            int Zone2 = 0;
            int Zone3 = 0;
            int Zone4 = 0;
            int Zone5 = 0;

            for (int i = 0; i < hr.Count; i++)
            {
                if (hr[i] >= hrZones.Zone5) Zone5++;
                if (hr[i] >= hrZones.Zone4 && hr[i] < hrZones.Zone5) Zone4++;
                if (hr[i] >= hrZones.Zone3 && hr[i] < hrZones.Zone4) Zone3++;
                if (hr[i] >= hrZones.Zone2 && hr[i] < hrZones.Zone3) Zone2++;
                if (hr[i] >= hrZones.Zone1 && hr[i] < hrZones.Zone2) Zone1++;
            }

            TimeInHrZone timeInHrZone = new TimeInHrZone
            {
                TimeInZ1 = Zone1,
                TimeInZ2 = Zone2,
                TimeInZ3 = Zone3,
                TimeInZ4 = Zone4,
                TimeInZ5 = Zone5
            };

            return timeInHrZone;
        }

        private TimeInPowerZone CalculateTimeInPowerZones(List<int> watts, Guid? userId, ProfilePower? powerZones)
        {
            int Zone1 = 0;
            int Zone2 = 0;
            int Zone3 = 0;
            int Zone4 = 0;
            int Zone5 = 0;
            int Zone6 = 0;
            int Zone7 = 0;

            for (int i = 0; i < watts.Count; i++)
            {
                if (watts[i] >= powerZones.Zone7) Zone7++;
                if (watts[i] >= powerZones.Zone6 && watts[i] < powerZones.Zone7) Zone6++;
                if (watts[i] >= powerZones.Zone5 && watts[i] < powerZones.Zone6) Zone5++;
                if (watts[i] >= powerZones.Zone4 && watts[i] < powerZones.Zone5) Zone4++;
                if (watts[i] >= powerZones.Zone3 && watts[i] < powerZones.Zone4) Zone3++;
                if (watts[i] >= powerZones.Zone2 && watts[i] < powerZones.Zone3) Zone2++;
                if (watts[i] >= powerZones.Zone1 && watts[i] < powerZones.Zone2) Zone1++;
            }

            TimeInPowerZone timeInPowerZone = new TimeInPowerZone
            {
                TimeInZ1 = Zone1,
                TimeInZ2 = Zone2,
                TimeInZ3 = Zone3,
                TimeInZ4 = Zone4,
                TimeInZ5 = Zone5,
                TimeInZ6 = Zone6,
                TimeInZ7 = Zone7,
            };

            return timeInPowerZone;
        }

        private async Task<string> UpdateTrainingLoad(Guid? userId)
        {
            List<TrainingLoads>? trainingLoads = _context.TrainingLoads
                .Where(tl => tl.UserId == userId)
                .OrderBy(tl => tl.Date)
                .ToList();

            List<TrainingLoads>? trainingLoadsToAdd = new List<TrainingLoads>();

            float alfa_lts = (float)2 / 43;
            float alfa_sts = (float)2 / 8;

            float prev_lts_hr = alfa_lts * trainingLoads[0].TrainingImpulse;
            float prev_sts_hr = alfa_sts * trainingLoads[0].TrainingImpulse;
            trainingLoads[0].LongTermStressHr = prev_lts_hr;
            trainingLoads[0].ShortTermStressHr = prev_sts_hr;

            float prev_lts = alfa_lts * trainingLoads[0].TrainingImpulse;
            float prev_sts = alfa_sts * trainingLoads[0].TrainingImpulse;
            trainingLoads[0].LongTermStress = prev_lts;
            trainingLoads[0].ShortTermStress = prev_sts;

            if (trainingLoads[0].TrainingLoad > 0)
            {
                prev_lts = alfa_lts * trainingLoads[0].TrainingLoad;
                prev_sts = alfa_sts * trainingLoads[0].TrainingLoad;
                trainingLoads[0].LongTermStress = prev_lts;
                trainingLoads[0].ShortTermStress = prev_sts;
            }

            float prev_lts_p = alfa_lts * trainingLoads[0].TrainingLoad;
            float prev_sts_p = alfa_sts * trainingLoads[0].TrainingLoad;
            trainingLoads[0].LongTermStressPower = prev_lts_p;
            trainingLoads[0].ShortTermStressPower = prev_sts_p;

            for (int i = 1; i < trainingLoads.Count; i++)
            {
                DateOnly prev_date = trainingLoads[i - 1].Date.AddDays(1);

                if (prev_date < trainingLoads[i].Date)
                {
                    while (prev_date != trainingLoads[i].Date)
                    {
                        float curr_lts = (1 - alfa_lts) * prev_lts;
                        float curr_sts = (1 - alfa_sts) * prev_sts;

                        float curr_lts_p = (1 - alfa_lts) * prev_lts_p;
                        float curr_sts_p = (1 - alfa_sts) * prev_sts_p;

                        float curr_lts_hr = (1 - alfa_lts) * prev_lts_hr;
                        float curr_sts_hr = (1 - alfa_sts) * prev_sts_hr;

                        trainingLoadsToAdd.Add(new TrainingLoads
                        {
                            Date = prev_date,

                            LongTermStress = curr_lts,
                            ShortTermStress = curr_sts,
                            StressBalance = curr_lts - curr_sts,

                            LongTermStressPower = curr_lts_p,
                            ShortTermStressPower = curr_sts_p,
                            StressBalancePower = curr_lts_p - curr_sts_p,

                            LongTermStressHr = curr_lts_hr,
                            ShortTermStressHr = curr_sts_hr,
                            StressBalanceHr = curr_lts_hr - curr_sts_hr,

                            UserId = (Guid)userId
                        });

                        prev_lts = curr_lts;
                        prev_sts = curr_sts;

                        prev_lts_hr = curr_lts_hr;
                        prev_sts_hr = curr_sts_hr;

                        prev_lts_p = curr_lts_p;
                        prev_sts_p = curr_sts_p;
                        prev_date = prev_date.AddDays(1);
                    }
                }

                trainingLoads[i].LongTermStress = 
                    alfa_lts * trainingLoads[i].TrainingImpulse + (1 - alfa_lts) * prev_lts;
                trainingLoads[i].ShortTermStress = 
                    alfa_sts * trainingLoads[i].TrainingImpulse + (1 - alfa_sts) * prev_sts;

                trainingLoads[i].LongTermStressHr =  
                    alfa_lts * trainingLoads[i].TrainingImpulse + (1 - alfa_lts) * prev_lts_hr;
                trainingLoads[i].ShortTermStressHr = 
                    alfa_sts * trainingLoads[i].TrainingImpulse + (1 - alfa_sts) * prev_sts_hr;
                trainingLoads[i].StressBalanceHr =
                    trainingLoads[i].LongTermStressHr - trainingLoads[i].ShortTermStressHr;

                if (trainingLoads[i].TrainingLoad > 0)
                {
                    trainingLoads[i].LongTermStress = 
                        alfa_lts * trainingLoads[i].TrainingLoad + (1 - alfa_lts) * prev_lts;
                    trainingLoads[i].ShortTermStress = 
                        alfa_sts * trainingLoads[i].TrainingLoad + (1 - alfa_sts) * prev_sts;
                }

                trainingLoads[i].StressBalance = trainingLoads[i].LongTermStress - trainingLoads[i].ShortTermStress;

                trainingLoads[i].LongTermStressPower =
                    alfa_lts * trainingLoads[i].TrainingLoad + (1 - alfa_lts) * prev_lts_p;
                trainingLoads[i].ShortTermStressPower = 
                    alfa_sts * trainingLoads[i].TrainingLoad + (1 - alfa_sts) * prev_sts_p;
                trainingLoads[i].StressBalancePower =
                    trainingLoads[i].LongTermStressPower - trainingLoads[i].ShortTermStressPower;

                prev_lts = trainingLoads[i].LongTermStress;
                prev_sts = trainingLoads[i].ShortTermStress;

                prev_lts_p = trainingLoads[i].LongTermStressPower;
                prev_sts_p = trainingLoads[i].ShortTermStressPower;

                prev_lts_hr = trainingLoads[i].LongTermStressHr;
                prev_sts_hr = trainingLoads[i].ShortTermStressHr;
            }

            foreach (var trainingLoad in trainingLoads)
            {
                _context.TrainingLoads.Update(trainingLoad);
            }

            _context.TrainingLoads.AddRange(trainingLoadsToAdd);
            _context.SaveChanges();

            return "updated";
        }

        private List<TrainingLoads> CalculateTrainingLoadToday(Guid? userId, List<TrainingLoads> trainingLoads)
        {
            List<TrainingLoads>? trainingLoadsToAdd = new List<TrainingLoads>();

            float alfa_lts = (float)2 / 43;
            float alfa_sts = (float)2 / 8;
            var last = trainingLoads.Last();

            var prev_lts = last.LongTermStress;
            var prev_sts = last.ShortTermStress;
 
            var prev_lts_hr = last.LongTermStressHr;
            var prev_sts_hr = last.ShortTermStressHr;
            
            var prev_lts_pwr = last.LongTermStressPower;
            var prev_sts_pwr = last.ShortTermStressPower;

            var date = last.Date.AddDays(1);
            while (date != DateOnly.FromDateTime(DateTime.Today).AddDays(7))
            {
                float curr_lts = (1 - alfa_lts) * prev_lts;
                float curr_sts = (1 - alfa_sts) * prev_sts;
                float curr_sb = curr_lts - curr_sts;
                
                float curr_lts_hr = (1 - alfa_lts) * prev_lts_hr;
                float curr_sts_hr = (1 - alfa_sts) * prev_sts_hr;
                float curr_sb_hr = curr_lts_hr - curr_sts_hr;
                
                float curr_lts_pwr = (1 - alfa_lts) * prev_lts_pwr;
                float curr_sts_pwr = (1 - alfa_sts) * prev_sts_pwr;
                float curr_sb_pwr = curr_lts_pwr - curr_sts_pwr;
                
                trainingLoadsToAdd.Add(new TrainingLoads {
                    UserId = (Guid)userId,
                    Date = date,
                    TrainingImpulse = 0,
                    TrainingLoad = 0,
                    LongTermStress = curr_lts,
                    ShortTermStress = curr_sts,
                    StressBalance = curr_sb,
                    LongTermStressHr = curr_lts_hr,
                    ShortTermStressHr = curr_sts_hr,
                    StressBalanceHr = curr_sb_hr,
                    LongTermStressPower = curr_lts_pwr,
                    ShortTermStressPower = curr_sts_pwr,
                    StressBalancePower = curr_sb_pwr
                });

                prev_lts = curr_lts;
                prev_sts = curr_sts;
                
                prev_lts_hr = curr_lts_hr;
                prev_sts_hr = curr_sts_hr;
                
                prev_lts_pwr = curr_lts_pwr;
                prev_sts_pwr = curr_sts_pwr;

                date = date.AddDays(1);
            }
            
            _context.TrainingLoads.AddRange(trainingLoadsToAdd);
            _context.SaveChanges();
            
            List<TrainingLoads>? res = _context.TrainingLoads
                .Where(tl => tl.UserId == userId)
                .OrderBy(tl => tl.Date)
                .ToList();
            
            return res;
        }
    }
}

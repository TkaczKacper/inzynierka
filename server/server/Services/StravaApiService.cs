using server.Responses.Strava.ActivityStreams;
using server.Responses.Strava;
using server.Models.Activity;

namespace server.Services
{
    public interface IStravaApiService
    {
        Task<ActivityDetailsResponse?> GetDetailsById(long id, HttpClient httpClient);
        Task<ActivityStreams> GetStreamsById(long id, HttpClient httpClient);
    }

    public class StravaApiService : IStravaApiService
    {

        public async Task<ActivityDetailsResponse?> GetDetailsById(long id, HttpClient stravaClient)
        {
            try
            {
                using HttpResponseMessage response = await stravaClient.GetAsync($"activities/{id}");
                var detailsResponse = await response.Content.ReadFromJsonAsync<ActivityDetailsResponse>();

                return detailsResponse;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"error: {ex.Message}");
                Console.WriteLine("Strava API limit reached. Try again later."); 
            }
            return null;
        }

        public async Task<ActivityStreams> GetStreamsById(long id, HttpClient stravaClient)
        {
            try
            {
                using HttpResponseMessage response = await stravaClient.GetAsync($"activities/{id}/streams?keys=time,distance,latlng,altitude,velocity_smooth,heartrate,cadence,watts,temp,moving,grade_smooth&series_type=time");
                var streamsResponse = await response.Content.ReadFromJsonAsync<List<Streams>>();
                
                List<int> time = new List<int>();
                List<float> distance = new List<float>();
                List<float> velocity = new List<float>();
                List<int> watts = new List<int>();
                List<int> cadence = new List<int>();
                List<int> hr = new List<int>();
                List<int> temp = new List<int>();
                List<float> altitude = new List<float>();
                List<float> grade = new List<float>();
                List<double> lat = new List<double>();
                List<double> lng = new List<double>();
                
                //TODO zmienic stream processing bo sie ranodmowo w jednej aktywnosci na stravie pierwszy odczyt jednego streamu to null zamiast 0 XDDDDDDDDDDD
                for (int j = 0; j < streamsResponse[0].data?.Length; j++)
                {
                    foreach (var stream in streamsResponse)
                    {
                        if (j < stream.data.Length)
                        {
                            if (stream.type == "latlng")
                            {
                                string[] parts = stream.data[j].ToString().Trim('[', ']').Split(',');
                                lat.Add(double.Parse(parts[0]));
                                lng.Add(double.Parse(parts[1]));
                            }

                            if (stream.type == "grade_smooth")
                            {
                                grade.Add(float.Parse(stream.data[j].ToString()));
                            }

                            if (stream.type == "altitude")
                            {
                                altitude.Add(float.Parse(stream.data[j].ToString()));
                            }

                            if (stream.type == "temp")
                            {
                                temp.Add(int.Parse(stream.data[j].ToString()));
                            }

                            if (stream.type == "time")
                            {
                                time.Add(int.Parse(stream.data[j].ToString()));
                            }

                            if (stream.type == "watts")
                            {
                                watts.Add(int.Parse(stream.data[j].ToString()));
                            }
                            
                            if (stream.type == "cadence")
                            {
                                cadence.Add(int.Parse(stream.data[j].ToString()));
                            }
                            
                            if (stream.type == "heartrate")
                            {
                                hr.Add(int.Parse(stream.data[j].ToString()));
                            }

                            if (stream.type == "distance")
                            {
                                distance.Add(float.Parse(stream.data[j].ToString()));
                            }

                            if (stream.type == "velocity_smooth")
                            {
                                velocity.Add(float.Parse(stream.data[j].ToString()));
                            }
                        }
                    }
                }

                ActivityStreams streamsToProcess = new ActivityStreams
                {
                    TimeStream = time,
                    Distance = distance,
                    Velocity = velocity,
                    Watts = watts,
                    Cadence = cadence,
                    HeartRate = hr,
                    Temperature = temp,
                    Altitude = altitude,
                    GradeSmooth = grade,
                    Lat = lat,
                    Lng = lng,
                };

                return ProcessStreams(streamsToProcess);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"error: {ex.Message}");
                Console.WriteLine("Strava API limit reached. Try again later."); 
            }
            return null;
        }

        public ActivityStreams ProcessStreams(ActivityStreams streams)
        {
            List<int> time_processed = new List<int>();
            List<float> distance_processed = new List<float>();
            List<float> velocity_processed = new List<float>();
            List<int> watts_processed = new List<int>();
            List<int> cadence_processed = new List<int>();
            List<int> hr_processed = new List<int>();
            List<int> temp_processed = new List<int>();
            List<float> altitude_processed = new List<float>();
            List<float> grade_processed = new List<float>();
            List<double> lat_processed = new List<double>();
            List<double> lng_processed = new List<double>();
            List<bool> moving = new List<bool>();
            List<bool> havedata = new List<bool>();

            int sec = 1;
            List<int> time = streams.TimeStream;
            List<float> distance = streams.Distance;
            List<float> velocity = streams.Velocity;
            List<int> watts = streams.Watts;
            List<int> cadence = streams.Cadence;
            List<int> hr = streams.HeartRate;
            List<int> temp = streams.Temperature;
            List<float> altitude = streams.Altitude;
            List<float> grade = streams.GradeSmooth;
            List<double> lat = streams.Lat;
            List<double> lng = streams.Lng;


            for (int i = 1; i < time.Count - 1; i++)
            {
                if (time[i - 1] + 1 == time[i])
                {
                    time_processed.Add(sec);
                    moving.Add(true);
                    havedata.Add(true);

                    if (distance.Count > 0)
                    {
                        distance_processed.Add(distance[i]);
                        velocity_processed.Add(velocity[i]);
                    }
                    
                    if (altitude.Count > 0)
                    {
                        altitude_processed.Add(altitude[i]);
                        grade_processed.Add(grade[i]);
                    }

                    if (lat.Count > 0)
                    {
                        lat_processed.Add(lat[i]);
                        lng_processed.Add(lng[i]);
                    }

                    if (watts.Count > 0)
                        watts_processed.Add(watts[i]);

                    if (cadence.Count > 0)
                        cadence_processed.Add(cadence[i]);
                    
                    if (hr.Count > 0)
                        hr_processed.Add(hr[i]);

                    if (temp.Count > 0)
                        temp_processed.Add(temp[i]);

                    sec++;
                }
                else
                {
                    int j = 0;
                    while (time[i - 1] + j != time[i])
                    {
                        if (time[i] - time[i - 1] > 3)
                        {
                            time_processed.Add(sec);
                            havedata.Add(false);
                            moving.Add(false);

                            if (distance.Count > 0)
                            {
                                distance_processed.Add(0);
                                velocity_processed.Add(0);
                            }
                            
                            if (altitude.Count > 0)
                            {
                                grade_processed.Add(0);
                                altitude_processed.Add(0);
                            }

                            if (lat.Count > 0)
                            {
                                lat_processed.Add(lat[i]);
                                lng_processed.Add(lng[i]);
                            }

                            if (watts.Count > 0)
                                watts_processed.Add(0);

                            if (cadence.Count > 0)
                                cadence_processed.Add(0);

                            if (hr.Count > 0)
                                hr_processed.Add(0);

                            if (temp.Count > 0)
                                temp_processed.Add(0);
                            sec++;
                            j++;
                        }
                        else
                        {
                            time_processed.Add(sec);
                            havedata.Add(false);
                            if (distance.Count > 0)
                            {
                                distance_processed.Add((distance[i - 1] + distance[i]) / 2);
                                velocity_processed.Add((velocity[i - 1] + velocity[i]) / 2);
                            }

                            if (altitude.Count > 0)
                            {
                                altitude_processed.Add((altitude[i - 1] + altitude[i]) / 2);
                                grade_processed.Add((grade[i - 1] + grade[i]) / 2);
                            }
                            
                            if (lat.Count > 0)
                            {
                                lat_processed.Add(lat[i]);
                                lng_processed.Add(lng[i]);
                            }

                            if (watts.Count > 0)
                                watts_processed.Add((watts[i] + watts[i + 1]) / 2);

                            if (cadence.Count > 0)
                                cadence_processed.Add((cadence[i] + cadence[i + 1]) / 2);

                            if (hr.Count > 0)
                                hr_processed.Add((hr[i] + hr[i + 1]) / 2);

                            if (temp.Count > 0)
                                temp_processed.Add((temp[i] + temp[i + 1]) / 2);
                            sec++;
                            j++;
                        }
                    }
                }

            }
            



            int lastIdx = time.Count - 1;
            time_processed.Add(sec);
            moving.Add(true);
            havedata.Add(true);
            
                            
            if (distance.Count > 0)
            {
                distance_processed.Add(0);
                velocity_processed.Add(0);
            }
            
            if (altitude.Count > 0)
            {
                altitude_processed.Add(altitude[lastIdx]);
                grade_processed.Add(grade[lastIdx]);
            }
            
            if (lat.Count > 0)
            {
                lat_processed.Add(lat[lastIdx]);
                lng_processed.Add(lng[lastIdx]);
            }

            if (watts.Count > 0)
                watts_processed.Add(watts[lastIdx]);

            if (cadence.Count > 0)
                cadence_processed.Add(cadence[lastIdx]);

            if (hr.Count > 0)
                hr_processed.Add(hr[lastIdx]);

            if (temp.Count > 0)
                temp_processed.Add(temp[lastIdx]);

            ActivityStreams streamsProcessed = new ActivityStreams
            {
                TimeStream = time_processed,
                Distance = distance_processed,
                Velocity = velocity_processed,
                Watts = watts_processed,
                Cadence = cadence_processed,
                HeartRate = hr_processed,
                Temperature = temp_processed,
                Altitude = altitude_processed,
                GradeSmooth = grade_processed,
                Lat = lat_processed,
                Lng = lng_processed,
                HaveData = havedata
            };
            
            return streamsProcessed;
        }
    }
}

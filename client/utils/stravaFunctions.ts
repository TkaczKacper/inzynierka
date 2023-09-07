import axios from "axios";
const client_id = process.env.NEXT_PUBLIC_STRAVA_CLIENT_ID;
const client_secret = process.env.NEXT_PUBLIC_STRAVA_CLIENT_SECRET;
import Cookies from "universal-cookie";

const cookies = new Cookies();
const stravaApiUrl = "https://www.strava.com/api/v3";

const updateProfileInfo = async (profile: any, refresh_token: string) => {
   try {
      const response = await axios.post(
         `http://localhost:5264/strava/profile/update`,
         {
            StravaRefreshToken: refresh_token,
            ProfileID: profile.id,
            Username: profile.username,
            FirstName: profile.firstname,
            LastName: profile.lastname,
            Sex: profile.sex,
            Bio: profile.bio,
            ProfileAvatar: profile.profile,
            Country: profile.country,
            State: profile.state,
            City: profile.city,
            Weight: profile.weight,
            ProfileCreatedAt: new Date(profile.created_at),
         },
         {
            headers: {
               Authorization: cookies.get("jwtToken"),
            },
         }
      );
      return response;
   } catch (error) {
      console.log(error);
   }
};

export const cleanUpAuthToken = (url: string) => {
   try {
      return url.split("&")[1].slice(5);
   } catch {}
};

export const getToken = async (authToken: string) => {
   try {
      const response = await axios.post(
         `https://www.strava.com/api/v3/oauth/token?client_id=${client_id}&client_secret=${client_secret}&code=${authToken}&grant_type=authorization_code`
      );
      cookies.set("strava_refresh_token", response.data.refresh_token, {
         path: "/",
      });
      cookies.set("strava_access_token", response.data.access_token, {
         path: "/",
         expires: new Date(response.data.expires_at * 1000),
      });
      updateProfileInfo(response.data.athlete, response.data.refresh_token);

      return response.data;
   } catch (err) {
      console.log(err);
   }
};

export const getUserActivites = async () => {
   try {
      const response = await axios.get(`${stravaApiUrl}/athlete/activities`, {
         headers: {
            Authorization: `Bearer ${cookies.get("strava_access_token")}`,
         },
      });
      return response;
   } catch (error) {
      console.log(error);
   }
};

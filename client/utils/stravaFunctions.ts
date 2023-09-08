import axios from "axios";
import Cookies from "universal-cookie";

const client_id = process.env.NEXT_PUBLIC_STRAVA_CLIENT_ID;
const client_secret = process.env.NEXT_PUBLIC_STRAVA_CLIENT_SECRET;
const strava_url = process.env.NEXT_PUBLIC_STRAVA_API_URL;

const cookies = new Cookies();

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
         `${strava_url}/oauth/token?client_id=${client_id}&client_secret=${client_secret}&code=${authToken}&grant_type=authorization_code`
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

export const refreshToken = async (token: string | undefined) => {
   try {
      const response = await fetch(
         `${strava_url}/oauth/token?client_id=${client_id}&client_secret=${client_secret}&grant_type=refresh_token&refresh_token=${token}`,
         { method: "POST" }
      ).then((res) => res.json());
      cookies.set("strava_access_token", response.access_token, {
         path: "/",
         expires: new Date(response.expires_at * 1000),
      });
      return response;
   } catch (err) {
      console.log(err);
   }
};

export const getUserActivites = async () => {
   try {
      const response = await axios.get(`${strava_url}/athlete/activities`, {
         headers: {
            Authorization: `Bearer ${cookies.get("strava_access_token")}`,
         },
      });
      return response;
   } catch (error) {
      console.log(error);
   }
};

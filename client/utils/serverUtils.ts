import { Activity } from "@/app/profile/connections/page";
import axios from "axios";
import Cookies from "universal-cookie";

const cookies = new Cookies();
const backend_url = process.env.NEXT_PUBLIC_BACKEND_API_URL;

export const updateProfileInfo = async (
   profile: any,
   refresh_token: string
) => {
   try {
      const response = await axios.post(
         `${backend_url}/strava/profile/update`,
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

export const getActivitiesDetails = async (activities: Activity[]) => {
   const activitiesId: number[] = [];
   activities.map((activity) => {
      activitiesId.push(activity.id);
   });

   try {
      const response = await axios.post(
         `${backend_url}/strava/get-activity-details`,
         activitiesId,
         {
            withCredentials: true,
            headers: {
               Authorization: cookies.get("jwtToken"),
            },
         }
      );
      return response;
   } catch (err) {
      console.log(err);
   }
};

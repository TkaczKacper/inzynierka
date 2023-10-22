import { Activity } from "@/app/profile/connections/page";
import axios from "axios";
import { getCookie } from "cookies-next";

const backend_url = process.env.NEXT_PUBLIC_BACKEND_API_URL;

export const updateProfileInfo = async (
  profile: any,
  refresh_token: string,
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
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
    return response;
  } catch (error) {
    console.log(error);
  }
};

export const getAthleteActivities = async () => {
  try {
    const response = await axios.get(
      `${backend_url}/strava/profile/get-activities`,
      {
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
    console.log(response);
    return response;
  } catch (err) {
    console.log(err);
  }
};
export const getActivitiesDetails = async (activitiesId: number[]) => {
  try {
    const response = await axios.post(
      `${backend_url}/strava/get-activity-details`,
      activitiesId,
      {
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
    console.log(response);
    const xd = await axios.get(`${backend_url}/strava/process-data`, {
      withCredentials: true,
      headers: {
        Authorization: getCookie("jwtToken"),
      },
    });
    console.log(xd);
    return response;
  } catch (err) {
    console.log(err);
  }
};

export const updateFtp = async (ftp: number) => {
  try {
    const response = await axios.post(
      `${backend_url}/strava/profile/power-update`,
      {
        FTP: ftp,
      },
      {
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
    console.log(response);
    return response;
  } catch (error) {
    console.log(error);
  }
};

export const updateHr = async (hrRest: number, hrMax: number) => {
  try {
    const response = await axios.post(
      `${backend_url}/strava/profile/hr-update`,
      {
        HrRest: hrRest,
        HrMax: hrMax,
      },
      {
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
    console.log(response);
    return response;
  } catch (error) {
    console.log(error);
  }
};

export const getAthleteStats = async () => {
  try {
    const res = await axios.get(`${backend_url}/strava/get-athlete-stats`, {
      withCredentials: true,
      headers: {
        Authorization: getCookie("jwtToken"),
      },
    });
    console.log(res);
    return res;
  } catch (error) {
    console.log(error);
  }
};

export const getSyncedActivities = async () => {
  try {
    const res = await axios.get(`${backend_url}/strava/get-synced-activites`, {
      withCredentials: true,
      headers: {
        Authorization: getCookie("jwtToken"),
      },
    });
    console.log(res);
    return res;
  } catch (error) {
    console.log(error);
  }
};

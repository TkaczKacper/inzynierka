import { Activity } from "@/app/profile/connections/page";
import axios from "axios";
import { getCookie } from "cookies-next";
import { hrZonesType } from "@/app/profile/heartrate-management/page";
import { powerZonesType } from "@/app/profile/power-management/page";

//TODO podzielic na osobne pliki

const backend_url = process.env.NEXT_PUBLIC_BACKEND_API_URL;

export const updateProfileInfo = async (
  profile: any,
  refresh_token: string,
) => {
  try {
    const response = await axios.post(
      `${backend_url}/profile/update`,
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

export const getAthleteActivities = async (
  lastActivity: Date | string | null,
  perPage: number | null,
) => {
  try {
    let requestUrl = `${backend_url}/profile/get-activities`;
    if (lastActivity && perPage) {
      requestUrl = `${backend_url}/profile/get-activities?lastActivityDate=${lastActivity}&perPage=${perPage}`;
    }
    const response = await axios.get(requestUrl, {
      withCredentials: true,
      headers: {
        Authorization: getCookie("jwtToken"),
      },
    });
    console.log(response);
    return response;
  } catch (err) {
    console.log(err);
  }
};

export const getAthleteActivitiesPeriod = async (
  month: number,
  yearOffset: number,
) => {
  try {
    return await axios.get(
      `${backend_url}/profile/get-activities-period?month=${month}&yearOffset=${yearOffset}`,
      {
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
  } catch (err) {
    console.log(err);
  }
};

export const getActivitiesDetails = async (activitiesId: number[]) => {
  try {
    const response = await axios.post(
      `${backend_url}/process-activity/get-activity-details`,
      activitiesId,
      {
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
    console.log(response);
    const xd = await axios.get(`${backend_url}/process-activity/process-data`, {
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
      `${backend_url}/profile/power-update`,
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

export const updatePowerZones = async (powerZones: powerZonesType) => {
  try {
    const res = await axios.post(
      `${backend_url}/profile/power-update`,
      powerZones,
      {
        headers: { Authorization: getCookie("jwtToken") },
      },
    );
    console.log(res);
    return res;
  } catch (error) {
    console.log(error);
  }
};

export const updateHr = async (
  hrRest: number,
  hrMax: number,
  ltHr: number | undefined,
) => {
  try {
    const response = await axios.post(
      `${backend_url}/profile/hr-update`,
      {
        HrRest: hrRest,
        HrMax: hrMax,
        ltHr: ltHr,
        SetAutoZones: true,
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

export const updateHrZones = async (hrZones: hrZonesType) => {
  try {
    const res = await axios.post(`${backend_url}/profile/hr-update`, hrZones, {
      headers: { Authorization: getCookie("jwtToken") },
    });
    console.log(res);
    return res;
  } catch (error) {
    console.log(error);
  }
};

export const getAthleteMonthlyStats = async (yearOffset: number) => {
  try {
    const res = await axios.get(
      `${backend_url}/profile/get-athlete-monthly-stats?yearOffset=${yearOffset}`,
      {
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
    console.log(res);
    return res;
  } catch (error) {
    console.log(error);
  }
};

export const getSyncedActivities = async () => {
  try {
    const res = await axios.get(
      `${backend_url}/profile/get-synced-activities`,
      {
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
    console.log(res);
    return res;
  } catch (error) {
    console.log(error);
  }
};

export const getActivityDataById = async (activityId: number) => {
  try {
    return await axios.get(
      `${backend_url}/activity/get-activity-by-id/${activityId}`,
      {
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
  } catch (error) {
    console.log(error);
  }
};

export const deleteActivityDataById = async (activityId: number) => {
  try {
    return await axios.delete(
      `${backend_url}/activity/delete-activity-by-id/${activityId}`,
      {
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
  } catch (error) {
    console.log(error);
  }
};

export const getUserTrainingLoad = async () => {
  try {
    return await axios.get(`${backend_url}/profile/get-user-training-load`, {
      withCredentials: true,
      headers: {
        Authorization: getCookie("jwtToken"),
      },
    });
  } catch (error) {
    console.log(error);
  }
};

export const deleteHrEntry = async (id: number) => {
  try {
    return await axios.delete(`${backend_url}/profile/hr-delete-entry/${id}`, {
      withCredentials: true,
      headers: {
        Authorization: getCookie("jwtToken"),
      },
    });
  } catch (error) {
    console.log(error);
  }
};

export const deletePowerEntry = async (id: number) => {
  try {
    return await axios.delete(
      `${backend_url}/profile/power-delete-entry/${id}`,
      {
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
  } catch (error) {
    console.log(error);
  }
};

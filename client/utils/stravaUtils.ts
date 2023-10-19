import axios from "axios";
import { updateProfileInfo } from "./serverUtils";
import { deleteCookie, getCookie, setCookie } from "cookies-next";

const client_id = process.env.NEXT_PUBLIC_STRAVA_CLIENT_ID;
const client_secret = process.env.NEXT_PUBLIC_STRAVA_CLIENT_SECRET;
const strava_url = process.env.NEXT_PUBLIC_STRAVA_API_URL;

export const cleanUpAuthToken = (url: string) => {
  try {
    return url.split("&")[1].slice(5);
  } catch {}
};

export const getToken = async (authToken: string) => {
  try {
    const response = await axios.post(
      `${strava_url}/oauth/token?client_id=${client_id}&client_secret=${client_secret}&code=${authToken}&grant_type=authorization_code`,
    );
    console.log(response);
    setCookie("strava_refresh_token", response.data.refresh_tokenm, {
      path: "/",
    });
    setCookie("strava_access_token", response.data.access_token, {
      path: "/",
      expires: new Date(response.data.expires_at * 1000),
    });
    const profileData = await updateProfileInfo(
      response.data.athlete,
      response.data.refresh_token,
    );
    console.log(profileData);
    return response.data;
  } catch (err) {
    console.log(err);
  }
};

export const refreshToken = async (token: string | undefined) => {
  try {
    const response = await fetch(
      `${strava_url}/oauth/token?client_id=${client_id}&client_secret=${client_secret}&grant_type=refresh_token&refresh_token=${token}`,
      { method: "POST" },
    ).then((res) => res.json());
    setCookie("strava_access_token", response.access_token, {
      path: "/",
      expires: new Date(response.expires_at * 1000),
    });
    return response;
  } catch (err) {
    console.log(err);
  }
};

export const deauthorize = async (token: string) => {
  try {
    const response = await axios.post(
      `${strava_url}/oauth/deauthorize`,
      {
        token,
      },
      {
        headers: {
          Authorization: `Bearer ${getCookie("strava_access_token")}`,
        },
      },
    );
    if (response.status === 200) {
      deleteCookie("strava_access_token");
      deleteCookie("strava_refresh_token");
    }
    console.log("Disconnected.");
    return response;
  } catch (error) {}
};

export const getAuthenticatedAthlete = async () => {
  try {
    const response = await axios.get(`${strava_url}/athlete`, {
      headers: {
        Authorization: `Bearer ${getCookie("strava_access_token")}`,
      },
    });
    return response;
  } catch (error) {}
};

export const getUserActivites = async (page: number) => {
  try {
    const response = await axios.get(
      `${strava_url}/athlete/activities?page=${page}&per_page=100`,
      {
        headers: {
          Authorization: `Bearer ${getCookie("strava_access_token")}`,
        },
      },
    );
    return response;
  } catch (error) {
    console.log(error);
  }
};

export const getActivityById = async (id: number) => {
  try {
    const response = await axios.get(`${strava_url}/activities/${id}`, {
      headers: {
        Authorization: `Bearer ${getCookie("strava_access_token")}`,
      },
    });
    return response;
  } catch (err) {
    console.log(err);
  }
};

export const getStreams = async (id: number) => {
  try {
    const response = await axios.get(
      `${strava_url}/activities/${id}/streams?keys=time,distance,latlng,altitude,velocity_smooth,heartrate,cadence,watts,temp,moving,grade_smooth&series_type=time`,
      {
        headers: {
          Authorization: `Bearer ${getCookie("strava_access_token")}`,
        },
      },
    );
    return response;
  } catch (err) {
    console.log(err);
  }
};

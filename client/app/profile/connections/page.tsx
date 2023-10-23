"use client";
import { getActivitiesDetails, getSyncedActivities } from "@/utils/serverUtils";
import {
  cleanUpAuthToken,
  getActivityById,
  getAuthenticatedAthlete,
  getStreams,
  getToken,
  deauthorize,
} from "@/utils/stravaUtils";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { getCookie } from "cookies-next";
import { useLocalStorage } from "@/hooks/useLocalStorage";
import Link from "next/link";

const client_id = process.env.NEXT_PUBLIC_STRAVA_CLIENT_ID;
const redirect_uri = "http://localhost:3000/profile/connections";
const scope = "read,activity:read_all";

export type Activity = {
  id: number;
  title: string;
  date: Date;
  duration: string;
  distance: number;
};

type Athlete = {
  id: number;
  firstname: string;
  lastname: string;
};

const page = () => {
  const router = useRouter();
  const [connectedAthlete, setConnectedAthlete] = useState<Athlete>();
  const [syncedActivities, setSyncedActivities] = useLocalStorage(
    "syncedActivities",
    [],
  );
  const [latestActivity, setLatestActivity] = useLocalStorage(
    "latestActivity",
    0,
  );
  console.log(connectedAthlete);
  useEffect(() => {
    const getSyncedActivitiesData = async () => {
      const response = await getSyncedActivities();

      setSyncedActivities(response?.data.syncedActivitiesId);
      setLatestActivity(
        new Date(response?.data.latestActivityDateTime).getTime() / 1000,
      );
    };
    getSyncedActivitiesData();
  }, []);
  const stravaAuth = () => {
    router.push(
      `https://www.strava.com/oauth/authorize?client_id=${client_id}&response_type=code&redirect_uri=${redirect_uri}&approval_prompt=force&scope=${scope}`,
    );
  };

  useEffect(() => {
    const authenticate = async () => {
      try {
        const stravaAuthToken = cleanUpAuthToken(location.search);
        if (stravaAuthToken) {
          const tokens = await getToken(stravaAuthToken);
          console.log(tokens);
          setConnectedAthlete({
            id: tokens.athlete.id,
            firstname: tokens.athlete.firstname,
            lastname: tokens.athlete.lastname,
          });
        }

        router.push(redirect_uri);
      } catch (err) {
        console.log(err);
      }
    };
    authenticate();
  }, []);

  useEffect(() => {
    const getConnectedProfile = async () => {
      const response = await getAuthenticatedAthlete();
      setConnectedAthlete({
        id: response?.data.id,
        firstname: response?.data.firstname,
        lastname: response?.data.lastname,
      });
    };
    getConnectedProfile();
  }, []);

  const disconnect = async () => {
    const token = getCookie("strava_access_token");
    const response = await deauthorize(typeof token === "string" ? token : "");
    setConnectedAthlete(undefined);
    return response;
  };

  return (
    <div>
      <h1>Connections</h1>
      {connectedAthlete?.id != undefined ? (
        <div>
          <h3>
            Connected as{" "}
            <a href={`https://www.strava.com/athletes/${connectedAthlete.id}`}>
              {connectedAthlete.firstname + " " + connectedAthlete.lastname}
            </a>
          </h3>
          <button onClick={disconnect}>Disconnect</button>
          <div>
            <Link href={"/profile/connections/import"}>
              Import rides from Strava.
            </Link>
          </div>
          <div>
            <Link href={"/profile/connections/quick-import"}>
              Import only latest rides from Strava.
            </Link>
          </div>
        </div>
      ) : (
        <div>
          Strava <button onClick={stravaAuth}>connect</button>
        </div>
      )}
    </div>
  );
};

export default page;

"use client";
import {
   cleanUpAuthToken,
   getToken,
   getUserActivites,
} from "@/utils/stravaFunctions";
import { useRouter, usePathname } from "next/navigation";
import { useEffect } from "react";

const client_id = process.env.NEXT_PUBLIC_STRAVA_CLIENT_ID;
const redirect_uri = "http://localhost:3000/profile/connections";
const scope = "read,activity:read_all";

const page = () => {
   const router = useRouter();

   const stravaAuth = () => {
      router.push(
         `https://www.strava.com/oauth/authorize?client_id=${client_id}&response_type=code&redirect_uri=${redirect_uri}&approval_prompt=force&scope=${scope}`
      );
   };

   useEffect(() => {
      const authenticate = async () => {
         try {
            const stravaAuthToken = cleanUpAuthToken(location.search);
            if (stravaAuthToken) {
               const tokens = await getToken(stravaAuthToken);
               console.log(tokens);
            }

            router.push(redirect_uri);
         } catch (err) {
            console.log(err);
         }
      };
      authenticate();
   }, []);
   const userActivities = async () => {
      const data = await getUserActivites();
      console.log(data);
   };
   return (
      <div>
         <h1>Connections</h1>
         <div>
            Strava{" "}
            {/* <a href="http://www.strava.com/oauth/authorize?client_id=111791&response_type=code&redirect_uri=http://localhost:3000/profile/connections&approval_prompt=force&scope=read,activity:read_all">
               connect
            </a> */}
            <button onClick={stravaAuth}>connect</button>
         </div>
         <div>
            <button onClick={userActivities}>Import</button>rides from Strava
         </div>
      </div>
   );
};

export default page;

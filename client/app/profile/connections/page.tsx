"use client";
import { getActivitiesDetails } from "@/utils/serverUtils";
import {
   cleanUpAuthToken,
   getActivityById,
   getStreams,
   getToken,
   getUserActivites,
} from "@/utils/stravaUtils";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

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

const page = () => {
   const router = useRouter();
   const [activities, setActivities] = useState<Activity[]>([]);
   const [sendVisibility, setSendVisibility] = useState<boolean>(false);

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

   const get = async () => {
      const data = await getActivityById(7521736676);
      console.log(data);
   };

   const getSterams = async () => {
      const data = await getStreams(9795708116);
      console.log(data);
   };

   const userActivities = async (page: number) => {
      const data = await getUserActivites(page);
      console.log(data);
      return data;
   };

   const getAllActivities = async () => {
      let page_number = 10;
      while (true) {
         const activities_temp: any = await userActivities(page_number);
         if (!activities_temp.data[0]) break;
         console.log(activities_temp.data);
         activities_temp.data.map((element: any) => {
            console.log(element);
            setActivities((prev) => [
               ...prev,
               {
                  id: element.id,
                  title: element.name,
                  date: new Date(element.start_date),
                  duration: element.elapsed_time,
                  distance: element.distance,
               },
            ]);
         });
         page_number++;
         setSendVisibility(true);
      }
   };

   const importActivities = async () => {
      const response = await getActivitiesDetails(activities);
      console.log(response);
   };

   return (
      <div>
         <h1>Connections</h1>
         <div>
            Strava <button onClick={stravaAuth}>connect</button>
         </div>
         <div>
            <button onClick={() => getAllActivities()}>Import</button>rides from
            Strava
         </div>
         <button onClick={get}>get1</button>
         <button onClick={getSterams}>getSterams</button>
         <div>
            {sendVisibility ? (
               <button onClick={importActivities}>import</button>
            ) : null}
            <div>
               {activities.map((activity, index) => {
                  return (
                     <div key={index}>
                        <h2>
                           {index + 1}. id: {activity.id}, title:{" "}
                           {activity.title}
                        </h2>
                     </div>
                  );
               })}
            </div>
         </div>
      </div>
   );
};

export default page;

"use client";

import { useRef, useState } from "react";
import { Activity } from "@/app/profile/connections/page";
import { getUserActivites } from "@/utils/stravaUtils";
import { getActivitiesDetails } from "@/utils/serverUtils";
import Link from "next/link";
import { useLocalStorage } from "@/hooks/useLocalStorage";

const page = () => {
  const [syncedActivities, setSyncedActivities] = useLocalStorage(
    "syncedActivities",
    [],
  );
  const [activities, setActivities] = useState<Activity[]>([]);
  const [fetching, setFetching] = useState<boolean>(false);
  const stopFetching = useRef(false);
  const getAllActivities = async () => {
    setFetching(true);
    let page_number = 1;
    while (!stopFetching.current) {
      console.log(stop);
      console.log(fetching);
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
    }
    setFetching(false);
  };

  const userActivities = async (page: number) => {
    const data = await getUserActivites(page, 0);
    console.log(data);
    return data;
  };
  const importActivities = async () => {
    const response = await getActivitiesDetails(activities);
    console.log(response);
  };

  return (
    <div>
      <h1>Strava import</h1>
      <Link href={"/profile/connections"}>back to connections page</Link>
      <div>
        <p>Select rides to import from Strava and click the Import button.</p>
        <p>
          If the ride already exists in database, it is unavailable for
          importing and is grayed out.
        </p>
      </div>
      <div>
        {fetching ? (
          <div>
            <h2>Is {activities.length} latest activities enough?</h2>
            <button
              onClick={() => {
                stopFetching.current = true;
              }}
            >
              Press to stop fetching.
            </button>
          </div>
        ) : (
          <div>
            {activities.length === 0 ? (
              <button onClick={getAllActivities}>Start importing.</button>
            ) : (
              <button onClick={importActivities}>import</button>
            )}
            {activities.map((activity, index) => {
              return (
                <div key={index}>
                  <h2>
                    {index + 1}. id: {activity.id}, title: {activity.title}
                  </h2>
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
};

export default page;

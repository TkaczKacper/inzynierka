"use client";

import Link from "next/link";
import { useLocalStorage } from "@/hooks/useLocalStorage";
import { useState } from "react";
import { Activity } from "@/app/profile/connections/page";
import { getUserActivites } from "@/utils/stravaUtils";
import { getActivitiesDetails } from "@/utils/serverUtils";

const page = () => {
  const [latestActivity, setLatestActivity] = useLocalStorage(
    "latestActivity",
    0,
  );
  const [fetching, setFetching] = useState<boolean>(false);
  const [activities, setActivities] = useState<Activity[]>([]);

  const getRecentActivities = async () => {
    setFetching(true);
    let page_number = 1;
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
    }
    setFetching(false);
  };

  const userActivities = async (page: number) => {
    const data = await getUserActivites(page, latestActivity);
    console.log(data);
    return data;
  };
  const importActivities = async () => {
    const response = await getActivitiesDetails(activities);
    console.log(response);
  };

  return (
    <div>
      <h1>Strava quick import.</h1>
      <Link href={"/profile/connections"}>back to connections page</Link>
      <p>
        Select rides to import from Strava and click Import button when
        finished.
      </p>
      <p>
        This shows only the rides on Strava that are most recent than any rides
        synced.
      </p>
      <div>
        {fetching ? (
          <div>
            <p>fetching activities...</p>
          </div>
        ) : (
          <div>
            {activities.length === 0 ? (
              <button onClick={getRecentActivities}>Start importing.</button>
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

"use client";

import { useRef, useState } from "react";
import { Activity } from "@/app/profile/connections/page";
import { getUserAllActivities } from "@/utils/stravaUtils";
import { getActivitiesDetails } from "@/utils/serverUtils";
import Link from "next/link";
import { useLocalStorage } from "@/hooks/useLocalStorage";
import { Field, Form, Formik } from "formik";

const page = () => {
  const [syncedActivities, setSyncedActivities] = useLocalStorage(
    "syncedActivities",
    [],
  );
  const syncedActivitiesSet = new Set(syncedActivities.map((a: number) => a));
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
    const data = await getUserAllActivities(page);
    //console.log(data);
    return data;
  };
  const importActivities = async (activitiesId: number[]) => {
    const response = await getActivitiesDetails(activitiesId);
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
              <Formik
                initialValues={{
                  activityId: new Set(activities.map((a) => a.id)),
                }}
                onSubmit={(values) => {
                  importActivities(Array.from(values.activityId));
                }}
              >
                {({ values, handleChange }) => (
                  <Form>
                    <div
                      role={"group"}
                      style={{ display: "flex", flexDirection: "column" }}
                    >
                      {activities.map((activity: Activity, index: number) => {
                        const [checked, setChecked] = useState(true);
                        return (
                          <label key={index}>
                            <Field
                              type={"checkbox"}
                              name={"activityId"}
                              value={activity.id}
                              checked={checked}
                              disabled={syncedActivitiesSet.has(activity.id)}
                              onChange={() => {
                                values.activityId.has(activity.id)
                                  ? values.activityId.delete(activity.id)
                                  : values.activityId.add(activity.id);
                                setChecked(!checked);
                              }}
                            />
                            {index + 1}. id: {activity.id}, title:{" "}
                            {activity.title}
                          </label>
                        );
                      })}
                    </div>
                    <button type={"submit"}>Import</button>
                  </Form>
                )}
              </Formik>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default page;

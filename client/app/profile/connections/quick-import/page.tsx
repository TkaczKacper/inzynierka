"use client";

import Link from "next/link";
import { useLocalStorage } from "@/hooks/useLocalStorage";
import { useState } from "react";
import { Activity } from "@/app/profile/connections/page";
import { getUserActivities } from "@/utils/stravaUtils";
import { getActivitiesDetails } from "@/utils/serverUtils";
import { Field, Form, Formik } from "formik";
import { parseDurationNumeric } from "@/utils/parseDuration";

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
    const data = await getUserActivities(page, latestActivity);
    console.log(data);
    return data;
  };
  const importActivities = async (activityIds: number[]) => {
    const response = await getActivitiesDetails(activityIds);
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
              <div>
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
                        style={{
                          display: "flex",
                          flexDirection: "column",
                          marginTop: "50px",
                        }}
                      >
                        <table>
                          <thead>
                            <tr>
                              <th></th>
                              <th>Date</th>
                              <th>Title</th>
                              <th>Duration</th>
                              <th>Distance</th>
                            </tr>
                          </thead>
                          <tbody>
                            {activities.map(
                              (activity: Activity, index: number) => {
                                const [checked, setChecked] = useState(true);
                                return (
                                  <tr>
                                    <td>
                                      <label key={index}>
                                        <Field
                                          type={"checkbox"}
                                          name={"activityId"}
                                          value={activity.id}
                                          checked={checked}
                                          onChange={() => {
                                            values.activityId.has(activity.id)
                                              ? values.activityId.delete(
                                                  activity.id,
                                                )
                                              : values.activityId.add(
                                                  activity.id,
                                                );
                                            setChecked(!checked);
                                          }}
                                        />
                                      </label>
                                    </td>
                                    <td>
                                      {activity.date.toLocaleDateString(
                                        "en-GB",
                                      ) + " "}
                                      {" " +
                                        activity.date.toLocaleTimeString(
                                          "en-GB",
                                        )}
                                    </td>
                                    <td>{activity.title}</td>
                                    <td>
                                      {parseDurationNumeric(
                                        Number(activity.duration),
                                      )}
                                    </td>
                                    <td>
                                      {(activity.distance / 1000).toFixed(1) +
                                        " km"}
                                    </td>
                                  </tr>
                                );
                              },
                            )}
                          </tbody>
                        </table>
                      </div>
                      <button type={"submit"}>Import</button>
                    </Form>
                  )}
                </Formik>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default page;

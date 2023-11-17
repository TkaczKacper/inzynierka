"use client";

import React, { useEffect, useState } from "react";
import { getActivityDataById } from "@/utils/serverUtils";
import { Activity } from "@/app/profile/[id]/types";
import Loading from "@/app/loading";
import Link from "next/link";
import { useUserContext } from "@/contexts/UserContextProvider";
import { MapContainer, TileLayer } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import styles from "./activitypage.module.css";
import MapController from "@/maps/MapController";
import ChartController from "@/charts/ChartController";
import { parseDurationNumeric } from "@/utils/parseDuration";
import ActivityTimeInZones from "@/app/activities/[id]/activityTimeInZones";

const page = () => {
  const [activity, setActivity] = useState<Activity>();
  const [loading, setLoading] = useState(true);
  const { userId } = useUserContext();

  useEffect(() => {
    const activityId = Number(window.location.pathname.split("/")[2]);
    const getActivityData = async (activityId: number) => {
      const res = await getActivityDataById(activityId);
      setLoading(false);
      setActivity(res?.data);
    };
    getActivityData(activityId);
  }, []);

  console.log(activity);
  return (
    <>
      {!loading ? (
        <>
          {activity ? (
            <div className={styles.activityContainer}>
              <div className={styles.activityInformation}>
                <div className={styles.activityHeader}>
                  <h1>{activity.title}</h1>
                  <p>
                    {activity.type} on{" "}
                    {new Date(activity.startDate).toLocaleDateString("en-PL", {
                      weekday: "long",
                      year: "numeric",
                      month: "long",
                      day: "numeric",
                    })}
                  </p>
                  <a
                    href={`https://www.strava.com/activities/${activity.stravaActivityID}`}
                    target={"_blank"}
                  >
                    Check activity on Strava.
                  </a>
                </div>
                <div className={styles.activityPrimary}>
                  <div>
                    <h2>{(activity.totalDistance / 1000).toFixed(2)}km</h2>
                    distance
                  </div>
                  <div>
                    <h2>{activity.totalElevationGain}m</h2>
                    elevation gain
                  </div>
                  <div>
                    <h2>{parseDurationNumeric(activity.movingTime)}</h2>
                    moving time
                  </div>
                  {activity.trimp > 0 ? (
                    <div>
                      <h2>{activity.trimp.toFixed(0)}</h2>
                      relative effort
                    </div>
                  ) : null}
                </div>
                {activity.hasPowerMeter ? (
                  <div className={styles.activityPrimary}>
                    <div>
                      <h2>{activity.normalizedPower.toFixed(0)}</h2>
                      normalized power
                    </div>
                    <div>
                      <h2>{activity.tss.toFixed(0)}</h2>
                      training load
                    </div>
                    <div>
                      <h2>{(activity.intensityFactor * 100).toFixed(0)}%</h2>
                      intensity
                    </div>
                    <div>
                      <h2>{activity.variabilityIndex.toFixed(2)}</h2>
                      variability
                    </div>
                  </div>
                ) : null}
                <div className={styles.activitySecondary}>
                  <div className={styles.activityStats}>
                    <table className={styles.activityStatsTable}>
                      <thead className={styles.activityStatsHeader}>
                        <tr>
                          <th></th>
                          <th>average</th>
                          <th>maximum</th>
                        </tr>
                      </thead>
                      <tbody className={styles.activityStatsBody}>
                        <tr>
                          <td>Speed</td>
                          <td>{(activity.avgSpeed * 3.6).toFixed(2)} km/h</td>
                          <td>{(activity.maxSpeed * 3.6).toFixed(2)} km/h</td>
                        </tr>
                        {activity.hasPowerMeter ? (
                          <tr>
                            <td>Power</td>
                            <td>{activity.avgWatts.toFixed(0)} W</td>
                            <td>{activity.maxWatts} W</td>
                          </tr>
                        ) : null}
                        {activity.avgCadence > 0 ? (
                          <tr>
                            <td>Cadence</td>
                            <td>{activity.avgCadence.toFixed(0)} rpm</td>
                            <td>{activity.maxCadence} rpm</td>
                          </tr>
                        ) : null}
                        {activity.avgHeartRate > 0 ? (
                          <tr>
                            <td>HeartRate</td>
                            <td>{activity.avgHeartRate.toFixed(0)} bpm</td>
                            <td>{activity.maxHeartRate} bpm</td>
                          </tr>
                        ) : null}
                        <tr>
                          <td>Calories</td>
                          <td>{activity.calories}</td>
                        </tr>
                        {activity.avgTemp > 0 ? (
                          <tr>
                            <td>Temperature</td>
                            <td>
                              {activity.avgTemp} {"\u00B0"}C
                            </td>
                          </tr>
                        ) : null}
                        <tr>
                          <td>Elapsed Time</td>
                          <td>{parseDurationNumeric(activity.elapsedTime)}</td>
                        </tr>
                      </tbody>
                    </table>
                    <div className={styles.activityStatsFoot}>
                      <p>{activity.deviceName}</p>
                      <p>{activity.gear}</p>
                    </div>
                  </div>
                  <div>
                    <ActivityTimeInZones
                      hrTimeInZone={activity.hrTimeInZone}
                      powerTimeInZone={activity.powerTimeInZone}
                      powerCurve={activity.powerCurve}
                      trimp={activity.trimp}
                      tss={activity.tss}
                    />
                  </div>
                </div>
              </div>
              {activity.detailedPolyline ? (
                <div className={styles.activityMap}>
                  <MapContainer center={[42, 22]} scrollWheelZoom={false}>
                    <MapController
                      polyline={activity.detailedPolyline}
                      startLatLng={activity.startLatLng}
                      endLatLng={activity.endLatLng}
                    />
                    <TileLayer
                      attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                      url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    />
                  </MapContainer>
                </div>
              ) : null}
              <ChartController data={activity.powerCurve} />
            </div>
          ) : (
            <>
              <h1>Activity not found.</h1>
              <Link href={`/profile/${userId}`}>Back to the profile page.</Link>
            </>
          )}
        </>
      ) : (
        <Loading />
      )}
    </>
  );
};
export default page;

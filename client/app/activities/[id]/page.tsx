﻿"use client";

import React, { useEffect, useState } from "react";
import {
  deleteActivityDataById,
  getActivityDataById,
} from "@/utils/serverUtils";
import { Activity } from "@/app/profile/[id]/types";
import Loading from "@/app/loading";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useUserContext } from "@/contexts/UserContextProvider";
import { MapContainer, TileLayer } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import styles from "./activitypage.module.css";
import MapController from "@/maps/MapController";
import ChartController from "@/charts/ChartController";
import { parseDurationNumeric } from "@/utils/parseDuration";
import ActivityTimeInZones from "@/app/activities/[id]/activityTimeInZones";
import PrevActivities from "@/app/activities/[id]/prevActivities";

//TODO rozdzielic na pliki (o ile mozliwe)
const page = () => {
  const [activity, setActivity] = useState<Activity>();
  const [loading, setLoading] = useState(true);
  const router = useRouter();
  const { userId } = useUserContext();

  const activityId = Number(window.location.pathname.split("/")[2]);

  useEffect(() => {
    const getActivityData = async (activityId: number) => {
      const res = await getActivityDataById(activityId);
      setLoading(false);
      setActivity(res?.data);
    };
    getActivityData(activityId);
  }, []);

  const deleteHandler = async () => {
    const res = await deleteActivityDataById(activityId);
    if (res?.status == 200) return router.push(`/profile/${userId}`);
  };

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
                  <button onClick={deleteHandler}>Delete activity</button>
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
                      <h2>{activity.effectivePower.toFixed(0)}</h2>
                      effective power
                    </div>
                    <div>
                      <h2>{(activity.intensity * 100).toFixed(0)}%</h2>
                      intensity
                    </div>
                    <div>
                      <h2>{activity.variabilityIndex.toFixed(2)}</h2>
                      variability
                    </div>{" "}
                    <div>
                      <h2>{activity.trainingLoad.toFixed(0)}</h2>
                      training load
                    </div>
                  </div>
                ) : null}
                <div className={styles.activityStatsContainer}>
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
                            <td>
                              {parseDurationNumeric(activity.elapsedTime)}
                            </td>
                          </tr>
                        </tbody>
                      </table>
                      <div className={styles.activityStatsFoot}>
                        <p>{activity.deviceName}</p>
                        <p>{activity.gear}</p>
                      </div>
                    </div>
                    <ActivityTimeInZones
                      hrTimeInZone={activity.hrTimeInZone}
                      powerTimeInZone={activity.powerTimeInZone}
                      powerCurve={activity.powerCurve}
                      trimp={activity.trimp}
                      tss={activity.trainingLoad}
                    />
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
                </div>
              </div>
              {activity.powerCurve.length > 0 ? (
                <div className={styles.powerCurveChart}>
                  <ChartController data={activity.powerCurve} />
                </div>
              ) : null}
              <PrevActivities date={activity.startDate} />
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

"use client";

import React, { useEffect, useState } from "react";
import {
  getAthleteActivities,
  getAthleteActivitiesPeriod,
} from "@/utils/serverUtils";
import { Activity } from "@/app/profile/[id]/types";
import Link from "next/link";
import styles from "./profileActivities.module.css";
import { MapContainer, TileLayer } from "react-leaflet";
import MapController from "@/maps/MapController";
import { parseDurationNumeric } from "@/utils/parseDuration";

interface props {
  month: number;
}

export const ProfileActivities = (month: props) => {
  const [activities, setActivities] = useState<Activity[]>();
  useEffect(() => {
    const userActivities = async () => {
      //@ts-ignore
      const res = await getAthleteActivitiesPeriod(month.month);
      setActivities(res?.data);
    };
    userActivities();
  }, [month]);

  console.log(activities);

  return (
    <div>
      {activities?.map((activity: Activity, index) => {
        return (
          <div className={styles.activity} key={index}>
            <div className={styles.activityMap}>
              <MapContainer center={[42, 22]} scrollWheelZoom={false}>
                <MapController
                  polyline={activity.summaryPolyline}
                  startLatLng={activity.startLatLng}
                  endLatLng={activity.endLatLng}
                />
                <TileLayer
                  attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                  url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
              </MapContainer>
            </div>
            <div className={styles.activitySummary}>
              <h2>
                <Link href={`/activities/${activity.id}`}>
                  {activity.title}
                </Link>
              </h2>
              <p className={styles.activityDate}>
                {new Date(activity.startDate).toLocaleDateString("en-UK", {
                  weekday: "long",
                  year: "numeric",
                  month: "long",
                  day: "numeric",
                  hour: "numeric",
                  minute: "numeric",
                })}
              </p>
              <div className={styles.activityAverages}>
                <p>{(activity.totalDistance / 1000).toFixed(1)} km</p>
                <span className={styles.dot}>|</span>
                <p className={styles.activityAverageCenter}>
                  {parseDurationNumeric(activity.elapsedTime)}
                </p>
                <span className={styles.dot}>|</span>
                <p>{activity.totalElevationGain} m</p>
              </div>
              <div className={styles.activityAverages}>
                <p>{(activity.avgSpeed * 3.6).toFixed(2)} km/h</p>
                {activity.avgHeartRate > 0 ? (
                  <>
                    <span className={styles.dot}>|</span>
                    <p>{activity.avgHeartRate} bpm</p>
                  </>
                ) : null}
                {activity.hasPowerMeter ? (
                  <>
                    <span className={styles.dot}>|</span>
                    <p>{activity.avgWatts} W</p>
                    <span className={styles.dot}>|</span>
                    <p>{activity.avgCadence} rpm</p>
                  </>
                ) : null}
              </div>
              {activity.hasPowerMeter ? (
                <div className={styles.activityAverages}>
                  <p>EP: {activity.effectivePower.toFixed(0)} W</p>
                  <span className={styles.dot}>|</span>
                  <p>Int: {(activity.intensity * 100).toFixed(0)} %</p>
                  <span className={styles.dot}>|</span>
                  <p>Load: {activity.trainingLoad.toFixed(0)}</p>
                </div>
              ) : activity.avgHeartRate > 0 ? (
                <p>Trimp: {activity.trimp.toFixed(0)}</p>
              ) : null}
            </div>
          </div>
        );
      })}
    </div>
  );
};

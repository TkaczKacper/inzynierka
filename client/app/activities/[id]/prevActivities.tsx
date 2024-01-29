"use client";

import { useEffect, useState } from "react";
import { getAthleteActivities } from "@/utils/serverUtils";
import { Activity } from "@/app/profile/[id]/types";
import Link from "next/link";
import styles from "./prevActivities.module.css";

interface props {
  date: Date;
}

//TODO dodac obsluge wczytywania wiekszej ilosci aktywnosci (on scroll)

const prevActivities = (props: props) => {
  const [recentActivities, setRecentActivities] = useState<Activity[]>();
  useEffect(() => {
    async function getData() {
      const recent = await getAthleteActivities(new Date().toUTCString(), 5);
      setRecentActivities(recent?.data);
    }
    getData();
  }, []);
  return (
    <>
      <div className={styles.otherActivity}>
        <h2>Your recent activities</h2>
        {recentActivities?.map((value, index) => {
          return (
            <div className={styles.otherActivityContainer} key={index}>
              <div>
                {new Date(value.startDate).toLocaleDateString("en-PL", {
                  year: "numeric",
                  month: "short",
                  day: "numeric",
                })}
              </div>
              <div>{(value.totalDistance / 1000).toFixed(0)}km</div>
              <div className={styles.otherActivityTitle}>
                <Link href={`${value.id}`}>{value.title}</Link>
              </div>
            </div>
          );
        })}
      </div>
    </>
  );
};

export default prevActivities;

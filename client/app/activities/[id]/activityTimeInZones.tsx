"use client";
import styles from "./activitypage.module.css";
import React, { useState } from "react";
import HrZoneChart from "@/charts/HrZoneChart";
import PowerZoneChart from "@/charts/PowerZoneChart";
import ChartController from "@/charts/ChartController";
import { useLocalStorage } from "@/hooks/useLocalStorage";
import {
  hrTimeInZoneType,
  powerTimeInZoneType,
} from "@/app/profile/[id]/types";

interface props {
  hrTimeInZone: hrTimeInZoneType;
  powerCurve: number[];
  powerTimeInZone: powerTimeInZoneType;
  trimp: number;
  tss: number;
}

const activityTimeInZones = (activity: props) => {
  const [activeChart, setActiveChart] = useState(true);
  const [userHrZones, setUserHrZones] = useLocalStorage("hrZones", {});
  const [userPowerZones, setUserPowerZones] = useLocalStorage("powerZones", {});

  return (
    <div className={styles.metricsContainer}>
      <div className={styles.metricsHeader}>
        <div>Metrics</div>
        <div onClick={() => setActiveChart(true)}>Heart rate</div>
        <div onClick={() => setActiveChart(false)}>Power</div>
      </div>
      <div>
        {activeChart ? (
          <>
            <p>Trimp: {activity.trimp.toFixed(0)}</p>
            {activity.hrTimeInZone ? (
              <HrZoneChart
                data={activity.hrTimeInZone}
                zones={userHrZones[userHrZones.length - 1]}
              />
            ) : null}
          </>
        ) : (
          <>
            <p>Trainig load: {activity.tss.toFixed(0)}</p>
            {activity.powerCurve.length > 0 ? (
              <PowerZoneChart
                data={activity.powerTimeInZone}
                zones={userPowerZones[userPowerZones.length - 1]}
              />
            ) : null}
          </>
        )}
      </div>
    </div>
  );
};

export default activityTimeInZones;

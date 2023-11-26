import React, { useEffect, useState } from "react";
import { getAthleteStats } from "@/utils/serverUtils";
import {
  AthleteInfo,
  AthleteStats,
  MonthlySummary,
} from "@/app/profile/[id]/types";
import { useLocalStorage } from "@/hooks/useLocalStorage";
import styles from "./profileStats.module.css";
import {
  parseDurationExact,
  parseDurationNumeric,
} from "@/utils/parseDuration";

const months = [
  "January",
  "February",
  "March",
  "April",
  "May",
  "June",
  "July",
  "August",
  "September",
  "October",
  "November",
  "December",
];
interface props {
  setMonth: any;
}
export const ProfileStats = (setMonth: props) => {
  const [athleteStats, setAthleteStats] = useState<AthleteStats>();
  const [athleteInfo, setAthleteInfo] = useState<AthleteInfo>();
  const [monthlySum, setMonthlySum] = useState<MonthlySummary[]>();
  const [currMonth, setCurrMonth] = useState<MonthlySummary>();
  const [max, setMax] = useState<number>(0);

  const [userPowerZones, setUserPowerZones] = useLocalStorage("powerZones", {});
  const [userHrZones, setUserHrZones] = useLocalStorage("hrZones", {});

  useEffect(() => {
    const stats = async () => {
      const res = await getAthleteStats();
      const sum: MonthlySummary[] = res?.data.monthlySummaries;
      setAthleteStats(res?.data.athleteStats);
      setAthleteInfo(res?.data.stravaProfileInfo);
      setUserHrZones(res?.data.hrZones);
      setUserPowerZones(res?.data.powerZones);
      setMonthlySum(sum);
      if (sum) {
        setMax(Math.max(...sum.map((val) => val.totalDistance)));
        setCurrMonth(
          res?.data.monthlySummaries[res?.data.monthlySummaries.length - 1],
        );
      }
    };
    stats();
  }, []);

  return (
    <div>
      {athleteInfo ? (
        <div>
          {/*<img src={athleteInfo.profileAvatar} alt={"profile photo"} />*/}
          <div>
            <h1>
              {athleteInfo.firstName} {athleteInfo.lastName}
            </h1>
            <div>
              {athleteInfo.city}, {athleteInfo.state}, {athleteInfo.country}
            </div>
            <div>{athleteInfo.bio}</div>
          </div>
        </div>
      ) : (
        <>
          <h1>Strava profile is not connected.</h1>
          Go to <a href={"/profile/connections"}>connections</a> page to
          connect.
        </>
      )}

      {currMonth && monthlySum ? (
        <div className={styles.monthlySummaryContainer}>
          <div className={styles.summaryInfo}>
            <h2>Activities in {months[currMonth.month - 1]}</h2>
            <ul className={styles.infoTotals}>
              <li>
                {parseDurationExact(currMonth.totalMovingTime)}
                <p className={styles.infoTotalsDescription}>Time</p>
              </li>
              <li>
                {(currMonth.totalDistance / 1000).toFixed(2)} km
                <p className={styles.infoTotalsDescription}>Distance</p>
              </li>
              <li>
                {currMonth.totalElevationGain.toFixed(0)} m
                <p className={styles.infoTotalsDescription}>Ascending</p>
              </li>
              <li>
                {currMonth.totalCalories}
                <p className={styles.infoTotalsDescription}>Calories</p>
              </li>
            </ul>
          </div>
          <div className={styles.summaryChart}>
            <ul className={styles.yAxis}>
              <li>{(max / 1000).toFixed(0)} km</li>
              <li>{((max / 1000) * 0.75).toFixed(0)} km</li>
              <li>{((max / 1000) * 0.5).toFixed(0)} km</li>
              <li>{((max / 1000) * 0.25).toFixed(0)} km</li>
              <li>0km</li>
            </ul>
            <ul className={styles.months}>
              {months.map((value, index) => {
                return (
                  <li key={index} className={styles.month}>
                    {monthlySum[index] ? (
                      <div
                        className={styles.bar}
                        onClick={() => {
                          setCurrMonth(monthlySum[index]);
                          setMonth.setMonth(monthlySum[index].month);
                        }}
                      >
                        <div
                          className={`${styles.fill} ${
                            index == currMonth.month - 1 ? styles.active : ""
                          }`}
                          style={{
                            height: `${(
                              (monthlySum[index].totalDistance / max) *
                              100
                            ).toFixed(0)}px`,
                          }}
                        ></div>
                      </div>
                    ) : (
                      <div></div>
                    )}
                  </li>
                );
              })}
            </ul>
          </div>
        </div>
      ) : null}
    </div>
  );
};

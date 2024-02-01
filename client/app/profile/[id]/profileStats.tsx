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
  setYearOffset: any;
  yearOffset: number;
}
export const ProfileStats = ({
  setMonth,
  yearOffset,
  setYearOffset,
}: props) => {
  const [athleteInfo, setAthleteInfo] = useState<AthleteInfo>();
  const [userPowerZones, setUserPowerZones] = useLocalStorage("powerZones", {});
  const [userHrZones, setUserHrZones] = useLocalStorage("hrZones", {});
  const [yearsAvailable, setYearsAvailable] = useState<number[]>([]);

  const [monthlySum, setMonthlySum] = useState<MonthlySummary[]>();
  const [max, setMax] = useState<number>(0);
  const [currMonth, setCurrMonth] = useState<MonthlySummary>();

  const [yearDropdown, setYearDropdown] = useState("none");

  useEffect(() => {
    const stats = async () => {
      const res = await getAthleteStats(yearOffset);
      const sum: MonthlySummary[] = res?.data.monthlySummaries;

      const summaries: MonthlySummary[] = [];

      for (let i = 1, j = 0; i <= 12; i++) {
        if (sum[j] && sum[j].month == i) {
          summaries.push(sum[j]);
          j++;
        } else {
          summaries.push({
            month: i,
            year: new Date().getFullYear() - yearOffset,
            totalCalories: 0,
            totalDistance: 0,
            totalElapsedTime: 0,
            totalElevationGain: 0,
            totalMovingTime: 0,
            trainingLoad: 0,
          });
        }
      }

      setAthleteInfo(res?.data.stravaProfileInfo);
      setUserHrZones(res?.data.hrZones);
      setUserPowerZones(res?.data.powerZones);
      setYearsAvailable(res?.data.yearsAvailable);
      setMonthlySum(summaries);
      if (sum) {
        setMax(Math.max(...sum.map((val) => val.totalDistance)));
        setCurrMonth(
          res?.data.monthlySummaries[res?.data.monthlySummaries.length - 1],
        );
      }
    };
    stats();
  }, [yearOffset]);

  return (
    <div>
      {athleteInfo ? (
        <div className={styles.athleteInfo}>
          <h1>
            {athleteInfo.firstName} {athleteInfo.lastName}
          </h1>
          <div>
            {athleteInfo.city}, {athleteInfo.state}, {athleteInfo.country}
          </div>
        </div>
      ) : (
        <>
          <h1>Strava profile is not connected.</h1>
          Go to{" "}
          <a
            style={{
              textDecoration: "underline",
              fontWeight: 600,
            }}
            href={"/profile/connections"}
          >
            connections
          </a>{" "}
          page to connect.
        </>
      )}

      {currMonth && monthlySum ? (
        <div className={styles.monthlySummaryContainer}>
          <div className={styles.summaryInfo}>
            <h2>
              Activities in {months[currMonth.month - 1]} {currMonth.year}{" "}
              <button
                onClick={() => {
                  yearDropdown === "none"
                    ? setYearDropdown("block")
                    : setYearDropdown("none");
                }}
              >
                xd
              </button>
            </h2>

            <div
              className={styles.yearManagement}
              style={{ display: `${yearDropdown}` }}
            >
              <ul>
                {yearsAvailable.map((value, index) => {
                  return (
                    <li key={index}>
                      <div
                        onClick={() => {
                          setYearDropdown("none");
                          setYearOffset(index);
                        }}
                      >
                        {value}
                      </div>
                    </li>
                  );
                })}
              </ul>
            </div>

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
            <a className={styles.yAxisTitle}>Total distance</a>
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
                          setMonth(monthlySum[index].month);
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
            <ul className={styles.monthsTitle}>
              <li>Jan</li>
              <li>Feb</li>
              <li>Mar</li>
              <li>Apr</li>
              <li>May</li>
              <li>Jun</li>
              <li>Jul</li>
              <li>Aug</li>
              <li>Sep</li>
              <li>Oct</li>
              <li>Nov</li>
              <li>Dec</li>
            </ul>
          </div>
        </div>
      ) : null}
    </div>
  );
};

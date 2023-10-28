import React, { useEffect, useState } from "react";
import { getAthleteStats } from "@/utils/serverUtils";
import { AthleteInfo, AthleteStats } from "@/app/profile/[id]/types";
import { useLocalStorage } from "@/hooks/useLocalStorage";

export const ProfileStats = () => {
  const [athleteStats, setAthleteStats] = useState<AthleteStats>();
  const [athleteInfo, setAthleteInfo] = useState<AthleteInfo>();

  const [userPowerZones, setUserPowerZones] = useLocalStorage("powerZones", {});
  const [userHrZones, setUserHrZones] = useLocalStorage("hrZones", {});

  console.log(userPowerZones);
  useEffect(() => {
    const stats = async () => {
      const res = await getAthleteStats();
      setAthleteStats(res?.data.athleteStats);
      setAthleteInfo(res?.data.stravaProfileInfo);
      setUserHrZones(res?.data.hrZones);
      setUserPowerZones(res?.data.powerZones);
    };
    stats();
  }, []);
  console.log(athleteStats);
  const parseDuration = (time: number) => {
    const hours = Math.floor(time / 3600);
    const minutes = Math.floor((time - hours * 3600) / 60);
    const seconds = Math.floor(time - hours * 3600 - minutes * 60);
    return `${hours > 0 ? hours + "h" : null} ${
      time > 60 ? minutes + "m" : null
    } ${seconds + "s"}`;
  };

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
      <h2>Stats</h2>
      {athleteStats ? (
        <div>
          <div>
            <h3>Last 4 weeks</h3>
            <div>
              Activities / Week:{" "}
              {(athleteStats.recentRideTotals.count / 4).toFixed()}
            </div>
            <div>
              Avg Distance / Week:{" "}
              {(athleteStats.recentRideTotals.distance / 4000).toFixed(2)}
              km
            </div>
            <div>
              Elev Gain / Week:{" "}
              {(athleteStats.recentRideTotals.elevationGain / 4).toFixed(1)}m
            </div>
            <div>
              Avg Time / Week:{" "}
              {parseDuration(athleteStats.recentRideTotals.elapsedTime / 4)}
            </div>
          </div>
          <div>
            <h3>This year</h3>
            <div>Activities: {athleteStats.ytdRideTotals.count}</div>
            <div>
              Distance:{" "}
              {(athleteStats.ytdRideTotals.distance / 1000).toFixed(2)}km
            </div>
            <div>Elev Gain: {athleteStats.ytdRideTotals.elevationGain}m</div>
            <div>
              Time: {parseDuration(athleteStats.ytdRideTotals.elapsedTime)}
            </div>
          </div>
          <div>
            <h3>All-Time</h3>
            <div>Activities: {athleteStats.allTimeRideTotals.count}</div>
            <div>
              Distance:{" "}
              {(athleteStats.allTimeRideTotals.distance / 1000).toFixed(2)}km
            </div>
            <div>
              Elev Gain: {athleteStats.allTimeRideTotals.elevationGain}m
            </div>
            <div>
              Time: {parseDuration(athleteStats.allTimeRideTotals.elapsedTime)}
            </div>
            <div>
              Longest Ride: {(athleteStats.longestRide / 1000).toFixed(2)}km
            </div>
            <div>Biggest Climb: {athleteStats.biggestClimb.toFixed()}m</div>
          </div>
        </div>
      ) : null}
    </div>
  );
};

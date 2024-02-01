import { hrZonesType } from "@/app/profile/heartrate-management/page";
import React, { useState } from "react";
import styles from "../zones.module.css";
import { updateHrZones } from "@/utils/serverUtils";

const heartRateZones = ({ data, setData }: any) => {
  const curr: hrZonesType = data[data.length - 1];
  const [zone1, setZone1] = useState(curr.zone1);
  const [zone2, setZone2] = useState(curr.zone2);
  const [zone3, setZone3] = useState(curr.zone3);
  const [zone4, setZone4] = useState(curr.zone4);
  const [zone5, setZone5] = useState(curr.zone5);
  const [zone5a, setZone5a] = useState(curr.zone5a);
  const [zone5b, setZone5b] = useState(curr.zone5b);
  const [zone5c, setZone5c] = useState(curr.zone5c);

  const saveChanges = async () => {
    curr.zone1 = zone1;
    curr.zone2 = zone2;
    curr.zone3 = zone3;
    curr.zone4 = zone4;
    curr.zone5 = zone5;
    curr.zone5a = zone5a;
    curr.zone5b = zone5b;
    curr.zone5c = zone5c;
    curr.setAutoZones = false;

    data[data.length - 1] = curr;
    console.log("updated", curr);
    await updateHrZones(curr);
  };

  return (
    <div>
      <div className={styles.zoneContainer}>
        <h2>Heart rate zones</h2>
        <div className={styles.zoneInfo}>
          Zone 1
          <input
            value={zone1}
            onChange={(e) => setZone1(Number(e.target.value))}
          />
          to {zone2 - 1} BPM
        </div>
        <div className={styles.zoneInfo}>
          Zone 2
          <input
            value={zone2}
            onChange={(e) => setZone2(Number(e.target.value))}
          />
          to {zone3 - 1} BPM
        </div>
        <div className={styles.zoneInfo}>
          Zone 3
          <input
            value={zone3}
            onChange={(e) => setZone3(Number(e.target.value))}
          />
          to {zone4 - 1} BPM
        </div>
        {zone5 ? (
          <>
            <div className={styles.zoneInfo}>
              Zone 4
              <input
                value={zone4}
                onChange={(e) => setZone4(Number(e.target.value))}
              />
              to {zone5 - 1} BPM
            </div>
            <div className={styles.zoneInfo}>
              Zone 5
              <input
                value={zone5}
                onChange={(e) => setZone5(Number(e.target.value))}
              />
              to {curr.hrMax} BPM
            </div>
          </>
        ) : zone5a && zone5b && zone5c ? (
          <>
            <div className={styles.zoneInfo}>
              Zone 4
              <input
                value={zone4}
                onChange={(e) => setZone4(Number(e.target.value))}
              />
              to {zone5a - 1} BPM
            </div>
            <div className={styles.zoneInfo}>
              Zone 5a
              <input
                value={zone5a}
                onChange={(e) => setZone5a(Number(e.target.value))}
              />
              to {zone5b - 1} BPM
            </div>
            <div className={styles.zoneInfo}>
              Zone 5b
              <input
                value={zone5b}
                onChange={(e) => setZone5b(Number(e.target.value))}
              />
              to {zone5c - 1} BPM
            </div>
            <div className={styles.zoneInfo}>
              Zone 5c
              <input
                value={zone5c}
                onChange={(e) => setZone5c(Number(e.target.value))}
              />
              to {curr.hrMax} BPM
            </div>
          </>
        ) : null}

        <button onClick={saveChanges}>Save changes</button>
      </div>
    </div>
  );
};

export default heartRateZones;

import { powerZonesType } from "@/app/profile/power-management/page";
import { useState } from "react";
import styles from "../zones.module.css";
import { updatePowerZones } from "@/utils/serverUtils";

const powerZones = ({ data, setData }: any) => {
  const curr: powerZonesType = data[data.length - 1];
  const [zone1, setZone1] = useState(curr.zone1);
  const [zone2, setZone2] = useState(curr.zone2);
  const [zone3, setZone3] = useState(curr.zone3);
  const [zone4, setZone4] = useState(curr.zone4);
  const [zone5, setZone5] = useState(curr.zone5);
  const [zone6, setZone6] = useState(curr.zone6);
  const [zone7, setZone7] = useState(curr.zone7);

  //TODO dodac wysylanie na backend
  const saveChanges = async () => {
    curr.zone1 = zone1;
    curr.zone2 = zone2;
    curr.zone3 = zone3;
    curr.zone4 = zone4;
    curr.zone5 = zone5;
    curr.zone6 = zone6;
    curr.zone7 = zone7;
    curr.setAutoZones = false;

    data[data.length - 1] = curr;
    console.log("updated", curr);
    await updatePowerZones(curr);
  };
  return (
    <div>
      <div className={styles.zoneContainer}>
        <div className={styles.zoneInfo}>
          Zone 1
          <input
            value={zone1}
            onChange={(e) => setZone1(Number(e.target.value))}
          />
          to {zone2 - 1} W
        </div>
        <div className={styles.zoneInfo}>
          Zone 2
          <input
            value={zone2}
            onChange={(e) => setZone2(Number(e.target.value))}
          />
          to {zone3 - 1} W
        </div>
        <div className={styles.zoneInfo}>
          Zone 3
          <input
            value={zone3}
            onChange={(e) => setZone3(Number(e.target.value))}
          />
          to {zone4 - 1} W
        </div>
        <div className={styles.zoneInfo}>
          Zone 4
          <input
            value={zone4}
            onChange={(e) => setZone4(Number(e.target.value))}
          />
          to {zone5 - 1} W
        </div>
        <div className={styles.zoneInfo}>
          Zone 5
          <input
            value={zone5}
            onChange={(e) => setZone5(Number(e.target.value))}
          />
          to {zone6 - 1} W
        </div>
        <div className={styles.zoneInfo}>
          Zone 6
          <input
            value={zone6}
            onChange={(e) => setZone6(Number(e.target.value))}
          />
          to {zone7 - 1} W
        </div>
        <div className={styles.zoneInfo}>
          Zone 7
          <input
            value={zone7}
            onChange={(e) => setZone7(Number(e.target.value))}
          />
          W +
        </div>
        <button onClick={saveChanges}>Save changes</button>
      </div>
    </div>
  );
};

export default powerZones;

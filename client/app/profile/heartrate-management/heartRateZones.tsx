import { hrZonesType } from "@/app/profile/heartrate-management/page";
import { useState } from "react";

const heartRateZones = ({ data, setData }: any) => {
  const curr: hrZonesType = data[data.length - 1];
  const [zone1, setZone1] = useState(curr.zone1);
  const [zone2, setZone2] = useState(curr.zone2);
  const [zone3, setZone3] = useState(curr.zone3);
  const [zone4, setZone4] = useState(curr.zone4);
  const [zone5, setZone5] = useState(curr.zone5);

  const saveChanges = () => {
    curr.zone1 = zone1;
    curr.zone2 = zone2;
    curr.zone3 = zone3;
    curr.zone4 = zone4;
    curr.zone5 = zone5;

    data[data.length - 1] = curr;
    console.log("updated", data);
  };

  return (
    <div>
      <h2>{curr.dateAdded}</h2>
      <div style={{ display: "flex", flexDirection: "column" }}>
        <div>
          Zone 1
          <input
            value={zone1}
            onChange={(e) => setZone1(Number(e.target.value))}
          />
          to {zone2 - 1}
        </div>
        <div>
          Zone 2
          <input
            value={zone2}
            onChange={(e) => setZone2(Number(e.target.value))}
          />
          to {zone3 - 1}
        </div>
        <div>
          Zone 3
          <input
            value={zone3}
            onChange={(e) => setZone3(Number(e.target.value))}
          />
          to {zone4 - 1}
        </div>
        <div>
          Zone 4
          <input
            value={zone4}
            onChange={(e) => setZone4(Number(e.target.value))}
          />
          to {zone5 - 1}
        </div>
        <div>
          Zone 5
          <input
            value={zone5}
            onChange={(e) => setZone5(Number(e.target.value))}
          />
          to {curr.hrMax}
        </div>
        <button onClick={saveChanges}>Save changes</button>
      </div>
    </div>
  );
};

export default heartRateZones;

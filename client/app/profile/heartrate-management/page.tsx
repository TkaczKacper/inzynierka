"use client";

import { HrUpdateForm } from "./updateHr";
import React, { useEffect, useState } from "react";
import { deleteHrEntry } from "@/utils/serverUtils";
import HeartRateZones from "@/app/profile/heartrate-management/heartRateZones";

export type hrZonesType = {
  id: number;
  dateAdded: string;
  hrRest: number;
  hrMax: number;
  zone1: number;
  zone2: number;
  zone3: number;
  zone4: number;
  zone5: number;
};

const page = () => {
  const [data, setData] = useState<hrZonesType[]>([]);

  useEffect(() => {
    const userHrZones = localStorage.getItem("hrZones");
    if (userHrZones)
      setData(userHrZones !== "undefined" ? JSON.parse(userHrZones) : []);
  }, []);

  const deleteEntry = async (id: number, index: number) => {
    const response = await deleteHrEntry(id);
    console.log(response);
    if (response?.status === 200 && data) {
      setData(data.filter((elem, idx) => idx != index));
    }
  };
  console.log(data);
  return (
    <>
      <HrUpdateForm data={data} setData={setData} />

      {data.length > 0 ? (
        <>
          <table>
            <thead>
              <tr>
                <th>Date</th>
                <th>RestingHr</th>
                <th>MaxHr</th>
              </tr>
            </thead>
            <tbody>
              {data.map((value: hrZonesType, index: number) => {
                return (
                  <tr key={value.id}>
                    <td>{value.dateAdded}</td>
                    <td>{value.hrRest}</td>
                    <td>{value.hrMax}</td>
                    <td>
                      <button onClick={() => deleteEntry(value.id, index)}>
                        Delete
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
          <h1>Heart rate zones</h1>
          <HeartRateZones data={data} setData={setData} />
        </>
      ) : (
        <h1>No entries</h1>
      )}
    </>
  );
};

export default page;

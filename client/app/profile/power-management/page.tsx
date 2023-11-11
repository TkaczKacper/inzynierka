"use client";

import { FtpUpdateForm } from "./updateFtp";
import React, { useEffect, useState } from "react";
import { hrZonesType } from "@/app/profile/heartrate-management/page";
import { deleteHrEntry, deletePowerEntry } from "@/utils/serverUtils";
import PowerZones from "@/app/profile/power-management/powerZones";

export type powerZonesType = {
  id: number;
  dateAdded: string;
  ftp: number;
  zone1: number;
  zone2: number;
  zone3: number;
  zone4: number;
  zone5: number;
  zone6: number;
  zone7: number;
};

const page = () => {
  const [data, setData] = useState<powerZonesType[]>([]);

  useEffect(() => {
    const powerZones = localStorage.getItem("powerZones");
    powerZones ? setData(JSON.parse(powerZones)) : null;
  }, []);

  const deleteEntry = async (id: number, index: number) => {
    const response = await deletePowerEntry(id);
    console.log(response);
    if (response?.status === 200 && data) {
      setData(data.filter((elem, idx) => idx != index));
    }
  };

  return (
    <>
      <FtpUpdateForm data={data} setData={setData} />
      {data.length > 0 ? (
        <>
          <table>
            <thead>
              <tr>
                <th>Date</th>
                <th>Ftp</th>
              </tr>
            </thead>
            <tbody>
              {data.map((value: powerZonesType, index: number) => {
                return (
                  <tr key={value.id}>
                    <td>{value.dateAdded}</td>
                    <td>{value.ftp}</td>
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
          <h1>Power zones</h1>
          <PowerZones data={data} setData={setData} />
        </>
      ) : null}
    </>
  );
};

export default page;

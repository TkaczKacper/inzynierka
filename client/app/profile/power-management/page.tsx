"use client";

import React, { useEffect, useState } from "react";
import { deletePowerEntry, updateFtp } from "@/utils/serverUtils";
import PowerZones from "@/app/profile/power-management/powerZones";
import styles from "../management.module.css";

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
  const [newFTP, setNewFTP] = useState(0);
  const [isSubmiting, setIsSubmiting] = useState(false);
  const [addNewDisplay, setAddNewDisplay] = useState("none");

  useEffect(() => {
    const powerZones = localStorage.getItem("powerZones");
    if (powerZones)
      setData(powerZones !== "undefined" ? JSON.parse(powerZones) : []);
  }, []);

  const deleteEntry = async (id: number, index: number) => {
    const response = await deletePowerEntry(id);
    console.log(response);
    if (response?.status === 200 && data) {
      setData(data.filter((elem, idx) => idx != index));
    }
  };

  const submitHandler = async () => {
    const response = await updateFtp(newFTP);
    console.log(response);
    if (response?.data) {
      setData([...data, response.data]);
      setIsSubmiting(false);
      setAddNewDisplay("none");
    }
    return response;
  };

  return (
    <div className={styles.management}>
      <a
        className={styles.managementAddNew}
        onClick={() => setAddNewDisplay("table-row")}
      >
        Add new measurement
      </a>
      <table className={styles.managementTable}>
        <thead>
          <tr>
            <th>Date</th>
            <th>Ftp</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr
            className={styles.newMeasurment}
            style={{ display: addNewDisplay }}
          >
            <td>{new Date().toISOString().split("T")[0]}</td>
            <td>
              <input
                className={styles.managementInput}
                value={newFTP}
                onChange={(e) => setNewFTP(Number(e.target.value))}
              />{" "}
              W
            </td>
            <td>
              <button
                className={styles.buttonAdd}
                disabled={isSubmiting}
                onClick={() => {
                  setIsSubmiting(true);
                  submitHandler();
                }}
              >
                Save
              </button>
              <button
                id={styles.buttonCancel}
                onClick={() => setAddNewDisplay("none")}
              >
                Cancel
              </button>
            </td>
          </tr>

          {data.length > 0 ? (
            <>
              {data.map((value: powerZonesType, index: number) => {
                return (
                  <tr key={value.id}>
                    <td>{value.dateAdded}</td>
                    <td>{value.ftp} W</td>
                    <td>
                      <button onClick={() => deleteEntry(value.id, index)}>
                        Delete
                      </button>
                    </td>
                  </tr>
                );
              })}
            </>
          ) : null}
        </tbody>
      </table>
      {data.length > 0 ? (
        <>
          <h1>Power zones</h1>
          <PowerZones data={data} setData={setData} />
        </>
      ) : (
        <h1>No entries yet!</h1>
      )}
    </div>
  );
};

export default page;

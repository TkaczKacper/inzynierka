"use client";

import React, { useEffect, useState } from "react";
import { deleteHrEntry, updateHr } from "@/utils/serverUtils";
import HeartRateZones from "@/app/profile/heartrate-management/heartRateZones";
import styles from "../management.module.css";

export type hrZonesType = {
  id: number;
  dateAdded: string;
  hrRest: number;
  hrMax: number;
  ltHr: number | null;
  setAutoZones: boolean;
  zone1: number;
  zone2: number;
  zone3: number;
  zone4: number;
  zone5: number | null;
  zone5a: number | null;
  zone5b: number | null;
  zone5c: number | null;
};

const page = () => {
  const [data, setData] = useState<hrZonesType[]>([]);
  const [newHrRest, setNewHrRest] = useState(0);
  const [newHrMax, setNewHrMax] = useState(0);
  const [newLtHr, setNewLtHr] = useState<number | undefined>(undefined);
  const [isSubmiting, setIsSubmiting] = useState(false);
  const [addNewDisplay, setAddNewDisplay] = useState("none");

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
  const submitHandler = async () => {
    const response = await updateHr(newHrRest, newHrMax, newLtHr);
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
            <th>Resting Hr</th>
            <th>Max Hr</th>
            <th>LT Hr</th>
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
                value={newHrRest}
                onChange={(e) => setNewHrRest(Number(e.target.value))}
              />
            </td>
            <td>
              <input
                className={styles.managementInput}
                value={newHrMax}
                onChange={(e) => setNewHrMax(Number(e.target.value))}
              />
            </td>
            <td>
              <input
                className={styles.managementInput}
                value={newLtHr}
                onChange={(e) => setNewLtHr(Number(e.target.value))}
              />
            </td>
            <td>
              <button
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
              {data.map((value: hrZonesType, index: number) => {
                return (
                  <tr key={index}>
                    <td>{value.dateAdded}</td>
                    <td>{value.hrRest} BPM</td>
                    <td>{value.hrMax} BPM</td>
                    {value.ltHr ? <td>{value.ltHr} BPM</td> : <td>--</td>}
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
        <HeartRateZones data={data} setData={setData} />
      ) : (
        <h1>No entries yet!</h1>
      )}
    </div>
  );
};

export default page;

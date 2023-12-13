"use client";

import { useEffect, useState } from "react";
import { getUserTrainingLoad } from "@/utils/serverUtils";
import TrainingLoadChart from "@/charts/TrainingLoadChart";
import Loading from "@/app/loading";
import styles from "./trainingLoad.module.css";

export type TrainingLoadResponseType = {
  date: string;
  trainingImpulse: number;
  trainingLoad: number;
  longTermStress: number;
  longTermStressHr: number;
  longTermStressPower: number;
  shortTermStress: number;
  shortTermStressHr: number;
  shortTermStressPower: number;
  stressBalance: number;
  stressBalanceHr: number;
  stressBalancePower: number;
};

export type TrainingLoadType = {
  date: Date;
  trainingLoad: number;
  longTermStress: number;
  shortTermStress: number;
  stressBalance: number;
};

const page = () => {
  const [trainingLoad, setTrainingLoad] =
    useState<TrainingLoadResponseType[]>();
  const [dataType, setDataType] = useState<number>(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      const res = await getUserTrainingLoad();
      setTrainingLoad(res?.data);
      setLoading(false);
    };
    fetchData();
  }, []);

  return (
    <>
      {!loading ? (
        <div className={styles.TLContainer}>
          <div className={styles.source}>
            Source:
            <button
              className={styles.sourceButton}
              onClick={() => setDataType(0)}
              disabled={dataType == 0}
            >
              Mixed
            </button>
            <button
              className={styles.sourceButton}
              onClick={() => setDataType(1)}
              disabled={dataType == 1}
            >
              Power
            </button>
            <button
              className={styles.sourceButton}
              onClick={() => setDataType(2)}
              disabled={dataType == 2}
            >
              HeartRate
            </button>
          </div>
          {trainingLoad ? (
            <TrainingLoadChart data={trainingLoad} dataType={dataType} />
          ) : null}
        </div>
      ) : (
        <Loading />
      )}
    </>
  );
};

export default page;

"use client";

import { useEffect, useState } from "react";
import { getUserTrainingLoad } from "@/utils/serverUtils";
import TrainingLoadChart from "@/charts/TrainingLoadChart";
import Loading from "@/app/loading";

export type TrainingLoadResponseType = {
  date: string;
  trainingImpulse: number;
  trainingStressScore: number;
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
        <div>
          <div>
            Source:
            <div onClick={() => setDataType(0)}>Both</div>
            <div onClick={() => setDataType(1)}>Power</div>
            <div onClick={() => setDataType(2)}>HeartRate</div>
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

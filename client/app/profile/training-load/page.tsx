"use client";

import { useEffect, useState } from "react";
import { getUserTrainingLoad } from "@/utils/serverUtils";
import TrainingLoadChart from "@/charts/TrainingLoadChart";

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

  useEffect(() => {
    const fetchData = async () => {
      const res = await getUserTrainingLoad();
      setTrainingLoad(res?.data);
    };
    fetchData();
  }, []);

  return (
    <div>
      training load
      {trainingLoad ? (
        <TrainingLoadChart data={trainingLoad} dataType={dataType} />
      ) : null}
    </div>
  );
};

export default page;

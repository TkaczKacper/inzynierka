"use client";
import { useEffect } from "react";
import { getAthleteActivities } from "@/utils/serverUtils";

//TODO ogarnac to
const page = () => {
  useEffect(() => {
    async function get() {
      const res = getAthleteActivities("12-24-2023", 5);
      console.log(res);
    }
    get();
  }, []);
  return (
    <div>
      <h1>x</h1>
    </div>
  );
};

export default page;

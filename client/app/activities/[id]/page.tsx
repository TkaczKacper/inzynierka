"use client";

import { useEffect, useState } from "react";
import { getActivityDataById } from "@/utils/serverUtils";
import { Activity } from "@/app/profile/[id]/types";

const page = () => {
  const [activity, setActivity] = useState<Activity>();

  useEffect(() => {
    const activityId = Number(window.location.pathname.split("/")[2]);
    const getActivityData = async (activityId: number) => {
      const res = await getActivityDataById(activityId);
      setActivity(res?.data);
    };
    getActivityData(activityId);
  }, []);
  console.log(activity);
  return (
    <div>
      <h1>activity page</h1>
    </div>
  );
};

export default page;

"use client";

import { ProfileStats } from "@/app/profile/[id]/profileStats";
import React, { useState } from "react";
import dynamic from "next/dynamic";

const ProfileActivities = dynamic(
  () => import("@/app/profile/[id]/profileActivities"),
  {
    loading: () => <p>loading...</p>,
    ssr: false,
  },
);
const page = () => {
  const [month, setMonth] = useState<number>(new Date().getMonth() + 1);
  return (
    <div>
      <ProfileStats setMonth={setMonth} />
      <ProfileActivities month={month} />
    </div>
  );
};

export default page;

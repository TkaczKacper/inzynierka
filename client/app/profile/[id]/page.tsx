"use client";

import { ProfileStats } from "@/app/profile/[id]/profileStats";
import React, { useState } from "react";
import dynamic from "next/dynamic";

//TODO to chyba mozna usunac
const ProfileActivities = dynamic(
  () => import("@/app/profile/[id]/profileActivities"),
  {
    loading: () => <p>loading...</p>,
    ssr: false,
  },
);
const page = () => {
  const [month, setMonth] = useState<number>(new Date().getMonth() + 1);
  const [yearOffset, setYearOffset] = useState<number>(0);

  return (
    <div>
      <ProfileStats
        setMonth={setMonth}
        setYearOffset={setYearOffset}
        yearOffset={yearOffset}
      />
      <ProfileActivities month={month} yearOffset={yearOffset} />
    </div>
  );
};

export default page;

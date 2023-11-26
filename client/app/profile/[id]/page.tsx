"use client";

import { ProfileStats } from "@/app/profile/[id]/profileStats";
import { ProfileActivities } from "@/app/profile/[id]/profileActivities";
import { useState } from "react";

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

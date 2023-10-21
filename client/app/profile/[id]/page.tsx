"use client";

import { ProfileStats } from "@/app/profile/[id]/profileStats";
import { ProfileActivities } from "@/app/profile/[id]/profileActivities";

const page = () => {
  return (
    <div>
      <ProfileStats />
      <ProfileActivities />
    </div>
  );
};

export default page;

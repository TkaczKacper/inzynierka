"use client";

import React, { Suspense, useEffect, useState } from "react";
import { useUserContext } from "@/contexts/UserContextProvider";
import { getCookie } from "cookies-next";
import { getAthleteActivities } from "@/utils/serverUtils";
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

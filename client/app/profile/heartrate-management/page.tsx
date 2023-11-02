"use client";

import { HrUpdateForm } from "./updateHr";
import { useLocalStorage } from "@/hooks/useLocalStorage";

const page = () => {
  const [userHrZones, setUserHrZones] = useLocalStorage("hrZones", {});
  console.log(userHrZones);
  return (
    <>
      <div> heart rate page</div>
      {userHrZones.count}

      <HrUpdateForm />
    </>
  );
};

export default page;

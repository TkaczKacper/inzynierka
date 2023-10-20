import { useEffect, useState } from "react";
import { getAthleteActivities } from "@/utils/serverUtils";
import { Activity } from "@/app/profile/[id]/types";

export const ProfileActivities = () => {
  const [activities, setActivities] = useState<Activity[]>();
  useEffect(() => {
    const userActivities = async () => {
      const res = await getAthleteActivities();
      setActivities(res?.data);
    };
    userActivities();
  }, []);

  console.log(activities);

  return (
    <div>
      <h1>Activities</h1>
      {activities?.map((activity, index) => {
        return (
          <div key={index}>
            <h2>
              {index + 1}. {activity.title}
            </h2>
          </div>
        );
      })}
    </div>
  );
};

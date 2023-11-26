import { useEffect, useState } from "react";
import {
  getAthleteActivities,
  getAthleteActivitiesPeriod,
} from "@/utils/serverUtils";
import { Activity } from "@/app/profile/[id]/types";
import Link from "next/link";

interface props {
  month: number;
}

export const ProfileActivities = (month: props) => {
  const [activities, setActivities] = useState<Activity[]>();
  useEffect(() => {
    const userActivities = async () => {
      //@ts-ignore
      const res = await getAthleteActivitiesPeriod(month.month);
      setActivities(res?.data);
    };
    userActivities();
  }, [month]);

  console.log(activities);

  return (
    <div>
      {activities?.map((activity: Activity, index) => {
        return (
          <div key={index}>
            <h2>
              <Link href={`/activities/${activity.id}`}>
                {index + 1}. {activity.title}
              </Link>
            </h2>
          </div>
        );
      })}
    </div>
  );
};

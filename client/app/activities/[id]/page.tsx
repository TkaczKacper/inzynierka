"use client";

import { useEffect, useState } from "react";
import { getActivityDataById } from "@/utils/serverUtils";
import { Activity } from "@/app/profile/[id]/types";
import Loading from "@/app/loading";
import Link from "next/link";
import { useUserContext } from "@/contexts/UserContextProvider";
import { MapContainer, Marker, Popup, TileLayer } from "react-leaflet";
import "leaflet/dist/leaflet.css";

const page = () => {
  const [activity, setActivity] = useState<Activity>();
  const [loading, setLoading] = useState(true);
  const { userId } = useUserContext();

  useEffect(() => {
    const activityId = Number(window.location.pathname.split("/")[2]);
    const getActivityData = async (activityId: number) => {
      const res = await getActivityDataById(activityId);
      setLoading(false);
      setActivity(res?.data);
    };
    getActivityData(activityId);
  }, []);
  console.log(activity);
  return (
    <>
      {!loading ? (
        <div>
          {activity ? (
            <>
              <h1>activity page</h1>
              <div style={{ height: 593, width: 890 }}>
                <MapContainer
                  center={[activity.startLatLng[0], activity.startLatLng[1]]}
                  zoom={13}
                  scrollWheelZoom={false}
                >
                  <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                  />
                  <Marker
                    position={[
                      activity.startLatLng[0],
                      activity.startLatLng[1],
                    ]}
                  >
                    <Popup>hi</Popup>
                  </Marker>
                </MapContainer>
              </div>
            </>
          ) : (
            <>
              <h1>Activity not found.</h1>
              <Link href={`/profile/${userId}`}>Back to the profile page.</Link>
            </>
          )}
        </div>
      ) : (
        <Loading />
      )}
    </>
  );
};

export default page;

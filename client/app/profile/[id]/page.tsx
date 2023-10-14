"use client";

import React, { useEffect, useState } from "react";
import { useUserContext } from "@/contexts/UserContextProvider";
import axios from "axios";
import { getCookie } from "cookies-next";
const jwt_token = getCookie("jwtToken");
const backend_url = process.env.NEXT_PUBLIC_BACKEND_API_URL;
const page = () => {
  const [data, setData] = useState([]);
  const { userId, setUserId } = useUserContext();
  useEffect(() => {
    const userStats = async () => {
      const res = await axios.get(`${backend_url}/strava/get-athlete-stats`, {
        withCredentials: true,
        headers: {
          Authorization: typeof jwt_token === "string" ? jwt_token : "",
        },
      });
      setData(res.data);
      console.log(res);
    };
    userStats();
  }, []);

  console.log(data);
  return (
    <div>
      xd
      <p></p>
    </div>
  );
};

export default page;

"use client";

import React from "react";
import { useRouter } from "next/navigation";
import styles from "./navbar.module.css";
import Link from "next/link";
import { useUserContext } from "@/contexts/UserContextProvider";
import axios from "axios";

const backend_url = process.env.NEXT_PUBLIC_BACKEND_API_URL;
const navbar = () => {
  const router = useRouter();
  const { userId, setUserId } = useUserContext();

  const logoutHandler = async () => {
    const response = await axios.post(
      `${backend_url}/api/auth/logout`,
      {},
      {
        withCredentials: true,
        headers: {
          Authorization: userId,
        },
      },
    );
    console.log(response.data);
    if (response.status === 200) {
      setUserId("");
      router.push("/login");
    }

    return response;
  };

  return (
    <nav className={styles.navbar}>
      {!userId ? (
        <>
          <Link href={"/login"}>Login</Link>
          <Link href={"/register"}>Register</Link>
        </>
      ) : (
        <>
          <Link href={"/profile/heartrate-management"}>Hr</Link>
          <Link href={"/profile/power-management"}>Power</Link>
          <Link href={`/profile/${userId}`}>Profile</Link>
          <Link href={"/profile/connections"}>Connections</Link>
          <button onClick={logoutHandler}>Logout</button>
        </>
      )}
    </nav>
  );
};

export default navbar;

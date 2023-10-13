"use client";

import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import styles from "./navbar.module.css";
import { getCookie, setCookie, hasCookie } from "cookies-next";
import jwtDecode from "jwt-decode";
import { jwtdecoded } from "@/app/login/loginForm";
import Link from "next/link";
import { useUserContext } from "@/contexts/UserContextProvider";

const navbar = () => {
  const router = useRouter();
  const { userId, setUserId } = useUserContext();

  const logoutHandler = async () => {
    router.push("/login");
    const response = await fetch("http://localhost:5264/api/auth/logout", {
      method: "POST",
      credentials: "include",
      headers: {
        Authorization: userId,
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    })
      .then((res) => {
        if (res.status === 200) {
          router.push("/login");
        }
        return res.json();
      })
      .then((data) => {
        setUserId("");
        console.log(data);
      });
    return response;
  };

  return (
    <nav className={styles.navbar}>
      <Link href={`/profile/${userId}`}>Profile</Link>
      <Link href={"/profile/connections"}>Connections</Link>
      {!userId ? (
        <>
          <Link href={"/login"}>Login</Link>
          <Link href={"/register"}>Register</Link>
        </>
      ) : (
        <div>
          <button onClick={logoutHandler}>Logout</button>
        </div>
      )}
    </nav>
  );
};

export default navbar;

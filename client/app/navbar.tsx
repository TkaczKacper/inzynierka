"use client";

import React, { useEffect } from "react";
import { useRouter } from "next/navigation";
import styles from "./navbar.module.css";
import Cookies from "universal-cookie";

const cookies = new Cookies();

const navbar = () => {
   const router = useRouter();

   const logoutHandler = async () => {
      router.push("/login");
      const response = await fetch("http://localhost:5264/api/auth/logout", {
         method: "POST",
         credentials: "include",
         headers: {
            Authorization: cookies.get("jwtToken"),
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
            console.log(data);
         });
      return response;
   };

   return (
      <div className={styles.navbar}>
         <div onClick={() => router.push("/profile")}>profile</div>
         <button onClick={logoutHandler}>Logout</button>
      </div>
   );
};

export default navbar;

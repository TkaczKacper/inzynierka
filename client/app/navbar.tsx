"use client";

import React, { useEffect } from "react";
import { useRouter } from "next/navigation";
import styles from "./navbar.module.css";
import Cookies from "universal-cookie";

const cookies = new Cookies();

const jwt = cookies.get("jwtToken");

const navbar = () => {
   const router = useRouter();

   const logoutHandler = async () => {
      await fetch("http://localhost:5264/api/auth/logout", {
         method: "POST",
         credentials: "include",
         headers: {
            Authorization: jwt,
            Accept: "application/json",
            "Content-Type": "application/json",
         },
      })
         .then((res) => res.json())
         .then((data) => {
            console.log(data);
            return router.push("/login");
         });
   };

   return (
      <div className={styles.navbar}>
         <div>profile</div>
         <button onClick={logoutHandler}>Logout</button>
      </div>
   );
};

export default navbar;

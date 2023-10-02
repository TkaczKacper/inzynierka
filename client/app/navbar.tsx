"use client";

import React, { useEffect } from "react";
import { useRouter } from "next/navigation";
import styles from "./navbar.module.css";
import Cookies from "universal-cookie";
import jwtDecode from "jwt-decode";
import { jwtdecoded } from "./login/loginForm";

const cookies = new Cookies();

const navbar = () => {
   const router = useRouter();
   const userId: jwtdecoded = cookies.get("jwtToken")
      ? jwtDecode(cookies.get("jwtToken"))
      : { id: "", exp: 0, iat: 0, nbf: 0 };

   console.log(userId);

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
         <div onClick={() => router.push(`/profile/${userId.id}`)}>profile</div>
         <button onClick={logoutHandler}>Logout</button>
      </div>
   );
};

export default navbar;

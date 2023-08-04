"use client";

import React from "react";
import styles from "./navbar.module.css";
const jwt =
   "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjMiLCJuYmYiOjE2OTExNTcxNDcsImV4cCI6MTY5MTI0MzU0NywiaWF0IjoxNjkxMTU3MTQ3fQ.j3yiAQx2rzuUt5aM-zDBTvmjsMOmohCNJTbpB-NX-oQ";

const navbar = () => {
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
         .then((data) => console.log(data));
   };
   return (
      <div className={styles.navbar}>
         <div>profile</div>
         <button onClick={logoutHandler}>Logout</button>
      </div>
   );
};

export default navbar;

"use client";

import React, {useEffect, useState} from "react";
import { useRouter } from "next/navigation";
import styles from "./navbar.module.css";
import Cookies from "universal-cookie";
import jwtDecode from "jwt-decode";
import { jwtdecoded} from "@/app/login/loginForm";
import Link from "next/link";

const cookies = new Cookies();
const navbar = () => {
   const router = useRouter();

   const [userId , setUserId] = useState<string>();
   
   useEffect(() => {
      const jwt: jwtdecoded | undefined = cookies.get("jwtToken")
          ? jwtDecode(cookies.get("jwtToken"))
          : undefined;
        if (jwt != undefined) setUserId(jwt.id);
      }, []);
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
           setUserId(undefined)
            console.log(data);
         });
      return response;
   };

   return (
      <div className={styles.navbar}>
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
      </div>
   );
};

export default navbar;

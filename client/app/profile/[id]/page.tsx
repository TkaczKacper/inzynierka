"use client";

import React, { useEffect } from "react";
import { usePathname, useSearchParams } from "next/navigation";
import Cookies from "universal-cookie";

const cookies = new Cookies();
const jwt = cookies.get("jwtToken");

const page = () => {
   const pathname = usePathname();
   const searchParams = useSearchParams();

   useEffect(() => {
      const profileID = `${pathname.split("/").at(2)}`;
      console.log(profileID);
      fetch(`http://localhost:5264/api/auth/${profileID}/refresh-tokens`, {
         method: "GET",
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
         });
   }, [pathname, searchParams]);

   return (
      <div>
         xd
         <p></p>
      </div>
   );
};

export default page;

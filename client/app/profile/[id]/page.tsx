"use client";

import React, { useEffect, useState } from "react";
import { usePathname, useSearchParams } from "next/navigation";
import Cookies from "universal-cookie";

const cookies = new Cookies();

const page = () => {
   const pathname = usePathname();
   const searchParams = useSearchParams();
   const [data, setData] = useState([]);
   useEffect(() => {
      const getData = async () => {
         const profileID = `${pathname.split("/").at(2)}`;
         console.log(profileID);
         const response = await fetch(
            `http://localhost:5264/api/auth/${profileID}/refresh-tokens`,
            {
               method: "GET",
               credentials: "include",
               headers: {
                  Authorization: cookies.get("jwtToken"),
                  Accept: "application/json",
                  "Content-Type": "application/json",
               },
            }
         )
            .then((res) => res.json())
            .then((data) => {
               return setData(data);
            });

         return response;
      };
      getData();
   }, [pathname, searchParams]);

   console.log(data);
   return (
      <div>
         xd
         <p></p>
      </div>
   );
};

export default page;

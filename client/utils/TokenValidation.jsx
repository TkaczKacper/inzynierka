import React from "react";
import Cookies from "universal-cookie";
import jwt_decode from "jwt-decode";

const cookies = new Cookies();
const jwt = cookies.get("jwtToken");

export default TokenValidation = () => {
   if (!jwt) return false;
   try {
      const { exp } = jwt_decode(jwt);
      console.log(exp);
      return Date.now() < exp * 1000;
   } catch (err) {
      return false;
   }
};

import { NextResponse } from "next/server";
import { NextRequest } from "next/server";

import jwt_decode from "jwt-decode";
import Cookies from "universal-cookie";
import { RequestCookie } from "next/dist/compiled/@edge-runtime/cookies";

const cookies = new Cookies();

type jwtdecoded = {
   id: string;
   exp: number;
   iat: number;
   nbf: number;
};

export async function middleware(request: NextRequest) {
   const isValid = TokenValidation(request.cookies.get("jwtToken"));
   if (!isValid) {
      try {
         console.log("middleware");
         let cookie = request.cookies.get("refreshToken");
         const requestHeaders = new Headers(request.headers);
         return await fetch("http://localhost:5264/api/auth/renew-token", {
            method: "GET",
            credentials: "include",
            headers: {
               Accept: "application/json",
               "Content-Type": "application/json",
               Cookie: `${cookie?.name}=${cookie?.value}`,
               Authorization: `${requestHeaders.get("Authorization")}`,
            },
         })
            .then((res) => res.json())
            .then((data) => {
               var decoded: jwtdecoded = jwt_decode(data.jwtToken);
               var ttl = new Date(decoded.exp * 1000);
               const response = NextResponse.next();

               response.cookies.set("jwtToken", data.jwtToken, {
                  path: "/",
                  expires: ttl,
               });
               return response;
            });
      } catch (error) {}
   } else {
      return NextResponse.next();
   }
}

export const config = {
   matcher: "/profile/:path*",
};

const TokenValidation = (token: RequestCookie | undefined) => {
   if (!token) return false;
   try {
      var decoded: jwtdecoded = jwt_decode(token.value);
      var exp = decoded.exp;
      return Date.now() < exp * 1000;
   } catch (err) {
      return false;
   }
};

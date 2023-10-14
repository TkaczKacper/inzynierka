import { NextResponse } from "next/server";
import { NextRequest } from "next/server";

import jwt_decode from "jwt-decode";
import Cookies from "universal-cookie";
import { RequestCookie } from "next/dist/compiled/@edge-runtime/cookies";
import { refreshToken } from "./utils/stravaUtils";

const backend_url = process.env.NEXT_PUBLIC_BACKEND_API_URL;
const cookies = new Cookies();

type jwtdecoded = {
  id: string;
  exp: number;
  iat: number;
  nbf: number;
};

export async function middleware(request: NextRequest) {
  console.log("reload");
  const isValid = TokenValidation(request.cookies.get("jwtToken"));
  if (
    !request.cookies.get("strava_access_token") &&
    request.cookies.get("strava_refresh_token")
  ) {
    console.log("refreshing strava token");
    var xd = await refreshToken(
      request.cookies.get("strava_refresh_token")?.value,
    );
    const response = NextResponse.next();
    response.cookies.set("strava_access_token", xd.access_token, {
      path: "/",
      expires: new Date(xd.expires_at * 1000),
    });
    return response;
  }
  if (!isValid) {
    try {
      console.log("refreshing jwt token");
      let cookie = request.cookies.get("refreshToken");
      const requestHeaders = new Headers(request.headers);
      return await fetch(`${backend_url}/api/auth/renew-token`, {
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

"use client";

import {
  useState,
  createContext,
  useContext,
  PropsWithChildren,
  Dispatch,
  SetStateAction,
} from "react";

import { getCookie } from "cookies-next";
import jwtDecode from "jwt-decode";
import { jwtdecoded } from "@/app/login/loginForm";

type UserContextType = {
  userId: string;
  setUserId: Dispatch<SetStateAction<string>>;
};
export const UserContext = createContext<UserContextType>({
  userId: "",
  setUserId: () => {},
});

const jwt_token = getCookie("jwtToken");

export const UserContextProvider = ({ children }: PropsWithChildren) => {
  const id =
    typeof jwt_token === "string" ? jwtDecode<jwtdecoded>(jwt_token).id : "";

  const [userId, setUserId] = useState(id);
  return (
    <UserContext.Provider value={{ userId, setUserId }}>
      {children}
    </UserContext.Provider>
  );
};
export const useUserContext = () => useContext(UserContext);

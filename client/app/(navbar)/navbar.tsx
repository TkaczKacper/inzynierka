"use client";

import React from "react";
import { useRouter } from "next/navigation";
import styles from "./navbar.module.css";
import Link from "next/link";
import { useUserContext } from "@/contexts/UserContextProvider";
import { FaUserTie } from "react-icons/fa";
import axios from "axios";
import { Black_Ops_One } from "next/font/google";

const backend_url = process.env.NEXT_PUBLIC_BACKEND_API_URL;

const black_ops_one = Black_Ops_One({
  variable: "--font-black-ops-one",
  subsets: [],
  weight: "400",
});

const navbar = () => {
  const router = useRouter();
  const { userId, setUserId } = useUserContext();

  const logoutHandler = async () => {
    const response = await axios.post(
      `${backend_url}/api/auth/logout`,
      {},
      {
        withCredentials: true,
        headers: {
          Authorization: userId,
        },
      },
    );
    console.log(response.data);
    if (response.status === 200) {
      setUserId("");
      router.push("/login");
    }

    return response;
  };

  return (
    <nav className={styles.navbar}>
      {!userId ? (
        <>
          <div className={black_ops_one.variable}>
            <Link href={"/login"} className={styles.logo}>
              Training Analytics
            </Link>
          </div>
          <div className={styles.navRight}>
            <Link href={"/login"} className={styles.navBlock}>
              Login
            </Link>
            <Link href={"/register"} className={styles.navBlock}>
              Register
            </Link>
          </div>
        </>
      ) : (
        <>
          <div className={black_ops_one.variable}>
            <Link href={`/profile/${userId}`} className={styles.logo}>
              Training Analytics
            </Link>
          </div>
          <div className={styles.navRight}>
            <Link href={"/"} className={styles.navBlock}>
              Activities
            </Link>
            <Link href={"/profile/training-load"} className={styles.navBlock}>
              Analysis
            </Link>
            <div className={`${styles.profileDropdown} ${styles.navBlock}`}>
              <span>Profile</span>
              <div
                className={styles.dropdownContent}
                id={styles.profileDropdownContent}
              >
                <Link href={"/profile/power-management"}>Power</Link>
                <Link href={"/profile/heartrate-management"}>Hr</Link>
                <Link href={"/profile/connections"}>Connections</Link>
              </div>
            </div>
            <div className={`${styles.userDropdown} ${styles.navBlock}`}>
              <span>
                <FaUserTie />
              </span>
              <div
                className={styles.dropdownContent}
                id={styles.userDropdownContent}
              >
                <Link href={"#"}>User information</Link>
                <Link href={"#"}>Settings</Link>
                <Link href={"#"}>Password</Link>
                <Link href={`/profile/${userId}`}>Profile</Link>
                <hr style={{ borderColor: "black" }} />
                <a className={styles.link} onClick={logoutHandler}>
                  Logout
                </a>
              </div>
            </div>
          </div>
        </>
      )}
    </nav>
  );
};

export default navbar;

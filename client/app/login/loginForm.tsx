"use client";

import { Formik, Form, Field } from "formik";
import * as Yup from "yup";

import { useRouter } from "next/navigation";
import { getCookie, setCookie } from "cookies-next";
import { useEffect, useState } from "react";
import { useUserContext } from "@/contexts/UserContextProvider";
import axios, { Axios } from "axios";
import jwtDecode from "jwt-decode";
import styles from "../register/authForm.module.css";

interface FormValues {
  username: string;
  password: string;
}

export interface jwtdecoded {
  id: string;
  exp: number;
  iat: number;
  nbf: number;
}

const jwt = getCookie("jwtToken");
const backend_url = process.env.NEXT_PUBLIC_BACKEND_API_URL;

const passwordRule =
  /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!-\/:-@[-\`]).{8,32}$/;
const LoginSchema = Yup.object().shape({
  username: Yup.string()
    .min(6, "Username too short.")
    .max(32, "Username too long.")
    .required("Field required."),
  password: Yup.string()
    .min(8, "Password too short.")
    .max(32, "Password too long.")
    .matches(passwordRule, "Weak Password.")
    .required("Field required."),
});

export const LoginForm = () => {
  const router = useRouter();
  const { userId, setUserId } = useUserContext();
  const [resError, setResError] = useState("");
  const initialValues: FormValues = { username: "", password: "" };

  const submitHandler = async (values: FormValues) => {
    const res = await axios
      .post(`${backend_url}/api/auth/login`, values, {
        withCredentials: true,
      })
      .then((res) => {
        if (res.status !== 200) {
        } else {
          var decoded: jwtdecoded = jwtDecode(res.data.jwtToken);
          var ttl = new Date(decoded.exp * 1000);
          setCookie("jwtToken", res.data.jwtToken, { path: "/", expires: ttl });
          setUserId(decoded.id);
          router.push(`/profile/${decoded.id}`);
        }
      })
      .catch((err) => {
        setResError(err.response.data.message);
      });
  };

  useEffect(() => {
    if (jwt) router.push("/profile");
  }, []);

  return (
    <div className={styles.formBox}>
      <h1>Log In</h1>
      <Formik
        initialValues={initialValues}
        validationSchema={LoginSchema}
        onSubmit={(values, actions) => {
          setResError("");
          actions.setSubmitting(false);
          submitHandler(values);
        }}
      >
        {({ errors, touched }: any) => (
          <Form className={styles.authForm}>
            <Field
              className={styles.authInput}
              placeholder={"username"}
              name="username"
            />
            {errors.username && touched.username ? (
              <div className={styles.authFormError}>{errors.username}</div>
            ) : null}
            <Field
              className={styles.authInput}
              placeholder={"password"}
              name="password"
              type="password"
            />
            {errors.password && touched.password ? (
              <div className={styles.authFormError}>{errors.password}</div>
            ) : null}
            <button type="submit">Login</button>
          </Form>
        )}
      </Formik>
      {resError ? (
        <div className={styles.responseError}>
          <a>{resError}</a>
        </div>
      ) : null}
    </div>
  );
};

"use client";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";

import { setCookie } from "cookies-next";
import { jwtdecoded } from "../login/loginForm";
import { useRouter } from "next/navigation";
import axios from "axios";
import jwtDecode from "jwt-decode";
import { useUserContext } from "@/contexts/UserContextProvider";
import styles from "./authForm.module.css";
import { useState } from "react";

interface FormValues {
  username: string;
  email: string;
  password: string;
  RepeatPassword: string;
}

const backend_url = process.env.NEXT_PUBLIC_BACKEND_API_URL;

const passwordRule =
  /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!-\/:-@[-\`]).{8,32}$/;
const RegisterSchema = Yup.object().shape({
  username: Yup.string()
    .min(6, "Username too short.")
    .max(32, "Username too long.")
    .required("Field required."),
  email: Yup.string().email("Invalid email.").required("Field required."),
  password: Yup.string()
    .min(8, "Password too short.")
    .max(32, "Password too long.")
    .matches(passwordRule, "Weak Password.")
    .required("Field required."),
  RepeatPassword: Yup.string()
    .oneOf([Yup.ref("password")], "Passwords not match.")
    .required("Field required."),
});

export const RegisterForm: React.FC<{}> = () => {
  const initialValues: FormValues = {
    username: "",
    email: "",
    password: "",
    RepeatPassword: "",
  };
  const router = useRouter();
  const { userId, setUserId } = useUserContext();
  const [resError, setResError] = useState("");

  const submitHandler = async (values: FormValues) => {
    const res = await axios
      .post(`${backend_url}/api/auth/register`, values, {
        withCredentials: true,
      })
      .then((res) => {
        var decoded: jwtdecoded = jwtDecode(res.data.jwtToken);
        var ttl = new Date(decoded.exp * 1000);
        setCookie("jwtToken", res.data.jwtToken, { path: "/", expires: ttl });
        setUserId(decoded.id);
        router.push(`profile/${decoded.id}`);
      })
      .catch((err) => {
        setResError(err.response.data.message);
      });
  };

  return (
    <div className={styles.formBox}>
      <h1>Sign Up</h1>
      <Formik
        initialValues={initialValues}
        validationSchema={RegisterSchema}
        onSubmit={(values, actions) => {
          setResError("");
          actions.setSubmitting(false);
          submitHandler(values);
        }}
      >
        {({ errors, touched }: any) => (
          <Form className={styles.authForm}>
            <Field name="username" placeholder={"username"} />
            {errors.username && touched.username ? (
              <div className={styles.authFormError}>{errors.username}</div>
            ) : null}
            <Field name="email" placeholder={"email"} />
            {errors.email && touched.email ? (
              <div className={styles.authFormError}>{errors.email}</div>
            ) : null}
            <Field name="password" type="password" placeholder={"password"} />
            {errors.password && touched.password ? (
              <div className={styles.authFormError}>{errors.password}</div>
            ) : null}
            <Field
              name="RepeatPassword"
              type="password"
              placeholder={"password"}
            />
            {errors.RepeatPassword && touched.RepeatPassword ? (
              <div className={styles.authFormError}>
                {errors.RepeatPassword}
              </div>
            ) : null}
            <button type="submit">Register</button>
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

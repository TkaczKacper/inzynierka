"use client";

import { Formik, Form, Field } from "formik";
import * as Yup from "yup";

import { useRouter } from "next/navigation";
import { getCookie, setCookie } from "cookies-next";
import { useEffect } from "react";
import { useUserContext } from "@/contexts/UserContextProvider";
import axios, { Axios } from "axios";
import jwtDecode from "jwt-decode";

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
  const initialValues: FormValues = { username: "", password: "" };
  const submitHandler = async (values: FormValues) => {
    const res = await axios.post(`${backend_url}/api/auth/login`, values, {
      withCredentials: true,
    });
    console.log(res.data);
    var decoded: jwtdecoded = jwtDecode(res.data.jwtToken);
    var ttl = new Date(decoded.exp * 1000);
    setCookie("jwtToken", res.data.jwtToken, { path: "/", expires: ttl });
    setUserId(decoded.id);
    router.push(`/profile/${decoded.id}`);
  };

  useEffect(() => {
    if (jwt) router.push("/profile");
  }, []);

  return (
    <Formik
      initialValues={initialValues}
      validationSchema={LoginSchema}
      onSubmit={(values, actions) => {
        console.log(values);
        actions.setSubmitting(false);
        submitHandler(values);
      }}
    >
      {({ errors, touched }: any) => (
        <Form>
          <Field name="username" />
          {errors.username && touched.username ? (
            <div>{errors.username}</div>
          ) : null}
          <Field name="password" type="password" />
          {errors.password && touched.password ? (
            <div>{errors.password}</div>
          ) : null}
          <button type="submit">Login</button>
        </Form>
      )}
    </Formik>
  );
};

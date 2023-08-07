"use client";

import { Formik, Form, Field } from "formik";
import * as Yup from "yup";

import jwt_decode from "jwt-decode";
import Cookies from "universal-cookie";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

interface FormValues {
   username: string;
   password: string;
}

interface jwtdecoded {
   id: string;
   exp: number;
   iat: number;
   nbf: number;
}

const cookies = new Cookies();
const jwt = cookies.get("jwtToken");

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

export const LoginForm: React.FC<{}> = () => {
   const initialValues: FormValues = { username: "", password: "" };
   const submitHandler = async (values: FormValues) => {
      await fetch("http://localhost:5264/api/auth/login", {
         method: "POST",
         credentials: "include",
         headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
         },
         body: JSON.stringify(values),
      })
         .then((res) => res.json())
         .then((data) => {
            console.log(data);
            var decoded: jwtdecoded = jwt_decode(data.jwtToken);
            var ttl = new Date(decoded.exp * 1000);
            cookies.set("jwtToken", data.jwtToken, { path: "/", expires: ttl });
            router.push(`/profile/${decoded.id}`);
         });
      console.log(JSON.stringify(values));
   };

   const router = useRouter();

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

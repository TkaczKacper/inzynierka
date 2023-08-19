"use client";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";

import Cookies from "universal-cookie";
import jwt_decode from "jwt-decode";
import { jwtdecoded } from "../login/loginForm";
import { useRouter } from "next/navigation";

interface FormValues {
   username: string;
   email: string;
   password: string;
   RepeatPassword: string;
}

const cookies = new Cookies();

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
   const submitHandler = async (values: FormValues) => {
      await fetch("http://localhost:5264/api/auth/register", {
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
   };

   const router = useRouter();

   return (
      <div>
         <Formik
            initialValues={initialValues}
            validationSchema={RegisterSchema}
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
                  <Field name="email" />
                  {errors.email && touched.email ? (
                     <div>{errors.email}</div>
                  ) : null}
                  <Field name="password" type="password" />
                  {errors.password && touched.password ? (
                     <div>{errors.password}</div>
                  ) : null}
                  <Field name="RepeatPassword" type="password" />
                  {errors.RepeatPassword && touched.RepeatPassword ? (
                     <div>{errors.RepeatPassword}</div>
                  ) : null}
                  <button type="submit">Register</button>
               </Form>
            )}
         </Formik>
      </div>
   );
};

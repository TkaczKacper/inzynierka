"use client";

import { Formik, Form, Field } from "formik";
import * as Yup from "yup";

interface FormValues {
   username: string;
   password: string;
}

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

   return (
      <Formik
         initialValues={initialValues}
         validationSchema={LoginSchema}
         onSubmit={(values, actions) => {
            console.log(values);
            actions.setSubmitting(false);
         }}
      >
         {({ errors, touched }: any) => (
            <Form>
               <Field name="username" />
               {errors.username && touched.username ? (
                  <div>{errors.username}</div>
               ) : null}
               <Field name="password" />
               {errors.password && touched.password ? (
                  <div>{errors.password}</div>
               ) : null}
               <button type="submit">Login</button>
            </Form>
         )}
      </Formik>
   );
};

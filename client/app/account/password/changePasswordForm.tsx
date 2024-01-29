"use client";

import * as Yup from "yup";
import { Field, Form, Formik } from "formik";
import axios from "axios";
import { getCookie } from "cookies-next";

interface FormValues {
  oldPassword: string;
  newPassword: string;
  repeatPassword: string;
}

const backend_url = process.env.NEXT_PUBLIC_BACKEND_API_URL;

const passwordRule =
  /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!-\/:-@[-\`]).{8,32}$/;

const changePasswordSchema = Yup.object().shape({
  oldPassword: Yup.string()
    .min(8, "Too short.")
    .max(32, "Too long.")
    .matches(passwordRule, "Weak password.")
    .required("Field required."),
  newPassword: Yup.string()
    .min(8, "Too short.")
    .max(32, "Too long.")
    .matches(passwordRule, "Weak password.")
    .required("Field required."),
  repeatPassword: Yup.string()
    .oneOf([Yup.ref("newPassword")], "Passwords not match.")
    .required("Field required."),
});

//TODO calosc z walidacja itp
export const ChangePasswordForm: React.FC<{}> = () => {
  const initialValues: FormValues = {
    oldPassword: "",
    newPassword: "",
    repeatPassword: "",
  };

  const submitHandler = async (values: FormValues) => {
    console.log(values);
    const res = await axios.put(
      `${backend_url}/api/auth/change-password`,
      values,
      {
        withCredentials: true,
        headers: {
          Authorization: getCookie("jwtToken"),
        },
      },
    );
  };

  return (
    <>
      <Formik
        initialValues={initialValues}
        validationSchema={changePasswordSchema}
        onSubmit={(values, actions) => {
          submitHandler(values);
        }}
      >
        {({ errors, touched }: any) => (
          <Form>
            <Field name="oldPassword" type="password" />
            {errors.oldPassword && touched.oldPassword ? (
              <div>{errors.oldPassword}</div>
            ) : null}
            <Field name="newPassword" type="password" />
            {errors.newPassword && touched.newPassword ? (
              <div>{errors.newPassword}</div>
            ) : null}
            <Field name="repeatPassword" type="password" />
            {errors.repeatPassword && touched.repeatPassword ? (
              <div>{errors.repeatPassword}</div>
            ) : null}
            <button type="submit">Change password</button>
          </Form>
        )}
      </Formik>
    </>
  );
};

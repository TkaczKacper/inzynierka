"use client";
import { Formik, Form, Field } from "formik";

import Cookies from "universal-cookie";
import { useRouter } from "next/navigation";
import { jwtdecoded } from "@/app/login/loginForm";
import jwtDecode from "jwt-decode";
import axios from "axios";
import { updateFtp } from "@/utils/serverUtils";

interface FormValues {
   ftp: number;
}

export const FtpUpdateForm: React.FC<{}> = () => {
   const initialValues: FormValues = {
      ftp: 0,
   };
   const submitHandler = async (values: FormValues) => {
      const response = await updateFtp(values.ftp);
      console.log(response);
      return response;
   };

   return (
      <div>
         <Formik
            initialValues={initialValues}
            onSubmit={(values, { setSubmitting }) => {
               console.log(values);
               setSubmitting(true);
               submitHandler(values);
            }}
         >
            {({ isSubmitting }) => (
               <Form>
                  <Field name="ftp" />
                  <button type="submit" disabled={isSubmitting}>
                     Update
                  </button>
               </Form>
            )}
         </Formik>
      </div>
   );
};

"use client";
import { Formik, Form, Field } from "formik";

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

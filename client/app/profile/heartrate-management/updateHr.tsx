"use client";
import { Formik, Form, Field } from "formik";

import { updateHr } from "@/utils/serverUtils";

interface FormValues {
   hrRest: number;
   hrMax: number;
}

export const HrUpdateForm: React.FC<{}> = () => {
   const initialValues: FormValues = {
      hrRest: 0,
      hrMax: 0,
   };
   const submitHandler = async (values: FormValues) => {
      const response = await updateHr(values.hrRest, values.hrMax);
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
                  <Field name="hrRest" />
                  <Field name="hrMax" />
                  <button type="submit" disabled={isSubmitting}>
                     Update
                  </button>
               </Form>
            )}
         </Formik>
      </div>
   );
};

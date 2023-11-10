"use client";
import { Formik, Form, Field } from "formik";

import { updateHr } from "@/utils/serverUtils";
import { hrZonesType } from "@/app/profile/heartrate-management/page";

interface FormValues {
  hrRest: number;
  hrMax: number;
}

export const HrUpdateForm = ({ data, setData }: any) => {
  const initialValues: FormValues = {
    hrRest: 0,
    hrMax: 0,
  };
  const submitHandler = async (values: FormValues) => {
    const response = await updateHr(values.hrRest, values.hrMax);
    console.log(response);
    if (response?.data) {
      setData([...data, response.data]);
    }
    return response;
  };

  return (
    <div>
      <Formik
        initialValues={initialValues}
        onSubmit={async (values, { setSubmitting }) => {
          console.log(values);
          setSubmitting(true);
          await submitHandler(values);
          setSubmitting(false);
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

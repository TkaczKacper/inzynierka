"use client";
import { Formik, Form, Field } from "formik";

import { updateFtp } from "@/utils/serverUtils";

interface FormValues {
  ftp: number;
}

export const FtpUpdateForm = ({ data, setData }: any) => {
  const initialValues: FormValues = {
    ftp: 0,
  };
  const submitHandler = async (values: FormValues) => {
    const response = await updateFtp(values.ftp);
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
            FTP: <Field name="ftp" />
            <button type="submit" disabled={isSubmitting}>
              Update
            </button>
          </Form>
        )}
      </Formik>
    </div>
  );
};

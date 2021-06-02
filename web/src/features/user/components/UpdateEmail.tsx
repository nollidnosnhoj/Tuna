import { Button } from "@chakra-ui/react";
import React from "react";
import * as yup from "yup";
import { useFormik } from "formik";
import TextInput from "~/components/form/inputs/TextInput";
import { useUser } from "~/features/user/hooks";
import { validationMessages, errorToast, toast } from "~/utils";
import api from "~/lib/api";

export default function UpdateEmail() {
  const { user, updateUser } = useUser();

  const formik = useFormik<{ email: string }>({
    initialValues: { email: user?.email ?? "" },
    validationSchema: yup.object().shape({
      email: yup
        .string()
        .required(validationMessages.required("Email"))
        .email("Email is invalid"),
    }),
    onSubmit: async (values, { setSubmitting }) => {
      const { email: newEmail } = values;
      if (newEmail.trim() === user?.email) return;

      try {
        await api.patch("me/email", { newEmail });
        toast("success", {
          title: "Email updated.",
          description: "You have successfully updated your email.",
        });
        if (user) {
          updateUser({ ...user, email: newEmail.trim() });
        }
      } catch (err) {
        errorToast(err);
      } finally {
        setSubmitting(false);
      }
    },
  });

  const { errors, values, handleSubmit, handleChange, isSubmitting } = formik;

  return (
    <form onSubmit={handleSubmit}>
      <TextInput
        name="email"
        value={values.email}
        onChange={handleChange}
        error={errors.email}
        label="Change Email"
        required
      />
      <Button
        type="submit"
        isLoading={isSubmitting}
        disabled={isSubmitting}
        loadingText="Submitting..."
      >
        Update Email
      </Button>
    </form>
  );
}

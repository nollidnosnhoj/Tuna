import React from "react";
import { Button } from "@chakra-ui/react";
import * as yup from "yup";
import { useFormik } from "formik";
import TextInput from "~/components/form/TextInput";
import api from "~/lib/api";
import { validationMessages, errorToast } from "~/utils";
import { useAuth } from "~/lib/hooks/useAuth";
import { passwordRule } from "../schemas";

type UpdatePasswordValues = {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
};

export default function UpdatePassword() {
  const { logout } = useAuth();
  const formik = useFormik<UpdatePasswordValues>({
    initialValues: {
      currentPassword: "",
      newPassword: "",
      confirmPassword: "",
    },
    validationSchema: yup.object().shape({
      currentPassword: yup
        .string()
        .required(validationMessages.required("Current Password")),
      newPassword: passwordRule("New Password"),
      confirmPassword: yup
        .string()
        .required()
        .oneOf([yup.ref("newPassword")], "Password does not match."),
    }),
    onSubmit: async (values, { resetForm, setSubmitting }) => {
      const { currentPassword, newPassword } = values;
      try {
        await api.patch("me/password", {
          currentPassword: currentPassword,
          newPassword: newPassword,
        });
        resetForm();
        await logout();
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
        name="currentPassword"
        value={values.currentPassword}
        onChange={handleChange}
        error={errors.currentPassword}
        type="password"
        label="Current Password"
        required
      />
      <TextInput
        name="newPassword"
        type="password"
        value={values.newPassword}
        onChange={handleChange}
        error={errors.newPassword}
        label="New Password"
        required
      />
      <TextInput
        name="confirmPassword"
        type="password"
        value={values.confirmPassword}
        onChange={handleChange}
        error={errors.confirmPassword}
        label="Confirm Password"
        required
      />
      <Button
        type="submit"
        isLoading={isSubmitting}
        disabled={isSubmitting}
        loadingText="Submitting..."
      >
        Update Password
      </Button>
    </form>
  );
}

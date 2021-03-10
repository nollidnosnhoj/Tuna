import React from "react";
import { Button } from "@chakra-ui/react";
import * as yup from "yup";
import { useFormik } from "formik";
import TextInput from "~/components/Form/TextInput";
import useUser from "~/contexts/userContext";
import api from "~/utils/api";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import { usernameRule } from "../schemas";

export default function UpdateUsername() {
  const { user, updateUser } = useUser();

  const formik = useFormik<{ username: string }>({
    initialValues: { username: user?.username ?? "" },
    validationSchema: yup.object().shape({
      username: usernameRule("Username").notOneOf(
        [user?.username],
        "Username cannot be the same."
      ),
    }),
    onSubmit: async (values, { setSubmitting }) => {
      const { username: newUsername } = values;
      if (newUsername.toLowerCase() === user?.username) return;

      try {
        await api.patch("me/username", { newUsername });
        successfulToast({
          title: "Username updated.",
          message: "You have successfully updated your username.",
        });
        if (user) {
          updateUser({ ...user, username: newUsername });
        }
      } catch (err) {
        apiErrorToast(err);
      } finally {
        setSubmitting(false);
      }
    },
  });

  const { errors, values, handleSubmit, handleChange, isSubmitting } = formik;

  return (
    <form onSubmit={handleSubmit}>
      <TextInput
        name="username"
        value={values.username}
        onChange={handleChange}
        error={errors.username}
        label="Change Username"
        required
      />
      <Button
        type="submit"
        isLoading={isSubmitting}
        disabled={isSubmitting}
        loadingText="Submitting..."
      >
        Update Username
      </Button>
    </form>
  );
}

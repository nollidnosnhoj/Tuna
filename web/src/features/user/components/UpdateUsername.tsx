import React from "react";
import { Button } from "@chakra-ui/react";
import * as yup from "yup";
import { useFormik } from "formik";
import TextInput from "~/components/form/inputs/TextInput";
import { useUser } from "~/features/user/hooks";
import { errorToast, toast } from "~/utils";
import { usernameRule } from "../schemas";
import request from "~/lib/http";

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
        await request({
          method: "patch",
          url: "me/username",
          data: { newUsername },
        });
        toast("success", {
          title: "Username updated.",
          description: "You have successfully updated your username.",
        });
        if (user) {
          updateUser({ ...user, username: newUsername });
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

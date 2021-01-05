import { Box, Button, Stack } from "@chakra-ui/react";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import { useForm } from "react-hook-form";
import InputField from "~/components/InputField";
import useUser from "~/lib/contexts/user_context";
import request from "~/lib/request";
import { apiErrorToast } from "~/utils/toast";
import React from "react";
import { usernameRule } from "~/utils/validators";

export default function UpdateUsername() {
  const { user, updateUser } = useUser();

  const updateUsername = async (values: { username: string }) => {
    const { username } = values;
    if (username.toLowerCase() === user.username) return;

    try {
      await request("me/change-username", { method: "patch", body: username });
      updateUser({ ...user, username: username });
    } catch (err) {
      apiErrorToast(err);
    }
  };

  const {
    handleSubmit,
    errors,
    register,
    formState: { isSubmitting, isValid },
  } = useForm<{ username: string }>({
    defaultValues: { username: user.username },
    mode: "onChange",
    resolver: yupResolver(
      yup.object().shape({
        username: usernameRule("Username", true).notOneOf(
          [user.username],
          "Username cannot be the same."
        ),
      })
    ),
  });

  return (
    <form onSubmit={handleSubmit(updateUsername)}>
      <InputField
        name="username"
        label="Change Username"
        ref={register}
        isRequired
        error={errors.username}
      />
      <Button
        type="submit"
        isLoading={isSubmitting}
        disabled={isSubmitting || !isValid}
        loadingText="Submitting..."
      >
        Update Username
      </Button>
    </form>
  );
}

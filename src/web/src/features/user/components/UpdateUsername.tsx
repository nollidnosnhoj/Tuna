import React from "react";
import { Button } from "@chakra-ui/react";
import * as yup from "yup";
import TextInput from "~/components/Forms/Inputs/Text";
import { useUser } from "~/features/user/hooks";
import { errorToast, toast } from "~/utils";
import { usernameRule } from "../schemas";
import request from "~/lib/http";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";

export default function UpdateUsername() {
  const { user, updateUser } = useUser();
  const {
    handleSubmit,
    register,
    formState: { errors, isSubmitting },
  } = useForm<{ username: string }>({
    resolver: yupResolver(
      yup.object().shape({
        username: usernameRule("Username").notOneOf(
          [user?.userName],
          "Username cannot be the same."
        ),
      })
    ),
  });

  const handleUsernameSubmit = async (values: { username: string }) => {
    const { username: newUsername } = values;
    if (newUsername.toLowerCase() === user?.userName) return;

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
        updateUser({ ...user, userName: newUsername });
      }
    } catch (err) {
      errorToast(err);
    }
  };

  return (
    <form onSubmit={handleSubmit(handleUsernameSubmit)}>
      <TextInput
        {...register("username")}
        error={errors.username?.message}
        label="Change Username"
        isRequired
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

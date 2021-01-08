import { Button } from "@chakra-ui/react";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import { useForm } from "react-hook-form";
import TextInput from "~/components/Form/TextInput";
import request from "~/lib/request";
import { apiErrorToast } from "~/utils/toast";
import React from "react";
import { passwordRule } from "~/utils/validators";
import { validationMessages } from "~/utils";

type UpdatePasswordValues = {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
};

const defaultValues = {
  currentPassword: "",
  newPassword: "",
  confirmPassword: "",
};

export default function UpdatePassword() {
  const {
    handleSubmit,
    errors,
    register,
    reset,
    formState: { isSubmitting, isValid },
  } = useForm<UpdatePasswordValues>({
    defaultValues: defaultValues,
    resolver: yupResolver(
      yup.object().shape({
        currentPassword: yup
          .string()
          .required(validationMessages.required("Current Password")),
        newPassword: passwordRule("New Password", true),
        confirmPassword: yup
          .string()
          .required()
          .oneOf([yup.ref("newPassword")], "Password does not match."),
      })
    ),
  });

  const updatePassword = async (values: UpdatePasswordValues) => {
    const { currentPassword, newPassword } = values;
    try {
      await request("me/change-password", {
        method: "post",
        body: {
          currentPassword: currentPassword,
          newPassword: newPassword,
        },
      });
      reset(defaultValues);
    } catch (err) {
      apiErrorToast(err);
    }
  };

  return (
    <form onSubmit={handleSubmit(updatePassword)}>
      <TextInput
        name="currentPassword"
        label="Current Password"
        ref={register}
        isRequired
        error={errors.currentPassword}
      />
      <TextInput
        name="newPassword"
        type="password"
        label="New Password"
        ref={register}
        isRequired
        error={errors.currentPassword}
      />
      <TextInput
        name="confirmPassword"
        type="password"
        label="Confirm Password"
        ref={register}
        isRequired
        error={errors.currentPassword}
      />
      <Button
        type="submit"
        isLoading={isSubmitting}
        disabled={isSubmitting || !isValid}
        loadingText="Submitting..."
      >
        Update Password
      </Button>
    </form>
  );
}

import React from "react";
import { Button } from "@chakra-ui/react";
import { useForm, FormProvider } from "react-hook-form";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import TextInput from "~/components/Form/TextInput";
import request from "~/lib/request";
import { passwordRule } from "~/lib/validationSchemas";
import { apiErrorToast } from "~/utils/toast";
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
  const methods = useForm<UpdatePasswordValues>({
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

  const {
    handleSubmit,
    reset,
    formState: { isSubmitting },
  } = methods;

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
    <FormProvider {...methods}>
      <form onSubmit={handleSubmit(updatePassword)}>
        <TextInput
          name="currentPassword"
          type="password"
          label="Current Password"
          required
        />
        <TextInput
          name="newPassword"
          type="password"
          label="New Password"
          required
        />
        <TextInput
          name="confirmPassword"
          type="password"
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
    </FormProvider>
  );
}

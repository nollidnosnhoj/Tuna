import { Button, Stack } from "@chakra-ui/react";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import { FormProvider, useForm } from "react-hook-form";
import TextInput from "~/components/Form/TextInput";
import useUser from "~/lib/contexts/user_context";
import request from "~/lib/request";
import { apiErrorToast } from "~/utils/toast";
import React from "react";
import { validationMessages } from "~/utils";

export default function UpdateEmail() {
  const { user, updateUser } = useUser();

  const updateEmail = async (values: { email: string }) => {
    const { email } = values;
    if (email.trim() === user?.email) return;

    try {
      await request("me/change-email", { method: "patch", body: email });
      updateUser({ ...user, email: email });
    } catch (err) {
      apiErrorToast(err);
    }
  };

  const methods = useForm<{ email: string }>({
    defaultValues: { email: user?.email },
    resolver: yupResolver(
      yup.object().shape({
        email: yup
          .string()
          .required(validationMessages.required("Email"))
          .email("Email is invalid."),
      })
    ),
  });

  const {
    handleSubmit,
    formState: { isSubmitting },
  } = methods;

  return (
    <FormProvider {...methods}>
      <form onSubmit={handleSubmit(updateEmail)}>
        <TextInput name="email" label="Change Email" required />
        <Button
          type="submit"
          isLoading={isSubmitting}
          disabled={isSubmitting}
          loadingText="Submitting..."
        >
          Update Email
        </Button>
      </form>
    </FormProvider>
  );
}

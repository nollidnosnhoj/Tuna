import React from "react";
import { Button, Stack } from "@chakra-ui/react";
import { FormProvider, useForm } from "react-hook-form";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import TextInput from "../Form/TextInput";
import useUser from "~/lib/contexts/user_context";
import { apiErrorToast, successfulToast } from "~/utils/toast";

export type LoginFormValues = {
  username: string;
  password: string;
};

interface LoginFormProps {
  onSuccess?: () => void;
}

export default function LoginForm(props: LoginFormProps) {
  const { login } = useUser();

  const methods = useForm<LoginFormValues>({
    resolver: yupResolver(
      yup.object().shape({
        username: yup.string().required(),
        password: yup.string().required(),
      })
    ),
  });

  const {
    handleSubmit,
    formState: { isSubmitting },
  } = methods;

  const onSubmit = async (values: LoginFormValues) => {
    try {
      await login(values);
      successfulToast({ message: "You have logged in successfully. " });
      if (props.onSuccess) props.onSuccess();
    } catch (err) {
      apiErrorToast(err);
    }
  };

  return (
    <FormProvider {...methods}>
      <form onSubmit={handleSubmit(onSubmit)}>
        <TextInput name="username" label="Username/Email" required />
        <TextInput name="password" type="password" label="Password" required />
        <Button
          marginTop={4}
          width="100%"
          type="submit"
          isLoading={isSubmitting}
          colorScheme="primary"
        >
          Login
        </Button>
      </form>
    </FormProvider>
  );
}

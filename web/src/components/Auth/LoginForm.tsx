import React from "react";
import { useForm } from "react-hook-form";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import useUser from "~/lib/contexts/user_context";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import InputField from "../InputField";
import { Button, Flex, flexboxParser, Stack } from "@chakra-ui/react";
import AuthButton from "./AuthButton";

export type LoginFormValues = {
  username: string;
  password: string;
};

interface LoginFormProps {
  onSuccess?: () => void;
}

export default function LoginForm(props: LoginFormProps) {
  const { login } = useUser();

  const {
    register,
    handleSubmit,
    errors,
    formState: { isSubmitting },
  } = useForm<LoginFormValues>({
    resolver: yupResolver(
      yup.object().shape({
        username: yup.string().required(),
        password: yup.string().required(),
      })
    ),
  });

  const onSubmit = async (values: LoginFormValues) => {
    try {
      await login(values);
      successfulToast({ message: "You have logged in successfully. " });
      props.onSuccess();
    } catch (err) {
      apiErrorToast(err);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <InputField
        name="username"
        label="Username/Email"
        ref={register}
        error={errors.username}
        isRequired
      />
      <InputField
        name="password"
        type="password"
        label="Password"
        ref={register}
        error={errors.password}
        isRequired
      />
      <Stack mt={4} spacing={4}>
        <Button type="submit" isLoading={isSubmitting} colorScheme="primary">
          Login
        </Button>
      </Stack>
    </form>
  );
}

import React from "react";
import { useForm } from "react-hook-form";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import useUser from "~/lib/contexts/user_context";
import { apiErrorToast } from "~/utils/toast";
import InputField from "../InputField";
import { Button, Flex } from "@chakra-ui/react";

export type LoginFormValues = {
  username: string;
  password: string;
};

export default function LoginForm() {
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
      <Flex justify="flex-end">
        <Button type="submit" mt={4} isLoading={isSubmitting}>
          Login
        </Button>
      </Flex>
    </form>
  );
}

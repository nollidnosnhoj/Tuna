import React, { useState } from "react";
import { Alert, Box, Button, CloseButton, Flex, Text } from "@chakra-ui/react";
import * as yup from "yup";
import TextInput from "~/components/Forms/Inputs/Text";
import { validationMessages } from "~/utils";
import { toast, isAxiosError } from "~/utils";
import { ErrorResponse } from "~/lib/types";
import { usernameRule, passwordRule } from "~/features/user/schemas";
import request from "~/lib/http";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";

type RegisterFormInputs = {
  username: string;
  password: string;
  email: string;
  confirmPassword: string;
};

const schema: yup.SchemaOf<RegisterFormInputs> = yup
  .object()
  .shape({
    username: usernameRule("Username"),
    password: passwordRule("Password"),
    email: yup
      .string()
      .required(validationMessages.required("Email"))
      .email()
      .defined(),
    confirmPassword: yup
      .string()
      .oneOf([yup.ref("password")], "Password does not match.")
      .defined(),
  })
  .defined();

interface RegisterFormProps {
  initialRef?: React.RefObject<HTMLInputElement>;
  onSuccess?: () => void;
}

export default function RegisterForm(props: RegisterFormProps) {
  const [error, setError] = useState("");
  const formik = useForm<RegisterFormInputs>({
    resolver: yupResolver(schema),
  });

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = formik;

  const handleRegisterSubmit = async (values: RegisterFormInputs) => {
    const registrationRequest = {
      username: values.username,
      password: values.password,
      email: values.email,
    };

    try {
      await request({
        method: "post",
        url: "auth/register",
        data: registrationRequest,
      });
      toast("success", {
        title: "Thank you for registering.",
        description: "You can now login to your account.",
      });
      if (props.onSuccess) props.onSuccess();
    } catch (err) {
      let errorMessage = "An error has occurred.";
      if (isAxiosError<ErrorResponse>(err)) {
        errorMessage = err.response?.data.message ?? errorMessage;
      }
      setError(errorMessage);
    }
  };

  return (
    <Box>
      {error && (
        <Alert status="error">
          <Box>{error}</Box>
          <CloseButton
            onClick={() => setError("")}
            position="absolute"
            right="8px"
            top="8px"
          />
        </Alert>
      )}
      <form onSubmit={handleSubmit(handleRegisterSubmit)}>
        <TextInput
          {...register("username")}
          ref={props.initialRef}
          error={errors.username?.message}
          label="Username"
          isRequired
        />
        <TextInput
          {...register("email")}
          error={errors.email?.message}
          label="Email"
          isRequired
        />
        <TextInput
          type="password"
          error={errors.password?.message}
          label="Password"
          isRequired
          {...register("password")}
        />
        <TextInput
          type="password"
          error={errors.confirmPassword?.message}
          label="Confirm Password"
          isRequired
          {...register("confirmPassword")}
        />
        <Text fontSize="sm">
          By registering, you agree to our terms and service.
        </Text>
        <Flex justify="flex-end">
          <Button
            marginTop={4}
            width="100%"
            type="submit"
            isLoading={isSubmitting}
            colorScheme="primary"
          >
            Register
          </Button>
        </Flex>
      </form>
    </Box>
  );
}

import React, { useState } from "react";
import { Alert, Box, Button, CloseButton, Flex, Text } from "@chakra-ui/react";
import * as yup from "yup";
import { useFormik } from "formik";
import TextInput from "~/components/form-inputs/TextInput";
import { validationMessages } from "~/utils";
import { toast, isAxiosError } from "~/utils";
import { ErrorResponse } from "~/lib/types";
import { usernameRule, passwordRule } from "~/features/user/schemas";
import request from "~/lib/http";

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
  const formik = useFormik<RegisterFormInputs>({
    initialValues: {
      username: "",
      password: "",
      email: "",
      confirmPassword: "",
    },
    validationSchema: schema,
    onSubmit: async (values, { setSubmitting }) => {
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
      } finally {
        setSubmitting(false);
      }
    },
  });

  const { values, errors, handleChange, handleSubmit, isSubmitting } = formik;

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
      <form onSubmit={handleSubmit}>
        <TextInput
          name="username"
          value={values.username}
          onChange={handleChange}
          error={errors.username}
          label="Username"
          focusRef={props.initialRef}
          required
        />
        <TextInput
          name="email"
          value={values.email}
          onChange={handleChange}
          error={errors.email}
          label="Email"
          required
        />
        <TextInput
          name="password"
          type="password"
          value={values.password}
          onChange={handleChange}
          error={errors.password}
          label="Password"
          required
        />
        <TextInput
          name="confirmPassword"
          type="password"
          value={values.confirmPassword}
          onChange={handleChange}
          error={errors.confirmPassword}
          label="Confirm Password"
          required
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

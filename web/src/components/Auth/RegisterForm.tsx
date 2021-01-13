import React from "react";
import { Button, Checkbox, Flex } from "@chakra-ui/react";
import { useForm } from "react-hook-form";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import TextInput from "../Form/TextInput";
import { passwordRule, usernameRule } from "~/lib/validationSchemas";
import request from "~/lib/request";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import { validationMessages } from "~/utils";

type RegisterFormInputs = {
  username: string;
  password: string;
  email: string;
  confirmPassword: string;
  acceptTerms: boolean;
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
    acceptTerms: yup
      .boolean()
      .oneOf([true], validationMessages.required("Terms of Service"))
      .defined(),
  })
  .defined();

export default function RegisterForm() {
  const {
    register,
    handleSubmit,
    errors,
    formState: { isSubmitting },
  } = useForm<RegisterFormInputs>({
    defaultValues: {
      username: "",
      password: "",
      email: "",
      confirmPassword: "",
      acceptTerms: false,
    },
    resolver: yupResolver(schema),
  });

  const onSubmit = async (values: RegisterFormInputs) => {
    const registrationRequest = {
      username: values.username,
      password: values.password,
      email: values.email,
    };

    try {
      await request("auth/register", {
        method: "post",
        body: registrationRequest,
      });

      successfulToast({
        title: "Thank you for registering.",
        message: "You can now login to your account.",
      });
    } catch (err) {
      apiErrorToast(err);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <TextInput
        name="username"
        label="Username"
        ref={register}
        error={errors.username}
        isRequired
      />
      <TextInput
        name="email"
        label="Email"
        ref={register}
        error={errors.email}
        isRequired
      />
      <TextInput
        name="password"
        type="password"
        label="Password"
        ref={register}
        error={errors.password}
        isRequired
      />
      <TextInput
        name="confirmPassword"
        type="password"
        label="Confirm Password"
        ref={register}
        error={errors.confirmPassword}
        isRequired
      />
      <Checkbox
        name="acceptTerms"
        ref={register}
        isInvalid={!!errors.acceptTerms}
      >
        I agree to the terms of service.
      </Checkbox>
      <Flex justify="flex-end">
        <Button type="submit" mt={4} isLoading={isSubmitting}>
          Register
        </Button>
      </Flex>
    </form>
  );
}

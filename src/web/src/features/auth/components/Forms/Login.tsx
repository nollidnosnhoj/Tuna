import React, { useState } from "react";
import { Alert, Box, Button, CloseButton } from "@chakra-ui/react";
import { useForm } from "react-hook-form";
import TextInput from "~/components/Forms/Inputs/Text";
import { toast, isAxiosError } from "~/utils";
import { ErrorResponse } from "~/lib/types";
import { useLogin } from "../../api/hooks";

export type LoginFormValues = {
  login: string;
  password: string;
};

interface LoginFormProps {
  initialRef?: React.RefObject<HTMLInputElement>;
  onSuccess?: () => void;
}

export default function LoginForm(props: LoginFormProps) {
  const [error, setError] = useState("");
  const formik = useForm<LoginFormValues>();
  const { mutateAsync: loginAsync } = useLogin();

  const {
    handleSubmit,
    register,
    reset,
    formState: { isSubmitting, errors },
  } = formik;

  const handleLoginSubmit = async (values: LoginFormValues) => {
    try {
      await loginAsync(values);
      toast("success", { title: "You have logged in successfully. " });
      if (props.onSuccess) props.onSuccess();
    } catch (err) {
      let errorMessage = "An error has occurred.";
      if (isAxiosError<ErrorResponse>(err)) {
        errorMessage = err.response?.data.message ?? errorMessage;
      }
      setError(errorMessage);
      reset(values);
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
      <form onSubmit={handleSubmit(handleLoginSubmit)}>
        <TextInput
          {...register("login", {
            required: true,
          })}
          // ref={props.initialRef}
          label="Username/Email"
          error={errors.login?.message}
          isRequired
        />
        <TextInput
          type="password"
          label="Password"
          error={errors.password?.message}
          {...register("password", {
            required: true,
          })}
          isRequired
        />
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
    </Box>
  );
}

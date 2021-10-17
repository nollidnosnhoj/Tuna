import React from "react";
import { Button } from "@chakra-ui/react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import TextInput from "~/components/form-inputs/Text";

export const loginValidationSchema = z.object({
  login: z.string().min(1),
  password: z.string().min(1),
});

export type LoginFormValues = z.infer<typeof loginValidationSchema>;

interface LoginFormProps {
  initialRef?: React.RefObject<HTMLInputElement>;
  onSubmit?: (values: LoginFormValues) => Promise<void>;
}

export default function LoginForm(props: LoginFormProps) {
  const formMethods = useForm<LoginFormValues>();

  const {
    handleSubmit,
    register,
    reset,
    formState: { isSubmitting, errors },
  } = formMethods;

  const handleLoginSubmit = async (values: LoginFormValues) => {
    try {
      await props.onSubmit?.(values);
    } catch {
      reset({ ...values });
    }
  };

  return (
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
  );
}

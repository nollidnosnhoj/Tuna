import React from "react";
import { Button, Flex, Text } from "@chakra-ui/react";
import { z } from "zod";
import InputField from "~/components/form-inputs/InputField";
import { passwordRule, usernameRule } from "~/lib/validators/user-schemas";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod/dist/zod";

export const artistSignUpValidationSchema = z
  .object({
    username: usernameRule("Username"),
    password: passwordRule("Password"),
    displayName: usernameRule("Display Name", {
      allowedCharacters:
        " 0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
    }).optional(),
    email: z.string().min(1).email(),
    confirmPassword: z.string().min(1),
  })
  .superRefine((arg, ctx) => {
    if (arg.confirmPassword !== arg.password) {
      ctx.addIssue({
        code: "custom",
        message: "Password does not match.",
      });
    }
  });

export type ArtistSignUpInputs = z.infer<typeof artistSignUpValidationSchema>;

interface ArtistSignUpFormProps {
  initialRef?: React.RefObject<HTMLInputElement>;
  onSubmit?: (values: ArtistSignUpInputs) => Promise<void>;
}

export default function ArtistSignUpForm(props: ArtistSignUpFormProps) {
  const formik = useForm<ArtistSignUpInputs>({
    resolver: zodResolver(artistSignUpValidationSchema),
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = formik;

  const handleRegisterSubmit = async (values: ArtistSignUpInputs) => {
    try {
      await props.onSubmit?.(values);
    } catch {
      reset({ ...values });
    }
  };

  return (
    <form onSubmit={handleSubmit(handleRegisterSubmit)}>
      <InputField
        {...register("username")}
        ref={props.initialRef}
        error={errors.username?.message}
        label="Username"
        isRequired
      />
      <InputField
        {...register("displayName")}
        ref={props.initialRef}
        error={errors.displayName?.message}
        label="Display Name"
        helperText="If left blank, the display name will be your username."
      />
      <InputField
        {...register("email")}
        error={errors.email?.message}
        label="Email"
        isRequired
      />
      <InputField
        type="password"
        error={errors.password?.message}
        label="Password"
        isRequired
        {...register("password")}
      />
      <InputField
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
  );
}

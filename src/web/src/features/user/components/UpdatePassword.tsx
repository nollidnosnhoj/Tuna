import React from "react";
import { Button } from "@chakra-ui/react";
import { z } from "zod";
import TextInput from "~/components/Forms/Inputs/Text";
import SETTINGS from "~/lib/config";
import { validationMessages } from "~/utils";
import { passwordRule, PasswordRulesType } from "../schemas";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

export const updatePasswordSchema = (
  rules: PasswordRulesType = SETTINGS.IDENTITY.passwordRules
) =>
  z
    .object({
      currentPassword: z
        .string()
        .min(1, validationMessages.required("Current Password")),
      newPassword: passwordRule("New Password", rules),
      confirmPassword: z
        .string()
        .min(1, validationMessages.required("Confirm Password")),
    })
    .superRefine((arg, ctx) => {
      if (arg.confirmPassword !== arg.newPassword) {
        ctx.addIssue({
          code: "custom",
          message: "Password does not match.",
        });
      }
    });

export type UpdatePasswordValues = z.infer<
  ReturnType<typeof updatePasswordSchema>
>;

interface UpdatePasswordFormProps {
  onSubmit?: (currentPassword: string, newPassword: string) => Promise<void>;
}

export default function UpdatePasswordForm({
  onSubmit,
}: UpdatePasswordFormProps) {
  const {
    register,
    reset,
    formState: { isSubmitting, errors },
    handleSubmit,
  } = useForm<UpdatePasswordValues>({
    resolver: zodResolver(updatePasswordSchema()),
  });

  const handlePasswordChange = async (values: UpdatePasswordValues) => {
    await onSubmit?.(values.currentPassword, values.newPassword);
    reset({});
  };

  return (
    <form onSubmit={handleSubmit(handlePasswordChange)}>
      <TextInput
        {...register("currentPassword")}
        error={errors.currentPassword?.message}
        type="password"
        label="Current Password"
        isRequired
      />
      <TextInput
        {...register("newPassword")}
        type="password"
        error={errors.newPassword?.message}
        label="New Password"
        isRequired
      />
      <TextInput
        {...register("confirmPassword")}
        type="password"
        error={errors.confirmPassword?.message}
        label="Confirm Password"
        isRequired
      />
      <Button
        type="submit"
        isLoading={isSubmitting}
        disabled={isSubmitting}
        loadingText="Submitting..."
      >
        Update Password
      </Button>
    </form>
  );
}

import React from "react";
import { Button } from "@chakra-ui/react";
import { z } from "zod";
import InputField from "~/components/form-inputs/InputField";
import SETTINGS from "~/lib/config";
import {
  passwordRule,
  PasswordRulesType,
} from "../../lib/validators/user-schemas";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod/dist/zod";

export const updatePasswordSchema = (
  rules: PasswordRulesType = SETTINGS.IDENTITY.passwordRules
) =>
  z
    .object({
      currentPassword: z.string().min(1),
      newPassword: passwordRule("New Password", rules),
      confirmPassword: z.string().min(1),
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
      <InputField
        {...register("currentPassword")}
        error={errors.currentPassword?.message}
        type="password"
        label="Current Password"
        isRequired
      />
      <InputField
        {...register("newPassword")}
        type="password"
        error={errors.newPassword?.message}
        label="New Password"
        isRequired
      />
      <InputField
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

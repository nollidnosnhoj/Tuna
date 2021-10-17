import { Button } from "@chakra-ui/react";
import React from "react";
import { z } from "zod";
import TextInput from "~/components/form-inputs/Text";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

export const updateEmailValidationSchema = z.object({
  email: z
    .string()
    .min(1)
    .email()
    .transform((arg) => arg.trim()),
});

type NewEmailRequest = z.infer<typeof updateEmailValidationSchema>;

interface UpdateEmailFormProps {
  onSubmit?: (newEmail: string) => Promise<void>;
}

export default function UpdateEmailForm({ onSubmit }: UpdateEmailFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<NewEmailRequest>({
    resolver: zodResolver(updateEmailValidationSchema),
  });

  const handleEmailSubmit = async (values: NewEmailRequest) => {
    await onSubmit?.(values.email);
    reset(values);
  };

  return (
    <form onSubmit={handleSubmit(handleEmailSubmit)}>
      <TextInput
        {...register("email")}
        error={errors.email?.message}
        label="Change Email"
        isRequired
      />
      <Button
        type="submit"
        isLoading={isSubmitting}
        disabled={isSubmitting}
        loadingText="Submitting..."
      >
        Update Email
      </Button>
    </form>
  );
}

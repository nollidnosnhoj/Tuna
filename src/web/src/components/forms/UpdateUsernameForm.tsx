import React from "react";
import { Button } from "@chakra-ui/react";
import { z } from "zod";
import InputField from "~/components/form-inputs/InputField";
import SETTINGS from "~/lib/config";
import {
  usernameRule,
  UsernameRulesType,
} from "../../lib/validators/user-schemas";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod/dist/zod";

export const updateUsernameSchema = (
  rules: UsernameRulesType = SETTINGS.IDENTITY.usernameRules
) =>
  z.object({
    username: usernameRule("Username", rules),
  });

type UpdateUsernameRequest = z.infer<ReturnType<typeof updateUsernameSchema>>;

interface UpdateUsernameFormProps {
  onSubmit?: (values: UpdateUsernameRequest) => Promise<void>;
}

export default function UpdateUsernameForm({
  onSubmit,
}: UpdateUsernameFormProps) {
  const {
    handleSubmit,
    register,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<UpdateUsernameRequest>({
    resolver: zodResolver(updateUsernameSchema()),
  });

  const handleUsernameSubmit = async (values: UpdateUsernameRequest) => {
    await onSubmit?.(values);
    reset({});
  };

  return (
    <form onSubmit={handleSubmit(handleUsernameSubmit)}>
      <InputField
        {...register("username", {
          setValueAs: (val: string) => val.toLowerCase(),
        })}
        error={errors.username?.message}
        label="Change Username"
        isRequired
      />
      <Button
        type="submit"
        isLoading={isSubmitting}
        disabled={isSubmitting}
        loadingText="Submitting..."
      >
        Update Username
      </Button>
    </form>
  );
}

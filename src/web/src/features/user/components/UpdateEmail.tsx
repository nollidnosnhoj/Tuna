import { Button } from "@chakra-ui/react";
import React from "react";
import { z } from "zod";
import TextInput from "~/components/Forms/Inputs/Text";
import { useUser } from "~/features/user/hooks";
import { validationMessages, errorToast, toast } from "~/utils";
import request from "~/lib/http";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";

const newEmailSchema = z.object({
  email: z.string().min(1, validationMessages.required("Email")).email(),
});

type NewEmailRequest = z.infer<typeof newEmailSchema>;

export default function UpdateEmail() {
  const { user, updateUser } = useUser();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<NewEmailRequest>({
    resolver: yupResolver(newEmailSchema),
  });

  const handleEmailSubmit = async (values: NewEmailRequest) => {
    const { email: newEmail } = values;
    if (newEmail.trim() === user?.email) return;

    try {
      await request({
        method: "patch",
        url: "me/email",
        data: { newEmail },
      });
      toast("success", {
        title: "Email updated.",
        description: "You have successfully updated your email.",
      });
      if (user) {
        updateUser({ ...user, email: newEmail.trim() });
      }
    } catch (err) {
      errorToast(err);
    }
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

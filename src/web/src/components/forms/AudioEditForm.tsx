import { Box, Button, Spacer, Stack } from "@chakra-ui/react";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod/dist/zod";
import { z } from "zod";
import TagInput from "~/components/form-inputs/TagField";
import InputField from "~/components/form-inputs/InputField";
import { audioSchema } from "../../lib/validators/audio-schemas";
import { Audio } from "~/lib/types";
import TextAreaField from "~/components/form-inputs/TextAreaField";

interface AudioFormProps {
  audio: Audio;
  onSubmit: (values: UpdateAudioFormValues) => Promise<void>;
  removeButton: JSX.Element;
  isDisabled?: boolean;
}

export type UpdateAudioFormValues = z.infer<typeof audioSchema>;

export default function EditForm({
  audio,
  onSubmit,
  removeButton,
  isDisabled = false,
}: AudioFormProps) {
  const formMethods = useForm<UpdateAudioFormValues>({
    defaultValues: {
      title: audio.title,
      description: audio.description,
      tags: audio.tags,
    },
    resolver: zodResolver(audioSchema),
  });

  const {
    control,
    register,
    reset,
    handleSubmit,
    formState: { errors },
  } = formMethods;

  const handleSubmission = async (values: UpdateAudioFormValues) => {
    try {
      await onSubmit(values);
    } catch {
      reset({ ...values });
    }
  };

  return (
    <form onSubmit={handleSubmit(handleSubmission)}>
      <Box marginBottom={8}>
        <InputField
          {...register("title")}
          label="Title"
          error={errors.title?.message}
          isDisabled={isDisabled}
        />
        <TextAreaField
          {...register("description")}
          label="Description"
          error={errors.description?.message}
          isDisabled={isDisabled}
        />
        <Controller
          name="tags"
          control={control}
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
          render={({ field: { ref, ...restField }, fieldState: { error } }) => (
            <TagInput
              placeholder="Add Tag..."
              error={error?.message}
              disabled={isDisabled}
              {...restField}
            />
          )}
        />
      </Box>
      <Stack direction="row">
        {removeButton}
        <Spacer />
        <Button isLoading={isDisabled} type="submit">
          Submit
        </Button>
      </Stack>
    </form>
  );
}

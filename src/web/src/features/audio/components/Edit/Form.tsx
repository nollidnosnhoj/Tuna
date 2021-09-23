import { Box, Button, Spacer, Stack } from "@chakra-ui/react";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import TagInput from "~/components/Forms/Inputs/Tags";
import TextInput from "~/components/Forms/Inputs/Text";
import { AudioView } from "../../api/types";
import { audioSchema } from "../../schama";

interface AudioFormProps {
  audio: AudioView;
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
    handleSubmit,
    formState: { errors },
  } = formMethods;

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Box marginBottom={8}>
        <TextInput
          {...register("title")}
          label="Title"
          error={errors.title?.message}
          isDisabled={isDisabled}
          helperText="Note: When you change the audio's title, the url (slug) will also change to correspond with the title."
        />
        <TextInput
          {...register("description")}
          isTextArea
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
              errors={error?.map((x) => x.message ?? "")}
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

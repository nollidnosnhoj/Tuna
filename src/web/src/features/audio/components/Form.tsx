import { Box } from "@chakra-ui/react";
import React from "react";
import TagInput from "~/components/Forms/Inputs/Tags";
import TextInput from "~/components/Forms/Inputs/Text";
import { AudioRequest } from "../api/types";
import { Controller, useFormContext } from "react-hook-form";

interface AudioFormProps {
  disableFields?: boolean;
}

export default function AudioForm({ disableFields = false }: AudioFormProps) {
  const {
    control,
    register,
    formState: { errors },
  } = useFormContext<AudioRequest>();

  return (
    <Box marginBottom={8}>
      <TextInput
        {...register("title")}
        label="Title"
        error={errors.title?.message}
        isDisabled={disableFields}
        helperText="Note: When you change the audio's title, the url (slug) will also change to correspond with the title."
      />
      <TextInput
        {...register("description")}
        isTextArea
        label="Description"
        error={errors.description?.message}
        isDisabled={disableFields}
      />
      <Controller
        name="tags"
        control={control}
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        render={({ field: { ref, ...restField }, fieldState: { error } }) => (
          <TagInput
            placeholder="Add Tag..."
            errors={error?.map((x) => x.message ?? "")}
            {...restField}
            disabled={disableFields}
          />
        )}
      />
    </Box>
  );
}

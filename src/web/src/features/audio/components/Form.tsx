import {
  Box,
  FormControl,
  FormErrorMessage,
  FormHelperText,
  FormLabel,
  Radio,
  RadioGroup,
  Stack,
} from "@chakra-ui/react";
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
            error={error?.message}
            {...restField}
            disabled={disableFields}
          />
        )}
      />
      <Controller
        name="visibility"
        control={control}
        render={({
          field: { value, onChange, name },
          fieldState: { error },
        }) => (
          <FormControl id="visibility" isInvalid={!!error} marginBottom={4}>
            <FormLabel>Privacy</FormLabel>
            <RadioGroup name={name} value={value} onChange={onChange}>
              <Stack spacing={2} direction="column">
                <Radio value="public" isDisabled={disableFields}>
                  Public
                </Radio>
                <Radio value="private" isDisabled={disableFields}>
                  Private
                </Radio>
              </Stack>
            </RadioGroup>
            <FormErrorMessage>{error?.message}</FormErrorMessage>
            <FormHelperText>
              Private audio will generate a secret key that should be used to
              access private audios.
            </FormHelperText>
          </FormControl>
        )}
      />
    </Box>
  );
}

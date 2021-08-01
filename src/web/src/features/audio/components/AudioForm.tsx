import {
  Box,
  FormControl,
  FormErrorMessage,
  FormLabel,
  Radio,
  RadioGroup,
  Stack,
} from "@chakra-ui/react";
import React from "react";
import TagInput from "~/components/form-inputs/TagInput";
import TextInput from "~/components/form-inputs/TextInput";
import { AudioRequest } from "../types";
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
                <Radio value="unlisted" isDisabled={disableFields}>
                  Unlisted
                </Radio>
              </Stack>
            </RadioGroup>
            <FormErrorMessage>{error?.message}</FormErrorMessage>
          </FormControl>
        )}
      />
    </Box>
  );
}

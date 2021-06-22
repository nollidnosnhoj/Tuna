import {
  Box,
  FormControl,
  FormErrorMessage,
  Radio,
  RadioGroup,
  Stack,
} from "@chakra-ui/react";
import { useFormikContext } from "formik";
import React from "react";
import * as yup from "yup";
import slugify from "slugify";
import TagInput from "~/components/form/inputs/TagInput";
import TextInput from "~/components/form/inputs/TextInput";
import { AudioRequest } from "../types";

interface AudioFormProps {
  disableFields?: boolean;
}

export default function AudioForm(props: AudioFormProps) {
  const { disableFields = false } = props;

  const { values, errors, isSubmitting, handleChange, setFieldValue } =
    useFormikContext<AudioRequest>();

  return (
    <Box marginBottom={8}>
      <TextInput
        name="title"
        type="text"
        label="Title"
        value={values.title}
        onChange={handleChange}
        error={errors.title}
        disabled={isSubmitting || disableFields}
      />
      <TextInput
        name="description"
        label="Description"
        value={values.description ?? ""}
        onChange={handleChange}
        error={errors.description}
        disabled={isSubmitting || disableFields}
        textArea
      />
      <TagInput
        name="tags"
        value={values.tags}
        placeholder="Add Tag..."
        validationSchema={yup
          .string()
          .ensure()
          .required("Input is invalid.")
          .min(3, "Input must have at least 3 characters long.")
          .max(25, "Input must have no more than 25 characters long.")
          .notOneOf([...values.tags], "No duplicate tags.")
          .defined()}
        formatTagCallback={(rawTag) => slugify(rawTag, { lower: true })}
        onChange={(tags) => {
          setFieldValue("tags", tags, true);
        }}
        error={Array.isArray(errors.tags) ? errors.tags[0] : errors.tags}
        disabled={isSubmitting || disableFields}
      />
      <FormControl id="visibility" isInvalid={!!errors.visibility}>
        <RadioGroup
          name="visibility"
          value={values.visibility}
          onChange={(val) => setFieldValue("visibility", val)}
        >
          <Stack spacing={4} direction="column">
            <Radio value="public">Public</Radio>
            <Radio value="unlisted">Unlisted</Radio>
            {/* <Radio
            value="private"
            isChecked={values.visibility === Visibility.Private}
            onClick={() => setFieldValue("visibility", Visibility.Private)}
          >
            Private
          </Radio> */}
          </Stack>
        </RadioGroup>
        <FormErrorMessage>{errors.visibility}</FormErrorMessage>
      </FormControl>
    </Box>
  );
}

import { Box } from "@chakra-ui/react";
import { useFormikContext } from "formik";
import React from "react";
import * as yup from "yup";
import slugify from "slugify";
import InputCheckbox from "~/components/form/Checkbox";
import TagInput from "~/components/form/TagInput";
import TextInput from "~/components/form/TextInput";
import { AudioRequest } from "../types";

interface AudioFormProps {
  disableFields?: boolean;
}

export default function AudioForm(props: AudioFormProps) {
  const { disableFields = false } = props;

  const {
    values,
    errors,
    isSubmitting,
    handleChange,
    setFieldValue,
  } = useFormikContext<AudioRequest>();

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
      <InputCheckbox
        name="isPublic"
        onChange={() => setFieldValue("isPublic", !values.isPublic)}
        value={values.isPublic ?? false}
        error={errors.isPublic}
        toggleSwitch
        label="Public"
        disabled={isSubmitting || disableFields}
      />
    </Box>
  );
}

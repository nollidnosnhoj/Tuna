import {
  Box,
  Button,
  FormControl,
  FormErrorMessage,
  Radio,
  RadioGroup,
  Spacer,
  Stack,
} from "@chakra-ui/react";
import { useFormik } from "formik";
import React from "react";
import * as yup from "yup";
import TagInput from "~/components/form-inputs/TagInput";
import TextInput from "~/components/form-inputs/TextInput";
import { AudioRequest, Visibility } from "../types";
import { validationMessages } from "~/utils";

interface AudioFormProps {
  id: string;
  initialValues: Partial<AudioRequest>;
  onSubmit: (values: AudioRequest) => Promise<void>;
  leftFooter?: React.ReactNode;
  submitText?: string;
  isDisabledButton?: boolean;
}

export default function AudioForm({
  id,
  initialValues,
  onSubmit,
  leftFooter,
  submitText = "Submit",
  isDisabledButton = false,
}: AudioFormProps) {
  const {
    values,
    errors,
    handleSubmit,
    handleChange,
    isSubmitting,
    setFieldValue,
  } = useFormik<AudioRequest>({
    initialValues: {
      title: "",
      description: "",
      tags: [],
      visibility: Visibility.Unlisted,
      ...initialValues,
    },
    onSubmit: async (values) => {
      await onSubmit(values);
    },
    validationSchema: yup
      .object()
      .shape({
        title: yup
          .string()
          .required(validationMessages.required("Title"))
          .max(30, validationMessages.max("Title", 30))
          .ensure()
          .defined(),
        description: yup
          .string()
          .max(500, validationMessages.max("Description", 500))
          .ensure()
          .defined(),
        tags: yup
          .array(yup.string())
          .max(10, validationMessages.max("Tags", 10))
          .ensure()
          .defined(),
        visibility: yup
          .string()
          .required(validationMessages.required("Visibility"))
          .oneOf(Object.values(Visibility)),
      })
      .defined(),
  });

  return (
    <Box marginBottom={8}>
      <form id={id} onSubmit={handleSubmit}>
        <TextInput
          name="title"
          type="text"
          label="Title"
          value={values.title}
          onChange={handleChange}
          error={errors.title}
          disabled={isSubmitting}
        />
        <TextInput
          name="description"
          label="Description"
          value={values.description ?? ""}
          onChange={handleChange}
          error={errors.description}
          disabled={isSubmitting}
          textArea
        />
        <TagInput
          name="tags"
          value={values.tags}
          placeholder="Add Tag..."
          onChange={(tags) => {
            setFieldValue("tags", tags, true);
          }}
          error={Array.isArray(errors.tags) ? errors.tags[0] : errors.tags}
          disabled={isSubmitting}
        />
        <FormControl
          id="visibility"
          isInvalid={!!errors.visibility}
          marginBottom={4}
        >
          <RadioGroup
            name="visibility"
            value={values.visibility}
            onChange={(val) => setFieldValue("visibility", val)}
          >
            <Stack spacing={2} direction="column">
              <Radio value="public">Public</Radio>
              <Radio value="unlisted">Unlisted</Radio>
            </Stack>
          </RadioGroup>
          <FormErrorMessage>{errors.visibility}</FormErrorMessage>
        </FormControl>
        <Stack direction="row">
          {leftFooter}
          <Spacer />
          <Button
            type="submit"
            colorScheme="primary"
            isLoading={isSubmitting}
            disabled={isDisabledButton}
          >
            {submitText}
          </Button>
        </Stack>
      </form>
    </Box>
  );
}

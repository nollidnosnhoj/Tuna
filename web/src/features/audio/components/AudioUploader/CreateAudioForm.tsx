import { Box, VStack, Button, Text } from "@chakra-ui/react";
import { Formik } from "formik";
import React from "react";
import * as yup from "yup";
import { validationMessages } from "~/utils";
import { AudioRequest, Visibility } from "../../types";
import AudioForm from "../AudioForm";

const validationSchema = yup
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
  .defined();

interface CreateAudioFormProps {
  isHidden?: boolean;
  initialValues: Partial<AudioRequest>;
  onSubmit: (values: AudioRequest) => void;
}

export default function CreateAudioForm({
  isHidden = false,
  initialValues,
  onSubmit,
}: CreateAudioFormProps) {
  if (isHidden) return null;

  const handleFormSubmit = (values: AudioRequest) => {
    onSubmit(values);
  };

  return (
    <Formik<AudioRequest>
      initialValues={{
        title: "",
        description: "",
        tags: [],
        visibility: Visibility.Unlisted,
        ...initialValues,
      }}
      validationSchema={validationSchema}
      onSubmit={handleFormSubmit}
    >
      {({ handleSubmit, isValid }) => (
        <Box marginBottom={10}>
          <form onSubmit={handleSubmit}>
            <AudioForm />
            <VStack>
              <Text>
                By publishing this audio, you have agreed to Audiochan's terms
                of service and license agreement.
              </Text>
              <Button disabled={!isValid} type="submit" width="75%">
                Upload
              </Button>
            </VStack>
          </form>
        </Box>
      )}
    </Formik>
  );
}

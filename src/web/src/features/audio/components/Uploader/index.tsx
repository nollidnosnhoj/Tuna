import {
  Box,
  Button,
  chakra,
  Flex,
  Spacer,
  Spinner,
  Stack,
} from "@chakra-ui/react";
import { yupResolver } from "@hookform/resolvers/yup";
import { useRouter } from "next/router";
import React, { useState } from "react";
import { useEffect } from "react";
import { FormProvider, useForm } from "react-hook-form";
import * as yup from "yup";
import { useNavigationLock } from "~/lib/hooks";
import { errorToast, toast, validationMessages } from "~/utils";
import { useCreateAudio } from "../../hooks";
import { AudioId, CreateAudioRequest, Visibility } from "../../api/types";
import AudioForm from "../Form";
import AudioDropzone from "./Dropzone";

const validationSchema: yup.SchemaOf<CreateAudioRequest> = yup
  .object({
    title: yup
      .string()
      .defined()
      .required(validationMessages.required("Title"))
      .min(5, validationMessages.min("Title", 5))
      .max(30, validationMessages.max("Title", 30))
      .ensure(),
    description: yup
      .string()
      .defined()
      .max(500, validationMessages.max("Description", 500))
      .ensure(),
    tags: yup
      .array()
      .required()
      .max(10, validationMessages.max("Tags", 10))
      .ensure()
      .defined(),
    visibility: yup
      .mixed<Visibility>()
      .required(validationMessages.required("Visibility"))
      .oneOf([...Object.values(Visibility)], "Visibility choice is invalid."),
    uploadId: yup.string().required("Audio file has not been uploaded."),
    fileName: yup.string().required(),
    fileSize: yup.number().required().min(0),
    duration: yup.number().required().min(0),
  })
  .defined();

export default function AudioUploader() {
  const router = useRouter();
  const [audioId, setAudioId] = useState<AudioId>(0);
  const { mutateAsync: createAudio, isLoading: isCreatingAudio } =
    useCreateAudio();
  const formMethods = useForm<CreateAudioRequest>({
    defaultValues: {
      tags: [],
    },
    resolver: yupResolver(validationSchema),
  });
  const { handleSubmit } = formMethods;

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [unsavedChanges, setUnsavedChanges] = useNavigationLock(
    "Are you sure you want to leave page? You will lose progress."
  );

  const handleFormSubmit = async (values: CreateAudioRequest) => {
    try {
      const { id } = await createAudio(values);
      setAudioId(id);
      setUnsavedChanges(false);
    } catch (err) {
      errorToast(err);
    }
  };

  useEffect(() => {
    if (audioId && !unsavedChanges) {
      router.push(`/audios/${audioId}`).then(() => {
        toast("success", { title: "Audio was successfully created." });
      });
    }
  }, [audioId, unsavedChanges]);

  if (audioId) {
    return (
      <Flex align="center" justify="center" height="25vh">
        <chakra.span>
          Audio created. You will be redirected to to newly created page.
        </chakra.span>
      </Flex>
    );
  }

  if (isCreatingAudio) {
    return (
      <Flex align="center" justify="center" height="25vh">
        <Spinner thickness="4px" speed="0.65s" color="primary.500" size="xl" />
      </Flex>
    );
  }

  return (
    <Box>
      <FormProvider {...formMethods}>
        <form onSubmit={handleSubmit(handleFormSubmit)}>
          <AudioDropzone
            onFileDrop={(isFileUpload) => setUnsavedChanges(isFileUpload)}
          />
          <AudioForm />
          <Stack direction="row">
            <Spacer />
            <Button type="submit">Submit</Button>
          </Stack>
        </form>
      </FormProvider>
    </Box>
  );
}

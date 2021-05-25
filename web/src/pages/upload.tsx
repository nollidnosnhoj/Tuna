import axios from "axios";
import {
  Flex,
  Box,
  Heading,
  Button,
  VStack,
  Text,
  Progress,
  chakra,
} from "@chakra-ui/react";
import { Formik, FormikHelpers } from "formik";
import { useRouter } from "next/router";
import React, { useState } from "react";
import * as yup from "yup";
import withRequiredAuth from "~/components/hoc/withRequiredAuth";
import Page from "~/components/Page";
import AudioDropzone from "~/features/audio/components/AudioDropzone";
import AudioForm from "~/features/audio/components/AudioForm";
import { useCreateAudio } from "~/features/audio/hooks";
import { getS3PresignedUrl } from "~/features/audio/services";
import { AudioRequest } from "~/features/audio/types";
import { useNavigationLock } from "~/lib/hooks";
import {
  toast,
  errorToast,
  getDurationFromAudioFile,
  validationMessages,
} from "~/utils";
import { useUser } from "~/features/user/hooks/useUser";

interface CreateAudioRequestValues extends AudioRequest {
  file: File | null;
}

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
    isPublic: yup.boolean().defined(),
  })
  .defined();

const AudioUploadNextPage: React.FC = () => {
  const router = useRouter();
  const { user } = useUser();
  const [audioId, setAudioId] = useState("");
  const [uploadProgress, setUploadProgress] = useState(0);
  const [uploading, setUploading] = useState(false);
  const { mutateAsync: createAudio } = useCreateAudio();

  useNavigationLock(
    uploading,
    "Are you sure you want to leave page? You will lose progress."
  );

  const handleUploading = async (
    values: CreateAudioRequestValues,
    { setValues }: FormikHelpers<CreateAudioRequestValues>
  ) => {
    const { file, ...formValues } = values;
    if (file && user) {
      try {
        setUploading(true);
        const { uploadId, uploadUrl } = await getS3PresignedUrl(file);
        await axios.put(uploadUrl, file, {
          headers: {
            "Content-Type": file.type,
            "x-amz-meta-userId": `${user.id}`,
            "x-amz-meta-originalFilename": `${file.name}`,
          },
          onUploadProgress: (evt) => {
            const currentProgress = (evt.loaded / evt.total) * 100;
            setUploadProgress(currentProgress);
          },
        });
        const duration = await getDurationFromAudioFile(file);
        const { id } = await createAudio({
          ...formValues,
          uploadId: uploadId,
          fileName: file.name,
          fileSize: file.size,
          duration: duration,
          contentType: file.type,
        });
        setAudioId(id);
      } catch (err) {
        errorToast(err);
        setValues(values);
      } finally {
        setUploading(false);
      }
    } else {
      toast("error", { description: "File and/or audio not defined." });
    }
  };

  if (audioId) {
    return (
      <Page title="Audio created!">
        <Flex justify="center" align="center" marginTop={10}>
          <Box textAlign="center">
            <Heading as="h2" size="xl" marginY={4}>
              Your audio has been uploaded!
            </Heading>
            <Button
              colorScheme="primary"
              onClick={() => router.push(`/audios/${audioId}`)}
            >
              Click me to view your audio
            </Button>
          </Box>
        </Flex>
      </Page>
    );
  }

  if (uploading) {
    return (
      <Page title="Uploading...">
        <Box display="flex" justifyContent="center">
          <VStack marginY={10} width="100%" spacing={4}>
            <Progress
              colorScheme="primary"
              hasStripe
              value={uploadProgress}
              width="full"
            />
            <chakra.div
              display="flex"
              flexDirection="column"
              alignItems="center"
            >
              <Heading as="h2" size="md">
                Uploading Your Audio into the Internet...
              </Heading>
              <chakra.p>Do not leave this page!</chakra.p>
            </chakra.div>
          </VStack>
        </Box>
      </Page>
    );
  }

  return (
    <Page title="Upload">
      <Formik<CreateAudioRequestValues>
        initialValues={{
          file: null,
          title: "",
          description: "",
          tags: [],
          isPublic: false,
        }}
        validate={(values) => {
          if (!values.file) {
            return { file: "File is required." };
          }

          return {};
        }}
        validationSchema={validationSchema}
        onSubmit={handleUploading}
      >
        {({ handleSubmit, isValid }) => (
          <Box marginBottom={10}>
            <form onSubmit={handleSubmit}>
              <AudioDropzone name="file" />
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
    </Page>
  );
};

export default withRequiredAuth(AudioUploadNextPage);

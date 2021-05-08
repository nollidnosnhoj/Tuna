import { Box, Button, Flex, Heading, Text, VStack } from "@chakra-ui/react";
import { Formik } from "formik";
import React, { useState } from "react";
import { useRouter } from "next/router";
import { usePublishAudio } from "../../hooks/mutations";
import { AudioRequest } from "../../types";
import AudioDropzone from "../AudioDropzone";
import AudioForm from "../AudioForm";
import { publishAudioSchema } from "../../schemas";
import useNavigationLock from "~/lib/hooks/useNavigationLock";
import { apiErrorToast } from "~/utils/toast";

export default function AudioUploadPage() {
  const router = useRouter();
  const [audioId, setAudioId] = useState("");
  const [uploaded, setUploaded] = useState(false);
  const [published, setPublished] = useState(false);
  const { mutateAsync: publishAudio } = usePublishAudio();

  useNavigationLock(
    Boolean(audioId) && !uploaded,
    "Are you sure you want to leave page? You will lose progress."
  );

  const handleSubmit = (values: AudioRequest) => {
    publishAudio({ ...values, audioId: audioId })
      .then(() => {
        setPublished(true);
      })
      .catch((err) => {
        apiErrorToast(err);
      });
  };

  if (audioId && published) {
    return (
      <Flex justify="center" align="center" marginTop={10}>
        <Box textAlign="center">
          <Heading as="h2" size="xl" marginY={4}>
            Your audio has been published!
          </Heading>
          <Button
            colorScheme="primary"
            onClick={() => router.push(`/audios/${audioId}`)}
          >
            Click me to view your audio
          </Button>
        </Box>
      </Flex>
    );
  }

  return (
    <Box>
      <AudioDropzone
        onUploaded={() => setUploaded(true)}
        onUploading={(aid) => setAudioId(aid)}
      />
      {audioId && (
        <Formik
          initialValues={{
            title: "",
            description: "",
            tags: [],
            isPublic: false,
          }}
          validationSchema={publishAudioSchema}
          onSubmit={handleSubmit}
        >
          {({ handleSubmit }) => (
            <form onSubmit={handleSubmit}>
              <AudioForm />
              <VStack>
                <Text>
                  By publishing this audio, you have agreed to Audiochan's terms
                  of service and license agreement.
                </Text>
                <Button disabled={!uploaded} type="submit" width="75%">
                  Publish
                </Button>
              </VStack>
            </form>
          )}
        </Formik>
      )}
    </Box>
  );
}

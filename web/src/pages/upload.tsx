import { Flex, Box, Heading, Button, VStack, Text } from "@chakra-ui/react";
import { Formik } from "formik";
import { useRouter } from "next/router";
import React, { useState } from "react";
import withRequiredAuth from "~/components/hoc/withRequiredAuth";
import Page from "~/components/Page";
import AudioDropzone from "~/features/audio/components/AudioDropzone";
import AudioForm from "~/features/audio/components/AudioForm";
import { usePublishAudio } from "~/features/audio/hooks";
import { publishAudioSchema } from "~/features/audio/schemas";
import { AudioRequest } from "~/features/audio/types";
import useNavigationLock from "~/lib/hooks/useNavigationLock";
import { apiErrorToast } from "~/utils";

const AudioUploadNextPage: React.FC = () => {
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
    <Page title="Upload">
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
    </Page>
  );
};

export default withRequiredAuth(AudioUploadNextPage);

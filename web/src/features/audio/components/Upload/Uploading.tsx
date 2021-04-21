import axios from "axios";
import { Box, Flex, Heading, Progress, Stack, Text } from "@chakra-ui/react";
import React, { useCallback, useEffect, useState } from "react";
import Router from "next/router";
import { useCreateAudio } from "~/features/audio/hooks/mutations";
import { useUser } from "~/contexts/UserContext";
import { CreateAudioRequest } from "~/features/audio/types";
import api from "~/utils/api";
import { successfulToast } from "~/utils/toast";
import { isAxiosError } from "~/utils/axios";
import { ErrorResponse } from "~/lib/types";

interface AudioUploadingProps {
  file: File;
  setToPublic?: boolean;
}

type S3PresignedUrl = {
  uploadId: string;
  url: string;
};

const STAGE_MESSAGES = [
  "Getting Upload URL",
  "Uploading Audio",
  "Creating Audio Record",
  "Uploading Picture",
];

export default function AudioUploading(props: AudioUploadingProps) {
  const { file, setToPublic } = props;
  const { user } = useUser();
  const [stage, setStage] = useState(0);
  const [uploadAudioProgress, setUploadAudioProgress] = useState(0);
  const [audioId, setAudioId] = useState(0);
  const [error, setError] = useState("");
  const { mutateAsync: createAudio } = useCreateAudio();

  const completed = audioId > 0;

  const getUploadUrl = useCallback(() => {
    return new Promise<S3PresignedUrl>((resolve, reject) => {
      api
        .post<S3PresignedUrl>("upload", {
          fileName: file.name,
          fileSize: file.size,
        })
        .then(({ data }) => {
          resolve(data);
        })
        .catch((err) => {
          const errorMessage = "Unable to upload audio.";
          if (isAxiosError<ErrorResponse>(err)) {
            reject(err.response?.data.message ?? errorMessage);
          } else {
            reject(errorMessage);
          }
        });
    });
  }, [file]);

  const uploadAudioToS3 = useCallback(
    (s3Url: string) => {
      setStage(1);
      return new Promise<void>((resolve, reject) => {
        axios
          .put(s3Url, file, {
            headers: {
              "Content-Type": file.type,
              "x-amz-meta-userId": `${user!.id}`,
              "x-amz-meta-originalFilename": `${file.name}`,
            },
            onUploadProgress: (evt) => {
              const currentProgress = (evt.loaded / evt.total) * 100;
              setUploadAudioProgress(currentProgress);
            },
          })
          .then(() => {
            resolve();
          })
          .catch(() => {
            reject("Unable to upload audio.");
          });
      });
    },
    [file, user]
  );

  const requestCreateAudioFromServer = useCallback(
    (uploadId: string) => {
      setStage(2);
      return new Promise<number>((resolve, reject) => {
        const audio = new Audio();
        audio.src = window.URL.createObjectURL(file);
        audio.onloadedmetadata = () => {
          const body: CreateAudioRequest = {
            uploadId: uploadId,
            fileName: file.name,
            duration: Math.round(audio.duration),
            fileSize: file.size,
            tags: [],
            visibility: setToPublic ? "public" : "unlisted",
          };

          createAudio(body)
            .then(({ id }) => {
              resolve(id);
            })
            .catch((err) => {
              // TODO: Add logging
              const errorMessage = "Unable to create audio";
              if (isAxiosError<ErrorResponse>(err)) {
                reject(err.response?.data.message ?? errorMessage);
              } else {
                reject(errorMessage);
              }
            });
        };
      });
    },
    [file]
  );

  // The process of creating the audio
  useEffect(() => {
    (async () => {
      try {
        if (file && user) {
          const { url, uploadId } = await getUploadUrl();
          await uploadAudioToS3(url);
          const createdAudioId = await requestCreateAudioFromServer(uploadId);
          setAudioId(createdAudioId);
        }
      } catch (err) {
        setError(err);
      }
    })();
  }, [file, user]);

  // Once completed, push the route to the newly created audio
  useEffect(() => {
    if (completed) {
      Router.push(`audios/${audioId}`).then(() => {
        successfulToast({
          title: "Audio uploaded",
          message: "You have successfully uploaded your audio.",
        });
      });
    }
  }, [completed, audioId]);

  if (!file) {
    return <Text>File is required.</Text>;
  }

  if (error) {
    <Flex justify="center" align="center" height="50vh">
      <Heading>{error}</Heading>
    </Flex>;
  }

  return (
    <Flex justify="center" align="center" height="50vh">
      <Stack direction="column" spacing={8}>
        <Heading>
          {!completed ? STAGE_MESSAGES[stage] : `Audio Uploaded!`}
        </Heading>
        <Box justifyContent="center" alignItems="center">
          {!completed && (
            <Progress
              hasStripe
              value={uploadAudioProgress}
              isAnimated
              isIndeterminate={stage !== 1}
            />
          )}
        </Box>
      </Stack>
    </Flex>
  );
}

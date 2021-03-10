import axios from "axios";
import {
  Box,
  Flex,
  Heading,
  Progress,
  Stack,
  Text,
  useToast,
} from "@chakra-ui/react";
import React, { useCallback, useEffect, useState } from "react";
import Router from "next/router";
import { useCreateAudio } from "~/features/audio/hooks/mutations";
import useUser from "~/contexts/userContext";
import { AudioRequest, CreateAudioRequest } from "~/features/audio/types";
import api from "~/utils/api";
import { successfulToast } from "~/utils/toast";
import { isAxiosError } from "~/utils/axios";
import { ErrorResponse } from "~/lib/types";
import { addAudioPicture } from "../../services/addAudioPicture";

interface AudioUploadingProps {
  file: File;
  form?: AudioRequest;
  picture?: string;
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
  const { file, form, picture } = props;
  const toast = useToast();
  const { user } = useUser();
  const [stage, setStage] = useState(0);
  const [uploadAudioProgress, setUploadAudioProgress] = useState(0);
  const [audioId, setAudioId] = useState(0);
  const [completed, setCompleted] = useState(false);
  const [error, setError] = useState("");
  const { mutateAsync: createAudio } = useCreateAudio();

  const getUploadUrl = useCallback(() => {
    return new Promise<S3PresignedUrl>((resolve, reject) => {
      api
        .post<S3PresignedUrl>("upload", { fileName: file.name })
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
            ...(form ?? {
              title: "",
              description: "",
              tags: [],
              isPublic: true,
            }),
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

  const addPictureAfterAudioCreation = useCallback(() => {
    return new Promise<string>((resolve) => {
      if (audioId > 0 && picture) {
        addAudioPicture(audioId, picture)
          .then(({ data }) => {
            resolve(data.image);
          })
          .catch((err) => {
            // TODO: Add warning toast notifying that the image cannot be uploaded, but still resolve the promise.
            resolve("");
          });
      } else {
        resolve("");
      }
    });
  }, [audioId, picture]);

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

  // When the audio is created, upload the picture if applicable
  useEffect(() => {
    if (audioId > 0 && picture) {
      addPictureAfterAudioCreation()
        .catch(() => {
          toast({
            title: "Unable to upload picture.",
            description: "Try again later.",
            status: "warning",
            duration: 1000,
          });
        })
        .finally(() => {
          setCompleted(true);
        });
    } else if (audioId > 0) {
      setCompleted(true);
    }
  }, [audioId, picture]);

  // Once completed, push the route to the newly created audio
  useEffect(() => {
    if (completed && audioId > 0) {
      Router.push(`audios/view/${audioId}`).then(() => {
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

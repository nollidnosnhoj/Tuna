import {
  Box,
  Button,
  HStack,
  CircularProgress,
  CircularProgressLabel,
  Heading,
  VStack,
} from "@chakra-ui/react";
import NextLink from "next/link";
import Router from "next/router";
import React, { useEffect, useState } from "react";
import { useUser } from "~/contexts/UserContext";
import { useCreateAudio } from "../hooks/mutations";
import {
  getDurationFromAudio,
  getS3PresignedUrl,
  uploadAudioToS3,
} from "../services";

interface AudioUploadingProps {
  file: File | null;
  isPublic: boolean;
}

const AudioUploading: React.FC<AudioUploadingProps> = (props) => {
  const { file, isPublic } = props;

  const { mutateAsync: createAudio } = useCreateAudio();

  const { user } = useUser();
  const [audioId, setAudioId] = useState("");
  const [progress, setProgress] = useState(0);

  useEffect(() => {
    const uploading = async () => {
      if (!!file && !!user) {
        const { uploadId, url } = await getS3PresignedUrl(file);
        await uploadAudioToS3(url, user!.id, file, (value) =>
          setProgress(value)
        );
        const audioDuration = await getDurationFromAudio(file);
        const audio = await createAudio({
          title: file.name.split(".").slice(0, -1).join("."),
          uploadId: uploadId,
          fileName: file.name,
          duration: Math.round(audioDuration),
          fileSize: file.size,
          tags: [],
          isPublic: isPublic,
        });
        setAudioId(audio.id);
      }
    };
    uploading();
  }, [file, user]);

  useEffect(() => {
    if (audioId) {
      Router.push(`/audios/${audioId}`);
    }
  }, [audioId]);

  if (audioId) {
    return (
      <Box>
        <VStack>
          <Heading>Audio Uploaded!</Heading>
        </VStack>
      </Box>
    );
  }

  if (!file || !user) {
    return null;
  }

  return (
    <Box>
      <HStack>
        <CircularProgress value={progress} color="primary.400">
          <CircularProgressLabel>
            {Math.round(progress || 0)}%
          </CircularProgressLabel>
        </CircularProgress>
        <Heading>"Uploading..."</Heading>
      </HStack>
    </Box>
  );
};

export default AudioUploading;

import axios from "axios";
import {
  Box,
  Button,
  Flex,
  Heading,
  Text,
  VStack,
  chakra,
  Checkbox,
  Spacer,
  HStack,
  CircularProgress,
  CircularProgressLabel,
} from "@chakra-ui/react";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useDropzone } from "react-dropzone";
import NextLink from "next/link";
import { useUser } from "~/contexts/UserContext";
import SETTINGS from "~/lib/config";
import { ErrorResponse } from "~/lib/types";
import api from "~/utils/api";
import { isAxiosError } from "~/utils/axios";
import { formatFileSize } from "~/utils/format";
import { errorToast } from "~/utils/toast";
import { useCreateAudio } from "../hooks/mutations";
import { CreateAudioRequest } from "../types";
import {
  getDurationFromAudio,
  getS3PresignedUrl,
  uploadAudioToS3,
} from "../services";

export default function AudioUploadDropzone() {
  const { user } = useUser();
  const [uploadingFile, setUploadingFile] = useState<File | null>(null);
  const [isPublic, setIsPublic] = useState(false);
  const [newAudioId, setNewAudioId] = useState(0);
  const [progressNumber, setProgressNumber] = useState<number | undefined>(
    undefined
  );
  const { mutateAsync: createAudio } = useCreateAudio();

  const {
    getRootProps,
    getInputProps,
    open,
    isDragReject,
    isDragAccept,
  } = useDropzone({
    multiple: false,
    maxSize: SETTINGS.UPLOAD.AUDIO.maxSize,
    accept: SETTINGS.UPLOAD.AUDIO.accept,
    noClick: true,
    onDropAccepted: ([acceptedFile]) => {
      setUploadingFile(acceptedFile);
    },
    onDropRejected: (fileRejections) => {
      fileRejections.forEach((fileRejection) => {
        fileRejection.errors.forEach((err) => {
          errorToast({
            title: "Invalid Audio",
            message: err.message,
          });
        });
      });
    },
  });

  const borderColor = useMemo<string>(() => {
    if (isDragReject) return "red.500";
    if (isDragAccept) return "green.500";
    return "gray.700";
  }, [isDragReject, isDragAccept]);

  useEffect(() => {
    const uploading = async () => {
      if (!!uploadingFile && !!user) {
        const { uploadId, url } = await getS3PresignedUrl(uploadingFile);
        await uploadAudioToS3(url, user!.id, uploadingFile, (value) =>
          setProgressNumber(value)
        );
        const audioDuration = await getDurationFromAudio(uploadingFile);
        const audio = await createAudio({
          title: uploadingFile.name.split(".").slice(0, -1).join("."),
          uploadId: uploadId,
          fileName: uploadingFile.name,
          duration: Math.round(audioDuration),
          fileSize: uploadingFile.size,
          tags: [],
          isPublic: isPublic,
        });
        setNewAudioId(audio.id);
      }
    };
    uploading();
  }, [uploadingFile]);

  if (newAudioId) {
    return (
      <Box width="100%">
        <VStack marginY={20}>
          <Heading>Audio Uploaded!</Heading>
          <NextLink href={`/audios/${newAudioId}`}>
            <Button colorScheme="primary">View Audio</Button>
          </NextLink>
        </VStack>
      </Box>
    );
  } else if (uploadingFile && !newAudioId) {
    return (
      <Flex width="100%" justifyContent="center" alignItems="center">
        <HStack marginY={20}>
          <CircularProgress value={progressNumber} color="primary.400">
            <CircularProgressLabel>
              {Math.round(progressNumber || 0)}%
            </CircularProgressLabel>
          </CircularProgress>
          <Heading>"Uploading..."</Heading>
        </HStack>
      </Flex>
    );
  } else {
    return (
      <Flex
        {...getRootProps()}
        borderRadius={4}
        borderWidth={1}
        borderColor={borderColor}
      >
        <Box width="100%">
          <input {...getInputProps()} />
          <VStack marginY={20}>
            <Heading size="md">Drag and drop your audio file here.</Heading>
            <chakra.div>
              <Button colorScheme="primary" onClick={open}>
                Or click to upload your file.
              </Button>
            </chakra.div>
            <chakra.div>
              <Checkbox
                checked={isPublic}
                onChange={() => setIsPublic((prev) => !prev)}
              >
                Set audio to public after upload.
              </Checkbox>
            </chakra.div>
            <Spacer />
          </VStack>
          <Flex
            direction="column"
            justifyContent="flex-end"
            alignItems="center"
            marginBottom={2}
          >
            <Text fontSize="sm">
              MP3 file only. Maximum file size:{" "}
              {formatFileSize(SETTINGS.UPLOAD.AUDIO.maxSize)}
            </Text>
          </Flex>
        </Box>
      </Flex>
    );
  }
}

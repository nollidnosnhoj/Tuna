import {
  Box,
  Button,
  chakra,
  Flex,
  Heading,
  VStack,
  Text,
  Progress,
} from "@chakra-ui/react";
import React, { useEffect, useMemo, useState } from "react";
import { useDropzone } from "react-dropzone";
import { useUser } from "~/lib/hooks/useUser";
import SETTINGS from "~/lib/config";
import { formatFileSize } from "~/utils/format";
import { errorToast } from "~/utils/toast";
import { getS3PresignedUrl, uploadAudioToS3 } from "../services";

interface AudioDropzoneProps {
  onUploading: (id: string) => void;
  onUploaded: () => void;
}

export default function AudioDropzone(props: AudioDropzoneProps) {
  const { onUploaded, onUploading } = props;
  const { user } = useUser();
  const [file, setFile] = useState<File | null>(null);
  const [progress, setProgress] = useState(0);
  const [uploaded, setUploaded] = useState(false);

  const {
    getRootProps,
    getInputProps,
    open,
    isDragReject,
    isDragAccept,
  } = useDropzone({
    accept: SETTINGS.UPLOAD.AUDIO.accept,
    maxSize: SETTINGS.UPLOAD.AUDIO.maxSize,
    maxFiles: 1,
    multiple: false,
    noClick: true,
    onDropAccepted: ([acceptedFile]) => {
      setFile(acceptedFile);
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
      if (!!file && !!user) {
        try {
          const { audioId, uploadUrl } = await getS3PresignedUrl(file);
          onUploading(audioId);
          await uploadAudioToS3(uploadUrl, user.id, file, (value) =>
            setProgress(value)
          );
          setUploaded(true);
          onUploaded();
        } catch (err) {
          errorToast({
            message: "Unable to complete upload. Please try again later.",
          });
        }
      }
    };
    uploading();
  }, [file, user]);

  if (file) {
    return (
      <Box display="flex" justifyContent="center">
        <VStack marginY={10}>
          <Progress hasStripe value={progress} />
          <Heading as="h2" size="md">
            {uploaded ? "Done" : "Uploading..."}
          </Heading>
        </VStack>
      </Box>
    );
  }

  return (
    <Flex
      borderRadius={4}
      borderWidth={1}
      borderColor={borderColor}
      {...getRootProps()}
    >
      <Box width="100%" marginY={10}>
        <input {...getInputProps()} />
        <VStack marginY={20}>
          <Heading size="md">Drag and drop your audio file here.</Heading>
          <chakra.div>
            <Button colorScheme="primary" onClick={open}>
              Or click to upload your file.
            </Button>
          </chakra.div>
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

import {
  Box,
  Button,
  Flex,
  Heading,
  Text,
  VStack,
  chakra,
} from "@chakra-ui/react";
import React, { useMemo } from "react";
import { useDropzone } from "react-dropzone";
import SETTINGS from "~/lib/config";
import { formatFileSize } from "~/utils/format";
import { errorToast } from "~/utils/toast";

interface AudioUploadDropzoneProps {
  onUpload: (file: File) => void;
}

export default function AudioUploadDropzone(props: AudioUploadDropzoneProps) {
  const { onUpload } = props;
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
      onUpload(acceptedFile);
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

  return (
    <Flex
      borderRadius={4}
      borderWidth={1}
      borderColor={borderColor}
      {...getRootProps()}
    >
      <Box width="100%" marginY={20}>
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

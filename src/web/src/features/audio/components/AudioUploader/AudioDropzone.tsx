import {
  Box,
  Button,
  chakra,
  Flex,
  Heading,
  VStack,
  Text,
} from "@chakra-ui/react";
import React, { useMemo } from "react";
import { useDropzone } from "react-dropzone";
import SETTINGS from "~/lib/config";
import { formatFileSize } from "~/utils/format";
import { errorToast } from "~/utils/toast";

interface AudioDropzoneProps {
  onFileDrop: (file: File) => Promise<void>;
  isHidden: boolean;
}

export default function AudioDropzone({
  onFileDrop,
  isHidden = false,
}: AudioDropzoneProps) {
  const { getRootProps, getInputProps, open, isDragReject, isDragAccept } =
    useDropzone({
      accept: SETTINGS.UPLOAD.AUDIO.accept,
      maxSize: SETTINGS.UPLOAD.AUDIO.maxSize,
      maxFiles: 1,
      multiple: false,
      noClick: true,
      onDropAccepted: async ([file]) => {
        await onFileDrop(file);
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
      align="center"
      justify="center"
      height="50vh"
      display={isHidden ? "none" : "flex"}
    >
      <Flex
        borderRadius={4}
        borderWidth={1}
        borderColor={borderColor}
        width="100%"
        maxW="500px"
        {...getRootProps()}
      >
        <Box width="100%" marginTop={10}>
          <input {...getInputProps()} />
          <VStack>
            <Heading size="sm">Drag and drop your audio file here.</Heading>
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
            marginTop={10}
            marginBottom={2}
          >
            <Text fontSize="sm">
              MP3 file only. Maximum file size:{" "}
              {formatFileSize(SETTINGS.UPLOAD.AUDIO.maxSize)}
            </Text>
          </Flex>
        </Box>
      </Flex>
    </Flex>
  );
}

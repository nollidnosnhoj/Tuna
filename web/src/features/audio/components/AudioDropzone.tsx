import {
  Box,
  Button,
  chakra,
  Flex,
  Heading,
  VStack,
  Text,
  CloseButton,
} from "@chakra-ui/react";
import { useField } from "formik";
import React, { useMemo } from "react";
import { useDropzone } from "react-dropzone";
import SETTINGS from "~/lib/config";
import { formatFileSize } from "~/utils/format";
import { errorToast } from "~/utils/toast";

interface AudioDropzoneProps {
  name: string;
}

export default function AudioDropzone(props: AudioDropzoneProps) {
  const { name } = props;

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [{ value }, { error }, { setValue }] = useField<File | null>({
    name: name,
  });

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
      setValue(acceptedFile, false);
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
      <Box width="100%" marginTop={10}>
        <input {...getInputProps()} />
        {value ? (
          <VStack>
            <Flex alignItems="center">
              <Heading size="md">{value.name}</Heading>
              <CloseButton
                marginLeft={4}
                aria-label="Remove File"
                onClick={() => setValue(null)}
                variant="ghost"
              />
            </Flex>
          </VStack>
        ) : (
          <VStack>
            <Heading size="md">Drag and drop your audio file here.</Heading>
            <chakra.div>
              <Button colorScheme="primary" onClick={open}>
                Or click to upload your file.
              </Button>
            </chakra.div>
            {error && (
              <chakra.span color="red.500" fontSize="sm">
                {error}
              </chakra.span>
            )}
          </VStack>
        )}
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
  );
}

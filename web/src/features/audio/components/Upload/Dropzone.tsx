import { Box, Flex, Icon, Text, VStack } from "@chakra-ui/react";
import React, { useMemo } from "react";
import { useDropzone } from "react-dropzone";
import { FaCloudUploadAlt, FaRegFileAudio } from "react-icons/fa";
import SETTINGS from "~/constants/settings";
import { formatFileSize } from "~/utils/format";
import { errorToast } from "~/utils/toast";

interface AudioUploadProps {
  files: File[];
  onDropAccepted: (files: File[]) => void;
  maxFileSize?: number;
  validContentTypes?: string[];
}

export default function AudioUploadDropzone(props: AudioUploadProps) {
  const {
    files,
    onDropAccepted,
    maxFileSize = SETTINGS.UPLOAD.AUDIO.maxSize,
    validContentTypes = SETTINGS.UPLOAD.AUDIO.accept,
  } = props;

  const { getRootProps, getInputProps, isDragReject } = useDropzone({
    multiple: false,
    maxSize: maxFileSize,
    accept: validContentTypes,
    onDropAccepted: (files) => {
      onDropAccepted(files);
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
    return "gray.500";
  }, [isDragReject]);

  return (
    <Flex
      {...getRootProps()}
      justify="center"
      align="center"
      height="250px"
      borderRadius={4}
      borderWidth={2}
      borderStyle="dashed"
      borderColor={borderColor}
      cursor="pointer"
    >
      <Box>
        <input {...getInputProps()} />
        <VStack align="center" spacing={2}>
          {files.length > 0 ? (
            <>
              <Icon as={FaRegFileAudio} boxSize={50} />
              {files.map((file, idx) => (
                <Box key={idx}>
                  <Text>{file.name}</Text>
                </Box>
              ))}
            </>
          ) : (
            <>
              <Icon as={FaCloudUploadAlt} boxSize={50} />
              <Text>Drop in an audio file or click to upload.</Text>
              <Box textAlign="center" fontSize="xs" textColor="gray.500">
                <Text>Maximum file size: {formatFileSize(maxFileSize)}</Text>
                <Text>We only accept .mp3 file. (for now)</Text>
              </Box>
            </>
          )}
        </VStack>
      </Box>
    </Flex>
  );
}

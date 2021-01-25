import { WarningIcon } from "@chakra-ui/icons";
import {
  Alert,
  AlertIcon,
  Box,
  Flex,
  Icon,
  ListIcon,
  Text,
} from "@chakra-ui/react";
import { ErrorMessage } from "@hookform/error-message";
import React, { useMemo } from "react";
import { useDropzone } from "react-dropzone";
import { get, useFormContext } from "react-hook-form";
import { FaCloudUploadAlt, FaFileAudio } from "react-icons/fa";
import CONSTANTS from "~/constants";
import { errorToast } from "~/utils/toast";

interface AudioDropzoneProps {
  name: string;
  onChange: (file: File | FileList) => void;
  maxSize?: number;
  accept?: string[];
}

const AudioDropzone: React.FC<AudioDropzoneProps> = ({
  name,
  onChange,
  maxSize = CONSTANTS.UPLOAD_RULES.maxSize,
  accept = CONSTANTS.UPLOAD_RULES.accept,
}) => {
  const { errors } = useFormContext();
  const {
    acceptedFiles,
    isDragReject,
    getRootProps,
    getInputProps,
    isDragActive,
  } = useDropzone({
    multiple: false,
    maxSize: maxSize,
    accept: accept,
    onDropRejected: (fileRejections) => {
      fileRejections.forEach((fileRejection) => {
        fileRejection.errors.forEach((err) => {
          errorToast({
            title: "Invalid audio",
            message: err.message,
          });
        });
      });
    },
  });

  const borderColor = useMemo<string>(() => {
    if (isDragActive) return "gray.200";
    if (acceptedFiles.length > 0) return "green.500";
    if (isDragReject) return "red.500";
    return "gray.500";
  }, [isDragActive, acceptedFiles, isDragReject]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    onChange(e.target.files[0]);
  };

  return (
    <>
      <ErrorMessage
        errors={errors}
        name={name}
        render={({ message }) => (
          <Alert status="error" marginBottom={4}>
            <AlertIcon />
            {message}
          </Alert>
        )}
      />
      <Box>
        <Flex
          {...getRootProps()}
          justify="center"
          align="center"
          borderRadius={4}
          borderWidth={2}
          borderStyle="dashed"
          borderColor={borderColor}
        >
          <input {...getInputProps({ name, onChange: handleChange })} />
          <Flex direction="column" align="center" padding={4}>
            {acceptedFiles.length > 0 ? (
              <>
                <Icon as={FaFileAudio} boxSize={50} />
                <Text marginTop={2} isTruncated>
                  {acceptedFiles[0].name}
                </Text>
              </>
            ) : (
              <>
                <Icon as={FaCloudUploadAlt} boxSize={50} />
                <Text marginTop={2}>
                  Drop in an audio file or click to upload.
                </Text>
              </>
            )}
          </Flex>
        </Flex>
      </Box>
    </>
  );
};

export default AudioDropzone;

import { WarningIcon } from "@chakra-ui/icons";
import {
  Box,
  Flex,
  FormControl,
  FormErrorMessage,
  Icon,
  List,
  ListIcon,
  ListItem,
  Text,
  UnorderedList,
} from "@chakra-ui/react";
import React, { useMemo, useState } from "react";
import { useDropzone } from "react-dropzone";
import { FieldError } from "react-hook-form";
import { FaCloudUploadAlt, FaFileAudio } from "react-icons/fa";
import CONSTANTS from "~/constants";

interface AudioDropzoneProps {
  name: string;
  onChange: (file: File | FileList) => void;
  maxSize?: number;
  accept?: string[];
  error?: string;
}

const AudioDropzone: React.FC<AudioDropzoneProps> = ({
  name,
  onChange,
  error: formError,
  maxSize = CONSTANTS.UPLOAD_RULES.maxSize,
  accept = CONSTANTS.UPLOAD_RULES.accept,
}) => {
  const [errors, setErrors] = useState<string[]>(formError ? [formError] : []);

  const {
    acceptedFiles,

    getRootProps,
    getInputProps,
    isDragActive,
  } = useDropzone({
    multiple: false,
    maxSize: maxSize,
    accept: accept,
    onDrop: () => setErrors([]),
    onDropRejected: (fileRejections) => {
      const errs: string[] = [];
      fileRejections[0].errors.map((err) => errs.push(err.message));
      setErrors(errs);
    },
  });

  const borderColor = useMemo<string>(() => {
    if (isDragActive) return "gray.200";
    if (acceptedFiles.length > 0) return "green.500";
    if (errors.length > 0) return "red.500";
    return "gray.500";
  }, [isDragActive, acceptedFiles, errors]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    onChange(e.target.files[0]);
  };

  return (
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
      <List fontSize="sm" padding={4}>
        {errors.map((error, idx) => (
          <ListItem key={idx}>
            <Flex align="center">
              <ListIcon as={WarningIcon} color="red.500" />
              <Text color="red.500">{error}</Text>
            </Flex>
          </ListItem>
        ))}
      </List>
    </Box>
  );
};

export default AudioDropzone;

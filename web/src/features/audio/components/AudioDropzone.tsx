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
import axios from "axios";
import React, { useMemo } from "react";
import { useState } from "react";
import { useDropzone } from "react-dropzone";
import { useUser } from "~/features/user/hooks";
import api from "~/lib/api";
import SETTINGS from "~/lib/config";
import { formatFileSize } from "~/utils/format";
import { errorToast } from "~/utils/toast";

interface AudioDropzoneProps {
  onFileDrop: (file: File | null) => void;
  onUploaded: (uploadId: string) => void;
}

export default function AudioDropzone(props: AudioDropzoneProps) {
  const { onUploaded, onFileDrop } = props;
  const [user] = useUser();
  const [uploadProgress, setUploadProgress] = useState(0);
  const [uploading, setUploading] = useState(false);
  const [uploaded, setUploaded] = useState(false);

  const { getRootProps, getInputProps, open, isDragReject, isDragAccept } =
    useDropzone({
      accept: SETTINGS.UPLOAD.AUDIO.accept,
      maxSize: SETTINGS.UPLOAD.AUDIO.maxSize,
      maxFiles: 1,
      multiple: false,
      noClick: true,
      onDropAccepted: async ([file]) => {
        onFileDrop(file);
        try {
          setUploading(true);
          const { data: response } = await api.post<{
            uploadId: string;
            uploadUrl: string;
          }>("upload", {
            fileName: file.name,
            fileSize: file.size,
          });
          await axios.put(response.uploadUrl, file, {
            headers: {
              "Content-Type": file.type,
              "x-amz-meta-userId": `${user?.id}`,
            },
            onUploadProgress: (evt) => {
              const currentProgress = (evt.loaded / evt.total) * 100;
              setUploadProgress(currentProgress);
            },
          });
          setUploaded(true);
          onUploaded(response.uploadId);
        } catch (err) {
          onFileDrop(null);
          errorToast(err);
        }
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

  if (uploading) {
    return (
      <Box marginY={10} width="100%" spacing={4}>
        {uploaded ? (
          <chakra.span>Uploaded</chakra.span>
        ) : (
          <chakra.span>Uploading...</chakra.span>
        )}
        <Progress
          colorScheme="primary"
          hasStripe
          value={uploadProgress}
          width="full"
        />
      </Box>
    );
  }

  return (
    <Flex align="center" justify="center" height="50vh">
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

import {
  Box,
  Button,
  chakra,
  CircularProgress,
  CircularProgressLabel,
  CloseButton,
  Divider,
  Spacer,
  Stack,
  Text,
} from "@chakra-ui/react";
import React from "react";
import { useDropzone } from "react-dropzone";
import SETTINGS from "~/lib/config";
import { formatFileSize } from "~/utils/format";
import { errorToast } from "~/utils/toast";
import { useUploaderContext } from "./UploaderProvider";

interface AudioDropzoneProps {
  onFileDropped: (file: File) => void;
  onFileUploaded: (uploadId: string) => void;
  onFileCleared: () => void;
}

export default function AudioDropzone({
  onFileDropped,
  onFileUploaded,
  onFileCleared,
}: AudioDropzoneProps) {
  const { uploadFile, isUploaded, isUploading, uploadProgress, reset } =
    useUploaderContext();
  const { getRootProps, getInputProps, open, isDragReject, acceptedFiles } =
    useDropzone({
      accept: SETTINGS.UPLOAD.AUDIO.accept,
      maxSize: SETTINGS.UPLOAD.AUDIO.maxSize,
      maxFiles: 1,
      multiple: false,
      noClick: true,
      onDropAccepted: async ([droppedFile]) => {
        try {
          onFileDropped(droppedFile);
          const uploadId = await uploadFile(droppedFile);
          onFileUploaded(uploadId);
        } catch (err) {
          onFileCleared();
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

  const handleRemoveFile = () => {
    if (!confirm("Are you sure you want to remove file?")) return;
    onFileCleared();
    reset();
  };

  if (isUploading) {
    return (
      <Box marginBottom={4}>
        <CircularProgress value={uploadProgress} color="primary.500">
          <CircularProgressLabel>
            {Math.floor(uploadProgress)}%
          </CircularProgressLabel>
        </CircularProgress>
      </Box>
    );
  }

  if (isUploaded) {
    return (
      <Stack align="center" direction="row" spacing={4} marginBottom={4}>
        <chakra.div>
          {acceptedFiles[0]?.name ?? "undefined"} uploaded.
        </chakra.div>
        <CloseButton onClick={handleRemoveFile} />
      </Stack>
    );
  }

  return (
    <React.Fragment>
      <Stack
        marginBottom={4}
        align="center"
        direction="row"
        width="100%"
        {...getRootProps()}
      >
        <chakra.div>
          <input {...getInputProps()} />
          <Button colorScheme={isDragReject ? "red" : "primary"} onClick={open}>
            Upload your audio.
          </Button>
        </chakra.div>
        <Spacer />
        <Text fontSize="sm">
          MP3 file only. Maximum file size:{" "}
          {formatFileSize(SETTINGS.UPLOAD.AUDIO.maxSize)}
        </Text>
      </Stack>
      <Divider />
    </React.Fragment>
  );
}

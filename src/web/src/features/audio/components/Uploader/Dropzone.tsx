import {
  Box,
  Button,
  chakra,
  Text,
  CloseButton,
  Stack,
  Spacer,
  Divider,
  CircularProgress,
  CircularProgressLabel,
} from "@chakra-ui/react";
import axios from "axios";
import React, { useState } from "react";
import { useDropzone } from "react-dropzone";
import { useUser } from "~/features/user/hooks";
import SETTINGS from "~/lib/config";
import request from "~/lib/http";
import { getDurationFromAudioFile } from "~/utils";
import { formatFileSize } from "~/utils/format";
import { errorToast } from "~/utils/toast";

interface AudioDropzoneProps {
  onFileDropped?: () => void;
  onFileUploaded: (
    fileName: string,
    fileSize: number,
    uploadId: string,
    duration: number
  ) => void;
  onFileCleared: () => void;
}

export default function AudioDropzone({
  onFileDropped,
  onFileUploaded,
  onFileCleared,
}: AudioDropzoneProps) {
  const { user } = useUser();
  const [isUploading, setUploading] = useState(false);
  const [isUploaded, setUploaded] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const { getRootProps, getInputProps, open, isDragReject, acceptedFiles } =
    useDropzone({
      accept: SETTINGS.UPLOAD.AUDIO.accept,
      maxSize: SETTINGS.UPLOAD.AUDIO.maxSize,
      maxFiles: 1,
      multiple: false,
      noClick: true,
      onDropAccepted: async ([droppedFile]) => {
        try {
          onFileDropped?.();
          setUploading(true);
          const duration = await getDurationFromAudioFile(droppedFile);
          const { data: response } = await request<{
            uploadId: string;
            uploadUrl: string;
          }>({
            method: "post",
            url: "upload",
            data: {
              fileName: droppedFile.name,
              fileSize: droppedFile.size,
            },
          });
          await axios.put(response.uploadUrl, droppedFile, {
            headers: {
              "Content-Type": droppedFile.type,
              "x-amz-meta-userId": `${user?.id}`,
            },
            onUploadProgress: (evt) => {
              const currentProgress = (evt.loaded / evt.total) * 100;
              setUploadProgress(currentProgress);
            },
          });
          onFileUploaded(
            droppedFile.name,
            droppedFile.size,
            response.uploadId,
            duration
          );
          setUploaded(true);
        } catch (err) {
          onFileCleared();
          errorToast(err);
        } finally {
          setUploading(false);
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
    setUploaded(false);
    setUploadProgress(0);
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
        <chakra.div>{acceptedFiles[0].name} uploaded.</chakra.div>
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

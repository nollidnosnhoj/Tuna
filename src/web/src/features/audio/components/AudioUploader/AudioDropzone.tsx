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
import { useFormContext } from "react-hook-form";
import { useUser } from "~/features/user/hooks";
import SETTINGS from "~/lib/config";
import request from "~/lib/http";
import { getDurationFromAudioFile } from "~/utils";
import { formatFileSize } from "~/utils/format";
import { errorToast } from "~/utils/toast";
import { CreateAudioRequest } from "../../types";

interface AudioDropzoneProps {
  onFileDrop: (isFileUpload: boolean) => void;
}

export default function AudioDropzone({ onFileDrop }: AudioDropzoneProps) {
  const { user } = useUser();
  const [isUploading, setUploading] = useState(false);
  const [isUploaded, setUploaded] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const formContext = useFormContext<CreateAudioRequest>();
  const { setValue, getValues } = formContext;
  const { getRootProps, getInputProps, open, isDragReject } = useDropzone({
    accept: SETTINGS.UPLOAD.AUDIO.accept,
    maxSize: SETTINGS.UPLOAD.AUDIO.maxSize,
    maxFiles: 1,
    multiple: false,
    noClick: true,
    onDropAccepted: async ([droppedFile]) => {
      try {
        onFileDrop(true);
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
        setValue("fileName", droppedFile.name);
        setValue("fileSize", droppedFile.size);
        setValue("uploadId", response.uploadId);
        setValue("duration", duration);
        setUploaded(true);
      } catch (err) {
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
    setValue("fileName", "");
    setValue("fileSize", -1);
    setValue("uploadId", "");
    setValue("duration", -1);
    setUploaded(false);
    setUploadProgress(0);
    onFileDrop(false);
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
        <chakra.div>{getValues("fileName")} uploaded.</chakra.div>
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

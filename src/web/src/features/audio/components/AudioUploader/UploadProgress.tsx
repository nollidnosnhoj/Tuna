import { Box, chakra, Progress } from "@chakra-ui/react";
import React from "react";

interface UploadProgressProps {
  isUploading: boolean;
  isUploaded: boolean;
  progress: number;
}

export default function UploadProgress(props: UploadProgressProps) {
  const { isUploading, isUploaded, progress } = props;
  return (
    <Box
      marginY={10}
      width="100%"
      spacing={4}
      visibility={isUploading || isUploaded ? "visible" : "hidden"}
    >
      <chakra.span>{isUploaded ? "Uploaded" : "Uploading..."}</chakra.span>
      <Progress colorScheme="primary" hasStripe value={progress} width="full" />
    </Box>
  );
}

import { Box, Checkbox, Flex } from "@chakra-ui/react";
import React, { useState } from "react";
import AudioUploadDropzone from "../AudioDropzone";
import AudioUploading from "../AudioUploading";

export default function AudioUploadPage() {
  const [file, setFile] = useState<File | null>(null);

  if (file) {
    return <AudioUploading file={file} />;
  }

  return (
    <Box>
      <AudioUploadDropzone onUpload={(file) => setFile(file)} />
    </Box>
  );
}

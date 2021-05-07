import { Box, Checkbox, Flex } from "@chakra-ui/react";
import React, { useState } from "react";
import AudioUploadDropzone from "../AudioDropzone";
import AudioUploading from "../AudioUploading";

export default function AudioUploadPage() {
  const [file, setFile] = useState<File | null>(null);
  const [isPublic, setPublic] = useState(false);

  if (file) {
    return <AudioUploading file={file} isPublic={isPublic} />;
  }

  return (
    <Box>
      <AudioUploadDropzone onUpload={(file) => setFile(file)} />
      <Flex justifyContent="center">
        <Checkbox
          checked={isPublic}
          onChange={() => setPublic((prev) => !prev)}
        >
          Set audio to public after upload.
        </Checkbox>
      </Flex>
    </Box>
  );
}

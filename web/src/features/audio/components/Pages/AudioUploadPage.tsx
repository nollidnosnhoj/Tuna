import { Flex } from "@chakra-ui/layout";
import { Box, Checkbox } from "@chakra-ui/react";
import React, { useState } from "react";
import Page from "~/components/Page";
import AudioUploadDropzone from "../Upload/Dropzone";
import AudioUploading from "../Upload/Uploading";

export default function AudioUploadPage() {
  const [file, setFile] = useState<File | undefined>(undefined);
  const [filePublic, setFilePublic] = useState(false);

  if (file) {
    return (
      <Page title="Uploading...">
        <AudioUploading file={file} setToPublic={filePublic} />
      </Page>
    );
  }

  return (
    <Page title="Upload audio!">
      <AudioUploadDropzone
        files={file ? [file] : []}
        onDropAccepted={(files) => {
          setFile(files[0]);
        }}
      />
      <Flex justifyContent="center">
        <Box>
          <Checkbox
            checked={filePublic}
            onChange={() => setFilePublic((prev) => !prev)}
          >
            Set audio to public after upload.
          </Checkbox>
        </Box>
      </Flex>
    </Page>
  );
}

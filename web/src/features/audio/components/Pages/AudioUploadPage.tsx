import React, { useState } from "react";
import Page from "~/components/Page";
import AudioUploadDropzone from "../Upload/Dropzone";
import AudioUploading from "../Upload/Uploading";

export default function AudioUploadPage() {
  const [file, setFile] = useState<File | undefined>(undefined);

  if (file) {
    return (
      <Page title="Uploading...">
        <AudioUploading file={file} />
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
    </Page>
  );
}

import React from "react";
import Page from "~/components/Page";
import AudioUploader from "~/features/audio/components/Uploader";

const AudioUploadNextPage: React.FC = () => {
  return (
    <Page title="Upload" requiresAuth>
      <AudioUploader />
    </Page>
  );
};

export default AudioUploadNextPage;

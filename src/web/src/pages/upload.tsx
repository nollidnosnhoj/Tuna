import React from "react";
import Page from "~/components/Page";
import AudioUploader from "~/features/audio/components/Uploader";
import UploaderProvider from "~/features/audio/components/Uploader/UploaderProvider";

const AudioUploadNextPage: React.FC = () => {
  return (
    <Page title="Upload" requiresAuth>
      <UploaderProvider>
        <AudioUploader />
      </UploaderProvider>
    </Page>
  );
};

export default AudioUploadNextPage;

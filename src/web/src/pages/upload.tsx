import React from "react";
import Page from "~/components/Page";
import AudioUploader from "~/components/AudioUploader";
import UploaderProvider from "~/components/AudioUploader/UploaderProvider";

const AudioUploadNextPage: React.FC = () => {
  return (
    <Page title="Upload" requiresAuth requiresArtist>
      <UploaderProvider>
        <AudioUploader />
      </UploaderProvider>
    </Page>
  );
};

export default AudioUploadNextPage;

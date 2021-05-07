import React from "react";
import withRequiredAuth from "~/components/hoc/withRequiredAuth";
import Page from "~/components/Page";
import AudioUploadPage from "~/features/audio/components/Pages/AudioUploadPage";

const AudioUploadNextPage: React.FC = () => {
  return (
    <Page title="Upload audio!">
      <AudioUploadPage />
    </Page>
  );
};

export default withRequiredAuth(AudioUploadNextPage);

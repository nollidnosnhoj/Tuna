import React from "react";
import withRequiredAuth from "~/components/hoc/withRequiredAuth";
import Page from "~/components/Page";
import AudioUploadDropzone from "~/features/audio/components/AudioDropzone";

const AudioUploadNextPage: React.FC = () => {
  return (
    <Page title="Upload audio!">
      <AudioUploadDropzone />
    </Page>
  );
};

export default withRequiredAuth(AudioUploadNextPage);

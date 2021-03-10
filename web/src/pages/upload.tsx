import React from "react";
import withRequiredAuth from "~/components/hoc/withRequiredAuth";
import Page from "~/components/Page";
import AudioUpload from "~/features/audio/components/Upload";

export function UploadPage() {
  return (
    <Page title="Upload Audio">
      <AudioUpload />
    </Page>
  );
}

export default withRequiredAuth(UploadPage);

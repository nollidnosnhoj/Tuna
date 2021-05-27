import React, { useState } from "react";
import withRequiredAuth from "~/components/hoc/withRequiredAuth";
import Page from "~/components/Page";
import { useNavigationLock } from "~/lib/hooks";
import AudioUploadPage from "~/features/audio/components/Page/AudioUpload";

const AudioUploadNextPage: React.FC = () => {
  const [blockFromLeaving, setBlockFromLeaving] = useState(false);

  useNavigationLock(
    blockFromLeaving,
    "Are you sure you want to leave page? You will lose progress."
  );

  return (
    <Page title="Upload">
      <AudioUploadPage
        onUploading={() => setBlockFromLeaving(true)}
        onComplete={() => setBlockFromLeaving(false)}
      />
    </Page>
  );
};

export default withRequiredAuth(AudioUploadNextPage);

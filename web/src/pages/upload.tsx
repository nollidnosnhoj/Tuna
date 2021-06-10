import React, { useState } from "react";
import Page from "~/components/Page";
import { useNavigationLock } from "~/lib/hooks";
import AudioUploader from "~/features/audio/components/AudioUploader";

const AudioUploadNextPage: React.FC = () => {
  const [blockFromLeaving, setBlockFromLeaving] = useState(false);

  useNavigationLock(
    blockFromLeaving,
    "Are you sure you want to leave page? You will lose progress."
  );

  return (
    <Page title="Upload" requiresAuth>
      <AudioUploader
        onUploading={() => setBlockFromLeaving(true)}
        onComplete={() => setBlockFromLeaving(false)}
      />
    </Page>
  );
};

export default AudioUploadNextPage;

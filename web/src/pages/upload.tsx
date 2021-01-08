import { Text } from "@chakra-ui/react";
import React from "react";
import AudioUpload from "~/components/Audio/Upload";
import Page from "~/components/Shared/Page";
import useUser from "~/lib/contexts/user_context";

export default function UploadPage() {
  const { isAuth } = useUser();

  return (
    <Page title="Upload Audio">
      {isAuth ? <AudioUpload /> : <Text>Sample text</Text>}
    </Page>
  );
}

import { Text } from "@chakra-ui/react";
import React from "react";
import AudioUpload from "~/components/AudioUpload";
import PageLayout from "~/components/Layout";
import useUser from "~/lib/contexts/user_context";

export default function UploadPage() {
  const { isAuth } = useUser();

  return (
    <PageLayout title="Upload Audio">
      {isAuth ? <AudioUpload /> : <Text>Sample text</Text>}
    </PageLayout>
  );
}

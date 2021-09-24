import { Box, chakra, Flex, Spinner } from "@chakra-ui/react";
import { useRouter } from "next/router";
import React, { useState } from "react";
import { useEffect } from "react";
import Link from "~/components/UI/Link";
import { useNavigationLock } from "~/lib/hooks";

import { errorToast, toast } from "~/utils";
import { useCreateAudio } from "../../api/hooks";
import UploadForm, { UploadAudioFormValues } from "./UploadForm";

export default function AudioUploader() {
  const router = useRouter();
  const [audioLink, setAudioLink] = useState<string>("");
  const { mutateAsync: createAudio, isLoading: isCreatingAudio } =
    useCreateAudio();

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [unsavedChanges, setUnsavedChanges] = useNavigationLock(
    "Are you sure you want to leave page? You will lose progress."
  );

  const handleUpload = async (values: UploadAudioFormValues) => {
    try {
      const { slug } = await createAudio(values);
      setAudioLink(`/audios/${slug}`);
      setUnsavedChanges(false);
    } catch (err) {
      errorToast(err);
    }
  };

  useEffect(() => {
    if (audioLink && !unsavedChanges) {
      router.push(audioLink).then(() => {
        toast("success", { title: "Audio was successfully created." });
      });
    }
  }, [audioLink, unsavedChanges]);

  if (audioLink) {
    return (
      <Flex align="center" justify="center" height="25vh">
        <chakra.p>
          Your audio has been uploaded. We will now redirect you to your newly
          uploaded audio.
        </chakra.p>
        <chakra.p>
          If the page did not redirect, click{" "}
          <Link href={audioLink}>here to be redirected.</Link>
        </chakra.p>
      </Flex>
    );
  }

  if (isCreatingAudio) {
    return (
      <Flex align="center" justify="center" height="25vh">
        <Spinner thickness="4px" speed="0.65s" color="primary.500" size="xl" />
      </Flex>
    );
  }

  return (
    <Box>
      <UploadForm
        onSubmit={handleUpload}
        onFileDropped={() => setUnsavedChanges(true)}
        onFileCleared={() => setUnsavedChanges(false)}
      />
    </Box>
  );
}

import {
  Box,
  Button,
  chakra,
  Flex,
  Spacer,
  Spinner,
  Stack,
} from "@chakra-ui/react";
import { useRouter } from "next/router";
import React, { useState } from "react";
import { useEffect } from "react";
import { useNavigationLock } from "~/lib/hooks";
import { ID } from "~/lib/types";
import { errorToast, toast } from "~/utils";
import { useCreateAudio } from "../../api/hooks";
import UploadForm, { UploadAudioFormValues } from "./UploadForm";

export default function AudioUploader() {
  const router = useRouter();
  const [audioId, setAudioId] = useState<ID>(0);
  const { mutateAsync: createAudio, isLoading: isCreatingAudio } =
    useCreateAudio();

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [unsavedChanges, setUnsavedChanges] = useNavigationLock(
    "Are you sure you want to leave page? You will lose progress."
  );

  const handleUpload = async (values: UploadAudioFormValues) => {
    try {
      const { id } = await createAudio(values);
      setAudioId(id);
      setUnsavedChanges(false);
    } catch (err) {
      errorToast(err);
    }
  };

  useEffect(() => {
    if (audioId && !unsavedChanges) {
      router.push(`/audios/${audioId}`).then(() => {
        toast("success", { title: "Audio was successfully created." });
      });
    }
  }, [audioId, unsavedChanges]);

  if (audioId) {
    return (
      <Flex align="center" justify="center" height="25vh">
        <chakra.span>
          Audio created. You will be redirected to to newly created page.
        </chakra.span>
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
      <Stack direction="row">
        <Spacer />
        <Button type="submit">Submit</Button>
      </Stack>
    </Box>
  );
}

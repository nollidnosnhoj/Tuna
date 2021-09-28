import { chakra, Flex, Spinner } from "@chakra-ui/react";
import { useRouter } from "next/router";
import React, { useState } from "react";
import { useEffect } from "react";
import Link from "~/components/UI/Link";
import { useNavigationLock } from "~/lib/hooks";

import { errorToast, getDurationFromAudioFile, toast } from "~/utils";
import { useCreateAudio } from "../../api/hooks";
import { useUploaderContext } from "./UploaderProvider";
import UploadForm, { UploadAudioFormValues } from "./UploadForm";

export default function AudioUploader() {
  const router = useRouter();
  const { isUploaded, isUploading } = useUploaderContext();
  const [audioLink, setAudioLink] = useState<string>("");
  const { mutateAsync: createAudio, isLoading: isCreatingAudio } =
    useCreateAudio();

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [unsavedChanges, setUnsavedChanges] = useNavigationLock(
    "Are you sure you want to leave page? You will lose progress."
  );

  const handleUpload = async (values: UploadAudioFormValues) => {
    try {
      const { file, uploadId, ...audioRequest } = values;
      const duration = await getDurationFromAudioFile(values.file);
      const { slug } = await createAudio({
        ...audioRequest,
        duration,
        uploadId,
        fileName: (file as File).name,
        fileSize: (file as File).size,
      });
      setAudioLink(`/audios/${slug}`);
      setUnsavedChanges(false);
    } catch (err) {
      errorToast(err);
    }
  };

  /**
   * Handles the navigation locks depending on the state of the uploading
   */
  useEffect(() => {
    if (!isUploaded && !isUploading) {
      setUnsavedChanges(false);
    }

    if (isUploaded || (!isUploaded && isUploading)) {
      setUnsavedChanges(true);
    }
  }, [isUploading, isUploaded]);

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

  return <UploadForm onSubmit={handleUpload} />;
}

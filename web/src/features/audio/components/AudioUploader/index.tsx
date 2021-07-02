import { Box, chakra, Flex, Spinner } from "@chakra-ui/react";
import axios from "axios";
import { useRouter } from "next/router";
import React, { useMemo, useState } from "react";
import { useEffect } from "react";
import { useCallback } from "react";
import { useUser } from "~/features/user/hooks";
import { useNavigationLock } from "~/lib/hooks";
import request from "~/lib/http";
import {
  getDurationFromAudioFile,
  errorToast,
  getFilenameWithoutExtension,
  toast,
} from "~/utils";
import { useCreateAudio } from "../../hooks";
import { AudioId, AudioRequest } from "../../types";
import AudioForm from "../AudioForm";
import AudioDropzone from "./AudioDropzone";
import UploadProgress from "./UploadProgress";

export default function AudioUploader() {
  const router = useRouter();
  const { user } = useUser();
  const [audioId, setAudioId] = useState<AudioId>("");
  const [uploadId, setUploadId] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [duration, setDuration] = useState(0);
  const [uploadProgress, setUploadProgress] = useState(0);
  const { mutateAsync: createAudio, isLoading: isCreatingAudio } =
    useCreateAudio();

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [unsavedChanges, setUnsavedChanges] = useNavigationLock(
    "Are you sure you want to leave page? You will lose progress."
  );

  const initialFormValues = useMemo(
    () => ({
      title: getFilenameWithoutExtension(file?.name || "").slice(0, 30),
    }),
    [file?.name]
  );

  const onFileDrop = async (droppedFile: File) => {
    try {
      const duration = await getDurationFromAudioFile(droppedFile);
      setDuration(duration);
      setFile(droppedFile);
      setUnsavedChanges(true);
      const { data: response } = await request<{
        uploadId: string;
        uploadUrl: string;
      }>({
        method: "post",
        url: "upload",
        data: {
          fileName: droppedFile.name,
          fileSize: droppedFile.size,
        },
      });
      await axios.put(response.uploadUrl, droppedFile, {
        headers: {
          "Content-Type": droppedFile.type,
          "x-amz-meta-userId": `${user?.id}`,
        },
        onUploadProgress: (evt) => {
          const currentProgress = (evt.loaded / evt.total) * 100;
          setUploadProgress(currentProgress);
        },
      });
      setUploadId(response.uploadId);
    } catch (err) {
      setFile(null);
      errorToast(err);
    }
  };

  const handleFormSubmit = useCallback(
    async (values: AudioRequest) => {
      if (!!file && !!uploadId) {
        try {
          const { id } = await createAudio({
            ...values,
            fileName: file.name,
            fileSize: file.size,
            duration: duration,
            uploadId: uploadId,
          });
          setAudioId(id);
          setUnsavedChanges(false);
        } catch (err) {
          errorToast(err);
        }
      }
    },
    [file, uploadId, duration]
  );

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
      <AudioDropzone
        isHidden={!!file}
        onFileDrop={(file) => onFileDrop(file)}
      />
      <UploadProgress
        isUploaded={Boolean(uploadId)}
        isUploading={(!!file || uploadProgress > 0) && !uploadId}
        progress={uploadProgress}
      />
      {!!file && (
        <AudioForm
          initialValues={initialFormValues}
          onSubmit={handleFormSubmit}
          id="create-audio"
          leftFooter={
            <chakra.span>
              By publishing this audio, you have agreed to Audiochan's terms of
              service and license agreement.
            </chakra.span>
          }
          submitText="Upload"
          isDisabledButton={!uploadId}
        />
      )}
    </Box>
  );
}

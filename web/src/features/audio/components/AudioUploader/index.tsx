import { Box, Flex, Spinner } from "@chakra-ui/react";
import axios from "axios";
import { useRouter } from "next/router";
import React, { useEffect, useMemo, useState } from "react";
import { useUser } from "~/features/user/hooks";
import request from "~/lib/http";
import {
  getDurationFromAudioFile,
  errorToast,
  getFilenameWithoutExtension,
  toast,
} from "~/utils";
import { useCreateAudio } from "../../hooks";
import { AudioRequest } from "../../types";
import AudioDropzone from "./AudioDropzone";
import CreateAudioForm from "./CreateAudioForm";
import UploadProgress from "./UploadProgress";

interface AudioUploaderProps {
  onUploading: () => void;
  onComplete: () => void;
}

export default function AudioUploader({
  onUploading,
  onComplete,
}: AudioUploaderProps) {
  const router = useRouter();
  const { user } = useUser();
  const [audioId, setAudioId] = useState(0);
  const [uploadId, setUploadId] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [duration, setDuration] = useState(0);
  const [submitValues, setSubmitValues] =
    useState<AudioRequest | undefined>(undefined);
  const [uploadProgress, setUploadProgress] = useState(0);
  const { mutateAsync: createAudio } = useCreateAudio();

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
      onUploading();
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

  useEffect(() => {
    if (audioId) {
      router.push(`/audios/${audioId}`).then(() => {
        toast("success", { title: "Audio was successfully created." });
      });
    }
  }, [audioId]);

  useEffect(() => {
    if (!!file && !!submitValues && !!uploadId) {
      createAudio({
        ...submitValues,
        fileName: file.name,
        fileSize: file.size,
        duration: duration,
        uploadId: uploadId,
      })
        .then(({ id }) => {
          onComplete();
          setAudioId(id);
        })
        .catch((err) => {
          errorToast(err);
        });
    }
  }, [file, duration, submitValues, uploadId]);

  if (uploadId && submitValues) {
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
      <CreateAudioForm
        initialValues={initialFormValues}
        isHidden={!!submitValues || !file}
        onSubmit={(values) => setSubmitValues(values)}
      />
    </Box>
  );
}

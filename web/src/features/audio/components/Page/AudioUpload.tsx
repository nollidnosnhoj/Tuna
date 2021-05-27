import { Box, chakra, Flex, Spinner } from "@chakra-ui/react";
import { useRouter } from "next/router";
import React, { useEffect, useState } from "react";
import { getDurationFromAudioFile, errorToast, toast } from "~/utils";
import { useCreateAudio } from "../../hooks";
import { AudioRequest } from "../../types";
import AudioDropzone from "../AudioDropzone";
import CreateAudioForm from "../CreateAudioForm";

interface AudioUploadPageProps {
  onUploading: () => void;
  onComplete: () => void;
}

export default function AudioUploadPage({
  onUploading,
  onComplete,
}: AudioUploadPageProps) {
  const router = useRouter();
  const [uploadId, setUploadId] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [submitValues, setSubmitValues] =
    useState<AudioRequest | undefined>(undefined);
  const [audioId, setAudioId] = useState("");
  const { mutateAsync: createAudio } = useCreateAudio();

  useEffect(() => {
    if (!!file && !!submitValues && !!uploadId) {
      getDurationFromAudioFile(file)
        .then((duration) =>
          createAudio({
            ...submitValues,
            uploadId: uploadId,
            fileName: file.name,
            fileSize: file.size,
            duration: duration,
            contentType: file.type,
          })
        )
        .then(({ id }) => {
          onComplete();
          setAudioId(id);
        })
        .catch((err) => {
          errorToast(err);
        });
    }
  }, [file, submitValues, uploadId]);

  useEffect(() => {
    if (audioId) {
      router.push(`/audios/${audioId}`).then(() => {
        toast("success", { title: "Audio was successfully created." });
      });
    }
  }, [audioId]);

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
        onFileDrop={(file) => {
          setFile(file);
          onUploading();
        }}
        onUploaded={(id) => setUploadId(id)}
      />
      {!submitValues ? (
        <CreateAudioForm
          file={file}
          onSubmit={(values) => setSubmitValues(values)}
        />
      ) : (
        <Flex>
          <chakra.span>
            Once the audio finish uploading, you will have access to your newly
            uploaded audio.
          </chakra.span>
        </Flex>
      )}
    </Box>
  );
}

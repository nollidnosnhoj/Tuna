import {
  Drawer,
  DrawerOverlay,
  DrawerContent,
  DrawerCloseButton,
  DrawerHeader,
  DrawerBody,
  Button,
} from "@chakra-ui/react";
import { FormikHelpers } from "formik";
import React, { useState } from "react";
import { errorToast, toast } from "~/utils/toast";
import { useEditAudio, useRemoveAudio } from "../../hooks";
import { AudioDetailData, AudioRequest } from "../../types";
import AudioForm from "../AudioForm";
import { useRouter } from "next/router";

interface AudioEditDrawerProps {
  audio: AudioDetailData;
  isOpen: boolean;
  onClose: () => void;
  buttonRef?: React.RefObject<HTMLButtonElement>;
}

const AudioEditDrawer: React.FC<AudioEditDrawerProps> = (props) => {
  const { audio, isOpen, onClose, buttonRef } = props;
  const { id: audioId } = audio;
  const router = useRouter();
  const { mutateAsync: updateAudio } = useEditAudio(audioId);
  const { mutateAsync: deleteAudio } = useRemoveAudio(audioId);
  const [isProcessing, setIsProcessing] = useState(false);

  const onEditSubmit = async (
    values: AudioRequest,
    helpers?: FormikHelpers<AudioRequest>
  ) => {
    setIsProcessing(true);
    updateAudio(values)
      .then(() => {
        toast("success", { title: "Audio updated" });
        onClose();
      })
      .catch((err) => {
        errorToast(err);
      })
      .finally(() => {
        setIsProcessing(false);
        helpers?.setSubmitting(false);
      });
  };

  const onDeleteSubmit = () => {
    if (
      !confirm(
        "Are you sure you want to remove audio? You cannot undo this action."
      )
    ) {
      return;
    }

    setIsProcessing(true);
    deleteAudio(undefined)
      .then(() => {
        router.push("/").then(() => {
          toast("success", { description: "Audio have successfully removed." });
        });
      })
      .catch((err) => {
        errorToast(err);
        setIsProcessing(false);
      });
  };

  return (
    <Drawer
      size="md"
      isOpen={isOpen}
      placement="right"
      onClose={onClose}
      finalFocusRef={buttonRef}
    >
      <DrawerOverlay />
      <DrawerContent>
        <DrawerCloseButton />
        <DrawerHeader>Edit</DrawerHeader>
        <DrawerBody>
          <AudioForm
            initialValues={{
              title: audio.title,
              description: audio.description,
              tags: audio.tags,
              visibility: audio.visibility,
            }}
            onSubmit={onEditSubmit}
            id="create-audio"
            leftFooter={
              <Button
                colorScheme="red"
                onClick={onDeleteSubmit}
                disabled={isProcessing}
              >
                Remove
              </Button>
            }
            submitText="Modify"
          />
        </DrawerBody>
      </DrawerContent>
    </Drawer>
  );
};

export default AudioEditDrawer;

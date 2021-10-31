import {
  Button,
  Drawer,
  DrawerBody,
  DrawerCloseButton,
  DrawerContent,
  DrawerHeader,
  DrawerOverlay,
} from "@chakra-ui/react";
import React, { useState } from "react";
import { errorToast, toast } from "~/utils/toast";
import EditForm, { UpdateAudioFormValues } from "./forms/AudioEditForm";
import { useRouter } from "next/router";
import { Audio } from "~/lib/types";
import { useEditAudio, useRemoveAudio } from "~/lib/hooks/api";

interface AudioEditDrawerProps {
  audio: Audio;
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

  const onEditSubmit = async (values: UpdateAudioFormValues) => {
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
      });
  };

  const onDeleteSubmit = () => {
    // TODO: Implement confirm modal
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
          <EditForm
            audio={audio}
            onSubmit={onEditSubmit}
            isDisabled={isProcessing}
            removeButton={
              <Button
                colorScheme="red"
                variant="outline"
                onClick={onDeleteSubmit}
                disabled={isProcessing}
              >
                Remove
              </Button>
            }
          />
        </DrawerBody>
      </DrawerContent>
    </Drawer>
  );
};

export default AudioEditDrawer;

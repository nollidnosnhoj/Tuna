import {
  Drawer,
  DrawerOverlay,
  DrawerContent,
  DrawerCloseButton,
  DrawerHeader,
  DrawerBody,
  Button,
  Stack,
  Spacer,
} from "@chakra-ui/react";
import React, { useState } from "react";
import { z } from "zod";
import { errorToast, toast } from "~/utils/toast";
import { useEditAudio, useRemoveAudio } from "../api/hooks";
import { AudioView } from "../api/types";
import AudioForm from "./Form";
import { useRouter } from "next/router";
import { FormProvider, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { audioSchema } from "../schama";

interface AudioEditDrawerProps {
  audio: AudioView;
  isOpen: boolean;
  onClose: () => void;
  buttonRef?: React.RefObject<HTMLButtonElement>;
}

type UpdateAudioFormValues = z.infer<typeof audioSchema>;

const AudioEditDrawer: React.FC<AudioEditDrawerProps> = (props) => {
  const { audio, isOpen, onClose, buttonRef } = props;
  const { id: audioId } = audio;
  const router = useRouter();
  const { mutateAsync: updateAudio } = useEditAudio(audioId);
  const { mutateAsync: deleteAudio } = useRemoveAudio(audioId);
  const [isProcessing, setIsProcessing] = useState(false);

  const formMethods = useForm<UpdateAudioFormValues>({
    defaultValues: {
      title: audio.title,
      description: audio.description,
      tags: audio.tags,
    },
    resolver: zodResolver(audioSchema),
  });

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
          <form onSubmit={formMethods.handleSubmit(onEditSubmit)}>
            <FormProvider {...formMethods}>
              <AudioForm />
              <Stack direction="row">
                <Button
                  colorScheme="red"
                  onClick={onDeleteSubmit}
                  disabled={isProcessing}
                >
                  Remove
                </Button>
                <Spacer />
                <Button type="submit">Submit</Button>
              </Stack>
            </FormProvider>
          </form>
        </DrawerBody>
      </DrawerContent>
    </Drawer>
  );
};

export default AudioEditDrawer;

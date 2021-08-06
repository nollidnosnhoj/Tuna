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
import * as yup from "yup";
import { errorToast, toast } from "~/utils/toast";
import { useEditAudio, useRemoveAudio } from "../../hooks";
import { AudioView, AudioRequest, Visibility } from "../../api/types";
import AudioForm from "../AudioForm";
import { useRouter } from "next/router";
import { FormProvider, useForm } from "react-hook-form";
import { validationMessages } from "~/utils";
import { yupResolver } from "@hookform/resolvers/yup";

interface AudioEditDrawerProps {
  audio: AudioView;
  isOpen: boolean;
  onClose: () => void;
  buttonRef?: React.RefObject<HTMLButtonElement>;
}

const validationSchema: yup.SchemaOf<AudioRequest> = yup
  .object({
    title: yup
      .string()
      .defined()
      .required(validationMessages.required("Title"))
      .min(5, validationMessages.min("Title", 5))
      .max(30, validationMessages.max("Title", 30))
      .ensure(),
    description: yup
      .string()
      .defined()
      .max(500, validationMessages.max("Description", 500))
      .ensure(),
    tags: yup
      .array()
      .required()
      .max(10, validationMessages.max("Tags", 10))
      .ensure()
      .defined(),
    visibility: yup
      .mixed<Visibility>()
      .required(validationMessages.required("Visibility"))
      .oneOf([...Object.values(Visibility)], "Visibility choice is invalid."),
  })
  .defined();

const AudioEditDrawer: React.FC<AudioEditDrawerProps> = (props) => {
  const { audio, isOpen, onClose, buttonRef } = props;
  const { id: audioId } = audio;
  const router = useRouter();
  const { mutateAsync: updateAudio } = useEditAudio(audioId);
  const { mutateAsync: deleteAudio } = useRemoveAudio(audioId);
  const [isProcessing, setIsProcessing] = useState(false);

  const formMethods = useForm<AudioRequest>({
    defaultValues: {
      title: audio.title,
      description: audio.description,
      tags: audio.tags,
      visibility: audio.visibility,
    },
    resolver: yupResolver(validationSchema),
  });

  const onEditSubmit = async (values: AudioRequest) => {
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

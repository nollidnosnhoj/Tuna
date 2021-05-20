import {
  Drawer,
  DrawerOverlay,
  DrawerContent,
  DrawerCloseButton,
  DrawerHeader,
  DrawerBody,
  DrawerFooter,
  Button,
  Spacer,
  HStack,
} from "@chakra-ui/react";
import { Formik, FormikHelpers } from "formik";
import React, { useState } from "react";
import Router from "next/router";
import * as yup from "yup";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import { useEditAudio, useRemoveAudio } from "../hooks";
import { AudioDetailData, AudioRequest } from "../types";
import AudioForm from "./AudioForm";
import { validationMessages } from "~/utils";

interface AudioEditDrawerProps {
  audio: AudioDetailData;
  isOpen: boolean;
  onClose: () => void;
  buttonRef?: React.RefObject<HTMLButtonElement>;
}

const validationSchema = yup
  .object()
  .shape({
    title: yup
      .string()
      .required(validationMessages.required("Title"))
      .max(30, validationMessages.max("Title", 30))
      .defined(),
    description: yup
      .string()
      .max(500, validationMessages.max("Description", 500))
      .ensure()
      .defined(),
    tags: yup
      .array(yup.string())
      .max(10, validationMessages.max("Tags", 10))
      .ensure()
      .defined(),
    isPublic: yup.boolean().defined(),
  })
  .defined();

const AudioEditDrawer: React.FC<AudioEditDrawerProps> = (props) => {
  const { audio, isOpen, onClose, buttonRef } = props;

  const { id: audioId } = audio;
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
        successfulToast({ title: "Audio updated" });
        onClose();
      })
      .catch((err) => {
        apiErrorToast(err);
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
        Router.push("/").then(() => {
          successfulToast({
            title: "Audio deleted!",
          });
        });
      })
      .catch((err) => {
        apiErrorToast(err);
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
          <Formik
            initialValues={{
              title: audio.title,
              description: audio.description,
              tags: audio.tags,
              isPublic: audio.isPublic,
            }}
            validationSchema={validationSchema}
            onSubmit={onEditSubmit}
          >
            {({ handleSubmit }) => (
              <form id="edit-form" onSubmit={handleSubmit}>
                <AudioForm disableFields={isProcessing} />
              </form>
            )}
          </Formik>
        </DrawerBody>

        <DrawerFooter>
          <HStack>
            <Button
              colorScheme="red"
              onClick={onDeleteSubmit}
              disabled={isProcessing}
            >
              Remove
            </Button>
            <Spacer />
            <Button
              colorScheme="blue"
              type="submit"
              isLoading={isProcessing}
              loadingText="Processing..."
              form="edit-form"
            >
              Modify
            </Button>
          </HStack>
        </DrawerFooter>
      </DrawerContent>
    </Drawer>
  );
};

export default AudioEditDrawer;

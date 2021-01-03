import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalCloseButton,
  useToast,
} from "@chakra-ui/react";
import React, { useMemo, useState } from "react";
import Router from "next/router";
import AudioForm from "~/components/AudioForm";
import { Audio, AudioRequest } from "~/lib/types";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import { deleteAudio, updateAudio } from "~/lib/services/audio";

interface AudioEditProps {
  model: Audio;
  isOpen: boolean;
  onClose: () => void;
}

function mapAudioToModifyInputs(audio: Audio): AudioRequest {
  return {
    title: audio.title,
    description: audio.description,
    tags: audio.tags,
    isPublic: audio.isPublic,
  };
}

const AudioEditModal: React.FC<AudioEditProps> = ({
  model,
  isOpen,
  onClose,
}) => {
  const [isSubmitting, setSubmitting] = useState(false);
  const currentValues = useMemo(() => mapAudioToModifyInputs(model), [model]);

  const onDeleteSubmit = async () => {
    setSubmitting(true);
    try {
      await deleteAudio(model.id);
      Router.push("/");
      successfulToast({
        title: "Audio deleted!",
      });
    } catch (err) {
      apiErrorToast(err);
    }
    setSubmitting(false);
  };

  const onEditSubmit = async (inputs: AudioRequest) => {
    setSubmitting(true);

    const newRequest = {};
    if (currentValues) {
      Object.entries(inputs).forEach(([key, value]) => {
        if (currentValues[key] !== value) {
          newRequest[key] = value;
        }
      });
    } else {
      Object.assign(newRequest, inputs ?? {});
    }

    try {
      await updateAudio(model, newRequest);
      successfulToast({ title: "Audio updated" });
      onClose();
    } catch (err) {
      apiErrorToast(err);
    }
    setSubmitting(false);
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Edit '{model.title}'</ModalHeader>
        {!isSubmitting && <ModalCloseButton />}
        <ModalBody>
          <AudioForm
            type="edit"
            currentValues={currentValues}
            onSubmit={onEditSubmit}
            onDelete={onDeleteSubmit}
          />
        </ModalBody>
      </ModalContent>
    </Modal>
  );
};

export default AudioEditModal;

import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalCloseButton,
  Box,
  Button,
  ButtonGroup,
  Flex,
  IconButton,
  Popover,
  PopoverArrow,
  PopoverBody,
  PopoverCloseButton,
  PopoverContent,
  PopoverFooter,
  PopoverHeader,
  PopoverTrigger,
  Spacer,
} from "@chakra-ui/react";
import { DeleteIcon } from "@chakra-ui/icons";
import React, { useEffect, useMemo, useState } from "react";
import Router from "next/router";
import { Controller, useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import TextInput from "../Form/TextInput";
import TagInput from "../Form/TagInput";
import { Audio, AudioRequest } from "~/lib/types";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import { deleteAudio, updateAudio } from "~/lib/services/audio";
import { audioSchema } from "~/lib/validationSchemas";
import InputCheckbox from "../Form/Checkbox";

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
  const currentValues = useMemo(() => mapAudioToModifyInputs(model), [model]);
  const [deleting, setDeleting] = useState(false);

  const {
    register,
    reset,
    handleSubmit,
    control,
    errors,
    formState: { isSubmitting },
  } = useForm<AudioRequest>({
    defaultValues: currentValues,
    resolver: yupResolver(audioSchema("edit")),
  });

  useEffect(() => {
    reset(currentValues);
  }, [reset, currentValues]);

  const onDeleteSubmit = async () => {
    setDeleting(true);
    try {
      await deleteAudio(model.id);
      Router.push("/");
      successfulToast({
        title: "Audio deleted!",
      });
    } catch (err) {
      apiErrorToast(err);
    }
    setDeleting(false);
  };

  const onEditSubmit = async (inputs: AudioRequest) => {
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
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Edit '{model.title}'</ModalHeader>
        {!isSubmitting && <ModalCloseButton />}
        <ModalBody>
          <form onSubmit={handleSubmit(onEditSubmit)}>
            <TextInput
              name="title"
              type="text"
              ref={register}
              label="Title"
              error={errors.title}
              disabled={isSubmitting || deleting}
              isRequired
            />
            <TextInput
              name="description"
              ref={register}
              label="Description"
              error={errors.description}
              disabled={isSubmitting || deleting}
              isTextArea
            />
            <Controller
              name="tags"
              control={control}
              render={({ name, value, onChange }) => (
                <TagInput
                  name={name}
                  value={value}
                  onChange={onChange}
                  error={errors.tags && errors.tags[0]}
                  disabled={isSubmitting || deleting}
                />
              )}
            />
            <InputCheckbox
              name="isPublic"
              label="Public?"
              ref={register}
              isInvalid={!!errors.isPublic}
              disabled={isSubmitting || deleting}
              isRequired={true}
              isSwitch={true}
            />
            <Flex marginY={4}>
              <Box>
                <Popover>
                  <PopoverTrigger>
                    <IconButton
                      colorScheme="red"
                      variant="outline"
                      aria-label="Remove upload"
                      icon={<DeleteIcon />}
                      isLoading={isSubmitting || deleting}
                    >
                      Delete
                    </IconButton>
                  </PopoverTrigger>
                  <PopoverContent>
                    <PopoverArrow />
                    <PopoverCloseButton />
                    <PopoverHeader>Remove Confirmation</PopoverHeader>
                    <PopoverBody>
                      Are you sure you want to remove this upload? You cannot
                      undo this action.
                    </PopoverBody>
                    <PopoverFooter d="flex" justifyContent="flex-end">
                      <ButtonGroup size="sm">
                        <Button
                          colorScheme="red"
                          onClick={onDeleteSubmit}
                          disabled={isSubmitting || deleting}
                        >
                          Remove
                        </Button>
                      </ButtonGroup>
                    </PopoverFooter>
                  </PopoverContent>
                </Popover>
              </Box>
              <Spacer />
              <Box>
                <Button
                  colorScheme="blue"
                  type="submit"
                  isLoading={isSubmitting || deleting}
                  loadingText="Processing..."
                >
                  Modify
                </Button>
              </Box>
            </Flex>
          </form>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
};

export default AudioEditModal;

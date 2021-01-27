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
import { Controller, FormProvider, useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import InputCheckbox from "../Form/Checkbox";
import TextInput from "../Form/TextInput";
import TagInput from "../Form/TagInput";
import { AudioDetailModel, EditAudioRequest } from "~/lib/types/audio";
import { useEditAudio, useRemoveAudio } from "~/lib/services/audio";
import { editAudioSchema } from "~/lib/validationSchemas";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import GenreSelect from "../Form/GenreSelect";

interface AudioEditProps {
  model: AudioDetailModel;
  isOpen: boolean;
  onClose: () => void;
  isDev?: boolean;
}

function mapAudioToModifyInputs(audio: AudioDetailModel): EditAudioRequest {
  return {
    title: audio.title,
    description: audio.description,
    tags: audio.tags,
    isPublic: audio.isPublic,
    genre: audio.genre.slug,
  };
}

const AudioEditModal: React.FC<AudioEditProps> = ({
  model,
  isOpen,
  onClose,
  isDev = false,
}) => {
  const currentValues = useMemo(() => mapAudioToModifyInputs(model), [model]);
  const { mutateAsync: updateAudio } = useEditAudio(model.id);
  const { mutateAsync: deleteAudio } = useRemoveAudio(model.id);
  const [deleting, setDeleting] = useState(false);

  const methods = useForm<EditAudioRequest>({
    defaultValues: currentValues,
    resolver: yupResolver(editAudioSchema),
  });

  const {
    reset,
    handleSubmit,
    control,
    errors,
    formState: { isSubmitting },
  } = methods;

  useEffect(() => {
    reset(currentValues);
  }, [reset, currentValues]);

  const onDeleteSubmit = async () => {
    setDeleting(true);
    try {
      await deleteAudio();
      Router.push("/");
      successfulToast({
        title: "Audio deleted!",
      });
    } catch (err) {
      apiErrorToast(err);
    }
    setDeleting(false);
  };

  const onEditSubmit = async (inputs: EditAudioRequest) => {
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
      await updateAudio(newRequest);
      successfulToast({ title: "Audio updated" });
      onClose();
    } catch (err) {
      apiErrorToast(err);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="xl">
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Edit '{model.title}'</ModalHeader>
        {!isSubmitting && <ModalCloseButton />}
        <ModalBody>
          <FormProvider {...methods}>
            <form onSubmit={handleSubmit(onEditSubmit)}>
              <TextInput
                name="title"
                type="text"
                label="Title"
                disabled={isSubmitting || deleting}
                required
              />
              <TextInput
                name="description"
                label="Description"
                disabled={isSubmitting || deleting}
                textArea
              />
              <GenreSelect
                name="genre"
                placeholder="Select Genre"
                required
                disabled={isSubmitting}
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
                disabled={isSubmitting || deleting}
                required
                toggleSwitch
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
          </FormProvider>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
};

export default AudioEditModal;

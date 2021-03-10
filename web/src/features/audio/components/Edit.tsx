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
import { useFormik } from "formik";
import InputCheckbox from "../../../components/Form/Checkbox";
import GenreSelect from "../../../components/Form/GenreSelect";
import TextInput from "../../../components/Form/TextInput";
import TagInput from "../../../components/Form/TagInput";
import { Audio, AudioRequest } from "~/features/audio/types";
import { useEditAudio, useRemoveAudio } from "~/features/audio/hooks/mutations";
import { editAudioSchema } from "~/features/audio/schemas";
import { apiErrorToast, successfulToast } from "~/utils/toast";

interface AudioEditProps {
  audio: Audio;
  isOpen: boolean;
  onClose: () => void;
}

function mapAudioToModifyInputs(audio: Audio): AudioRequest {
  return {
    title: audio.title,
    description: audio.description,
    tags: audio.tags,
    isPublic: audio.isPublic,
    genre: audio.genre?.slug,
  };
}

const AudioEditModal: React.FC<AudioEditProps> = ({
  audio,
  isOpen,
  onClose,
}) => {
  const { id: audioId } = audio;
  const currentValues = useMemo(() => mapAudioToModifyInputs(audio), [audio]);
  const { mutateAsync: updateAudio } = useEditAudio(audioId);
  const { mutateAsync: deleteAudio } = useRemoveAudio(audioId);
  const [deleting, setDeleting] = useState(false);

  const formik = useFormik<AudioRequest>({
    initialValues: currentValues,
    validationSchema: editAudioSchema,
    onSubmit: (inputs, { setSubmitting }) => {
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

      updateAudio(newRequest)
        .then(() => {
          successfulToast({ title: "Audio updated" });
          onClose();
        })
        .catch((err) => {
          apiErrorToast(err);
        })
        .finally(() => setSubmitting(false));
    },
  });

  const {
    isSubmitting,
    values,
    errors,
    handleChange,
    handleSubmit,
    setFieldValue,
  } = formik;

  const onDeleteSubmit = () => {
    setDeleting(true);
    deleteAudio()
      .then(() => {
        Router.push("/").then(() => {
          successfulToast({
            title: "Audio deleted!",
          });
        });
      })
      .catch((err) => {
        apiErrorToast(err);
        setDeleting(false);
      });
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="xl">
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Edit '{audio.title}'</ModalHeader>
        {!isSubmitting && <ModalCloseButton />}
        <ModalBody>
          <form onSubmit={handleSubmit}>
            <TextInput
              name="title"
              type="text"
              label="Title"
              value={values.title}
              onChange={handleChange}
              error={errors.title}
              disabled={isSubmitting || deleting}
              required
            />
            <TextInput
              name="description"
              label="Description"
              value={values.description ?? ""}
              onChange={handleChange}
              error={errors.description}
              disabled={isSubmitting || deleting}
              textArea
            />
            <GenreSelect
              name="genre"
              label="Genre"
              value={values.genre ?? ""}
              onChange={handleChange}
              error={errors.genre}
              placeholder="Select Genre"
              disabled={isSubmitting}
            />
            <TagInput
              name="tags"
              value={values.tags}
              onAdd={(tag) => {
                setFieldValue("tags", [...values.tags, tag]);
              }}
              onRemove={(index) => {
                setFieldValue(
                  "tags",
                  values.tags.filter((_, i) => i !== index)
                );
              }}
              error={errors.tags}
              disabled={isSubmitting || deleting}
            />
            <InputCheckbox
              name="isPublic"
              label="Public?"
              value={values.isPublic}
              onChange={() => setFieldValue("isPublic", !values.isPublic)}
              error={errors.isPublic}
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
        </ModalBody>
      </ModalContent>
    </Modal>
  );
};

export default AudioEditModal;

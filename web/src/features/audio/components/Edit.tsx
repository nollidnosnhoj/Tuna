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
  FormControl,
  FormLabel,
  Select,
  FormHelperText,
  ListItem,
  UnorderedList,
} from "@chakra-ui/react";
import { DeleteIcon } from "@chakra-ui/icons";
import React, { useMemo, useState } from "react";
import Router from "next/router";
import { useFormik } from "formik";
import TextInput from "../../../components/form/TextInput";
import TagInput from "../../../components/form/TagInput";
import { AudioDetail, AudioRequest } from "~/features/audio/types";
import { useRemoveAudio, useEditAudio } from "~/features/audio/hooks/mutations";
import { editAudioSchema } from "~/features/audio/schemas";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import InputCheckbox from "~/components/form/Checkbox";

interface AudioEditProps {
  audio: AudioDetail;
  isOpen: boolean;
  onClose: () => void;
}

function mapAudioToModifyInputs(audio: AudioDetail): AudioRequest {
  return {
    title: audio.title,
    description: audio.description,
    tags: audio.tags,
    isPublic: audio.isPublic,
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
      updateAudio(inputs)
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
              value={values.title ?? ""}
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
              onChange={() => setFieldValue("isPublic", !values.isPublic)}
              value={values.isPublic ?? false}
              required
              error={errors.isPublic}
              toggleSwitch
              label="Public"
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

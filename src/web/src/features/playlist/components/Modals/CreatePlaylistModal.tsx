import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  ModalFooter,
  Button,
} from "@chakra-ui/react";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import TagInput from "~/components/Forms/Inputs/Tags";
import TextInput from "~/components/Forms/Inputs/Text";
import { useAddToPlaylist } from "~/lib/stores";
import { errorToast, toast } from "~/utils";
import { useCreatePlaylist } from "../../api/hooks";
import { PlaylistRequest } from "../../api/types";

interface CreatePlaylistModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function CreatePlaylistModal({
  isOpen,
  onClose,
}: CreatePlaylistModalProps) {
  const selectedAudioIds = useAddToPlaylist((state) => state.selectedIds);
  const { mutateAsync } = useCreatePlaylist();
  const formMethods = useForm<PlaylistRequest>({
    defaultValues: {
      title: "",
      tags: [],
    },
  });

  const { handleSubmit, register, control, formState } = formMethods;
  const { errors, isSubmitting } = formState;

  const handleCreatePlaylist = async (values: PlaylistRequest) => {
    const input = { ...values, audioIds: selectedAudioIds };
    await mutateAsync(input, {
      onSuccess() {
        toast("success", {
          title: "Create Playlist",
          description: "Success!",
        });
      },
      onError(err) {
        errorToast(err);
      },
      onSettled() {
        onClose();
      },
    });
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Create Playlist</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <form>
            <TextInput
              {...register("title")}
              label="Title"
              error={errors.title?.message}
            />
            <TextInput
              {...register("description")}
              label="Description"
              error={errors.description?.message}
              isTextArea
            />
            <Controller
              name="tags"
              control={control}
              render={({
                // eslint-disable-next-line @typescript-eslint/no-unused-vars
                field: { ref: _, ...restField },
                fieldState: { error },
              }) => (
                <TagInput
                  placeholder="Add Tag..."
                  error={error?.message}
                  {...restField}
                  disabled={isSubmitting}
                />
              )}
            />
          </form>
        </ModalBody>

        <ModalFooter>
          <Button
            onClick={handleSubmit(handleCreatePlaylist)}
            variant="ghost"
            isLoading={isSubmitting}
          >
            Create
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  );
}

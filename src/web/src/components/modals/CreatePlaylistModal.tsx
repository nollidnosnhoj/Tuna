import {
  Button,
  Modal,
  ModalBody,
  ModalCloseButton,
  ModalContent,
  ModalFooter,
  ModalHeader,
  ModalOverlay,
} from "@chakra-ui/react";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import TagInput from "~/components/form-inputs/TagField";
import InputField from "~/components/form-inputs/InputField";
import { useAddToPlaylist } from "~/lib/stores";
import { errorToast, toast } from "~/utils";
import { PlaylistRequest } from "~/lib/types";
import { useCreatePlaylist } from "~/lib/hooks/api";
import TextAreaField from "~/components/form-inputs/TextAreaField";

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
            <InputField
              {...register("title")}
              label="Title"
              error={errors.title?.message}
            />
            <TextAreaField
              {...register("description")}
              label="Description"
              error={errors.description?.message}
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

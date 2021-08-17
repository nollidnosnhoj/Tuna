import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  chakra,
  ModalFooter,
  Button,
} from "@chakra-ui/react";
import React from "react";
import { useAddToPlaylist } from "~/lib/stores";
import { errorToast, toast } from "~/utils";
import { useAddAudiosToPlaylist } from "../../api/hooks";
import { PlaylistId } from "../../api/types";

interface DuplicateAudiosModalProps {
  playlistId: PlaylistId;
  onModalClose: () => void;
}

export default function DuplicateAudiosModal({
  playlistId,
  onModalClose,
}: DuplicateAudiosModalProps) {
  const { addDups, duplicateOpen, selectedIds } = useAddToPlaylist();

  const { mutateAsync: addToPlaylistAsync, isLoading: isAddingAudios } =
    useAddAudiosToPlaylist();

  const handleSubmitAfterDuplicateCheck = async () => {
    const input = {
      id: playlistId,
      audioIds: selectedIds,
    };
    await addToPlaylistAsync(input, {
      onSuccess() {
        toast("success", {
          title: "Adding to Playlist",
          description: "Success!",
        });
      },
      onError(err) {
        errorToast(err);
      },
      onSettled() {
        handleCloseModal();
      },
    });
  };

  const handleCloseModal = () => {
    addDups([]);
    onModalClose();
  };

  return (
    <Modal isOpen={duplicateOpen} onClose={handleCloseModal}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Duplicate Audios</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <chakra.span>
            One of the audios you are trying to add already exist in the
            playlist.
          </chakra.span>
        </ModalBody>

        <ModalFooter>
          <Button onClick={handleCloseModal} disabled={isAddingAudios}>
            Skip
          </Button>
          <Button
            onClick={handleSubmitAfterDuplicateCheck}
            variant="ghost"
            isLoading={isAddingAudios}
          >
            Add Anyways
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  );
}

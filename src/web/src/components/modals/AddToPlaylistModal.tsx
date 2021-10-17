import {
  Button,
  Link as ChakraLink,
  Modal,
  ModalBody,
  ModalCloseButton,
  ModalContent,
  ModalFooter,
  ModalHeader,
  ModalOverlay,
  Table,
  Tbody,
  Td,
  Tfoot,
  Th,
  Tr,
} from "@chakra-ui/react";
import React, { useState } from "react";
import { useAddToPlaylist } from "~/lib/stores";
import { ID } from "~/lib/types";
import { checkDuplicatedAudiosRequest, toast } from "~/utils";
import CreatePlaylistModal from "./CreatePlaylistModal";
import DuplicateAudiosModal from "./DuplicateAudiosModal";
import { useAddAudiosToPlaylist, useYourPlaylists } from "~/lib/hooks/api";

export default function AddToPlaylistModal() {
  const { open, addDups, closeDialog, selectedIds } = useAddToPlaylist();
  const [playlistId, setPlaylistId] = useState<ID>(0);
  const [createPlaylist, setCreatePlaylist] = useState(false);

  const {
    items: playlists,
    fetchNextPage,
    hasNextPage,
  } = useYourPlaylists({
    enabled: selectedIds.length > 0,
  });

  const { mutateAsync: addToPlaylistAsync, isLoading: isAddingAudios } =
    useAddAudiosToPlaylist();

  const handleClosingModal = () => {
    setCreatePlaylist(false);
    setPlaylistId(0);
    closeDialog();
  };

  const loadMorePlaylists = () => {
    fetchNextPage();
  };

  const handleSubmit = async (playlistId: ID) => {
    try {
      setPlaylistId(playlistId);
      const dupIds = await checkDuplicatedAudiosRequest(
        playlistId,
        selectedIds
      );
      if (dupIds.length > 0) {
        return addDups(dupIds);
      }

      const input = {
        playlistId,
        audioIds: selectedIds,
      };

      await addToPlaylistAsync(input, {
        onSuccess() {
          toast("success", {
            title: "Adding to Playlist",
            description: "Success!",
          });
          handleClosingModal();
        },
      });
    } catch (err) {
      console.log(err);
      handleClosingModal();
    }
  };

  return (
    <>
      <Modal isOpen={open} onClose={handleClosingModal}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Add To Playlist</ModalHeader>
          {!isAddingAudios && <ModalCloseButton />}
          <ModalBody>
            <>
              {playlists.length > 0 && (
                <Table variant="striped">
                  <Tbody>
                    {playlists.map((playlist, index) => (
                      <Tr key={index}>
                        <Td>
                          <ChakraLink onClick={() => handleSubmit(playlist.id)}>
                            {playlist.title}
                          </ChakraLink>
                        </Td>
                      </Tr>
                    ))}
                  </Tbody>
                  {hasNextPage && (
                    <Tfoot>
                      <Tr>
                        <Th onClick={loadMorePlaylists}>Load More</Th>
                      </Tr>
                    </Tfoot>
                  )}
                </Table>
              )}
            </>
          </ModalBody>
          <ModalFooter>
            <Button variant="ghost" onClick={() => setCreatePlaylist(true)}>
              Create Playlist
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
      <DuplicateAudiosModal
        playlistId={playlistId}
        onModalClose={handleClosingModal}
      />
      <CreatePlaylistModal
        isOpen={createPlaylist}
        onClose={handleClosingModal}
      />
    </>
  );
}

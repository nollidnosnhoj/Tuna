import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  ModalFooter,
  Button,
  Table,
  Tbody,
  Td,
  Tfoot,
  Th,
  Link as ChakraLink,
  Tr,
  chakra,
  Flex,
  CircularProgress,
} from "@chakra-ui/react";
import React from "react";
import { useState } from "react";
import { Visibility } from "~/features/audio/types";
import { useYourPlaylists } from "~/features/auth/hooks";
import { useAddToPlaylist } from "~/lib/stores";
import { checkDuplicatedAudiosRequest } from "../api";
import { useAddAudiosToPlaylist, useCreatePlaylist } from "../hooks";

export default function AddAudiosToPlaylistDialog() {
  const {
    open,
    duplicateOpen,
    addDups,
    closeDialog,
    selectedIds,
    defaultPlaylistTitle,
  } = useAddToPlaylist();
  const [playlistId, setPlaylistId] = useState("");

  const {
    items: playlists,
    fetchNextPage,
    hasNextPage,
  } = useYourPlaylists({
    enabled: selectedIds.length > 0,
  });
  const { mutateAsync: createPlaylistAsync, isLoading: isCreatingPlaylist } =
    useCreatePlaylist();
  const { mutateAsync: addToPlaylistAsync, isLoading: isAddingAudios } =
    useAddAudiosToPlaylist();

  const handleClosingDialog = () => {
    setPlaylistId("");
    closeDialog();
  };

  const handleClosingDuplicateDialog = () => {
    addDups([]);
  };

  const loadMorePlaylists = () => {
    fetchNextPage();
  };

  const handleCreatePlaylist = async () => {
    try {
      await createPlaylistAsync({
        audioIds: selectedIds,
        title: defaultPlaylistTitle,
        tags: [],
        visibility: Visibility.Private,
      });
    } catch (err) {
      console.log(err);
    } finally {
      handleClosingDialog();
    }
  };

  const handleSubmitAfterDuplicateCheck = async () => {
    try {
      handleClosingDuplicateDialog();
      await addToPlaylistAsync({
        id: playlistId,
        audioIds: selectedIds,
      });
    } catch (err) {
      console.log(err);
    } finally {
      handleClosingDialog();
    }
  };

  const handleSubmit = async (id: string) => {
    try {
      setPlaylistId(id);
      const dupIds = await checkDuplicatedAudiosRequest(id, selectedIds);
      if (dupIds.length > 0) {
        return addDups(dupIds);
      }

      await addToPlaylistAsync({
        id,
        audioIds: selectedIds,
      });

      handleClosingDialog();
    } catch (err) {
      console.log(err);
      handleClosingDialog();
    }
  };

  return (
    <>
      <Modal isOpen={open} onClose={handleClosingDialog}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Add To Playlist</ModalHeader>
          {!isAddingAudios && !isCreatingPlaylist && <ModalCloseButton />}
          <ModalBody>
            {isAddingAudios || isCreatingPlaylist ? (
              <Flex justify="center" align="center" marginY={8}>
                <CircularProgress isIndeterminate size="md" />
              </Flex>
            ) : (
              <>
                {playlists.length > 0 && (
                  <Table variant="striped">
                    <Tbody>
                      {playlists.map((playlist, index) => (
                        <Tr key={index}>
                          <Td>
                            <ChakraLink
                              onClick={() => handleSubmit(playlist.id)}
                            >
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
            )}
          </ModalBody>
          <ModalFooter>
            <Button variant="ghost" onClick={handleCreatePlaylist}>
              Create Playlist
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
      <Modal isOpen={duplicateOpen} onClose={handleClosingDuplicateDialog}>
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
            <Button onClick={handleClosingDuplicateDialog}>Close</Button>
            <Button onClick={handleSubmitAfterDuplicateCheck} variant="ghost">
              Add Anyways
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
}

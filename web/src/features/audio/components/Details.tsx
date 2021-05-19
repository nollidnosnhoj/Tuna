import {
  Badge,
  Box,
  Flex,
  Heading,
  IconButton,
  Spacer,
  Stack,
  Text,
  Tooltip,
  useColorModeValue,
  useDisclosure,
  VStack,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
} from "@chakra-ui/react";
import Router from "next/router";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { EditIcon } from "@chakra-ui/icons";
import { FaPause, FaPlay } from "react-icons/fa";
import { MdQueueMusic } from "react-icons/md";
import { HiDotsVertical } from "react-icons/hi";
import Link from "~/components/Link";
import Picture from "~/components/Picture";
import { mapAudioForAudioQueue } from "~/utils/audioplayer";
import { useAddAudioPicture } from "~/features/audio/hooks/useAddAudioPicture";
import { AudioDetail } from "~/features/audio/types";
import { useAudioPlayer } from "~/lib/hooks/useAudioPlayer";
import { useUser } from "~/lib/hooks/useUser";
import { relativeDate } from "~/utils/time";
import AudioEditDrawer from "./AudioEditDrawer";

interface AudioDetailProps {
  audio: AudioDetail;
}

const AudioDetails: React.FC<AudioDetailProps> = ({ audio }) => {
  const secondaryColor = useColorModeValue("black.300", "gray.300");
  const { user: currentUser } = useUser();
  const { state, dispatch } = useAudioPlayer();
  const { isPlaying, currentAudio } = state;

  const {
    mutateAsync: uploadArtwork,
    isLoading: isAddingArtwork,
  } = useAddAudioPicture(audio.id);

  const {
    isOpen: isEditOpen,
    onOpen: onEditOpen,
    onClose: onEditClose,
  } = useDisclosure();

  const isActivelyPlaying = useMemo(() => {
    if (!currentAudio) return false;
    return currentAudio.audioId === audio.id;
  }, [currentAudio?.audioId, audio]);

  const [picture, setPicture] = useState(audio?.picture ?? "");

  const clickPlayButton = useCallback(() => {
    if (isActivelyPlaying) {
      dispatch({ type: "SET_PLAYING", payload: !isPlaying });
    } else {
      dispatch({
        type: "SET_NEW_QUEUE",
        payload: mapAudioForAudioQueue(audio),
        index: 0,
      });
    }
  }, [isActivelyPlaying, isPlaying, audio.id]);

  useEffect(() => {
    Router.prefetch(`/users/${audio.author.username}`);
  }, []);

  return (
    <Box>
      <Flex marginBottom={4} justifyContent="center">
        <Box flex="1" marginRight={4}>
          <Picture
            title={audio.title}
            src={picture}
            onChange={async (croppedData) => {
              const data = await uploadArtwork(croppedData);
              setPicture(data.image);
            }}
            isUploading={isAddingArtwork}
            canEdit={audio.author.id === currentUser?.id}
          />
        </Box>
        <Box flex="5">
          <Stack direction="row" marginBottom={4}>
            <Tooltip label="Play" placement="top">
              <span>
                <IconButton
                  isRound
                  colorScheme="pink"
                  size="lg"
                  icon={
                    isPlaying && isActivelyPlaying ? <FaPause /> : <FaPlay />
                  }
                  aria-label="Play"
                  onClick={clickPlayButton}
                />
              </span>
            </Tooltip>
            <Stack direction="column" spacing="0" fontSize="sm">
              <Link href={`/users/${audio.author.username}`}>
                <Text fontWeight="500">{audio.author.username}</Text>
              </Link>
              <Text color={secondaryColor}>{relativeDate(audio.uploaded)}</Text>
            </Stack>
            <Spacer />
            <Menu placement="bottom-end">
              <MenuButton
                as={IconButton}
                icon={<HiDotsVertical />}
                variant="ghost"
                isRound
              />
              <MenuList>
                {audio.author.id === currentUser?.id && (
                  <MenuItem icon={<EditIcon />} onClick={onEditOpen}>
                    Edit
                  </MenuItem>
                )}
                <MenuItem
                  icon={<MdQueueMusic />}
                  onClick={() =>
                    dispatch({
                      type: "ADD_TO_QUEUE",
                      payload: mapAudioForAudioQueue(audio),
                    })
                  }
                >
                  Add to queue
                </MenuItem>
              </MenuList>
            </Menu>
            <AudioEditDrawer
              audio={audio}
              isOpen={isEditOpen}
              onClose={onEditClose}
            />
          </Stack>
          <Stack direction="column" spacing={2} width="100%">
            <Flex as="header">
              <Box>
                <Flex alignItems="center">
                  <Heading as="h1" fontSize="2xl">
                    {audio.title}
                  </Heading>
                </Flex>
              </Box>
              <Spacer />
              <VStack spacing={2} alignItems="normal" textAlign="right">
                {!audio.isPublic && <Badge>PRIVATE</Badge>}
              </VStack>
            </Flex>
          </Stack>
        </Box>
      </Flex>
    </Box>
  );
};

export default AudioDetails;

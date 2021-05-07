import {
  Badge,
  Box,
  Flex,
  Heading,
  HStack,
  IconButton,
  Spacer,
  Stack,
  Tag,
  Text,
  Tooltip,
  useColorModeValue,
  useDisclosure,
  VStack,
  Wrap,
  WrapItem,
  chakra,
  TagLabel,
  TagLeftIcon,
} from "@chakra-ui/react";
import Router from "next/router";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { EditIcon } from "@chakra-ui/icons";
import NextLink from "next/link";
import { FaHashtag, FaPause, FaPlay } from "react-icons/fa";
import { MdQueueMusic } from "react-icons/md";
import AudioEdit from "./Edit";
import Link from "~/components/Link";
import Picture from "~/components/Picture";
import PictureDropzone from "~/components/Picture/PictureDropzone";
import { mapAudioForAudioQueue } from "~/components/AudioPlayer/utils";
import { useAddAudioPicture } from "~/features/audio/hooks/mutations/useAddAudioPicture";
import { AudioDetail } from "~/features/audio/types";
import { useAudioPlayer } from "~/contexts/AudioPlayerContext";
import { useUser } from "~/contexts/UserContext";
import { formatDuration } from "~/utils/format";
import { relativeDate } from "~/utils/time";

interface AudioDetailProps {
  audio: AudioDetail;
}

const AudioDetails: React.FC<AudioDetailProps> = ({ audio }) => {
  const secondaryColor = useColorModeValue("black.300", "gray.300");
  const { user: currentUser } = useUser();
  const { state, dispatch } = useAudioPlayer();
  const { currentTime, isPlaying, audioRef, currentAudio } = state;

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

  const [picture, setPicture] = useState(() => {
    return audio?.picture
      ? `https://audiochan.s3.amazonaws.com/${audio.picture}`
      : "";
  });

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

  const audioDurationFormatted = useMemo(() => {
    return formatDuration(audio.duration);
  }, [audio.duration]);

  const audioCreatedDateRelative = useMemo(() => {
    return relativeDate(audio.uploaded);
  }, [audio.uploaded]);

  useEffect(() => {
    Router.prefetch(`/users/${audio.author.username}`);
  }, []);

  return (
    <Flex marginBottom={4} justifyContent="center">
      <Box flex="1" marginRight={4}>
        <PictureDropzone
          disabled={isAddingArtwork && currentUser?.id === audio.author.id}
          onChange={async (croppedData) => {
            const { data } = await uploadArtwork(croppedData);
            setPicture(data.image);
          }}
        >
          <Picture source={picture} imageSize={250} borderWidth="1px" />
        </PictureDropzone>
      </Box>
      <Box flex="5">
        <Stack direction="row" marginBottom={4}>
          <Tooltip label="Play" placement="top">
            <span>
              <IconButton
                isRound
                colorScheme="pink"
                size="lg"
                icon={isPlaying && isActivelyPlaying ? <FaPause /> : <FaPlay />}
                aria-label="Play"
                onClick={clickPlayButton}
              />
            </span>
          </Tooltip>
          <Stack direction="column" spacing="0" fontSize="sm">
            <Link href={`/users/${audio.author.username}`}>
              <Text fontWeight="500">{audio.author.username}</Text>
            </Link>
            <Text color={secondaryColor}>{audioCreatedDateRelative}</Text>
          </Stack>
          <Spacer />
          <HStack justifyContent="flex-end">
            <Tooltip label="Add to queue" placement="top">
              <span>
                <IconButton
                  isRound
                  size="lg"
                  icon={<MdQueueMusic />}
                  aria-label="Add to queue"
                  onClick={() =>
                    dispatch({
                      type: "ADD_TO_QUEUE",
                      payload: mapAudioForAudioQueue(audio),
                    })
                  }
                />
              </span>
            </Tooltip>
          </HStack>
        </Stack>
        <Stack direction="column" spacing={2} width="100%">
          <Flex as="header">
            <Box>
              <Flex alignItems="center">
                <Heading as="h1" fontSize="3xl">
                  {audio.title}
                </Heading>
                <Box>
                  {audio.author.id === currentUser?.id && (
                    <Tooltip label="Edit" placement="top">
                      <chakra.span marginLeft={4}>
                        <IconButton
                          isRound
                          variant="ghost"
                          size="lg"
                          icon={<EditIcon />}
                          aria-label="Edit"
                          onClick={onEditOpen}
                        />
                      </chakra.span>
                    </Tooltip>
                  )}
                </Box>
              </Flex>
              <Text fontSize="sm" color={secondaryColor} marginTop={4}>
                {audio.description || "No information given."}
              </Text>
            </Box>
            <Spacer />
            <VStack spacing={2} alignItems="normal" textAlign="right">
              <Box color={secondaryColor}>{audioDurationFormatted}</Box>
              {audio.visibility !== "public" && (
                <Badge textTransform="uppercase">{audio.visibility}</Badge>
              )}
            </VStack>
          </Flex>
          {audio.tags && (
            <Flex alignItems="flex-end">
              <Wrap marginTop={2}>
                {audio.tags.map((tag, idx) => (
                  <WrapItem key={idx}>
                    <NextLink href={`/search?tag=${tag}`}>
                      <Tag size="sm" cursor="pointer">
                        <TagLeftIcon as={FaHashtag} />
                        <TagLabel>{tag}</TagLabel>
                      </Tag>
                    </NextLink>
                  </WrapItem>
                ))}
              </Wrap>
            </Flex>
          )}
        </Stack>
      </Box>
      <AudioEdit audio={audio} isOpen={isEditOpen} onClose={onEditClose} />
    </Flex>
  );
};

export default AudioDetails;

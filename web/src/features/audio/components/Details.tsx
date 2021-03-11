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
} from "@chakra-ui/react";
import Router from "next/router";
import React, { useEffect, useMemo, useState } from "react";
import { EditIcon } from "@chakra-ui/icons";
import { AiFillHeart, AiOutlineHeart } from "react-icons/ai";
import AudioEdit from "./Edit";
import Link from "../../../components/Link";
import Picture from "../../../components/Picture";
import {
  useAddAudioPicture,
  useAudioFavorite,
} from "~/features/audio/hooks/mutations";
import { Audio, AudioDetail } from "~/features/audio/types";
import { formatDuration, relativeDate } from "~/utils/time";
import useUser from "~/hooks/useUser";
import { FaPlay } from "react-icons/fa";
import { MdQueueMusic } from "react-icons/md";
import useAudioPlayer from "~/hooks/useAudioPlayer";
import { mapToAudioListProps } from "~/utils";
import PictureDropzone from "~/components/Picture/PictureDropzone";

interface AudioDetailProps {
  audio: AudioDetail;
}

const AudioDetails: React.FC<AudioDetailProps> = ({ audio }) => {
  const secondaryColor = useColorModeValue("black.300", "gray.300");
  const { user: currentUser } = useUser();
  const { startPlay, addToQueue } = useAudioPlayer();

  const {
    isFavorite,
    onFavorite: favorite,
    isLoading: isFavoriteLoading,
  } = useAudioFavorite(audio.id, audio.isFavorited);

  const {
    mutateAsync: uploadArtwork,
    isLoading: isAddingArtwork,
  } = useAddAudioPicture(audio.id);

  const {
    isOpen: isEditOpen,
    onOpen: onEditOpen,
    onClose: onEditClose,
  } = useDisclosure();

  const [picture, setPicture] = useState(() => {
    return audio?.picture
      ? `https://audiochan-public.s3.amazonaws.com/${audio.picture}`
      : "";
  });

  const audioDurationFormatted = useMemo(() => {
    return formatDuration(audio.duration);
  }, [audio.duration]);

  const audioCreatedDateRelative = useMemo(() => {
    return relativeDate(audio.created);
  }, [audio.created]);

  useEffect(() => {
    Router.prefetch(`/users/${audio.user.username}`);
  }, []);

  return (
    <Flex marginBottom={4} justifyContent="center">
      <Box flex="1">
        <PictureDropzone
          disabled={isAddingArtwork && currentUser?.id === audio.user.id}
          onChange={async (croppedData) => {
            const { data } = await uploadArtwork(croppedData);
            setPicture(data.image);
          }}
        >
          <Picture source={picture} imageSize={250} />
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
                icon={<FaPlay />}
                aria-label="Play"
                onClick={() => startPlay([mapToAudioListProps(audio)], 0)}
              />
            </span>
          </Tooltip>
          <Stack direction="column" spacing="0" fontSize="sm">
            <Link href={`/users/${audio.user.username}`}>
              <Text fontWeight="500">{audio.user.username}</Text>
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
                  onClick={() => addToQueue(mapToAudioListProps(audio))}
                />
              </span>
            </Tooltip>
            {currentUser && currentUser.id !== audio.user.id && (
              <Tooltip
                label={isFavorite ? "Unfavorite" : "Favorite"}
                placement="top"
              >
                <span>
                  <IconButton
                    isRound
                    colorScheme="pink"
                    variant="ghost"
                    size="lg"
                    icon={isFavorite ? <AiFillHeart /> : <AiOutlineHeart />}
                    aria-label={isFavorite ? "Unfavorite" : "Favorite"}
                    onClick={favorite}
                    isLoading={isFavoriteLoading}
                  />
                </span>
              </Tooltip>
            )}
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
                  {audio.user.id === currentUser?.id && (
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
              <Box>
                {audio.genre && (
                  <Badge fontSize="sm" letterSpacing="1.1" fontWeight="800">
                    {audio.genre.name}
                  </Badge>
                )}
              </Box>
              <Box color={secondaryColor}>{audioDurationFormatted}</Box>
            </VStack>
          </Flex>
          {audio.tags && (
            <Box>
              <Wrap marginTop={2}>
                {audio.tags.map((tag, idx) => (
                  <WrapItem key={idx}>
                    <Tag size="sm">{tag}</Tag>
                  </WrapItem>
                ))}
              </Wrap>
            </Box>
          )}
        </Stack>
      </Box>
      <AudioEdit audio={audio} isOpen={isEditOpen} onClose={onEditClose} />
    </Flex>
  );
};

export default AudioDetails;

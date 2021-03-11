import {
  Badge,
  Box,
  Button,
  Flex,
  Heading,
  HStack,
  IconButton,
  Spacer,
  Stack,
  Text,
  Tooltip,
} from "@chakra-ui/react";
import React, { useMemo } from "react";
import { Audio } from "~/features/audio/types";
import { formatDuration } from "~/utils/time";
import Link from "~/components/Link";
import Picture from "~/components/Picture";
import { FaPause, FaPlay } from "react-icons/fa";
import { useAudioFavorite } from "../../hooks/mutations";
import { AiFillHeart } from "react-icons/ai";
import useUser from "~/hooks/useUser";

interface AudioListItemProps {
  audio: Audio;
  isPlaying?: boolean;
  onPlayClick?: () => void;
  removeArtistName?: boolean;
}

const AudioListItem: React.FC<AudioListItemProps> = ({
  audio,
  onPlayClick,
  isPlaying,
  removeArtistName = false,
}) => {
  const { user: currentUser } = useUser();

  const {
    isFavorite,
    onFavorite: favorite,
    isLoading: isFavoriteLoading,
  } = useAudioFavorite(audio.id, audio.isFavorited);

  const picture = useMemo(() => {
    return audio?.picture
      ? `https://audiochan-public.s3.amazonaws.com/${audio.picture}`
      : "";
  }, [audio.picture]);

  return (
    <Box
      as="article"
      display="flex"
      borderWidth="1px"
      marginBottom={4}
      overflow="hidden"
      boxShadow="md"
    >
      <Picture source={picture} imageSize={150} isLazy />
      <Box width="100%" marginLeft={4} padding={4}>
        <Flex width="100%">
          <Flex flex="3">
            <Box marginRight={2}>
              <Tooltip label="Play" placement="top">
                <span>
                  <IconButton
                    isRound
                    colorScheme="pink"
                    size="lg"
                    icon={isPlaying ? <FaPause /> : <FaPlay />}
                    aria-label="Play"
                    onClick={onPlayClick}
                  />
                </span>
              </Tooltip>
            </Box>
            <Box>
              <Heading as="h3" size="md">
                <Link href={`/audios/view/${audio.id}`}>{audio.title}</Link>
              </Heading>
              {!removeArtistName && (
                <Link href={`/users/${audio.user.username}`}>
                  <Text as="i">{audio.user.username}</Text>
                </Link>
              )}
            </Box>
          </Flex>
          <Flex flex="1" justify="flex-end">
            <Stack direction="column" spacing={1} textAlign="right">
              <Text fontSize="sm">{formatDuration(audio.duration)}</Text>
              <Badge>{audio.genre?.name}</Badge>
            </Stack>
          </Flex>
        </Flex>
        <HStack marginTop={4}>
          {currentUser && (
            <Button
              size="xs"
              leftIcon={<AiFillHeart />}
              colorScheme={isFavorite ? "primary" : "gray"}
              onClick={favorite}
              isLoading={isFavoriteLoading}
            >
              Favorite
            </Button>
          )}
        </HStack>
      </Box>
    </Box>
  );
};

export default AudioListItem;

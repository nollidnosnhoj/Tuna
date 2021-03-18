import {
  Badge,
  Box,
  Flex,
  HStack,
  IconButton,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
  Stack,
  Text,
  Tooltip,
} from "@chakra-ui/react";
import React, { useCallback, useMemo } from "react";
import Router from "next/router";
import { Audio } from "~/features/audio/types";
import { formatDuration } from "~/utils/time";
import Link from "~/components/Link";
import Picture from "~/components/Picture";
import { FaPause, FaPlay } from "react-icons/fa";
import { useAudioFavorite } from "../../hooks/mutations";
import { AiFillHeart } from "react-icons/ai";
import { HiDotsVertical } from "react-icons/hi";
import useUser from "~/hooks/useUser";
import { errorToast } from "~/utils/toast";
import { MdQueueMusic } from "react-icons/md";

export interface AudioListItemProps {
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
    <Box as="article" display="flex" marginBottom={4}>
      <Picture source={picture} imageSize={100} isLazy borderRightWidth={1} />
      <Flex width="100%" align="center" mx={4}>
        <Flex flex="3">
          <Box marginRight={4}>
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
            <Box>
              <Link
                href={`/audios/view/${audio.id}`}
                _hover={{ textDecoration: "none" }}
              >
                <Text as="b">{audio.title}</Text>
              </Link>
            </Box>
            {!removeArtistName && (
              <Link href={`/users/${audio.user.username}`}>
                <Text as="i">{audio.user.username}</Text>
              </Link>
            )}
          </Box>
        </Flex>
        <HStack flex="1" justify="flex-end" spacing={4}>
          <Stack direction="column" spacing={1} textAlign="right">
            <Text fontSize="sm">{formatDuration(audio.duration)}</Text>
            <Badge>{audio.genre?.name}</Badge>
          </Stack>
          <Box>
            <Menu placement="bottom-end">
              <MenuButton
                as={IconButton}
                aria-label="Show actions"
                icon={<HiDotsVertical />}
                variant="ghost"
              >
                Actions
              </MenuButton>
              <MenuList>
                {currentUser && (
                  <MenuItem
                    onClick={() => favorite()}
                    icon={<AiFillHeart />}
                    disabled={isFavoriteLoading}
                  >
                    {isFavorite ? "Unfavorite" : "Favorite"}
                  </MenuItem>
                )}
                <MenuItem icon={<MdQueueMusic />}>Add To Queue</MenuItem>
              </MenuList>
            </Menu>
          </Box>
        </HStack>
      </Flex>
    </Box>
  );
};

export default AudioListItem;

import {
  Badge,
  Box,
  Flex,
  IconButton,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
  Stack,
  Text,
  Tooltip,
} from "@chakra-ui/react";
import React, { useMemo } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import { HiDotsVertical } from "react-icons/hi";
import { MdQueueMusic } from "react-icons/md";
import Link from "~/components/Link";
import Picture from "~/components/Picture";
import { mapAudiosForAudioQueue } from "~/utils/audioplayer";
import { useAudioPlayer } from "~/lib/hooks/useAudioPlayer";
import { Audio } from "~/features/audio/types";
import { formatDuration } from "~/utils/format";

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
  const { dispatch } = useAudioPlayer();

  const picture = useMemo(() => {
    return audio?.picture
      ? `https://audiochan-public.s3.amazonaws.com/${audio.picture}`
      : "";
  }, [audio.picture]);

  return (
    <Box as="article" display="flex">
      <Picture source={picture} imageSize={100} isLazy borderWidth="1px" />
      <Flex width="100%" mx={4} marginTop={2}>
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
            <Flex align="center">
              <Link
                href={`/audios/${audio.id}`}
                _hover={{ textDecoration: "none" }}
              >
                <Text as="b" fontSize="lg">
                  {audio.title}
                </Text>
              </Link>
              <Text fontSize="sm" marginLeft={2}>
                {formatDuration(audio.duration)}
              </Text>
            </Flex>
            {!removeArtistName && (
              <Link href={`/users/${audio.author.username}`}>
                <Text as="i">{audio.author.username}</Text>
              </Link>
            )}
          </Box>
        </Flex>
        <Flex flex="1" justify="flex-end">
          <Stack direction="column" spacing={1} textAlign="right">
            {!audio.isPublic && <Badge>PRIVATE</Badge>}
          </Stack>
        </Flex>
      </Flex>
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
          <MenuItem
            icon={<MdQueueMusic />}
            onClick={() =>
              dispatch({
                type: "ADD_TO_QUEUE",
                payload: mapAudiosForAudioQueue([audio]),
              })
            }
          >
            Add To Queue
          </MenuItem>
        </MenuList>
      </Menu>
    </Box>
  );
};

export default AudioListItem;

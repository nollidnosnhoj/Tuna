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
import React, { useMemo } from "react";
import { Audio } from "~/features/audio/types";
import { formatDuration } from "~/utils/time";
import Link from "~/components/Link";
import Picture from "~/components/Picture";
import { FaPause, FaPlay } from "react-icons/fa";
import { AiFillHeart } from "react-icons/ai";
import { HiDotsVertical } from "react-icons/hi";
import useUser from "~/hooks/useUser";
import { MdQueueMusic } from "react-icons/md";
import useAudioQueue from "~/hooks/useAudioQueue";
import { mapAudiosForAudioQueue } from "~/utils";

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
  const { addToQueue } = useAudioQueue();

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
                href={`/audios/view/${audio.id}`}
                _hover={{ textDecoration: "none" }}
              >
                <Text as="b">{audio.title}</Text>
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
            {audio.visibility !== "public" && <Badge>{audio.visibility}</Badge>}
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
            onClick={() => addToQueue(mapAudiosForAudioQueue([audio]))}
          >
            Add To Queue
          </MenuItem>
        </MenuList>
      </Menu>
    </Box>
  );
};

export default AudioListItem;

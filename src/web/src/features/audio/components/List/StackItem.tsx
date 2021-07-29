import {
  Box,
  Flex,
  IconButton,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
  Text,
  useColorModeValue,
} from "@chakra-ui/react";
import NextImage from "next/image";
import React, { useState } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import { HiDotsVertical } from "react-icons/hi";
import { MdQueueMusic } from "react-icons/md";
import Link from "~/components/ui/Link";
import { mapAudiosForAudioQueue } from "~/utils/audioplayer";
import { AudioData } from "~/features/audio/types";
import { formatDuration } from "~/utils/format";
import PictureContainer from "~/components/Picture/PictureContainer";
import { useAudioQueue } from "~/lib/stores";

export interface AudioListItemProps {
  audio: AudioData;
  isPlaying?: boolean;
  onPlayClick?: () => void;
  removeArtistName?: boolean;
}

const AudioStackItem: React.FC<AudioListItemProps> = ({
  audio,
  onPlayClick,
  isPlaying,
  removeArtistName = false,
}) => {
  const addToQueue = useAudioQueue((state) => state.addToQueue);
  const [hoverItem, setHoverItem] = useState(false);
  const hoverBg = useColorModeValue("inherit", "whiteAlpha.200");

  return (
    <Box
      as="article"
      display="flex"
      alignItems="center"
      onMouseOver={() => setHoverItem(true)}
      onMouseLeave={() => setHoverItem(false)}
      backgroundColor={hoverItem ? hoverBg : undefined}
      borderRadius="md"
      padding={1}
      _notLast={{
        marginBottom: 1,
      }}
    >
      <Flex marginX={2}>
        <IconButton
          visibility={hoverItem || isPlaying ? "visible" : "hidden"}
          variant="unstyled"
          justifyContent="center"
          alignItems="center"
          display="flex"
          size="sm"
          icon={isPlaying ? <FaPause /> : <FaPlay />}
          aria-label="Play"
          onClick={onPlayClick}
        />
      </Flex>
      <PictureContainer
        width={75}
        borderWidth="1px"
        position="relative"
        display="flex"
        justifyContent="center"
        alignItems="center"
      >
        {audio.picture && (
          <NextImage
            src={audio.picture}
            layout="fill"
            objectFit="cover"
            loading="lazy"
          />
        )}
      </PictureContainer>
      <Flex width="100%" align="center">
        <Box marginX={4}>
          <Flex align="center">
            <Link
              href={`/audios/${audio.id}`}
              _hover={{ textDecoration: "none" }}
            >
              <Text as="b" fontSize="md">
                {audio.title}
              </Text>
            </Link>
          </Flex>
          {!removeArtistName && (
            <Link href={`/users/${audio.user.username}`}>
              <Text as="i">{audio.user.username}</Text>
            </Link>
          )}
        </Box>
      </Flex>
      <Flex paddingX={4}>{formatDuration(audio.duration)}</Flex>
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

export default AudioStackItem;

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
} from "@chakra-ui/react";
import NextImage from "next/image";
import React, { useState } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import { HiDotsVertical } from "react-icons/hi";
import { MdQueueMusic } from "react-icons/md";
import Link from "~/components/Link";
import { mapAudiosForAudioQueue } from "~/utils/audioplayer";
import { useAudioPlayer } from "~/lib/hooks/useAudioPlayer";
import { AudioData } from "~/features/audio/types";
import { formatDuration } from "~/utils/format";
import PictureContainer from "~/components/Picture/PictureContainer";

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
  const { dispatch } = useAudioPlayer();
  const [hoverImage, setHoverImage] = useState(false);

  return (
    <Box as="article" display="flex">
      <PictureContainer
        width={125}
        borderWidth="1px"
        position="relative"
        display="flex"
        justifyContent="center"
        alignItems="center"
        onMouseOver={() => setHoverImage(true)}
        onMouseLeave={() => setHoverImage(false)}
      >
        {audio.picture && (
          <NextImage
            src={audio.picture}
            layout="fill"
            objectFit="cover"
            loading="lazy"
          />
        )}
        <IconButton
          display={hoverImage ? "flex" : "none"}
          isRound
          colorScheme="pink"
          size="lg"
          icon={isPlaying ? <FaPause /> : <FaPlay />}
          aria-label="Play"
          onClick={onPlayClick}
        />
      </PictureContainer>
      <Flex width="100%" mx={4} marginTop={2}>
        <Flex flex="3">
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

export default AudioStackItem;

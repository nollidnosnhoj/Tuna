import {
  Box,
  chakra,
  Flex,
  IconButton,
  useColorModeValue,
} from "@chakra-ui/react";
import NextImage from "next/image";
import React, { useState } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import Link from "~/components/ui/Link";
import { AudioView } from "~/features/audio/api/types";
import { formatDuration } from "~/utils/format";
import PictureContainer from "~/components/Picture/PictureContainer";
import AudioMiscMenu from "../ContextMenu";
import { useAudioPlayer } from "~/lib/stores";

export interface AudioListItemProps {
  audio: AudioView;
  isActive?: boolean;
  onPlayClick?: () => void;
  removeArtistName?: boolean;
}

const AudioStackItem: React.FC<AudioListItemProps> = ({
  audio,
  onPlayClick,
  isActive,
  removeArtistName = false,
}) => {
  const isPlaying = useAudioPlayer((state) => state.isPlaying);
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
      <Flex paddingX={{ base: 2, md: 4 }}>
        <IconButton
          opacity={hoverItem || isActive ? 1 : 0}
          variant="unstyled"
          justifyContent="center"
          alignItems="center"
          display="flex"
          size="sm"
          icon={isPlaying && isActive ? <FaPause /> : <FaPlay />}
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
              href={`/audios/${audio.slug}`}
              _hover={{ textDecoration: "none" }}
            >
              <chakra.b fontSize="md">{audio.title}</chakra.b>
            </Link>
          </Flex>
          {!removeArtistName && (
            <Link href={`/users/${audio.user.username}`}>
              <chakra.span>{audio.user.username}</chakra.span>
            </Link>
          )}
        </Box>
      </Flex>
      <Flex paddingX={{ base: 2, md: 4 }}>
        {formatDuration(audio.duration)}
      </Flex>
      <Box paddingX={{ base: 2, md: 4 }}>
        <AudioMiscMenu audio={audio} size="sm" />
      </Box>
    </Box>
  );
};

export default AudioStackItem;

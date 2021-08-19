import {
  Box,
  chakra,
  Flex,
  IconButton,
  Stack,
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
import AudioFavoriteButton from "../Buttons/Favorite";
import AddToPlaylistButton from "../Buttons/AddToPlaylist";

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
  children,
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
      <Flex flex={2} align="center" marginX={4}>
        <Box>
          <Link
            href={`/audios/${audio.slug}`}
            _hover={{ textDecoration: "none" }}
          >
            <chakra.b fontSize="md">{audio.title}</chakra.b>
          </Link>
          {!removeArtistName && (
            <Link href={`/users/${audio.user.username}`}>
              <chakra.div>{audio.user.username}</chakra.div>
            </Link>
          )}
        </Box>
      </Flex>
      <Flex paddingX={{ base: 2, md: 4 }}>
        {formatDuration(audio.duration)}
      </Flex>
      <Stack direction="row" spacing={2} paddingX={{ base: 2, md: 4 }}>
        {children}
        <AddToPlaylistButton audio={audio} />
        <AudioFavoriteButton
          audioId={audio.id}
          isFavorite={audio.isFavorited}
        />
        <AudioMiscMenu audio={audio} />
      </Stack>
    </Box>
  );
};

export default AudioStackItem;

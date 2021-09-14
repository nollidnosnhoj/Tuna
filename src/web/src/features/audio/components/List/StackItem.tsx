import {
  Box,
  chakra,
  Flex,
  IconButton,
  Stack,
  useColorModeValue,
} from "@chakra-ui/react";
import React, { useState } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import Link from "~/components/UI/Link";
import { AudioView } from "~/features/audio/api/types";
import { formatDuration } from "~/utils/format";
import AudioMiscMenu from "../../../../components/UI/ContextMenu";
import { useAudioPlayer, useAudioQueue } from "~/lib/stores";
import AudioFavoriteButton from "../Buttons/Favorite";
import AddToPlaylistButton from "../Buttons/AddToPlaylist";
import AudioShareButton from "../Buttons/Share";
import { MdQueueMusic } from "react-icons/md";

export interface AudioListItemProps {
  audio: AudioView;
  isActive?: boolean;
  onPlayClick?: () => void;
}

const AudioStackItem: React.FC<AudioListItemProps> = ({
  audio,
  onPlayClick,
  isActive,
  children,
}) => {
  const isPlaying = useAudioPlayer((state) => state.isPlaying);
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
      padding={2}
      _notLast={{
        marginBottom: 1,
      }}
    >
      <Flex paddingX={2}>
        <IconButton
          justifyContent="center"
          alignItems="center"
          display="flex"
          size="md"
          isRound
          icon={isPlaying && isActive ? <FaPause /> : <FaPlay />}
          aria-label="Play"
          onClick={onPlayClick}
        />
      </Flex>
      <Flex flex={2} align="center" marginX={4}>
        <Box>
          <Link
            href={`/audios/${audio.slug}`}
            _hover={{ textDecoration: "none" }}
          >
            <chakra.b fontSize="lg">{audio.title}</chakra.b>
          </Link>
          <Link href={`/users/${audio.user.userName}`}>
            <chakra.div>{audio.user.userName}</chakra.div>
          </Link>
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
        <AudioShareButton audio={audio} />
        <AudioMiscMenu
          items={[
            {
              items: [
                {
                  name: "Add to Queue",
                  isVisible: true,
                  icon: <MdQueueMusic />,
                  onClick: async () => await addToQueue("custom", [audio]),
                },
              ],
            },
          ]}
        />
      </Stack>
    </Box>
  );
};

export default AudioStackItem;

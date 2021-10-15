import {
  Box,
  chakra,
  Flex,
  IconButton,
  Stack,
  useColorModeValue,
} from "@chakra-ui/react";
import React, { memo, useCallback, useState } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import Link from "~/components/UI/Link";
import { AudioView } from "~/features/audio/api/types";
import { formatDuration } from "~/utils/format";
import AudioMiscMenu from "~/components/UI/ContextMenu";
import { useAudioPlayer } from "~/lib/stores";
import AudioFavoriteButton from "../Buttons/Favorite";
import AddToPlaylistButton from "../Buttons/AddToPlaylist";
import AudioShareButton from "../Buttons/Share";
import { MdQueueMusic } from "react-icons/md";
import { ID } from "~/lib/types";
import { Playlist } from "~/features/playlist/api/types";
import RemoveFromPlaylistButton from "../Buttons/RemoveFromPlaylist";

export interface AudioListItemProps {
  audio: AudioView;
  index: number;
  playlist?: Playlist;
  playlistAudioId?: ID;
  isPlaying?: boolean;
  actions?: ActionChoice[];
}

type ActionChoice =
  | "addToPlaylist"
  | "removeFromPlaylist"
  | "share"
  | "favorite";

const AudioStackItem: React.FC<AudioListItemProps> = ({
  audio,
  playlist,
  playlistAudioId,
  isPlaying,
  actions = [],
}) => {
  const setIsPlaying = useAudioPlayer((state) => state.setIsPlaying);
  const { currentAudioPlaying, addToQueue, setNewQueue } = useAudioPlayer(
    (state) => ({
      currentAudioPlaying: state.current,
      addToQueue: state.addToQueue,
      setNewQueue: state.setNewQueue,
    })
  );

  const clickPlayButton = useCallback(() => {
    if (currentAudioPlaying?.audioId === audio.id) {
      setIsPlaying(!isPlaying);
    } else {
      setNewQueue("custom", [audio], 0);
    }
  }, [audio, isPlaying, currentAudioPlaying?.queueId]);

  const [hoverItem, setHoverItem] = useState(false);
  const hoverBg = useColorModeValue("inherit", "whiteAlpha.200");

  const mapToActionButton = useCallback(
    (action: ActionChoice) => {
      switch (action) {
        case "addToPlaylist":
          return <AddToPlaylistButton audio={audio} />;
        case "removeFromPlaylist": {
          if (playlist && playlistAudioId) {
            return (
              <RemoveFromPlaylistButton
                playlist={playlist}
                playlistAudioId={playlistAudioId}
              />
            );
          }
          return null;
        }
        case "share":
          return <AudioShareButton audio={audio} />;
        case "favorite":
          return (
            <AudioFavoriteButton
              audioId={audio.id}
              isFavorite={audio.isFavorited}
            />
          );
        default:
          return null;
      }
    },
    [audio, playlist, playlistAudioId]
  );

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
          icon={isPlaying ? <FaPause /> : <FaPlay />}
          aria-label="Play"
          onClick={clickPlayButton}
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
        {actions.map((action, index) => (
          <React.Fragment key={action + index}>
            {mapToActionButton(action)}
          </React.Fragment>
        ))}
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

export default memo(
  AudioStackItem,
  (prev, next) => prev.isPlaying === next.isPlaying
);

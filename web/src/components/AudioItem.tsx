import {
  Box,
  chakra,
  Flex,
  IconButton,
  Stack,
  useColorModeValue,
} from "@chakra-ui/react";
import React, { memo, PropsWithChildren, useCallback, useState } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import Link from "~/components/ui/Link";
import { formatDuration } from "~/utils/format";
import { useAudioPlayer } from "~/lib/stores";
import { getValidChildren } from "@chakra-ui/react-utils";
import { Audio } from "~/lib/types";

export interface IAudioItemProps {
  audio: Audio;
  context?: string;
}

const AudioListItemContainer: React.FC = ({ children }) => {
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
      {children}
    </Box>
  );
};

function AudioListItem(props: PropsWithChildren<IAudioItemProps>) {
  const { audio, context = "custom", children } = props;
  const validChildren = getValidChildren(children);
  const { currentAudioPlaying, setIsPlaying, setNewQueue } = useAudioPlayer(
    (state) => ({
      currentAudioPlaying: state.current,
      setIsPlaying: state.setIsPlaying,
      setNewQueue: state.setNewQueue,
    })
  );

  const isPlaying = useAudioPlayer(
    useCallback(
      (state) => state.current?.audioId === audio.id && state.isPlaying,
      [audio.id]
    )
  );

  const clickPlayButton = useCallback(() => {
    if (currentAudioPlaying?.audioId === audio.id) {
      setIsPlaying(!isPlaying);
      return;
    }
    setNewQueue(context, [audio], 0);
  }, [audio, context, isPlaying, currentAudioPlaying?.queueId]);

  return (
    <AudioListItemContainer>
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
          <Link href={`/artists/${audio.user.userName}`}>
            <chakra.div>{audio.user.userName}</chakra.div>
          </Link>
        </Box>
      </Flex>
      <Flex paddingX={{ base: 2, md: 4 }}>
        {formatDuration(audio.duration)}
      </Flex>
      <Stack direction="row" spacing={2} paddingX={{ base: 2, md: 4 }}>
        {validChildren}
      </Stack>
    </AudioListItemContainer>
  );
}

const AudioListItemMemo = memo(AudioListItem);

export { AudioListItemMemo as AudioListItem };

import { Box, List, ListItem, chakra } from "@chakra-ui/react";
import React, { useCallback } from "react";
import AudioStackMiniItem from "./StackItem";
import { AudioView } from "~/features/audio/api/types";
import { useAudioPlayer, useAudioQueue } from "~/lib/stores";

type AudioListProps = {
  audios: AudioView[];
  context?: string;
};

export default function AudioList(props: AudioListProps) {
  const { audios, context = "custom" } = props;
  const [isPlaying, setIsPlaying] = useAudioPlayer((state) => [
    state.isPlaying,
    state.setIsPlaying,
  ]);

  const {
    context: queueContext,
    current: currentAudio,
    setNewQueue,
    setPlayIndex,
  } = useAudioQueue();

  const isAudioPlaying = useCallback(
    (audio: AudioView) =>
      currentAudio !== undefined && currentAudio?.audioId === audio.id,
    [currentAudio?.queueId]
  );

  const onPlayClick = useCallback(
    (audio: AudioView, index: number) => {
      if (isAudioPlaying(audio)) {
        setIsPlaying(!isPlaying);
      } else if (queueContext === context) {
        setPlayIndex(index);
      } else {
        setNewQueue(context, audios, index);
      }
    },
    [isAudioPlaying, audios, isPlaying, context, queueContext]
  );

  return (
    <Box>
      {audios.length > 0 ? (
        <React.Fragment>
          <List>
            {audios.map((audio, index) => (
              <ListItem key={index}>
                <AudioStackMiniItem
                  audio={audio}
                  isActive={isAudioPlaying(audio)}
                  onPlayClick={() => onPlayClick(audio, index)}
                />
              </ListItem>
            ))}
          </List>
        </React.Fragment>
      ) : (
        <chakra.span>No audios found.</chakra.span>
      )}
    </Box>
  );
}

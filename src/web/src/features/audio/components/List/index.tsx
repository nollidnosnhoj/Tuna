import { Box, List, ListItem, chakra } from "@chakra-ui/react";
import React from "react";
import AudioStackMiniItem from "./Item";
import { AudioView } from "~/features/audio/api/types";
import { useAudioPlayer, useAudioQueue } from "~/lib/stores";

type AudioListProps = {
  audios: AudioView[];
};

export default function AudioList(props: AudioListProps) {
  const { audios } = props;
  const [isPlaying] = useAudioPlayer((state) => [state.isPlaying]);

  const { current: currentAudio } = useAudioQueue();

  return (
    <Box>
      {audios.length > 0 ? (
        <React.Fragment>
          <List>
            {audios.map((audio, index) => (
              <ListItem key={`${index}.${audio.slug}.${audio.id}`}>
                <AudioStackMiniItem
                  audio={audio}
                  index={index}
                  isPlaying={currentAudio?.audioId === audio.id && isPlaying}
                  actions={["addToPlaylist", "favorite", "share"]}
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

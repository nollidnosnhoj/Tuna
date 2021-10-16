import { Box, List, ListItem, chakra } from "@chakra-ui/react";
import React from "react";
import { AudioListItem } from "./Item";
import { AudioView } from "~/features/audio/api/types";
import { useAudioPlayer } from "~/lib/stores";
import AudioFavoriteButton from "~/features/audio/components/Buttons/Favorite";
import AudioShareButton from "~/features/audio/components/Buttons/Share";
import AudioMiscMenu from "~/features/audio/components/Buttons/Menu";
import AddToPlaylistButton from "~/features/audio/components/Buttons/AddToPlaylist";

type AudioListProps = {
  audios: AudioView[];
};

export default function AudioList(props: AudioListProps) {
  const { audios } = props;
  const { isPlaying, current: currentAudio } = useAudioPlayer();

  return (
    <Box>
      {audios.length > 0 ? (
        <React.Fragment>
          <List>
            {audios.map((audio, index) => (
              <ListItem key={`${index}.${audio.slug}.${audio.id}`}>
                <AudioListItem
                  audio={audio}
                  isPlaying={currentAudio?.audioId === audio.id && isPlaying}
                >
                  <AddToPlaylistButton audio={audio} />
                  <AudioFavoriteButton
                    audioId={audio.id}
                    isFavorite={audio.isFavorited}
                  />
                  <AudioShareButton audio={audio} />
                  <AudioMiscMenu audio={audio} />
                </AudioListItem>
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

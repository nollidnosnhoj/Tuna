import { List, ListItem } from "@chakra-ui/layout";
import React, { useMemo } from "react";
import AudioStackMiniItem from "../../audio/components/List/Item";
import { ID } from "~/lib/types";
import { useGetPlaylistAudios } from "../api/hooks/useGetPlaylistAudios";
import { useAudioPlayer, useAudioQueue } from "~/lib/stores";

interface PlaylistAudioListProps {
  playlistId: ID;
}

export default function PlaylistAudioList({
  playlistId,
}: PlaylistAudioListProps) {
  const [isPlaying] = useAudioPlayer((state) => [state.isPlaying]);
  const { current: currentAudio } = useAudioQueue();
  const { items: playlistAudios } = useGetPlaylistAudios(playlistId);
  const audios = useMemo(
    () => playlistAudios.map((x) => x.audio),
    [playlistAudios]
  );
  return (
    <List>
      {audios.map((audio, index) => (
        <ListItem key={`${index}.${playlistId}.${audio.slug}.${audio.id}`}>
          <AudioStackMiniItem
            audio={audio}
            index={index}
            isPlaying={currentAudio?.audioId === audio.id && isPlaying}
            actions={["removeFromPlaylist", "favorite", "share"]}
          />
        </ListItem>
      ))}
    </List>
  );
}

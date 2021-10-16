import { List, ListItem } from "@chakra-ui/layout";
import React, { useMemo } from "react";
import { AudioListItem } from "../../audio/components/List/Item";
import { ID } from "~/lib/types";
import { useGetPlaylistAudios } from "../api/hooks/queries/useGetPlaylistAudios";
import { useAudioPlayer } from "~/lib/stores";
import AudioFavoriteButton from "~/features/audio/components/Buttons/Favorite";
import AudioShareButton from "~/features/audio/components/Buttons/Share";
import AudioMiscMenu from "~/features/audio/components/Buttons/Menu";
import RemoveFromPlaylistButton from "~/features/audio/components/Buttons/RemoveFromPlaylist";
import { Playlist } from "~/features/playlist/api/types";

interface PlaylistAudioListProps {
  playlist: Playlist;
}

export default function PlaylistAudioList({
  playlist,
}: PlaylistAudioListProps) {
  const isPlaying = useAudioPlayer((state) => state.isPlaying);
  const currentAudio = useAudioPlayer((state) => state.current);
  const { items: playlistAudios } = useGetPlaylistAudios(playlist.id);
  return (
    <List>
      {playlistAudios.map((playlistAudio, index) => {
        const playlistAudioId = playlistAudio.id;
        const audio = playlistAudio.audio;
        return (
          <ListItem key={`${index}.${playlist.id}.${audio.slug}.${audio.id}`}>
            <AudioListItem
              audio={audio}
              isPlaying={currentAudio?.audioId === audio.id && isPlaying}
            >
              <RemoveFromPlaylistButton
                playlist={playlist}
                playlistAudioId={playlistAudioId}
              />
              <AudioFavoriteButton
                audioId={audio.id}
                isFavorite={audio.isFavorited}
              />
              <AudioShareButton audio={audio} />
              <AudioMiscMenu audio={audio} />
            </AudioListItem>
          </ListItem>
        );
      })}
    </List>
  );
}

import { GetServerSideProps } from "next";
import React, { useMemo } from "react";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { useGetPlaylist } from "~/features/playlist/api/hooks";
import { useGetPlaylistAudios } from "~/features/playlist/api/hooks/useGetPlaylistAudios";
import { Playlist } from "~/features/playlist/api/types";
import PlaylistDetails from "~/features/playlist/components/Details";
import request from "~/lib/http";
import { ID } from "~/lib/types";

interface PlaylistPageProps {
  playlist: Playlist;
  playlistId: ID;
}

export const getServerSideProps: GetServerSideProps<PlaylistPageProps> = async (
  context
) => {
  const { req, res, params } = context;
  try {
    const id = params?.id as string;

    const { data } = await request<Playlist>({
      method: "GET",
      url: `playlists/${id}`,
      req,
      res,
    });

    return {
      props: {
        playlist: data,
        playlistId: id,
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function PlaylistPage({
  playlist: initPlaylist,
  playlistId,
}: PlaylistPageProps) {
  const { data: playlist } = useGetPlaylist(playlistId, {
    enabled: !!playlistId,
    initialData: initPlaylist,
  });
  const { items: playlistAudios } = useGetPlaylistAudios(playlist?.id);

  const audios = useMemo(
    () => playlistAudios.map((x) => x.audio),
    [playlistAudios]
  );

  if (!playlist) return null;

  return (
    <Page title="Playlist">
      <PlaylistDetails playlist={playlist} />
      <AudioList audios={audios} context={`playlist:${playlist.id}`} />
    </Page>
  );
}

import { GetServerSideProps } from "next";
import React from "react";
import Page from "~/components/Page";
import { useGetPlaylist } from "~/features/playlist/api/hooks";
import { Playlist } from "~/features/playlist/api/types";
import PlaylistDetails from "~/features/playlist/components/Details";
import PlaylistAudioList from "~/features/playlist/components/PlaylistAudioList";
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

  if (!playlist) return null;

  return (
    <Page title="Playlist">
      <PlaylistDetails playlist={playlist} />
      <PlaylistAudioList playlistId={playlist.id} />
    </Page>
  );
}

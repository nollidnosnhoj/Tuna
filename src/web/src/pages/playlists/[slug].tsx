import { GetServerSideProps } from "next";
import React from "react";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { useGetPlaylist } from "~/features/playlist/api/hooks";
import { useGetPlaylistAudios } from "~/features/playlist/api/hooks/useGetPlaylistAudios";
import { Playlist } from "~/features/playlist/api/types";
import PlaylistDetails from "~/features/playlist/components/Details";
import request from "~/lib/http";
import { IdSlug } from "~/lib/types";

interface PlaylistPageProps {
  playlist: Playlist;
  slug: IdSlug;
  secret?: string;
}

export const getServerSideProps: GetServerSideProps<PlaylistPageProps> = async (
  context
) => {
  const slug = context.params?.slug as IdSlug;
  const { req, res } = context;
  try {
    const { data } = await request<Playlist>({
      method: "GET",
      url: `playlists/${slug}`,
      req,
      res,
    });
    return {
      props: {
        playlist: data,
        slug,
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
  slug,
}: PlaylistPageProps) {
  const { data: playlist } = useGetPlaylist(slug, {
    enabled: !!slug,
    initialData: initPlaylist,
  });
  const { items: playlistAudios } = useGetPlaylistAudios(playlist?.id);

  if (!playlist) return null;

  return (
    <Page title="Playlist">
      <PlaylistDetails playlist={playlist} />
      <AudioList audios={playlistAudios} context={`playlist:${playlist.id}`} />
    </Page>
  );
}

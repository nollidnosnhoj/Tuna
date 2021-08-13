import { Heading } from "@chakra-ui/react";
import { GetServerSideProps } from "next";
import React from "react";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { getPlaylistRequest } from "~/features/playlist/api";
import { useGetPlaylist } from "~/features/playlist/api/hooks";
import { useGetPlaylistAudios } from "~/features/playlist/api/hooks/useGetPlaylistAudios";
import { Playlist } from "~/features/playlist/api/types";
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
  try {
    const data = await getPlaylistRequest(slug, context);
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
      <Heading>{playlist.title}</Heading>
      <AudioList audios={playlistAudios} />
    </Page>
  );
}

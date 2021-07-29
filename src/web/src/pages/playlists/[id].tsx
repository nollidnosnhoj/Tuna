import { GetServerSideProps } from "next";
import React from "react";
import Page from "~/components/Page";
import { getPlaylistRequest } from "~/features/playlist/api";
import { Playlist } from "~/features/playlist/types";

interface PlaylistPageProps {
  playlist: Playlist;
  playlistId: string;
}

export const getServerSideProps: GetServerSideProps<PlaylistPageProps> = async (
  context
) => {
  const id = context.params?.id as string;

  try {
    const data = await getPlaylistRequest(id, context);
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

const PlaylistDetailPage: React.FC<PlaylistPageProps> = () => {
  return (
    <Page title="Playlist Page">
      <div>test</div>
    </Page>
  );
};

export default PlaylistDetailPage;

import { GetServerSideProps } from "next";
import React, { useCallback, useEffect } from "react";
import Page from "~/components/Page";
import request from "~/lib/http";
import { ID, Playlist } from "~/lib/types";
import { Box, chakra, Flex, Heading, Stack } from "@chakra-ui/react";
import PictureController from "~/components/Picture";
import Link from "~/components/ui/Link";
import PlaylistPlayButton from "~/components/buttons/PlayPlaylist";
import { useUser } from "~/components/providers/UserProvider";
import { useAudioPlayer } from "~/lib/stores";
import { useGetPlaylistAudios } from "~/lib/hooks/api/queries/useGetPlaylistAudios";
import Router from "next/router";
import { useGetPlaylist } from "~/lib/hooks/api";

interface PlaylistPageProps {
  playlist: Playlist;
  playlistId: ID;
}

function PlaylistDetailPicture(props: { playlist: Playlist }) {
  const { user: currentUser } = useUser();
  const { playlist } = props;

  return (
    <PictureController
      title={playlist.title}
      src={playlist.picture || ""}
      onChange={async () => console.log("")}
      onRemove={async () => console.log("")}
      isMutating={false}
      canEdit={currentUser?.id === playlist.user.id}
    />
  );
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

  const setNewQueue = useAudioPlayer((state) => state.setNewQueue);
  const { items: playlistAudios } = useGetPlaylistAudios(playlist!.id);

  const playPlaylist = useCallback(async () => {
    await setNewQueue(
      `playlist:${playlist!.id}`,
      playlistAudios.map((x) => x.audio)
    );
  }, [playlist!.id, playlistAudios.length]);

  useEffect(() => {
    if (playlist) {
      Router.prefetch(`/users/${playlist.user.userName}`);
    }
  }, []);

  if (!playlist) return null;

  return (
    <Page title="Playlist">
      <Flex
        marginBottom={4}
        justifyContent="center"
        direction={{ base: "column", md: "row" }}
      >
        <Flex
          flex="1"
          marginRight={4}
          justify={{ base: "center", md: "normal" }}
        >
          <PlaylistDetailPicture playlist={playlist} />
        </Flex>
        <Box flex="6">
          <chakra.div marginTop={{ base: 4, md: 0 }} marginBottom={4}>
            <Heading as="h1" fontSize={{ base: "3xl", md: "5xl" }}>
              {playlist.title}
            </Heading>
            <chakra.div display="flex">
              <Link href={`/users/${playlist.user.userName}`} fontWeight="500">
                {playlist.user.userName}
              </Link>
            </chakra.div>
          </chakra.div>
          <Stack direction="row" alignItems="center">
            <PlaylistPlayButton playlist={playlist} onPlay={playPlaylist} />
          </Stack>
        </Box>
      </Flex>
    </Page>
  );
}

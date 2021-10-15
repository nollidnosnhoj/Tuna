import { Box, Flex, Heading, Stack, chakra } from "@chakra-ui/react";
import Router from "next/router";
import React, { useEffect, useMemo } from "react";
import Link from "~/components/UI/Link";
import { useUser } from "~/features/user/hooks";
import PictureController from "~/components/Picture";
import { Playlist } from "../../api/types";
import { useAudioPlayer } from "~/lib/stores";
import { useGetPlaylistAudios } from "../../api/hooks/queries/useGetPlaylistAudios";
import { useCallback } from "react";
import PlaylistPlayButton from "../Buttons/Play";

interface PlaylistDetailsProps {
  playlist: Playlist;
}

const PlaylistDetails: React.FC<PlaylistDetailsProps> = ({ playlist }) => {
  const setNewQueue = useAudioPlayer((state) => state.setNewQueue);
  const { items: playlistAudios } = useGetPlaylistAudios(playlist.id);
  const audios = useMemo(
    () => playlistAudios.map((x) => x.audio),
    [playlistAudios]
  );
  const { user: currentUser } = useUser();

  const playPlaylist = useCallback(async () => {
    await setNewQueue(`playlist:${playlist.id}`, audios);
  }, [playlist.id, audios.length]);

  useEffect(() => {
    Router.prefetch(`/users/${playlist.user.userName}`);
  }, []);

  return (
    <Flex
      marginBottom={4}
      justifyContent="center"
      direction={{ base: "column", md: "row" }}
    >
      <Flex flex="1" marginRight={4} justify={{ base: "center", md: "normal" }}>
        <PictureController
          title={playlist.title}
          src={playlist.picture || ""}
          onChange={async () => console.log("")}
          onRemove={async () => console.log("")}
          isMutating={false}
          canEdit={currentUser?.id === playlist.user.id}
        />
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
          {/* <AudioPlayButton audio={audio} size="lg" />
          <AudioFavoriteButton audioId={audio.id} size="lg" />
          <AudioMiscMenu audio={audio} size="lg" /> */}
        </Stack>
      </Box>
    </Flex>
  );
};

export default PlaylistDetails;

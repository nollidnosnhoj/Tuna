import { Box } from "@chakra-ui/layout";
import React from "react";
import InfiniteListControls from "~/components/List/InfiniteListControls";
import AudioList from "~/features/audio/components/List";
import { useAudiosInfinite } from "~/features/audio/hooks/queries";

interface UserFavoriteAudioListProps {
  username: string;
}

export default function UserFavoriteAudioList(
  props: UserFavoriteAudioListProps & Record<string, any>
) {
  const { username, ...params } = props;
  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudiosInfinite(`users/${username}/favorites/audios`, params, 15, {
    staleTime: 10000,
  });

  return (
    <Box>
      <AudioList
        audios={audios}
        defaultLayout="list"
        hideLayoutToggle
        notFoundContent={<p>No audios found.</p>}
      />
      <InfiniteListControls
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Box>
  );
}

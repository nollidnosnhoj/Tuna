import React from "react";
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
  } = useAudiosInfinite(`users/${username}/favorites/audios`, params, 15);

  return (
    <AudioList
      type="infinite"
      audios={audios}
      fetchNext={fetchNextPage}
      hasNext={hasNextPage}
      isFetching={isFetchingNextPage}
    />
  );
}

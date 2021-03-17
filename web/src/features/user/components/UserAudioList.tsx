import React from "react";
import InfiniteListControls from "~/components/List/InfiniteListControls";
import AudioList from "~/features/audio/components/List";
import { useAudiosInfinite } from "~/features/audio/hooks/queries";

interface UserAudioListProps {
  username: string;
}

export default function UserAudioList(
  props: UserAudioListProps & Record<string, any>
) {
  const { username, ...params } = props;
  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudiosInfinite(`users/${username}/audios`, params, 15, {
    staleTime: 10000,
  });

  return (
    <React.Fragment>
      <AudioList
        audios={audios}
        notFoundContent={<p>No uploads.</p>}
        defaultLayout="list"
        hideLayoutToggle
      />
      <InfiniteListControls
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </React.Fragment>
  );
}

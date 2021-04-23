import React from "react";
import InfiniteListControls from "~/components/List/InfiniteListControls";
import AudioList from "~/features/audio/components/List";
import { useGetAudioListInfinite } from "~/features/audio/hooks/queries";

interface UserAudioListProps {
  username: string;
  hidePaginationControls?: boolean;
}

export default function UserAudioList(
  props: UserAudioListProps & Record<string, any>
) {
  const { username, hidePaginationControls = false } = props;
  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useGetAudioListInfinite(`users/${username}/audios`, {
    staleTime: 1000,
  });

  return (
    <React.Fragment>
      <AudioList
        audios={audios}
        notFoundContent={<p>No uploads.</p>}
        defaultLayout="list"
        hideLayoutToggle
      />
      {!hidePaginationControls && (
        <InfiniteListControls
          fetchNext={fetchNextPage}
          hasNext={hasNextPage}
          isFetching={isFetchingNextPage}
        />
      )}
    </React.Fragment>
  );
}

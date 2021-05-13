import React from "react";
import InfiniteListControls from "~/components/InfiniteListControls";
import AudioList from "~/features/audio/components/List";
import { useUserAudioListQuery } from "../hooks";

interface UserAudioListProps {
  username: string;
  hidePaginationControls?: boolean;
}

export default function UserAudioList(
  props: UserAudioListProps & Record<string, unknown>
) {
  const { username, hidePaginationControls = false } = props;
  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useUserAudioListQuery(username);

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

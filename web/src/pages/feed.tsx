import React from "react";
import InfiniteListControls from "~/components/InfiniteListControls";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { AudioData } from "~/features/audio/types";
import useInfinitePagination from "~/lib/hooks/useInfinitePagination";
import { PagedList } from "~/lib/types";

export interface AudioFeedPageProps {
  filter: Record<string, unknown>;
  initialPage?: PagedList<AudioData>;
}

export default function UserAudioFeedNextPage() {
  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfinitePagination<AudioData>("me/feed");

  return (
    <Page title="Your Feed">
      <AudioList audios={audios} />
      <InfiniteListControls
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
}

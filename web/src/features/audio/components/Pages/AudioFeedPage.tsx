import React from "react";
import InfiniteListControls from "~/components/List/InfiniteListControls";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { useGetAudioListInfinite } from "~/features/audio/hooks/queries/useAudiosInfinite";
import { Audio } from "~/features/audio/types";
import { PagedList } from "~/lib/types";

export interface AudioFeedPageProps {
  filter: Record<string, any>;
  initialPage?: PagedList<Audio>;
}

export default function AudioFeedPage(props: AudioFeedPageProps) {
  const { filter, initialPage } = props;

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useGetAudioListInfinite("me/feed", {
    params: {
      ...filter,
    },
    staleTime: 1000,
    initialData: initialPage && {
      pages: [initialPage],
      pageParams: [1],
    },
  });

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

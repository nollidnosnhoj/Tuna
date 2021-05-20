import { Text } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import InfiniteListControls from "~/components/InfiniteListControls";
import AudioList from "~/features/audio/components/List";
import useInfiniteCursorPagination from "~/lib/hooks/useInfiniteCursorPagination";
import { AudioData } from "~/features/audio/types";

const Index = () => {
  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteCursorPagination<AudioData>("audios", undefined, undefined);

  return (
    <Page title="Audiochan | Listen and Share Your Music">
      <AudioList
        audios={audios}
        notFoundContent={<Text>No audio found. Be the first to upload!</Text>}
        hideLayoutToggle
      />
      <InfiniteListControls
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
};

export default Index;

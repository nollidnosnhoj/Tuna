import { Text } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import InfiniteListControls from "~/components/ui/ListControls/InfiniteListControls";
import AudioList from "~/features/audio/components/List";
import { useGetAudioList } from "~/features/audio/hooks";

const Index = () => {
  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useGetAudioList();

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

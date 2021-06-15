import React from "react";
import { Heading, Text } from "@chakra-ui/react";
import InfiniteListControls from "~/components/ui/InfiniteListControls";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { useGetTagAudioList } from "~/features/audio/hooks";
import { useRouter } from "next/router";

export default function TagAudioPage() {
  const router = useRouter();
  const { tag, ...otherParams } = router.query;

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useGetTagAudioList((tag as string) || "", {
    ...otherParams,
  });

  return (
    <Page title={`Showing '${tag}' audios`}>
      {tag && (
        <Heading as="h2" size="lg">
          Showing '{tag}' audios
        </Heading>
      )}
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
}

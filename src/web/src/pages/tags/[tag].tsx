import React from "react";
import { Heading } from "@chakra-ui/react";
import InfiniteListControls from "~/components/UI/ListControls/Infinite";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { useGetTagAudioList } from "~/features/audio/api/hooks";
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
      <AudioList audios={audios} />
      <InfiniteListControls
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
}

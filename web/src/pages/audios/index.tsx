import { useRouter } from "next/router";
import React from "react";
import { Heading } from "@chakra-ui/react";
import InfiniteListControls from "~/components/InfiniteListControls";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { useAudioListQuery } from "~/features/audio/hooks";

export default function BrowseAudioNextPage() {
  const { query } = useRouter();

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudioListQuery();

  return (
    <Page title="Browse Latest Public Audios">
      {query["tag"] && (
        <Heading as="h2" size="lg">
          Showing '{query["tag"]}' audios
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

import React from "react";
import { useRouter } from "next/router";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { Audio } from "~/features/audio/types";
import useAudioList from "~/features/audio/hooks/queries/useAudioList";
import InfiniteListControls from "~/components/InfiniteListControls";
import { CursorPagedList } from "~/lib/types";
import { Heading } from "@chakra-ui/layout";

export interface AudioListPageProps {
  initialPage?: CursorPagedList<Audio>;
}

export default function AudioListPage(props: AudioListPageProps) {
  const { query } = useRouter();
  const { initialPage } = props;

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudioList({ ...query }, 1, {
    staleTime: 1000,
    initialData: initialPage && {
      pages: [initialPage],
      pageParams: [0],
    },
  });

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

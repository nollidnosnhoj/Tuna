import { Heading } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import PaginationListControls from "~/components/PaginationListControls";
import AudioList from "~/features/audio/components/List";
import { useAudioSearchQuery } from "~/features/audio/hooks";

export default function AudioSearchNextPage() {
  const {
    items: audios,
    page,
    setPage,
    totalPages,
    searchQuery,
  } = useAudioSearchQuery();

  return (
    <Page title="Search audios | Audiochan">
      <Heading>
        Search {searchQuery ? `results for ${searchQuery}` : ""}
      </Heading>
      <AudioList audios={audios} />
      {audios.length && (
        <PaginationListControls
          currentPage={page}
          onPageChange={setPage}
          pageNeighbors={2}
          totalPages={totalPages}
        />
      )}
    </Page>
  );
}

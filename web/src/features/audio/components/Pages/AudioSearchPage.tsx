import React, { useMemo } from "react";
import Page from "~/components/Page";
import { Heading } from "@chakra-ui/react";
import AudioList from "~/features/audio/components/List";
import { useGetAudioListPagination } from "../../hooks/queries";
import PaginationListControls from "~/components/PaginationListControls";

export type AudioSearchValues = {
  q?: string;
  sort?: string;
  tags?: string[];
};

export default function AudioSearchPage(props: AudioSearchValues) {
  const { q, sort, tags } = props;
  const queryParams = useMemo(
    () => ({
      q,
      sort,
      tags: tags?.join(","),
    }),
    [q, sort, tags]
  );

  const {
    items: audios,
    page,
    setPage,
    totalPages,
  } = useGetAudioListPagination("search/audios", {
    params: {
      ...queryParams,
    },
  });

  return (
    <Page title="Search audios | Audiochan" removeSearchBar>
      <Heading>Search {q ? `results for ${q}` : ""}</Heading>
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

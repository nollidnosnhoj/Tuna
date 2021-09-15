import { Heading } from "@chakra-ui/react";
import { useRouter } from "next/router";
import React from "react";
import Page from "~/components/Page";
import PaginationListControls from "~/components/UI/ListControls/Pagination";
import AudioList from "~/features/audio/components/List";
import { useSearchAudio } from "~/features/audio/api/hooks";
import { useGetPageParam } from "~/lib/hooks";

export default function AudioSearchNextPage() {
  const { query } = useRouter();
  const [queryPage, queryParams] = useGetPageParam(query);
  const { q } = queryParams;
  const {
    items: audios,
    page,
    setPage,
    totalPages,
  } = useSearchAudio(q as string, queryPage, queryParams);

  return (
    <Page title="Search audios | Audiochan">
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

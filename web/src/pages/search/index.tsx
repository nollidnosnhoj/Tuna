import { Heading } from "@chakra-ui/react";
import { useRouter } from "next/router";
import React from "react";
import Page from "~/components/Page";
import PaginationListControls from "~/components/PaginationListControls";
import AudioList from "~/features/audio/components/List";
import { Audio } from "~/features/audio/types";
import { useGetPageParam } from "~/lib/hooks/useGetPageParam";
import usePagination from "~/lib/hooks/usePagination";

export default function AudioSearchNextPage() {
  const { query } = useRouter();
  const [queryPage, queryParams] = useGetPageParam(query);
  const { q } = queryParams;
  const { items: audios, page, setPage, totalPages } = usePagination<Audio>(
    "search/audios",
    { ...queryParams },
    queryPage
  );

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

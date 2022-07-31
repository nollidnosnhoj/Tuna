import { Heading, List, ListItem } from "@chakra-ui/react";
import { useRouter } from "next/router";
import React from "react";
import Page from "~/components/Page";
import PaginationListControls from "~/components/ui/ListControls/Pagination";
import { useGetPageParam } from "~/lib/hooks";
import { AudioListItem } from "~/components/AudioItem";
import AudioShareButton from "~/components/buttons/Share";
import AudioMiscMenu from "~/components/buttons/Menu";
import { useSearchAudio } from "~/lib/hooks/api";

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
      {audios.length === 0 ? (
        <span>No audios found.</span>
      ) : (
        <List>
          {audios.map((audio) => (
            <ListItem key={audio.id}>
              <AudioListItem audio={audio}>
                <AudioShareButton audio={audio} />
                <AudioMiscMenu audio={audio} />
              </AudioListItem>
            </ListItem>
          ))}
        </List>
      )}
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

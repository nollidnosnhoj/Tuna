import React from "react";
import { Heading, Text } from "@chakra-ui/react";
import InfiniteListControls from "~/components/InfiniteListControls";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { GetServerSideProps } from "next";
import { getAccessToken } from "~/utils";
import { fetch } from "~/lib/api";
import { Audio } from "~/features/audio/types";
import { CursorPagedList } from "~/lib/types";
import useInfiniteCursorPagination from "~/lib/hooks/useInfiniteCursorPagination";
import { useAuth } from "~/lib/hooks/useAuth";

interface TagAudioPageProps {
  tag: string;
  audios: Audio[];
  nextCursor?: string;
}

export const getServerSideProps: GetServerSideProps<TagAudioPageProps> = async (
  context
) => {
  const tag = context.params?.tag as string;
  const accessToken = getAccessToken(context);

  const response = await fetch<CursorPagedList<Audio>>(
    "audios",
    { tag: tag },
    { accessToken }
  );

  return {
    props: {
      tag: tag,
      audios: response.items,
      nextCursor: response.next,
    },
  };
};

export default function TagAudioPage(props: TagAudioPageProps) {
  const { tag, audios: initAudio, nextCursor } = props;
  const { accessToken } = useAuth();

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteCursorPagination<Audio>(
    "audios",
    (cursor) =>
      fetch<CursorPagedList<Audio>>(
        "audios",
        { tag: tag, cursor: cursor },
        { accessToken }
      ),
    undefined,
    {
      initialData: {
        pageParams: [nextCursor],
        pages: [{ items: initAudio, next: nextCursor }],
      },
    }
  );

  return (
    <Page title="Browse Latest Public Audios">
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

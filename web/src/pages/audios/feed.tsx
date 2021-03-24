import { GetServerSideProps } from "next";
import React from "react";
import InfiniteListControls from "~/components/List/InfiniteListControls";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import AudioListSubHeader from "~/features/audio/components/ListSubheader";
import { useAudiosInfinite } from "~/features/audio/hooks/queries";
import { Audio } from "~/features/audio/types";
import { PagedList } from "~/lib/types";
import { fetchPages } from "~/utils/api";
import { getAccessToken } from "~/utils/cookies";

interface AudioFeedPageProps {
  filter: Record<string, any>;
  initialPage: PagedList<Audio>;
}

export const getServerSideProps: GetServerSideProps<AudioFeedPageProps> = async ({
  query,
  req,
}) => {
  const accessToken = getAccessToken({ req });
  const { page, ...filter } = query;

  const resultPage = await fetchPages<Audio>("me/feed", filter, 1, {
    accessToken,
  });

  return {
    props: {
      filter: filter,
      initialPage: resultPage,
    },
  };
};

export default function AudioFeedPage(props: AudioFeedPageProps) {
  const { filter, initialPage } = props;

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudiosInfinite("me/feed", filter, undefined, {
    staleTime: 1000, // Prevent double fetching
    initialData: {
      pages: [initialPage],
      pageParams: [1],
    },
  });

  return (
    <Page
      title="Your Feed"
      beforeContainer={<AudioListSubHeader current="feed" />}
    >
      <AudioList audios={audios} />
      <InfiniteListControls
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
}

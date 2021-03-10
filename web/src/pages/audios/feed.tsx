import { GetServerSideProps } from "next";
import React from "react";
import { QueryClient } from "react-query";
import { dehydrate, DehydratedState } from "react-query/hydration";
import Page from "~/components/Page";
import AudioInfiniteList from "~/features/audio/components/List/AudioInfiniteList";
import AudioListSubHeader from "~/features/audio/components/ListSubheader";
import { Audio } from "~/features/audio/types";
import { fetchPages } from "~/utils/api";
import { getAccessToken } from "~/utils/cookies";

interface AudioFeedPageProps {
  filter: Record<string, any>;
}

export const getServerSideProps: GetServerSideProps<
  AudioFeedPageProps & { dehydratedState: DehydratedState }
> = async ({ req, query }) => {
  const accessToken = getAccessToken({ req });
  const queryClient = new QueryClient();

  const { page, ...filter } = query;

  queryClient.prefetchQuery(["me/feed", filter], () =>
    fetchPages<Audio>("me/feed", filter, 1, { accessToken })
  );

  return {
    props: {
      filter: filter,
      dehydratedState: dehydrate(queryClient),
    },
  };
};

export default function AudioFeedPage(props: AudioFeedPageProps) {
  const { filter } = props;

  return (
    <Page
      title="Your Feed"
      beforeContainer={<AudioListSubHeader current="feed" />}
    >
      <AudioInfiniteList
        queryKey="me/feed"
        queryParams={{ ...filter }}
        size={15}
      />
    </Page>
  );
}

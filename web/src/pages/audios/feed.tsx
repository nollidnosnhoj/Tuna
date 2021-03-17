import { GetServerSideProps } from "next";
import React from "react";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import AudioListSubHeader from "~/features/audio/components/ListSubheader";
import { useAudiosInfinite } from "~/features/audio/hooks/queries";

interface AudioFeedPageProps {
  filter: Record<string, any>;
}

export const getServerSideProps: GetServerSideProps<AudioFeedPageProps> = async ({
  query,
}) => {
  const { page, ...filter } = query;

  return {
    props: {
      filter: filter,
    },
  };
};

export default function AudioFeedPage(props: AudioFeedPageProps) {
  const { filter } = props;

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudiosInfinite("me/feed", filter);

  return (
    <Page
      title="Your Feed"
      beforeContainer={<AudioListSubHeader current="feed" />}
    >
      <AudioList
        audios={audios}
        type="infinite"
        fetchNext={fetchNextPage}
        hasNextPage={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
}

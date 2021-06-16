import { Text } from "@chakra-ui/react";
import { GetServerSideProps } from "next";
import React from "react";
import Page from "~/components/Page";
import InfiniteListControls from "~/components/ui/InfiniteListControls";
import AudioList from "~/features/audio/components/List";
import { useGetAudioList } from "~/features/audio/hooks";
import { getAccessToken } from "~/lib/http/utils";

export const getServerSideProps: GetServerSideProps = async (context) => {
  const accessToken = getAccessToken(context?.req);
  console.log(accessToken);
  if (accessToken) {
    return {
      redirect: {
        destination: "/feed",
        permanent: false,
      },
    };
  }

  return {
    props: {},
  };
};

const Index = () => {
  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useGetAudioList();

  return (
    <Page title="Audiochan | Listen and Share Your Music">
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
};

export default Index;

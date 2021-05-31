import React from "react";
import { Heading, Text } from "@chakra-ui/react";
import NextLink from "next/link";
import InfiniteListControls from "~/components/InfiniteListControls";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { GetServerSideProps } from "next";
import { getAccessToken } from "~/utils";
import { AudioData } from "~/features/audio/types";
import {
  fetchUserAudios,
  useGetUserAudios,
} from "~/features/user/hooks/useGetUserAudios";

interface TagAudioPageProps {
  username: string;
  audios: AudioData[];
  nextCursor?: string;
}

export const getServerSideProps: GetServerSideProps<TagAudioPageProps> = async (
  context
) => {
  const username = context.params?.username as string;
  const accessToken = getAccessToken(context);

  const response = await fetchUserAudios(username, undefined, {}, accessToken);

  return {
    props: {
      username: username,
      audios: response.items,
      nextCursor: response.next,
    },
  };
};

export default function UserAudiosPage(props: TagAudioPageProps) {
  const { username, audios: initAudio, nextCursor } = props;

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useGetUserAudios(
    username,
    {},
    {
      initialData: {
        pageParams: [nextCursor],
        pages: [{ items: initAudio, next: nextCursor }],
      },
    }
  );

  return (
    <Page title={`Browse ${username}'s Audios`}>
      {username && (
        <Heading as="h2" size="lg">
          Showing {<NextLink href={`/users/${username}/`}>{username}</NextLink>}
          's audios
        </Heading>
      )}
      <AudioList
        audios={audios}
        notFoundContent={<Text>The user hasn't uploaded anything.</Text>}
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

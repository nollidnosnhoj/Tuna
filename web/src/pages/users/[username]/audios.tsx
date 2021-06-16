import React from "react";
import { Heading, Text } from "@chakra-ui/react";
import NextLink from "next/link";
import InfiniteListControls from "~/components/ui/InfiniteListControls";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { GetServerSideProps } from "next";
import { useGetUserAudios } from "~/features/user/hooks/useGetUserAudios";

interface UserAudiosPageProps {
  username: string;
}

export const getServerSideProps: GetServerSideProps<UserAudiosPageProps> =
  async (context) => {
    const username = context.params?.username as string;

    return {
      props: {
        username: username,
      },
    };
  };

export default function UserAudiosPage(props: UserAudiosPageProps) {
  const { username } = props;

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useGetUserAudios(username);

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

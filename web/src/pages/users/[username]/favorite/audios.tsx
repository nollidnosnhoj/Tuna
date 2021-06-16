import React from "react";
import { Heading, Text } from "@chakra-ui/react";
import NextLink from "next/link";
import InfiniteListControls from "~/components/ui/InfiniteListControls";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { GetServerSideProps } from "next";
import { useGetUserFavoriteAudios } from "~/features/user/hooks";

interface UserFavoriteAudiosPageProps {
  username: string;
}

export const getServerSideProps: GetServerSideProps<UserFavoriteAudiosPageProps> =
  async (context) => {
    const username = context.params?.username as string;

    return {
      props: {
        username: username,
      },
    };
  };

export default function UserFavoriteAudiosPage(
  props: UserFavoriteAudiosPageProps
) {
  const { username } = props;

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useGetUserFavoriteAudios(username);

  return (
    <Page title={`Browse ${username}'s Audios`}>
      {username && (
        <Heading as="h2" size="lg">
          Showing {<NextLink href={`/users/${username}/`}>{username}</NextLink>}
          's favorite audios
        </Heading>
      )}
      <AudioList
        audios={audios}
        notFoundContent={<Text>The user hasn't favorited anything.</Text>}
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

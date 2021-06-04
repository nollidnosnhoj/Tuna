import React from "react";
import { Heading, Text } from "@chakra-ui/react";
import NextLink from "next/link";
import InfiniteListControls from "~/components/ui/InfiniteListControls";
import Page from "~/components/Page";
import AudioList from "~/features/audio/components/List";
import { GetServerSideProps } from "next";
import { getAccessToken } from "~/utils";
import { AudioData } from "~/features/audio/types";
import { PagedList } from "~/lib/types";
import {
  fetchUserFavoriteAudios,
  useGetUserFavoriteAudios,
} from "~/features/user/hooks/useGetUserFavoriteAudios";

interface UserFavoriteAudiosPageProps {
  username: string;
  response: PagedList<AudioData>;
}

export const getServerSideProps: GetServerSideProps<UserFavoriteAudiosPageProps> =
  async (context) => {
    const username = context.params?.username as string;
    const accessToken = getAccessToken(context);

    const response = await fetchUserFavoriteAudios(
      username,
      1,
      {},
      accessToken
    );

    return {
      props: {
        username: username,
        response,
      },
    };
  };

export default function UserFavoriteAudiosPage(
  props: UserFavoriteAudiosPageProps
) {
  const { username, response } = props;

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useGetUserFavoriteAudios(
    username,
    {},
    {
      initialData: {
        pageParams: [response.page],
        pages: [response],
      },
    }
  );

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

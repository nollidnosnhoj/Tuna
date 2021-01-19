import {
  Avatar,
  Box,
  Button,
  Flex,
  Tab,
  TabList,
  TabPanel,
  TabPanels,
  Tabs,
  Text,
} from "@chakra-ui/react";
import React from "react";
import { GetServerSideProps } from "next";
import { useRouter } from "next/router";
import Page from "~/components/Shared/Page";
import AudioList from "~/components/Audio/List";
import { Profile } from "~/lib/types/user";

import { fetchUserProfile, useFollow, useProfile } from "~/lib/services/users";
import { getAccessToken } from "~/utils/cookies";
import { QueryClient, useQuery } from "react-query";
import { dehydrate } from "react-query/hydration";

export const getServerSideProps: GetServerSideProps = async (context) => {
  const queryClient = new QueryClient();
  const username = context.params?.username as string;
  const accessToken = getAccessToken(context);
  try {
    await queryClient.fetchQuery(["users", username], () =>
      fetchUserProfile(username, { accessToken })
    );
    return {
      props: {
        dehydratedState: dehydrate(queryClient),
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function ProfilePage() {
  const { query } = useRouter();
  const username = query.username as string;
  const { data: profile } = useProfile(username, { staleTime: 1000 });
  const { isFollowing, follow } = useFollow(username);

  return (
    <Page title={`${profile.username} | Audiochan`}>
      <Flex direction="row">
        <Box flex="1">
          <Box textAlign="center" marginBottom={4}>
            <Avatar size="2xl" name={profile.username} src="" />
            <br />
            <Text fontSize="lg" as="strong">
              {profile.username}
            </Text>
          </Box>
          <Flex justifyContent="center">
            <Button
              colorScheme="primary"
              variant={isFollowing ? "solid" : "outline"}
              disabled={isFollowing === undefined}
              paddingX={12}
              onClick={() => follow()}
            >
              {isFollowing ? "Followed" : "Follow"}
            </Button>
          </Flex>
        </Box>
        <Box flex="2">
          <Tabs isLazy>
            <TabList>
              <Tab>Uploads</Tab>
              <Tab>Favorites</Tab>
            </TabList>
            <TabPanels>
              <TabPanel>
                <AudioList type="user" username={profile.username} />
              </TabPanel>
              <TabPanel>
                <AudioList type="favorites" username={profile.username} />
              </TabPanel>
            </TabPanels>
          </Tabs>
        </Box>
      </Flex>
    </Page>
  );
}

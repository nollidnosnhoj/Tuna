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

import {
  fetchUserProfile,
  useAddUserPicture,
  useFollow,
  useProfile,
} from "~/lib/services/users";
import { getAccessToken } from "~/utils/cookies";
import { QueryClient, useQuery } from "react-query";
import { dehydrate } from "react-query/hydration";
import UserPicture from "~/components/User/Picture";
import useUser from "~/lib/contexts/user_context";

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
  const { user } = useUser();
  const { query } = useRouter();
  const username = query.username as string;
  const { data: profile } = useProfile(username, { staleTime: 1000 });
  const { mutateAsync: addPictureAsync } = useAddUserPicture(user?.username);
  const { isFollowing, follow } = useFollow(username);

  return (
    <Page title={`${profile.username} | Audiochan`}>
      <Flex direction="row">
        <Flex flex="1" direction="column" justify="center">
          <Box textAlign="center">
            <UserPicture
              user={profile}
              canReplace={profile.id === user?.id}
              name="image"
              size="2xl"
              onChange={async (file) => {
                await addPictureAsync(file);
              }}
            />
          </Box>
          <Box textAlign="center" marginY={4}>
            <Text fontSize="2xl" as="strong">
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
        </Flex>
        <Box flex="3">
          <Tabs isLazy>
            <TabList>
              <Tab>Uploads</Tab>
              <Tab>Favorites</Tab>
            </TabList>
            <TabPanels>
              <TabPanel>
                <AudioList
                  type="user"
                  username={profile.username}
                  removeArtistName
                />
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

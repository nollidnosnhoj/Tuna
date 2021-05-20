import {
  Flex,
  Box,
  Heading,
  VStack,
  Spacer,
  Tab,
  TabList,
  TabPanel,
  TabPanels,
  Tabs,
} from "@chakra-ui/react";
import { QueryClient, useQuery } from "react-query";
import { dehydrate } from "react-query/hydration";
import { GetServerSideProps } from "next";
import { useRouter } from "next/router";
import React from "react";
import Page from "~/components/Page";
import ProfileFollowButton from "~/features/user/components/ProfileFollowButton";
import ProfileEditButton from "~/features/user/components/ProfileEditButton";
import ProfilePicture from "~/features/user/components/ProfilePicture";
import ProfileLatestAudios from "~/features/user/components/ProfileLatestAudios";
import { fetchUserProfile } from "~/features/user/services";
import { Profile } from "~/features/user/types";
import { useAuth } from "~/lib/hooks/useAuth";
import { useUser } from "~/lib/hooks/useUser";
import { ErrorResponse } from "~/lib/types";
import { getAccessToken } from "~/utils";

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

export default function UserProfileNextPage() {
  const { user } = useUser();
  const { query } = useRouter();
  const username = query.username as string;
  const { accessToken } = useAuth();

  const { data: profile } = useQuery<Profile, ErrorResponse>(
    ["users", username],
    () => fetchUserProfile(username, { accessToken }),
    {
      staleTime: 1000,
    }
  );

  if (!profile) return null;

  return (
    <Page title={`${profile.username} | Audiochan`}>
      <Flex marginBottom={4}>
        <Box flex="1" marginRight={4}>
          <ProfilePicture
            pictureSrc={profile.picture}
            username={profile.username}
            canModify={user?.id === profile.id}
          />
        </Box>
        <Flex flex="4">
          <VStack alignItems="flex-start" width="100%">
            <Box paddingY={2} flex="3">
              <Heading as="strong">{profile.username}</Heading>
            </Box>
            <Spacer />
            <Flex justifyContent="flex-end" flex="1">
              <ProfileFollowButton
                profileId={profile.id}
                username={profile.username}
                isFollowing={profile.isFollowing}
              />
              <ProfileEditButton profileId={profile.id} />
            </Flex>
          </VStack>
          <Box></Box>
        </Flex>
      </Flex>
      <Box>
        <Tabs>
          <TabList>
            <Tab>Uploads</Tab>
          </TabList>

          <TabPanels>
            <TabPanel>
              <ProfileLatestAudios username={profile.username} />
            </TabPanel>
          </TabPanels>
        </Tabs>
      </Box>
    </Page>
  );
}

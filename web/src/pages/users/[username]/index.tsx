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
  Button,
} from "@chakra-ui/react";
import { QueryClient } from "react-query";
import { dehydrate } from "react-query/hydration";
import { GetServerSideProps } from "next";
import NextLink from "next/link";
import { useRouter } from "next/router";
import React, { useState } from "react";
import Page from "~/components/Page";
import Picture from "~/components/Picture";
import ProfileFollowButton from "~/features/user/components/ProfileFollowButton";
import ProfileEditButton from "~/features/user/components/ProfileEditButton";
import { fetchUserProfile } from "~/features/user/services";
import {
  useAddUserPicture,
  useUserAudioListQuery,
} from "~/features/user/hooks";
import { useProfile } from "~/features/user/hooks";
import AudioList from "~/features/audio/components/List";
import { useUser } from "~/lib/hooks/useUser";
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

  const { data: profile } = useProfile(username, { staleTime: 1000 });

  const {
    mutateAsync: addPictureAsync,
    isLoading: isAddingPicture,
  } = useAddUserPicture(username);

  const [picture, setPicture] = useState(profile?.picture ?? "");

  const { items: latestAudios } = useUserAudioListQuery(username, 5, {
    staleTime: 1000,
  });

  if (!profile) return null;

  return (
    <Page title={`${profile.username} | Audiochan`}>
      <Flex marginBottom={4}>
        <Box flex="1" marginRight={4}>
          <Picture
            title={profile.username}
            src={picture}
            onChange={async (croppedData) => {
              const data = await addPictureAsync(croppedData);
              setPicture(data.image);
            }}
            isUploading={isAddingPicture}
            canEdit={profile.id === user?.id}
          />
        </Box>
        <Flex flex="4">
          <VStack alignItems="flex-start" width="100%">
            <Box paddingY={2} flex="3">
              <Heading as="strong">{profile.username}</Heading>
            </Box>
            <Spacer />
            <Flex justifyContent="flex-end" flex="1">
              <ProfileFollowButton profile={profile} />
              <ProfileEditButton profile={profile} />
            </Flex>
          </VStack>
          <Box></Box>
        </Flex>
      </Flex>
      <Box>
        <Tabs>
          <TabList>
            <Tab>Latest Audios</Tab>
          </TabList>

          <TabPanels>
            <TabPanel>
              <AudioList
                audios={latestAudios}
                notFoundContent={<p>No uploads.</p>}
                defaultLayout="list"
                hideLayoutToggle
              />
              <NextLink href={`/users/${username}/audios`}>
                <Button width="100%">View More</Button>
              </NextLink>
            </TabPanel>
          </TabPanels>
        </Tabs>
      </Box>
    </Page>
  );
}

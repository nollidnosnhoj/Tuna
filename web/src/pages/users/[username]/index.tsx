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
import { GetServerSideProps } from "next";
import { useRouter } from "next/router";
import React from "react";
import Page from "~/components/Page";
import ProfileFollowButton from "~/features/user/components/ProfileFollowButton";
import ProfileEditButton from "~/features/user/components/ProfileEditButton";
import ProfileLatestAudios from "~/features/user/components/ProfileLatestAudios";
import { useAddUserPicture, useUser } from "~/features/user/hooks";
import { useGetProfile } from "~/features/user/hooks";
import ProfileFavoriteAudios from "~/features/user/components/ProfileFavoriteAudios";
import PictureController from "~/components/Picture";
import { fetchProfile } from "~/features/user/api";
import { Profile } from "~/features/user/types";

interface ProfilePageProps {
  profile?: Profile;
}

export const getServerSideProps: GetServerSideProps<ProfilePageProps> = async (
  context
) => {
  const username = context.params?.username as string;
  try {
    const data = await fetchProfile(username, context);
    return {
      props: {
        profile: data,
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function UserProfileNextPage(props: ProfilePageProps) {
  const { user } = useUser();
  const { query } = useRouter();
  const username = query.username as string;

  const { data: profile } = useGetProfile(username, {
    staleTime: 1000,
    initialData: props.profile,
  });

  const { mutateAsync: addPictureAsync, isLoading: isAddingPicture } =
    useAddUserPicture(username);

  if (!profile) return null;

  return (
    <Page title={`${profile.username} | Audiochan`}>
      <Flex marginBottom={4}>
        <Box flex="1" marginRight={4}>
          <PictureController
            title={profile.username}
            src={profile.picture}
            onChange={async (croppedData) => {
              await addPictureAsync(croppedData);
            }}
            isUploading={isAddingPicture}
            canEdit={user?.id === profile.id}
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
        <Tabs isLazy>
          <TabList>
            <Tab>Uploads</Tab>
            <Tab>Favorites</Tab>
          </TabList>

          <TabPanels>
            <TabPanel>
              <ProfileLatestAudios username={profile.username} />
            </TabPanel>
            <TabPanel>
              <ProfileFavoriteAudios username={profile.username} />
            </TabPanel>
          </TabPanels>
        </Tabs>
      </Box>
    </Page>
  );
}

import {
  Box,
  Button,
  Tab,
  TabList,
  TabPanel,
  TabPanels,
  Tabs,
} from "@chakra-ui/react";
import { GetServerSideProps } from "next";
import NextLink from "next/link";
import React from "react";
import Page from "~/components/Page";
import {
  useGetProfile,
  useGetUserAudios,
  useGetUserFavoriteAudios,
} from "~/features/user/api/hooks";
import { getProfileRequest } from "~/features/user/api";
import { Profile } from "~/features/user/api/types";
import ProfileDetails from "~/features/user/components/Profile";
import AudioList from "~/features/audio/components/List";

interface ProfilePageProps {
  profile?: Profile;
  username: string;
}

export const getServerSideProps: GetServerSideProps<ProfilePageProps> = async (
  context
) => {
  const username = context.params?.username as string;
  try {
    const data = await getProfileRequest(username, context);
    return {
      props: {
        profile: data,
        username,
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function UserProfileNextPage(props: ProfilePageProps) {
  const { data: profile } = useGetProfile(props.username, {
    staleTime: 1000,
    initialData: props.profile,
  });

  const { items: latestAudios } = useGetUserAudios(
    profile?.username ?? "",
    { size: 30 },
    { staleTime: 1000 * 60 * 5 }
  );

  const { items: latestFavoriteAudios } = useGetUserFavoriteAudios(
    profile?.username ?? "",
    { size: 30 },
    { staleTime: 1000 * 60 * 5 }
  );

  if (!profile) return null;

  return (
    <Page title={`${profile.username} | Audiochan`}>
      <ProfileDetails profile={profile} />
      <Tabs isLazy>
        <TabList>
          <Tab>Uploads</Tab>
          <Tab>Favorites</Tab>
        </TabList>
        <TabPanels>
          <TabPanel>
            <Box>
              <AudioList
                audios={latestAudios}
                context={`users:${profile.username}`}
              />
              {latestAudios.length > 0 && (
                <NextLink href={`/users/${profile.username}/audios`}>
                  <Button width="100%">View More</Button>
                </NextLink>
              )}
            </Box>
          </TabPanel>
          <TabPanel>
            <Box>
              <AudioList
                audios={latestFavoriteAudios}
                context={`user_favorites:${profile.username}`}
              />
              {latestFavoriteAudios.length > 0 && (
                <NextLink href={`/users/${profile.username}/favorite/audios`}>
                  <Button width="100%">View More</Button>
                </NextLink>
              )}
            </Box>
          </TabPanel>
        </TabPanels>
      </Tabs>
    </Page>
  );
}

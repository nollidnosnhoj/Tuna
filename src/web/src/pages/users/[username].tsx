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
import { Profile } from "~/features/user/api/types";
import ProfileDetails from "~/features/user/components/Profile";
import AudioList from "~/features/audio/components/List";
import request from "~/lib/http";

interface ProfilePageProps {
  profile?: Profile;
  username: string;
}

export const getServerSideProps: GetServerSideProps<ProfilePageProps> = async (
  context
) => {
  const username = context.params?.username as string;
  const { req, res } = context;
  try {
    const { data } = await request<Profile>({
      method: "get",
      url: `users/${username}`,
      req,
      res,
    });
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
    profile?.userName ?? "",
    { size: 5 },
    { staleTime: 1000 * 60 * 5 }
  );

  const { items: latestFavoriteAudios } = useGetUserFavoriteAudios(
    profile?.userName ?? "",
    { size: 5 },
    { staleTime: 1000 * 60 * 5 }
  );

  if (!profile) return null;

  return (
    <Page title={`${profile.userName} | Audiochan`}>
      <ProfileDetails profile={profile} />
      <Tabs isLazy>
        <TabList>
          <Tab>Uploads</Tab>
          <Tab>Favorites</Tab>
        </TabList>
        <TabPanels>
          <TabPanel>
            <Box>
              <Box marginBottom={4}>
                <AudioList audios={latestAudios} />
              </Box>
              {latestAudios.length > 0 && (
                <NextLink href={`/users/${profile.userName}/audios`}>
                  <Button width="100%">View More</Button>
                </NextLink>
              )}
            </Box>
          </TabPanel>
          <TabPanel>
            <Box>
              <Box marginBottom={4}>
                <AudioList audios={latestFavoriteAudios} />
              </Box>
              {latestFavoriteAudios.length > 0 && (
                <NextLink href={`/users/${profile.userName}/favorite/audios`}>
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

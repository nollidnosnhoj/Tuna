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
import React, { useEffect, useState } from "react";
import { GetServerSideProps, InferGetServerSidePropsType } from "next";
import { useRouter } from "next/router";
import useSWR from "swr";
import PageLayout from "~/components/Layout";
import request from "~/lib/request";
import { ErrorResponse, Profile } from "~/lib/types";
import AudioList from "~/components/AudioList";

interface PageProps {
  initialData?: Profile;
}

export const getServerSideProps: GetServerSideProps<PageProps> = async (
  context
) => {
  const username = context.params?.username as string;

  try {
    const { data: initialData } = await request<Profile>(`/users/${username}`);
    return {
      props: {
        initialData,
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function ProfilePage(
  props: InferGetServerSidePropsType<typeof getServerSideProps>
) {
  const { query } = useRouter();
  const username = query.username as string;
  const [isFollowing, setIsFollowing] = useState(false);

  const { data: profile } = useSWR<Profile, ErrorResponse>(`/me/${username}`, {
    initialData: props.initialData,
  });

  useEffect(() => {
    const checkIsFollowing = async () => {
      try {
        await request(`users/${username}/followers`, { method: "head" });
        setIsFollowing(true);
      } catch (err) {
        setIsFollowing(false);
      }
    };
    checkIsFollowing();
  }, [profile]);

  return (
    <PageLayout title={`${profile.username} | Audiochan`}>
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
              paddingX={12}
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
    </PageLayout>
  );
}

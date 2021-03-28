import {
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
import React, { useState } from "react";
import { useRouter } from "next/router";
import Page from "~/components/Page";
import Picture from "~/components/Picture";
import { useProfile } from "~/features/user/hooks/queries/useProfile";
import useUser from "~/hooks/useUser";
import UserAudioList from "~/features/user/components/UserAudioList";
import { useAddUserPicture } from "~/features/user/hooks/mutations/useAddUserPicture";
import { useFollow } from "~/features/user/hooks/mutations";
import PictureDropzone from "~/components/Picture/PictureDropzone";

export default function ProfilePage() {
  const { user } = useUser();
  const { query } = useRouter();
  const username = query.username as string;
  const { data: profile } = useProfile(username, { staleTime: 1000 });
  if (!profile) return null;
  const {
    mutateAsync: addPictureAsync,
    isLoading: isAddingPicture,
  } = useAddUserPicture(username);

  const [picture, setPicture] = useState(() => {
    return profile?.picture
      ? `https://audiochan-public.s3.amazonaws.com/${profile.picture}`
      : "";
  });

  const { isFollowing, follow } = useFollow(username, profile.isFollowing);

  return (
    <Page title={`${profile.username} | Audiochan`}>
      <Flex direction="row">
        <Flex flex="1" direction="column" align="center">
          <Box textAlign="center">
            <PictureDropzone
              disabled={isAddingPicture && user?.id === profile.id}
              onChange={async (imageData) => {
                const { data } = await addPictureAsync(imageData);
                setPicture(data.image);
              }}
            >
              <Picture source={picture} imageSize={250} />
            </PictureDropzone>
          </Box>
          <Box textAlign="center" marginY={4}>
            <Text fontSize="2xl" as="strong">
              {profile!.username}
            </Text>
          </Box>
          <Flex justifyContent="center">
            {user && user.id !== profile.id && (
              <Button
                colorScheme="primary"
                variant={isFollowing ? "solid" : "outline"}
                disabled={isFollowing === undefined}
                paddingX={12}
                onClick={() => follow()}
              >
                {isFollowing ? "Followed" : "Follow"}
              </Button>
            )}
          </Flex>
        </Flex>
        <Box flex="3">
          <Tabs isLazy>
            <TabList>
              <Tab>Uploads</Tab>
            </TabList>
            <TabPanels>
              <TabPanel>
                <UserAudioList username={profile.username} />
              </TabPanel>
            </TabPanels>
          </Tabs>
        </Box>
      </Flex>
    </Page>
  );
}

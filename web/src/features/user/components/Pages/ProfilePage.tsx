import {
  Box,
  Button,
  Flex,
  Heading,
  HStack,
  Spacer,
  Text,
} from "@chakra-ui/react";
import React, { useState } from "react";
import NextLink from "next/link";
import { useRouter } from "next/router";
import Page from "~/components/Page";
import Picture from "~/components/Picture";
import { useProfile } from "~/features/user/hooks/queries/useProfile";
import { useUser } from "~/lib/contexts/UserContext";
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
      <Flex marginBottom={4}>
        <Box flex="1">
          <PictureDropzone
            disabled={isAddingPicture && user?.id === profile.id}
            onChange={async (imageData) => {
              const { data } = await addPictureAsync(imageData);
              setPicture(data.image);
            }}
          >
            <Picture source={picture} imageSize={200} borderWidth="1px" />
          </PictureDropzone>
        </Box>
        <Flex flex="4">
          <Flex width="100%">
            <Box paddingY={2} flex="3">
              <Heading as="strong">{profile!.username}</Heading>
            </Box>
            <Flex justifyContent="flex-end" flex="1">
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
              {user && user.id === profile.id && (
                <Button paddingX={12}>Edit Profile</Button>
              )}
            </Flex>
          </Flex>
          <Box></Box>
        </Flex>
      </Flex>
      <UserAudioList username={profile.username} hidePaginationControls />
    </Page>
  );
}

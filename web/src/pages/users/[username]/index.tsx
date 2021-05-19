import { GetServerSideProps } from "next";
import { QueryClient } from "react-query";
import { dehydrate } from "react-query/hydration";
import { fetchUserProfile } from "~/features/user/services";
import { getAccessToken } from "~/utils";
import { useRouter } from "next/router";
import React, { useState } from "react";
import { useAddUserPicture, useFollow } from "~/features/user/hooks";
import { useProfile } from "~/features/user/hooks";
import { useUser } from "~/lib/hooks/useUser";
import { Flex, Box, Heading, Button } from "@chakra-ui/react";
import Page from "~/components/Page";
import Picture from "~/components/Picture";
import UserAudioList from "~/features/user/components/UserAudioList";

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

  // TODO: Put this into it's own component
  const [picture, setPicture] = useState(profile?.picture ?? "");

  // TODO: Put this into it's own component
  const { isFollowing, follow } = useFollow(username, profile?.isFollowing);

  if (!profile) return null;

  return (
    <Page title={`${profile.username} | Audiochan`}>
      <Flex marginBottom={4}>
        <Box flex="1">
          <Picture
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
          <Flex width="100%">
            <Box paddingY={2} flex="3">
              <Heading as="strong">{profile.username}</Heading>
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

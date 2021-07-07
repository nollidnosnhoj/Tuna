import { Flex, Box, VStack, Heading, Spacer } from "@chakra-ui/react";
import React from "react";
import PictureController from "~/components/Picture";
import { useAddUserPicture, useUser } from "../../hooks";
import { Profile } from "../../types";
import ProfileEditButton from "./ProfileEditButton";
import ProfileFollowButton from "./ProfileFollowButton";

interface ProfileDetailsProps {
  profile: Profile;
}

export default function ProfileDetails({ profile }: ProfileDetailsProps) {
  const { user } = useUser();

  const { mutateAsync: addPictureAsync, isLoading: isAddingPicture } =
    useAddUserPicture(profile.username);

  return (
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
            />
            <ProfileEditButton profileId={profile.id} />
          </Flex>
        </VStack>
        <Box></Box>
      </Flex>
    </Flex>
  );
}

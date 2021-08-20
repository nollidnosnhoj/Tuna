import { Flex, Box, Heading, Stack } from "@chakra-ui/react";
import React from "react";
import PictureController from "~/components/Picture";
import { useAddUserPicture, useRemoveUserPicture } from "../../api/hooks";
import { Profile } from "../../api/types";
import { useUser } from "../../hooks";
import ProfileFollowButton from "./Buttons/Follow";

interface ProfileDetailsProps {
  profile: Profile;
}

export default function ProfileDetails({ profile }: ProfileDetailsProps) {
  const { user } = useUser();

  const { mutateAsync: addPictureAsync, isLoading: isAddingPicture } =
    useAddUserPicture(profile.username);

  const { mutateAsync: removePictureAsync, isLoading: isRemovingPicture } =
    useRemoveUserPicture(profile.username);

  return (
    <>
      <Flex
        marginBottom={4}
        justifyContent="center"
        direction={{ base: "column", md: "row" }}
      >
        <Flex
          flex="1"
          marginRight={4}
          justify={{ base: "center", md: "normal" }}
        >
          <PictureController
            title={profile.username}
            src={profile.picture}
            onChange={async (croppedData) => {
              await addPictureAsync(croppedData);
            }}
            onRemove={removePictureAsync}
            isMutating={isAddingPicture || isRemovingPicture}
            canEdit={user?.id === profile.id}
          />
        </Flex>
        <Box flex="6">
          <Stack
            direction="row"
            marginTop={{ base: 4, md: 0 }}
            marginBottom={4}
          >
            <Heading as="h1" fontSize={{ base: "3xl", md: "5xl" }}>
              {profile.username}
            </Heading>
            <Flex justifyContent="flex-end" flex="1">
              <ProfileFollowButton profileId={profile.id} />
            </Flex>
          </Stack>
        </Box>
      </Flex>
    </>
  );
}

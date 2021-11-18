import {
  Box,
  Button,
  Flex,
  Heading,
  List,
  ListItem,
  Stack,
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
  useAddArtistPicture,
  useFollow,
  useGetArtistProfile,
  useGetArtistAudios,
  useGetUserFavoriteAudios,
  useRemoveArtistProfile,
} from "~/lib/hooks/api";
import request from "~/lib/http";
import { Profile } from "~/lib/types";
import PictureController from "~/components/Picture";
import { useUser } from "~/components/providers/UserProvider";
import { AudioListItem } from "~/components/AudioItem";
import AudioShareButton from "~/components/buttons/Share";
import AudioMiscMenu from "~/components/buttons/Menu";

interface ProfilePageProps {
  profile?: Profile;
  username: string;
}

function ProfilePicture(props: { profile: Profile }) {
  const { profile } = props;
  const { user } = useUser();

  const { mutateAsync: addPictureAsync, isLoading: isAddingPicture } =
    useAddArtistPicture(profile.userName);

  const { mutateAsync: removePictureAsync, isLoading: isRemovingPicture } =
    useRemoveArtistProfile(profile.userName);
  return (
    <PictureController
      title={profile.userName}
      src={profile.picture}
      onChange={async (croppedData) => {
        await addPictureAsync(croppedData);
      }}
      onRemove={removePictureAsync}
      isMutating={isAddingPicture || isRemovingPicture}
      canEdit={user?.id === profile.id}
      width={250}
    />
  );
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
  const { user: currentUser } = useUser();
  const { data: profile } = useGetArtistProfile(props.username, {
    staleTime: 1000,
    initialData: props.profile,
  });

  const { isFollowing, follow } = useFollow(profile!.id);

  const { items: latestAudios } = useGetArtistAudios(
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
          <ProfilePicture profile={profile} />
        </Flex>
        <Box flex="7">
          <Stack
            direction="row"
            marginTop={{ base: 4, md: 0 }}
            marginBottom={4}
          >
            <Heading as="h1" fontSize={{ base: "3xl", md: "5xl" }}>
              {profile.userName}
            </Heading>
            <Flex justifyContent="flex-end" flex="1">
              {currentUser?.id !== profile.id && (
                <Button
                  colorScheme="primary"
                  variant={isFollowing ? "solid" : "outline"}
                  disabled={isFollowing === undefined}
                  paddingX={12}
                  onClick={follow}
                >
                  {isFollowing ? "Followed" : "Follow"}
                </Button>
              )}
            </Flex>
          </Stack>
        </Box>
      </Flex>
      <Tabs isLazy>
        <TabList>
          <Tab>Uploads</Tab>
          <Tab>Favorites</Tab>
        </TabList>
        <TabPanels>
          <TabPanel>
            <Box>
              <Box marginBottom={4}>
                <List>
                  {latestAudios.map((audio) => (
                    <ListItem key={`${audio.id}:${audio.slug}`}>
                      <AudioListItem audio={audio}>
                        <AudioShareButton audio={audio} />
                        <AudioMiscMenu audio={audio} />
                      </AudioListItem>
                    </ListItem>
                  ))}
                </List>
              </Box>
              {latestAudios.length > 0 && (
                <NextLink href={`/artists/${profile.userName}/audios`}>
                  <Button width="100%">View More</Button>
                </NextLink>
              )}
            </Box>
          </TabPanel>
          <TabPanel>
            <Box>
              <Box marginBottom={4}>
                <List>
                  {latestFavoriteAudios.map((audio) => (
                    <ListItem key={`${audio.id}:${audio.slug}`}>
                      <AudioListItem audio={audio}>
                        <AudioShareButton audio={audio} />
                        <AudioMiscMenu audio={audio} />
                      </AudioListItem>
                    </ListItem>
                  ))}
                </List>
              </Box>
              {latestFavoriteAudios.length > 0 && (
                <NextLink href={`/artists/${profile.userName}/favorite/audios`}>
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

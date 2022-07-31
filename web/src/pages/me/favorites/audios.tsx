import React from "react";
import Page from "~/components/Page";
import { GetServerSideProps } from "next";
import { CurrentUser } from "~/lib/types";
import { authRoute } from "~/lib/server/authRoute";
import Container from "~/components/ui/Container";
import UserDashboardSubHeader from "~/components/DashboardNavbar";
import { useGetUserFavoriteAudios } from "~/lib/hooks/api";
import { Spacer } from "@chakra-ui/layout";
import { chakra } from "@chakra-ui/system";
import { AudioListItem } from "~/components/AudioItem";
import AudioList from "~/components/AudioList";
import AudioListFilterInput from "~/components/AudioList/FilterListInput";
import AudioListToggleView from "~/components/AudioList/ToggleListView";
import AudioMiscMenu from "~/components/buttons/Menu";
import AudioShareButton from "~/components/buttons/Share";
import InfiniteListControls from "~/components/ui/ListControls/Infinite";

interface YourFavoriteAudiosPageProps {
  user: CurrentUser;
}

export const getServerSideProps: GetServerSideProps =
  authRoute<YourFavoriteAudiosPageProps>(async (_, user) => {
    return {
      props: {
        user,
      },
    };
  });

export default function YourFavoriteAudiosPage(
  props: YourFavoriteAudiosPageProps
) {
  const { user } = props;

  const { items, hasNextPage, isFetching, fetchNextPage } =
    useGetUserFavoriteAudios(user.userName);

  return (
    <Page title="Your Favorite audios" removeContainer requiresAuth>
      <UserDashboardSubHeader active="favoriteAudios" />
      <Container>
        <chakra.div>
          <AudioList
            audios={items}
            renderItem={(audio) => (
              <AudioListItem audio={audio}>
                <AudioShareButton audio={audio} />
                <AudioMiscMenu audio={audio} />
              </AudioListItem>
            )}
            customNoAudiosFoundMessage="You have not favorited an audio yet."
          >
            <Spacer />
            <AudioListToggleView />
            <AudioListFilterInput />
          </AudioList>
          <InfiniteListControls
            hasNext={hasNextPage}
            fetchNext={fetchNextPage}
            isFetching={isFetching}
          />
        </chakra.div>
      </Container>
    </Page>
  );
}

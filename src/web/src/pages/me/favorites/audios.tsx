import React from "react";
import Page from "~/components/Page";
import FavoriteAudiosList from "~/components/FavoriteAudiosList";
import { GetServerSideProps } from "next";
import { CurrentUser } from "~/lib/types";
import { authRoute } from "~/lib/server/authRoute";
import Container from "~/components/ui/Container";
import UserDashboardSubHeader from "~/components/DashboardNavbar";

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
  if (!user) return null;
  return (
    <Page title="Your Favorite audios" removeContainer requiresAuth>
      <UserDashboardSubHeader active="favoriteAudios" />
      <Container>
        <FavoriteAudiosList userName={user.userName} />
      </Container>
    </Page>
  );
}

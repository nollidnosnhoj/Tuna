import { Heading, List, ListItem } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import InfiniteListControls from "~/components/ui/ListControls/Infinite";
import { useGetUserAudios } from "~/lib/hooks/api";
import { AudioListItem } from "~/components/AudioItem";
import AudioShareButton from "~/components/buttons/Share";
import AudioMiscMenu from "~/components/buttons/Menu";
import { GetServerSideProps } from "next";
import { authRoute } from "~/lib/server/authRoute";
import { CurrentUser } from "~/lib/types";
import Container from "~/components/ui/Container";
import UserDashboardSubHeader from "~/components/DashboardNavbar";

interface YourAudiosPageProps {
  user: CurrentUser;
}

export const getServerSideProps: GetServerSideProps =
  authRoute<YourAudiosPageProps>(async (_, user) => {
    return {
      props: {
        user,
      },
    };
  });

export default function YourAudiosPage({ user }: YourAudiosPageProps) {
  const { items, hasNextPage, isFetching, fetchNextPage } = useGetUserAudios(
    user.userName
  );

  return (
    <Page title="Your audios" removeContainer requiresAuth>
      <UserDashboardSubHeader active="audios" />
      <Container>
        <Heading>Your audios</Heading>
        {items.length === 0 ? (
          <span>No audios found.</span>
        ) : (
          <List>
            {items.map((audio) => (
              <ListItem key={audio.id}>
                <AudioListItem audio={audio}>
                  <AudioShareButton audio={audio} />
                  <AudioMiscMenu audio={audio} />
                </AudioListItem>
              </ListItem>
            ))}
          </List>
        )}
        <InfiniteListControls
          hasNext={hasNextPage}
          fetchNext={fetchNextPage}
          isFetching={isFetching}
        />
      </Container>
    </Page>
  );
}

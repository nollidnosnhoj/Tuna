import { Text } from "@chakra-ui/layout";
import React from "react";
import Page from "~/components/Page";
import { CurrentUser } from "~/lib/types";
import { GetServerSideProps } from "next";
import { authRoute } from "~/lib/server/authRoute";
import Container from "~/components/ui/Container";
import UserDashboardSubHeader from "~/components/DashboardNavbar";

interface DashboardOverviewPageProps {
  user: CurrentUser;
}

export const getServerSideProps: GetServerSideProps =
  authRoute<DashboardOverviewPageProps>(async (_, user) => {
    return {
      props: {
        user,
      },
    };
  });

export default function DashboardOverviewPage() {
  return (
    <Page removeContainer requiresAuth>
      <UserDashboardSubHeader active="overview" />
      <Container>
        <Text>Work in progress</Text>
      </Container>
    </Page>
  );
}

import React from "react";
import { GetServerSideProps } from "next";
import request from "~/lib/request";
import { User } from "~/lib/types";
import PageLayout from "~/components/Layout";
import AudioList from "~/components/AudioList";

export const getServerSideProps: GetServerSideProps = async (context) => {
  try {
    const { data } = await request<User>("me/feed", { ctx: context });
    return { props: { user: data } };
  } catch (err) {
    return {
      props: {},
      redirect: {
        destination: "/login",
        permanent: false,
      },
    };
  }
};

export default function FeedPage() {
  return (
    <PageLayout title="Dashboard">
      <AudioList headerText="Your Feed" type="feed" />
    </PageLayout>
  );
}

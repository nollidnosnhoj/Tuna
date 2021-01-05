import React from "react";
import PageLayout from "~/components/Layout";
import AudioList from "~/components/AudioList";
import AuthRequired from "~/components/Auth/AuthRequired";

export default function FeedPage() {
  return (
    <PageLayout title="Dashboard">
      <AuthRequired>
        <AudioList headerText="Your Feed" type="feed" />
      </AuthRequired>
    </PageLayout>
  );
}

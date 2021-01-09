import React from "react";
import Page from "~/components/Shared/Page";
import AudioList from "~/components/Audio/List";

export default function FeedPage() {
  return (
    <Page title="Your Feed" loginRequired={true}>
      <AudioList headerText="Your Feed" type="feed" />
    </Page>
  );
}

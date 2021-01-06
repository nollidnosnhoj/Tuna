import React from "react";
import Page from "~/components/Layout";
import AudioList from "~/components/Audio/List";
import AuthRequired from "~/components/Auth/AuthRequired";

export default function FeedPage() {
  return (
    <Page title="Your Feed" loginRequired={true}>
      <AudioList headerText="Your Feed" type="feed" />
    </Page>
  );
}

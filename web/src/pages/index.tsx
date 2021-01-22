import React from "react";
import useUser from "~/lib/contexts/user_context";
import Page from "~/components/Shared/Page";
import AudioList from "~/components/Audio/List";

const Index = () => {
  return (
    <Page>
      <AudioList type="audios" />
    </Page>
  );
};

export default Index;

import React from "react";
import PageLayout from "~/components/Layout";
import AudioList from "~/components/AudioList";

const Index = () => {
  return (
    <PageLayout title="Audiochan">
      <AudioList type="audios" size={2} />
    </PageLayout>
  );
};

export default Index;

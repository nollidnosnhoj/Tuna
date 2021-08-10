import { Heading } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import InfiniteListControls from "~/components/ui/ListControls/Infinite";
import AudioList from "~/features/audio/components/List";
import { useYourAudios } from "~/features/auth/api/hooks";

export default function YourAudiosPage() {
  const { items, hasNextPage, isFetching, fetchNextPage } = useYourAudios();

  return (
    <Page title="Your audios" requiresAuth>
      <Heading>Your audios</Heading>
      <AudioList audios={items} />
      <InfiniteListControls
        hasNext={hasNextPage}
        fetchNext={fetchNextPage}
        isFetching={isFetching}
      />
    </Page>
  );
}

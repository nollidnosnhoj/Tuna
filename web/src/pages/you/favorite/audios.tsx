import { Heading } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import InfiniteListControls from "~/components/ui/InfiniteListControls";
import AudioList from "~/features/audio/components/List";
import { useYourFavoriteAudios } from "~/features/auth/hooks";

export default function YourAudiosPage() {
  const { items, hasNextPage, isFetching, fetchNextPage } =
    useYourFavoriteAudios();

  return (
    <Page title="Your Favorite audios" requiresAuth>
      <Heading>Your Favorite Audios</Heading>
      <AudioList audios={items} />
      <InfiniteListControls
        hasNext={hasNextPage}
        fetchNext={fetchNextPage}
        isFetching={isFetching}
      />
    </Page>
  );
}

import { Heading } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import InfiniteListControls from "~/components/ui/ListControls/Infinite";
import AudioList from "~/features/audio/components/List";
import { useYourFavoriteAudios } from "~/features/auth/api/hooks";

export default function YourAudiosPage() {
  const { items, hasNextPage, isFetching, fetchNextPage } =
    useYourFavoriteAudios();

  return (
    <Page title="Your Favorite audios" requiresAuth>
      <Heading>Your Favorite Audios</Heading>
      <AudioList audios={items} context={`me_favorites`} />
      <InfiniteListControls
        hasNext={hasNextPage}
        fetchNext={fetchNextPage}
        isFetching={isFetching}
      />
    </Page>
  );
}

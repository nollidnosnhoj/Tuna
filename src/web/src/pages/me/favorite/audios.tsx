import { Heading, List, ListItem } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import InfiniteListControls from "~/components/ui/ListControls/Infinite";
import { AudioListItem } from "~/components/AudioItem";
import AudioShareButton from "~/components/buttons/Share";
import AudioMiscMenu from "~/components/buttons/Menu";
import { useYourFavoriteAudios } from "~/lib/hooks/api";

export default function YourAudiosPage() {
  const { items, hasNextPage, isFetching, fetchNextPage } =
    useYourFavoriteAudios();

  return (
    <Page title="Your Favorite audios" requiresAuth>
      <Heading>Your Favorite Audios</Heading>
      {items.length === 0 ? (
        <span>No audios found.</span>
      ) : (
        <List>
          {items.map((audio) => (
            <ListItem key={audio.id}>
              <AudioListItem audio={audio}>
                <AudioShareButton audio={audio} />
                <AudioMiscMenu audio={audio} />
              </AudioListItem>
            </ListItem>
          ))}
        </List>
      )}
      <InfiniteListControls
        hasNext={hasNextPage}
        fetchNext={fetchNextPage}
        isFetching={isFetching}
      />
    </Page>
  );
}

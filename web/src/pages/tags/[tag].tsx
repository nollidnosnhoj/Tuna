import React from "react";
import { Heading, List, ListItem } from "@chakra-ui/react";
import InfiniteListControls from "~/components/ui/ListControls/Infinite";
import Page from "~/components/Page";
import { useRouter } from "next/router";
import { AudioListItem } from "~/components/AudioItem";
import AudioShareButton from "~/components/buttons/Share";
import AudioMiscMenu from "~/components/buttons/Menu";
import { useGetTagAudioList } from "~/lib/hooks/api";

export default function TagAudioPage() {
  const router = useRouter();
  const { tag, ...otherParams } = router.query;

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useGetTagAudioList((tag as string) || "", {
    ...otherParams,
  });

  return (
    <Page title={`Showing '${tag}' audios`}>
      {tag && (
        <Heading as="h2" size="lg">
          Showing '{tag}' audios
        </Heading>
      )}
      {audios.length === 0 ? (
        <span>No audios found.</span>
      ) : (
        <List>
          {audios.map((audio) => (
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
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
}

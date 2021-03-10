import { Button } from "@chakra-ui/react";
import React from "react";
import { useAudiosInfinite } from "~/features/audio/hooks/queries";
import AudioListItem from "../../audio/components/List/Item";

interface UserAudioListProps {
  username: string;
}

export default function UserAudioList(
  props: UserAudioListProps & Record<string, any>
) {
  const { username, ...params } = props;
  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudiosInfinite(`users/${username}/audios`, params, 15);

  return (
    <React.Fragment>
      {audios.length === 0 && <p>No uploads.</p>}
      {audios.map((audio, index) => (
        <AudioListItem key={audio.id} audio={audio} listIndex={index} />
      ))}
      {hasNextPage && (
        <Button
          width="100%"
          variant="outline"
          disabled={isFetchingNextPage}
          onClick={() => fetchNextPage()}
        >
          {isFetchingNextPage ? "Loading..." : "Load more"}
        </Button>
      )}
    </React.Fragment>
  );
}

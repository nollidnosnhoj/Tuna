import { chakra } from "@chakra-ui/system";
import React from "react";
import { useGetUserFavoriteAudios } from "~/lib/hooks/api";
import { AudioListItem } from "./AudioItem";
import AudioMiscMenu from "./buttons/Menu";
import AudioShareButton from "./buttons/Share";
import AudioList from "./lists/AudioList";
import InfiniteListControls from "./ui/ListControls/Infinite";

interface FavoriteAudioListProps {
  userName: string;
}

export default function FavoriteAudiosList({
  userName,
}: FavoriteAudioListProps) {
  const { items, hasNextPage, isFetching, fetchNextPage } =
    useGetUserFavoriteAudios(userName);

  return (
    <chakra.div>
      <AudioList
        audios={items}
        customNoAudiosFoundMessage="You have not favorited an audio yet."
      >
        {(audio) => (
          <AudioListItem audio={audio}>
            <AudioShareButton audio={audio} />
            <AudioMiscMenu audio={audio} />
          </AudioListItem>
        )}
      </AudioList>
      <InfiniteListControls
        hasNext={hasNextPage}
        fetchNext={fetchNextPage}
        isFetching={isFetching}
      />
    </chakra.div>
  );
}

import { Box, Divider } from "@chakra-ui/react";
import React, { useCallback, useEffect, useState } from "react";
import useAudioPlayer from "~/hooks/useAudioPlayer";
import { mapToAudioListProps } from "~/utils";
import { useAudiosPagination } from "../../hooks/queries";
import AudioListItem from "./Item";

interface AudioPaginatedListProps {
  queryKey: string;
  queryParams?: Record<string, any>;
}

export default function AudioPaginatedList(props: AudioPaginatedListProps) {
  const { queryKey, queryParams, ...otherProps } = props;
  const { isPlaying, playIndex, startPlay } = useAudioPlayer();

  const [key, setKey] = useState(queryKey);
  const [params, setParams] = useState(queryParams);

  useEffect(() => {
    setKey(queryKey);
  }, [queryKey]);

  useEffect(() => {
    setParams(queryParams);
  }, [queryParams]);

  const {
    items: audios,
    page,
    setPage,
    totalCount,
    totalPages,
    hasNext,
    hasPrevious,
    isFetching,
    isLoading,
    isPreviousData,
  } = useAudiosPagination(key, params, 30);

  const onPlayClick = useCallback(
    (index: number) => {
      startPlay(
        audios.map((a) => mapToAudioListProps(a)),
        index
      );
    },
    [startPlay, audios]
  );

  return (
    <Box>
      {audios.length === 0 && <p>No audio found.</p>}
      {audios.map((audio, index) => (
        <Box marginTop={4} key={index}>
          <AudioListItem
            audio={audio}
            isPlaying={isPlaying && playIndex === index}
            onPlayClick={() => onPlayClick(index)}
          />
          {index !== audios.length - 1 && <Divider />}
        </Box>
      ))}
    </Box>
  );
}

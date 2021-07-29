import {
  Box,
  Flex,
  List,
  ListItem,
  SimpleGrid,
  ButtonGroup,
  IconButton,
  Tooltip,
  chakra,
} from "@chakra-ui/react";
import React, { useCallback, useState } from "react";
import { FaList } from "react-icons/fa";
import { IoMdGrid } from "react-icons/io";
import AudioGridItem from "./GridItem";
import AudioStackMiniItem from "./StackItem";

import { mapAudiosForAudioQueue } from "~/utils/audioplayer";
import { AudioData } from "~/features/audio/types";
import { useAudioPlayer, useAudioQueue } from "~/lib/stores";

type AudioListLayout = "list" | "grid";

type AudioListProps = {
  audios: AudioData[];
  defaultLayout?: AudioListLayout;
  hideLayoutToggle?: boolean;
  notFoundContent?: string | React.ReactNode;
};

export default function AudioList(props: AudioListProps) {
  const {
    audios,
    notFoundContent,
    defaultLayout = "list",
    hideLayoutToggle = false,
  } = props;
  const [isPlaying, setIsPlaying] = useAudioPlayer((state) => [
    state.isPlaying,
    state.setIsPlaying,
  ]);
  const currentAudio = useAudioQueue((state) => state.current);
  const setNewQueue = useAudioQueue((state) => state.setNewQueue);

  const [layout, setLayout] = useState<AudioListLayout>(defaultLayout);

  const isAudioPlaying = useCallback(
    (audio: AudioData) =>
      currentAudio !== undefined && currentAudio?.audioId === audio.id,
    [currentAudio?.queueId]
  );

  const onPlayClick = useCallback(
    (audio: AudioData, index: number) => {
      if (isAudioPlaying(audio)) {
        setIsPlaying(!isPlaying);
      } else {
        setNewQueue(mapAudiosForAudioQueue(audios), index);
      }
    },
    [isAudioPlaying, audios, isPlaying]
  );

  const onLayoutChange = useCallback(
    (type: AudioListLayout) => {
      if (layout === type) return;
      setLayout(type);
    },
    [layout, setLayout]
  );

  return (
    <Box>
      {!hideLayoutToggle && audios.length > 0 && (
        <Flex my={4} justify="flex-end">
          <ButtonGroup size="sm" isAttached variant="outline">
            <Tooltip label="List View" placement="top">
              <IconButton
                onClick={() => onLayoutChange("list")}
                aria-label="List View"
                icon={<FaList />}
                color={layout === "list" ? "primary.500" : undefined}
              />
            </Tooltip>
            <Tooltip label="Grid View" placement="top">
              <IconButton
                onClick={() => onLayoutChange("grid")}
                aria-label="Grid View"
                icon={<IoMdGrid />}
                color={layout === "grid" ? "primary.500" : undefined}
              />
            </Tooltip>
          </ButtonGroup>
        </Flex>
      )}
      <Box>
        {audios.length > 0 ? (
          <React.Fragment>
            {layout === "list" && (
              <List spacing={4}>
                {audios.map((audio, index) => (
                  <ListItem
                    _notLast={{ borderBottomWidth: 1 }}
                    paddingBottom={4}
                    key={index}
                  >
                    <AudioStackMiniItem
                      audio={audio}
                      isPlaying={isAudioPlaying(audio) && isPlaying}
                      onPlayClick={() => onPlayClick(audio, index)}
                    />
                  </ListItem>
                ))}
              </List>
            )}
            {layout === "grid" && (
              <SimpleGrid columns={[2, null, 5]} spacing={4}>
                {audios.map((audio, index) => (
                  <AudioGridItem key={index} audio={audio} />
                ))}
              </SimpleGrid>
            )}
          </React.Fragment>
        ) : (
          notFoundContent || <chakra.span>No audios found.</chakra.span>
        )}
      </Box>
    </Box>
  );
}

import {
  Box,
  Flex,
  List,
  ListItem,
  SimpleGrid,
  ButtonGroup,
  IconButton,
  Tooltip,
} from "@chakra-ui/react";
import React, { useCallback, useState } from "react";
import { FaList } from "react-icons/fa";
import { IoMdGrid } from "react-icons/io";
import AudioGridItem from "./GridItem";
import AudioListItem from "./ListItem";
import { mapAudiosForAudioQueue } from "~/utils/audioplayer";
import { Audio } from "~/features/audio/types";
import { useAudioPlayer } from "~/lib/hooks/useAudioPlayer";

type AudioListLayout = "list" | "grid";

type AudioListProps = {
  audios: Audio[];
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
  const { state, dispatch } = useAudioPlayer();
  const { currentAudio: currentPlaying, isPlaying } = state;

  const [layout, setLayout] = useState<AudioListLayout>(defaultLayout);

  const isAudioPlaying = useCallback(
    (audio: Audio) => !!currentPlaying && currentPlaying.audioId === audio.id,
    [currentPlaying?.queueId]
  );

  const onPlayClick = useCallback(
    (audio: Audio, index: number) => {
      if (isAudioPlaying(audio)) {
        dispatch({ type: "SET_PLAYING", payload: !isPlaying });
      } else {
        dispatch({
          type: "SET_NEW_QUEUE",
          payload: mapAudiosForAudioQueue(audios),
          index: index,
        });
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
      {!hideLayoutToggle && (
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
        {audios.length === 0 && notFoundContent}
        {audios.length > 0 && (
          <React.Fragment>
            {layout === "list" && (
              <List spacing={4}>
                {audios.map((audio, index) => (
                  <ListItem
                    _notLast={{ borderBottomWidth: 1 }}
                    paddingBottom={4}
                    key={index}
                  >
                    <AudioListItem
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
        )}
      </Box>
    </Box>
  );
}

import {
  Box,
  HStack,
  Flex,
  chakra,
  Icon,
  useColorModeValue,
  useDisclosure,
} from "@chakra-ui/react";
import React, { useCallback, useRef } from "react";
import { RiPlayListFill } from "react-icons/ri";
import { REPEAT_MODE } from "~/contexts/AudioPlayerContext";
import AudioQueuePanel from "./AudioQueuePanel";
import NowPlayingSection from "./NowPlayingSection";
import PlayerControls from "./PlayerControls";
import ProgressBar from "./ProgressBar";
import RepeatControl from "./RepeatControl";
import { AudioPlayerItem } from "../types";
import VolumeControl from "./VolumeControl";
import { useAudioPlayer } from "~/contexts/AudioPlayerContext";

interface DesktopAudioPlayerProps {
  audioRef: React.RefObject<HTMLAudioElement>;
  isHidden?: boolean;
}

export default function DesktopAudioPlayer(props: DesktopAudioPlayerProps) {
  const { audioRef, isHidden = false } = props;

  const { state, dispatch } = useAudioPlayer();

  const {
    queue,
    currentPlaying,
    currentTime,
    isPlaying,
    playIndex,
    repeat,
    volume,
  } = state;

  const containerRef = useRef<HTMLDivElement>(null);

  const {
    isOpen: isQueuePanelOpen,
    onClose: onQueuePanelClose,
    onToggle: onQueuePanelToggle,
  } = useDisclosure();

  const playerBackgroundColor = useColorModeValue("gray.100", "gray.800");

  const handleTogglePlay = useCallback(
    (e: React.SyntheticEvent) => {
      e.stopPropagation();
      const audio = audioRef.current;
      if (audio && playIndex !== undefined) {
        dispatch({ type: "TOGGLE_PLAYING" });
      }
    },
    [playIndex]
  );

  const handleKeyDown = useCallback(
    (e: React.KeyboardEvent) => {
      switch (e.keyCode) {
        case 32: // Space
          if (e.target === containerRef.current) {
            e.preventDefault();
            handleTogglePlay(e);
          }
          break;
        case 38: // Up Arrow
          e.preventDefault();
          dispatch({
            type: "SET_VOLUME",
            payload: Math.max(0, Math.min(volume + 5, 100)),
          });
          break;
        case 40: // Down Arrow
          e.preventDefault();
          dispatch({
            type: "SET_VOLUME",
            payload: Math.max(0, Math.min(volume - 5, 100)),
          });
          break;
      }
    },
    [volume, handleTogglePlay]
  );

  if (isHidden) return null;

  return (
    <React.Fragment>
      <AudioQueuePanel isOpen={isQueuePanelOpen} onClose={onQueuePanelClose} />
      <Box
        role="group"
        tabIndex={0}
        ref={containerRef}
        aria-label="Audio Player"
        display="flex"
        alignItems="center"
        pos="fixed"
        bottom={0}
        left={0}
        height="100px"
        boxSizing="border-box"
        lineHeight={1}
        width="100%"
        borderTopWidth={1}
        fontSize="26px"
        onKeyDown={handleKeyDown}
        bgColor={playerBackgroundColor}
      >
        <HStack justifyContent="space-between" width="100%" marginX={4}>
          <Box width="30%">
            <NowPlayingSection current={currentPlaying} />
          </Box>
          <Flex flexDirection="column" alignItems="center" width="500px">
            <ProgressBar
              audioNode={audioRef.current}
              duration={audioRef.current?.duration || currentPlaying?.duration}
            />
            <PlayerControls
              isPlaying={isPlaying}
              hasNoPrevious={playIndex === 0}
              hasNoNext={playIndex === queue.length - 1}
              onTogglePlay={handleTogglePlay}
              onPrevious={() => dispatch({ type: "PLAY_PREVIOUS" })}
              onNext={() => dispatch({ type: "PLAY_NEXT" })}
            />
          </Flex>
          <Flex width="30%" justifyContent="flex-end">
            <Box>
              <HStack>
                <Box width="150px">
                  <VolumeControl
                    audioNode={audioRef.current}
                    volume={volume}
                    onChange={(value) =>
                      dispatch({ type: "SET_VOLUME", payload: value })
                    }
                  />
                </Box>
                <RepeatControl
                  repeat={repeat}
                  onRepeatChange={(value) =>
                    dispatch({ type: "SET_REPEAT", payload: value })
                  }
                />
                <chakra.button
                  onClick={onQueuePanelToggle}
                  aria-label="Toggle Queue Panel"
                  title="Toggle Queue Panel"
                >
                  <Icon as={RiPlayListFill} />
                </chakra.button>
              </HStack>
            </Box>
          </Flex>
        </HStack>
      </Box>
    </React.Fragment>
  );
}

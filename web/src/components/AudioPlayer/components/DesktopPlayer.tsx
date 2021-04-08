import {
  Box,
  HStack,
  Flex,
  chakra,
  Icon,
  useColorModeValue,
  useDisclosure,
} from "@chakra-ui/react";
import React, { useRef } from "react";
import { RiPlayListFill } from "react-icons/ri";
import { REPEAT_MODE } from "~/contexts/AudioPlayerContext";
import AudioQueueDesktop from "./AudioQueue";
import NowPlayingSection from "./NowPlayingSection";
import PlayerControls from "./PlayerControls";
import ProgressBar from "./ProgressBar";
import RepeatControl from "./RepeatControl";
import { AudioPlayerItem } from "../types";
import VolumeControl from "./VolumeControl";

interface DesktopAudioPlayerProps {
  audioRef: React.RefObject<HTMLAudioElement>;
  isPlaying: boolean;
  volume: number;
  repeat: REPEAT_MODE;
  currentTime?: number;
  currentPlaying?: AudioPlayerItem;
  handleSeekChange: (seek: number) => void;
  handleTogglePlay: (e: React.SyntheticEvent) => void;
  handlePrevious: () => void;
  handleNext: () => void;
  handleVolume: (value: number) => void;
  handleRepeat: (value: REPEAT_MODE) => void;
}

export default function DesktopAudioPlayer(props: DesktopAudioPlayerProps) {
  const {
    audioRef,
    isPlaying,
    volume,
    repeat,
    currentTime,
    currentPlaying,
    handleSeekChange,
    handleTogglePlay,
    handlePrevious,
    handleNext,
    handleVolume,
    handleRepeat,
  } = props;

  const containerRef = useRef<HTMLDivElement>(null);

  const {
    isOpen: isQueuePanelOpen,
    onClose: onQueuePanelClose,
    onToggle: onQueuePanelToggle,
  } = useDisclosure();

  const playerBackgroundColor = useColorModeValue("gray.100", "gray.800");

  const handleKeyDown = (e: React.KeyboardEvent) => {
    switch (e.keyCode) {
      case 32: // Space
        if (e.target === containerRef.current) {
          e.preventDefault();
          handleTogglePlay(e);
        }
        break;
      case 38: // Up Arrow
        e.preventDefault();
        handleVolume(volume + 5);
        break;
      case 40: // Down Arrow
        e.preventDefault();
        handleVolume(volume - 5);
        break;
    }
  };

  return (
    <React.Fragment>
      <AudioQueueDesktop
        isOpen={isQueuePanelOpen}
        onClose={onQueuePanelClose}
      />
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
              currentTime={currentTime}
              duration={currentPlaying?.duration}
              onSeekChange={handleSeekChange}
            />
            <PlayerControls
              isPlaying={isPlaying}
              onTogglePlay={handleTogglePlay}
              onPrevious={handlePrevious}
              onNext={handleNext}
            />
          </Flex>
          <Flex width="30%" justifyContent="flex-end">
            <Box>
              <HStack>
                <Box width="100px">
                  <VolumeControl
                    audioNode={audioRef.current}
                    volume={volume}
                    onChange={handleVolume}
                  />
                </Box>
                <RepeatControl repeat={repeat} onRepeatChange={handleRepeat} />
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

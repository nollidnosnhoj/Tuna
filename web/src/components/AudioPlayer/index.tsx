import H5AudioPlayer from "react-h5-audio-player";
import React, { useCallback, useMemo, useRef } from "react";
import useAudioPlayer from "~/hooks/useAudioPlayer";
import { Box } from "@chakra-ui/react";
import { REPEAT_MODE } from "~/contexts/audioPlayerContext";
import RepeatButton from "./RepeatButton";

export default function AudioPlayer() {
  const audioPlayerRef = useRef<H5AudioPlayer | null>(null);
  const repeatModeOrder: [string, REPEAT_MODE][] = [
    ["Repeat", REPEAT_MODE.DISABLE],
    ["Repeat One", REPEAT_MODE.REPEAT],
    ["Disable", REPEAT_MODE.REPEAT_SINGLE],
  ];
  const {
    audioList,
    playIndex,
    volume,
    changeVolume,
    repeatMode,
    changeRepeatMode,
    playPrevious,
    playNext,
    changePlaying,
  } = useAudioPlayer();

  const currentAudio = useMemo(() => {
    if (playIndex === undefined) return undefined;
    return audioList[playIndex];
  }, [audioList, playIndex]);

  const repeatTuple = useMemo(() => {
    let tuple = repeatModeOrder.find(([_, x]) => x === repeatMode);
    if (!tuple) tuple = repeatModeOrder[0];
    return tuple;
  }, [repeatMode]);

  const handlePlay = useCallback(() => {
    changePlaying(true);
  }, [changePlaying]);

  const handlePause = useCallback(() => {
    changePlaying(false);
  }, [changePlaying]);

  const handleClickPrevious = useCallback(() => {
    playPrevious();
  }, [playPrevious]);

  const handleClickNext = useCallback(() => {
    playNext();
  }, [playNext]);

  const handleVolumeChange = useCallback(() => {
    const audio = audioPlayerRef.current?.audio.current;
    if (audio) {
      console.log("CHANGING VOLUME STATE", audio.volume);
      changeVolume(audio.volume);
    }
  }, [changeVolume]);

  const handleRepeatModeChange = useCallback(() => {
    const i = repeatModeOrder.findIndex(([_, m]) => m === repeatMode);
    if (i === -1) return;
    let newIndex = i + 1;
    if (newIndex > repeatModeOrder.length - 1) newIndex = 0;
    changeRepeatMode(repeatModeOrder[newIndex][1]);
  }, [repeatMode, changeRepeatMode]);

  return (
    <React.Fragment>
      <Box
        borderTopWidth="1px"
        pos="fixed"
        bottom="0"
        left="0"
        width="100%"
        height="120px"
        zIndex="99"
        paddingY={4}
      >
        <H5AudioPlayer
          ref={audioPlayerRef}
          loop={repeatMode === REPEAT_MODE.REPEAT_SINGLE}
          autoPlayAfterSrcChange={true}
          showSkipControls={true}
          showJumpControls={false}
          customAdditionalControls={[
            <RepeatButton
              label={repeatTuple[0]}
              mode={repeatTuple[1]}
              onClick={handleRepeatModeChange}
            />,
          ]}
          src={currentAudio?.source || ""}
          onClickPrevious={handleClickPrevious}
          onClickNext={handleClickNext}
          volume={volume}
          onVolumeChange={handleVolumeChange}
          onPlay={handlePlay}
          onPause={handlePause}
        />
      </Box>
    </React.Fragment>
  );
}

import H5AudioPlayer from "react-h5-audio-player";
import React, { useCallback, useEffect, useMemo, useRef } from "react";
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
    nowPlaying,
    volume,
    changeVolume,
    repeatMode,
    changeRepeatMode,
    playPrevious,
    playNext,
    changePlaying,
    isPlaying,
  } = useAudioPlayer();

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

  const handleClickNext = useCallback(async () => {
    await playNext();
  }, [playNext]);

  const handleEndAudio = useCallback(async () => {
    if (repeatMode === REPEAT_MODE.REPEAT) {
      await playNext();
    }
  }, [playNext, repeatMode]);

  const handleVolumeChange = useCallback(() => {
    const audio = audioPlayerRef.current?.audio.current;
    if (audio) {
      changeVolume(audio.volume);
    }
  }, [changeVolume]);

  const handleRepeatModeChange = useCallback(() => {
    return new Promise<REPEAT_MODE>((resolve, reject) => {
      let newIndex =
        repeatModeOrder.findIndex(([_, m]) => m === repeatMode) + 1;
      // If it's zero, then before it was -1, which means the repeat mode was not found in list
      if (newIndex === 0) return reject("Cannot find repeat mode.");
      if (newIndex > repeatModeOrder.length - 1) newIndex = 0;
      changeRepeatMode(repeatModeOrder[newIndex][1]);
      return resolve(repeatModeOrder[newIndex][1]);
    });
  }, [repeatMode, changeRepeatMode]);

  useEffect(() => {
    if (audioPlayerRef.current) {
      if (isPlaying) {
        audioPlayerRef.current.playAudioPromise();
      } else {
        audioPlayerRef.current.audio.current?.pause();
      }
    }
  }, [isPlaying]);

  const loop = audioPlayerRef.current?.audio.current?.loop ?? false;

  useEffect(() => {
    const audio = audioPlayerRef.current?.audio.current;
    if (audio) {
      audio.loop = repeatMode === REPEAT_MODE.REPEAT_SINGLE;
    }
  }, [repeatMode]);

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
          loop={loop}
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
          src={nowPlaying?.source}
          onClickPrevious={handleClickPrevious}
          onClickNext={handleClickNext}
          volume={volume}
          onVolumeChange={handleVolumeChange}
          onPlay={handlePlay}
          onPause={handlePause}
          onEnded={handleEndAudio}
          onPlayError={(err) => console.log(err)}
          volumeJumpStep={1}
        />
      </Box>
    </React.Fragment>
  );
}

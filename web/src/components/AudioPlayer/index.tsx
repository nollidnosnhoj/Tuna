import React, { useCallback, useEffect, useRef } from "react";
import DesktopAudioPlayer from "./components/DesktopPlayer";
import { REPEAT_MODE } from "~/contexts/AudioPlayerContext";
import useAudioPlayer from "~/hooks/useAudioPlayer";

interface AudioPlayerProps {
  preload?: "none" | "metadata" | "auto";
}

export default function AudioPlayer(props: AudioPlayerProps) {
  const { preload = "auto" } = props;
  const { state, dispatch } = useAudioPlayer();
  const {
    currentPlaying,
    currentTime,
    isPlaying,
    playIndex,
    repeat,
    volume,
  } = state;
  const audioRef = useRef<HTMLAudioElement>(null);

  const playAudioPromise = () => {
    const playPromise = audioRef.current?.play();
    if (playPromise) {
      playPromise.then(null).catch((err) => {
        console.error(err);
      });
    }
  };

  const handleTogglePlay = useCallback(
    (e: React.SyntheticEvent) => {
      e.stopPropagation();
      const audio = audioRef.current;
      if (audio && playIndex !== undefined) {
        if (!isPlaying) {
          dispatch({ type: "SET_PLAYING", payload: true });
        } else {
          dispatch({ type: "SET_PLAYING", payload: false });
        }
      }
    },
    [playIndex, isPlaying]
  );

  const handlePreviousClick = () => {
    dispatch({ type: "PLAY_PREVIOUS" });
  };

  const handleNextClick = () => {
    dispatch({ type: "PLAY_NEXT" });
  };

  const handleOnEnded = () => {
    if (repeat === REPEAT_MODE.REPEAT) {
      dispatch({ type: "PLAY_NEXT" });
    }
  };

  const handleVolumeChange = (value: number) => {
    dispatch({
      type: "SET_VOLUME",
      payload: Math.max(0, Math.min(value, 100)),
    });
  };

  const handleRepeatModeClick = (value: REPEAT_MODE) => {
    dispatch({ type: "SET_REPEAT", payload: value });
  };

  const handleSeekChange = (time: number) => {
    dispatch({ type: "SET_CURRENT_TIME", payload: time });
  };

  // Register any audio event listeners here
  useEffect(() => {
    const audio = audioRef.current;

    if (audio) {
      audio.addEventListener("ended", handleOnEnded);
    }

    return () => {
      if (audio) {
        audio.removeEventListener("ended", handleOnEnded);
      }
    };
  }, []);

  // Play/pause audio based on the audio player's state
  useEffect(() => {
    const audio = audioRef.current;
    if (audio) {
      if (isPlaying && currentPlaying?.source) {
        playAudioPromise();
      } else {
        audio.pause();
      }
    }
  }, [isPlaying, currentPlaying?.source]);

  return (
    <React.Fragment>
      <audio
        src={currentPlaying?.source}
        ref={audioRef}
        controls={false}
        preload={preload}
        loop={repeat === REPEAT_MODE.REPEAT_SINGLE}
      />
      <DesktopAudioPlayer
        audioRef={audioRef}
        isPlaying={isPlaying}
        volume={volume}
        repeat={repeat}
        currentTime={currentTime}
        currentPlaying={currentPlaying}
        handleSeekChange={handleSeekChange}
        handleTogglePlay={handleTogglePlay}
        handlePrevious={handlePreviousClick}
        handleNext={handleNextClick}
        handleVolume={handleVolumeChange}
        handleRepeat={handleRepeatModeClick}
      />
    </React.Fragment>
  );
}

import React, { useEffect, useRef } from "react";
import DesktopAudioPlayer from "./components/DesktopPlayer";
import { REPEAT_MODE, useAudioPlayer } from "~/contexts/AudioPlayerContext";

interface AudioPlayerProps {
  preload?: "none" | "metadata" | "auto";
}

export default function AudioPlayer(props: AudioPlayerProps) {
  const { preload = "auto" } = props;
  const { state, dispatch } = useAudioPlayer();
  const { currentAudio: currentPlaying, isPlaying, repeat } = state;
  const audioRef = useRef<HTMLAudioElement>(null);

  const playAudioPromise = () => {
    const playPromise = audioRef.current?.play();
    if (playPromise) {
      playPromise.then(null).catch((err) => {
        console.error(err);
      });
    }
  };

  useEffect(() => {
    dispatch({ type: "SET_AUDIO_REF", payload: audioRef.current });
  }, []);

  // Register any audio event listeners here
  useEffect(() => {
    const handleOnEnded = () => {
      if (repeat === REPEAT_MODE.REPEAT) {
        dispatch({ type: "PLAY_NEXT" });
      }
    };
    const audio = audioRef.current;

    if (audio) {
      audio.addEventListener("ended", handleOnEnded);
    }

    return () => {
      if (audio) {
        audio.removeEventListener("ended", handleOnEnded);
      }
    };
  }, [repeat]);

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
      <DesktopAudioPlayer />
    </React.Fragment>
  );
}

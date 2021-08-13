import React, { useEffect, useRef } from "react";
import { useAudioQueue } from "~/lib/stores";
import { useAudioPlayer } from "~/lib/stores/useAudioPlayer";
import { REPEAT_MODE } from "~/lib/stores/useAudioQueue";
import DesktopAudioPlayer from "./DesktopPlayer";

interface AudioPlayerProps {
  preload?: "none" | "metadata" | "auto";
}

export default function AudioPlayer(props: AudioPlayerProps) {
  const { preload = "auto" } = props;
  const audioRef = useRef<HTMLAudioElement>(null);
  const currentAudio = useAudioQueue((state) => state.current);
  const playNext = useAudioQueue((state) => state.playNext);
  const repeat = useAudioQueue((state) => state.repeat);
  const isPlaying = useAudioPlayer((state) => state.isPlaying);
  const setAudioRef = useAudioPlayer((state) => state.setAudioRef);

  const playAudioPromise = () => {
    const playPromise = audioRef.current?.play();
    if (playPromise) {
      playPromise.then(null).catch((err) => {
        console.error(err);
      });
    }
  };

  useEffect(() => {
    setAudioRef(audioRef.current);
  }, []);

  // Register any audio event listeners here
  useEffect(() => {
    const handleOnEnded = () => {
      // Repeat one means to keep repeating the single song
      if (repeat !== REPEAT_MODE.REPEAT_ONE) {
        playNext();
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
      if (isPlaying && currentAudio?.source) {
        playAudioPromise();
      } else {
        audio.pause();
      }
    }
  }, [isPlaying, currentAudio?.source]);

  return (
    <React.Fragment>
      <audio
        src={currentAudio?.source}
        ref={audioRef}
        controls={false}
        preload={preload}
        loop={repeat === REPEAT_MODE.REPEAT_ONE}
      >
        <track kind="captions"></track>
      </audio>
      <DesktopAudioPlayer />
    </React.Fragment>
  );
}

import React, { useEffect, useRef } from "react";
import { REPEAT_MODE, useAudioPlayer } from "~/lib/stores/useAudioPlayer";
import DesktopAudioPlayer from "./DesktopPlayer";

interface AudioPlayerProps {
  preload?: "none" | "metadata" | "auto";
}

export default function AudioPlayer(props: AudioPlayerProps) {
  const { preload = "auto" } = props;
  const { queue, playIndex, isPlaying, repeat, setAudioRef, playNext } =
    useAudioPlayer();
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
    setAudioRef(audioRef.current);
  }, []);

  // Register any audio event listeners here
  useEffect(() => {
    const handleOnEnded = () => {
      if (repeat === REPEAT_MODE.REPEAT) {
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
      if (isPlaying && queue[playIndex]?.source) {
        playAudioPromise();
      } else {
        audio.pause();
      }
    }
  }, [isPlaying, queue[playIndex]?.source]);

  return (
    <React.Fragment>
      <audio
        src={queue[playIndex]?.source}
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

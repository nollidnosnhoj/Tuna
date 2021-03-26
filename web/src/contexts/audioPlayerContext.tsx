import React, {
  createContext,
  PropsWithChildren,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";
import { useMutex } from "react-context-mutex";
import { AudioPlayerItem } from "~/features/audio/types";
import useAudioQueue from "~/hooks/useAudioQueue";

export enum REPEAT_MODE {
  DISABLE = "disable",
  REPEAT = "repeat",
  REPEAT_SINGLE = "repeat-one",
}

type AudioPlayerContexType = {
  nowPlaying: AudioPlayerItem | undefined;
  isPlaying: boolean;
  volume: number;
  repeatMode: REPEAT_MODE;
  playPrevious: () => Promise<void>;
  playNext: () => Promise<void>;
  changePlaying: (state?: boolean) => void;
  changeVolume: (volumeLevel: number) => void;
  changeRepeatMode: (mode: REPEAT_MODE) => void;
};

export const AudioPlayerContext = createContext<AudioPlayerContexType>(
  {} as AudioPlayerContexType
);

export default function AudioPlayerProvider(props: PropsWithChildren<any>) {
  const { audioList, playIndex, setNewQueue, goToIndex } = useAudioQueue();
  const [repeatMode, setRepeatMode] = useState<REPEAT_MODE>(
    REPEAT_MODE.DISABLE
  );
  const [isPlaying, setIsPlaying] = useState(false);
  const [volume, setVolume] = useState<number>(() => {
    if (typeof window !== "undefined") {
      return (
        parseFloat(window.localStorage.getItem("playerVolume") || "0.5") || 0.5
      );
    }

    return 0.5;
  });

  const nowPlaying = useMemo(() => {
    if (playIndex === undefined) return undefined;
    return audioList[playIndex];
  }, [audioList, playIndex]);

  const changeVolume = (level: number) => {
    setVolume(level);
    if (typeof window !== "undefined") {
      localStorage.setItem("playerVolume", level + "");
    }
  };

  const changePlayingState = (state?: boolean) => {
    if (state === true) {
      setIsPlaying(() => true);
    } else if (state === false) {
      setIsPlaying(() => false);
    } else {
      setIsPlaying((prev) => !prev);
    }
  };

  const playPrevious = useCallback(async () => {
    if (playIndex === undefined) return;
    let newIndex = Math.max(0, Math.min(audioList.length - 1, playIndex - 1));
    await goToIndex(newIndex);
  }, [audioList, goToIndex, playIndex]);

  const playNext = useCallback(async () => {
    if (playIndex === undefined) return;
    let newIndex = playIndex + 1;

    if (newIndex > audioList.length - 1) {
      newIndex = repeatMode === REPEAT_MODE.REPEAT ? 0 : audioList.length - 1;
    }

    await goToIndex(newIndex);
  }, [audioList, playIndex, goToIndex]);

  const changeRepeat = (mode: REPEAT_MODE) => {
    setRepeatMode(mode);
  };

  useEffect(() => {
    window.localStorage.setItem("playerVolume", volume + "");
  }, [volume]);

  const values: AudioPlayerContexType = useMemo(
    () => ({
      nowPlaying: nowPlaying,
      isPlaying: isPlaying,
      volume: volume,
      repeatMode: repeatMode,
      playPrevious: playPrevious,
      playNext: playNext,
      changePlaying: changePlayingState,
      changeVolume: changeVolume,
      changeRepeatMode: changeRepeat,
    }),
    [
      isPlaying,
      volume,
      repeatMode,
      playPrevious,
      playNext,
      changePlayingState,
      changeVolume,
      changeRepeat,
      nowPlaying,
    ]
  );

  return (
    <AudioPlayerContext.Provider value={values}>
      {props.children}
    </AudioPlayerContext.Provider>
  );
}

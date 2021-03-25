import React, {
  createContext,
  PropsWithChildren,
  useEffect,
  useMemo,
  useState,
} from "react";
import { AudioPlayerItem } from "~/features/audio/types";

export enum REPEAT_MODE {
  DISABLE,
  REPEAT,
  REPEAT_SINGLE,
}

type AudioPlayerContextType = {
  isPlaying: boolean;
  audioList: AudioPlayerItem[];
  playIndex: number | undefined;
  volume: number;
  repeatMode: REPEAT_MODE;
  addToQueue: (item: AudioPlayerItem) => void;
  clearQueue: () => void;
  startPlay: (list: AudioPlayerItem[], index: number) => void;
  playPrevious: () => void;
  playNext: () => void;
  changePlaying: (state?: boolean) => void;
  changeVolume: (level: number) => void;
  changePlayIndex: (index: number) => void;
  changeRepeatMode: (mode: REPEAT_MODE) => void;
};

export const AudioPlayerContext = createContext<AudioPlayerContextType>(
  {} as AudioPlayerContextType
);

export default function AudioPlayerProvider(props: PropsWithChildren<any>) {
  const [repeatMode, setRepeatMode] = useState<REPEAT_MODE>(
    REPEAT_MODE.DISABLE
  );
  const [audioList, setAudioList] = useState<AudioPlayerItem[]>([]);
  const [isPlaying, setIsPlaying] = useState(false);
  const [playIndex, setPlayIndex] = useState<number | undefined>(undefined);
  const [volume, setVolume] = useState<number>(() => {
    if (typeof window !== "undefined") {
      return (
        parseInt(window.localStorage.getItem("playerVolume") || "0.5") || 0.5
      );
    }

    return 0.5;
  });

  const startPlay = (list: AudioPlayerItem[], index: number = 0) => {
    setAudioList(() => list.slice(index, Math.min(list.length, 50)));
    setPlayIndex(0);
  };

  const addToQueue = (item: AudioPlayerItem) => {
    if (audioList.some((x) => x.audioId == item.audioId)) return;
    setAudioList((previousList) => [...previousList, item]);
  };

  const changeVolume = (level: number) => {
    setVolume(level);
    if (typeof window !== "undefined") {
      localStorage.setItem("playerVolume", level + "");
    }
  };

  const clearQueue = () => {
    if (isPlaying === true && playIndex && playIndex > 0) {
      const index = playIndex;
      setPlayIndex(0);
      setAudioList((prev) => prev.filter((_, i) => i === index));
    } else {
      setAudioList([]);
      setPlayIndex(undefined);
      changePlayingState(false);
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

  const playPrevious = () => {
    if (playIndex === undefined) return;
    let newIndex = Math.max(0, Math.min(audioList.length - 1, playIndex - 1));
    setPlayIndex(newIndex);
  };

  const playNext = () => {
    if (playIndex === undefined) return;
    let newIndex: number | undefined = playIndex;
    // When repeat mode is repeated, go to the next index. If the index reaches the end, loop back to zero.
    if (repeatMode === REPEAT_MODE.REPEAT) {
      newIndex = Math.max(0, playIndex + 1);
      if (newIndex > audioList.length - 1) {
        newIndex = 0;
      }
      // When repeat mode is disabled, the playIndex is kept at zero, but the list is shifted to the left
    } else if (repeatMode === REPEAT_MODE.DISABLE) {
      newIndex = 0;
    }

    setPlayIndex(newIndex);

    // If the repeat mode is disabled, then remove the previously played track from the list.
    if (repeatMode === REPEAT_MODE.DISABLE) {
      setAudioList((prev) => prev.slice(1));
    }
  };

  const changePlayIndex = (index: number) => {
    if (playIndex === undefined) return;
    let newIndex: number | undefined = Math.max(
      0,
      Math.min(audioList.length - 1, index)
    );

    setPlayIndex(newIndex);

    if (repeatMode === REPEAT_MODE.REPEAT_SINGLE) {
      setAudioList((prev) => prev.slice(newIndex));
    }
  };

  const changeRepeat = (mode: REPEAT_MODE) => {
    setRepeatMode(mode);
  };

  // If there's nothing in the list, play index should be undefined
  useEffect(() => {
    if (audioList.length === 0) {
      setPlayIndex(undefined);
    }
  }, [audioList.length, setPlayIndex]);

  const values: AudioPlayerContextType = useMemo(
    () => ({
      isPlaying: isPlaying,
      audioList: audioList,
      playIndex: playIndex,
      volume: volume,
      repeatMode: repeatMode,
      addToQueue: addToQueue,
      clearQueue: clearQueue,
      startPlay: startPlay,
      playPrevious: playPrevious,
      playNext: playNext,
      changePlaying: changePlayingState,
      changeVolume: changeVolume,
      changePlayIndex: changePlayIndex,
      changeRepeatMode: changeRepeat,
    }),
    [
      isPlaying,
      audioList,
      playIndex,
      volume,
      repeatMode,
      addToQueue,
      clearQueue,
      startPlay,
      playPrevious,
      playNext,
      changePlayingState,
      changeVolume,
      changePlayIndex,
      changeRepeat,
    ]
  );

  return (
    <AudioPlayerContext.Provider value={values}>
      {props.children}
    </AudioPlayerContext.Provider>
  );
}

import React, {
  createContext,
  PropsWithChildren,
  useMemo,
  useState,
} from "react";
import { AudioPlayerItem } from "~/features/audio/types";

type AudioPlayerContextType = {
  isPlaying: boolean;
  audioList: AudioPlayerItem[];
  clearPriorAudioList: boolean;
  playIndex: number | undefined;
  volume: number;
  addToQueue: (item: AudioPlayerItem) => void;
  clearQueue: () => void;
  startPlay: (list: AudioPlayerItem[], index: number) => void;
  setPlaying: (state?: boolean) => void;
  syncQueue: (list: AudioPlayerItem[]) => void;
  volumeChange: (level: number) => void;
  setPlayIndex: (index: number) => void;
  playPrevious: () => void;
  playNext: () => void;
};

export const AudioPlayerContext = createContext<AudioPlayerContextType>(
  {} as AudioPlayerContextType
);

export default function AudioPlayerProvider(props: PropsWithChildren<any>) {
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
  const [clearPriorAudioList, setClearPriorAudioList] = useState(true);

  const startPlay = (list: AudioPlayerItem[], index: number = 0) => {
    setAudioList(() => list.slice(index, Math.min(list.length, 50)));
    setPlayIndex(0);
    setClearPriorAudioList(true);
  };

  const addToQueue = (item: AudioPlayerItem) => {
    if (audioList.some((x) => x.audioId == item.audioId)) return;
    setAudioList((previousList) => [...previousList, item]);
    setClearPriorAudioList(false);
    setPlayIndex(undefined);
  };

  const volumeChange = (level: number) => {
    setVolume(level);
    if (typeof window !== "undefined") {
      localStorage.setItem("playerVolume", level + "");
    }
  };

  const clearQueue = () => {
    setAudioList([]);
    setPlayIndex(undefined);
    setPlaying(false);
  };

  const syncQueue = (list: AudioPlayerItem[]) => {
    setAudioList(list);
  };

  const setPlaying = (state?: boolean) => {
    if (state === true) {
      setIsPlaying(() => true);
    } else if (state === false) {
      setIsPlaying(() => false);
    } else {
      setIsPlaying((prev) => !prev);
    }
  };

  const setIndex = (index: number) => {
    console.log(index);
    setPlayIndex(() => Math.max(0, Math.min(index, audioList.length - 1)));
  };

  const playPrevious = () => {
    setPlayIndex((prev) => Math.max(0, (prev ?? 0) - 1));
  };

  const playNext = () => {
    setPlayIndex((prev) => Math.min(audioList.length, (prev ?? 0) + 1));
  };

  const values: AudioPlayerContextType = useMemo(
    () => ({
      isPlaying: isPlaying,
      audioList: audioList,
      clearPriorAudioList: clearPriorAudioList,
      playIndex: playIndex,
      volume: volume,
      addToQueue: addToQueue,
      clearQueue: clearQueue,
      startPlay: startPlay,
      syncQueue: syncQueue,
      volumeChange: volumeChange,
      setPlaying: setPlaying,
      setPlayIndex: setIndex,
      playPrevious: playPrevious,
      playNext: playNext,
    }),
    [
      isPlaying,
      audioList,
      clearPriorAudioList,
      playIndex,
      clearQueue,
      syncQueue,
      volume,
      volumeChange,
      startPlay,
      addToQueue,
      setPlaying,
      setIndex,
      playPrevious,
      playNext,
    ]
  );

  return (
    <AudioPlayerContext.Provider value={values}>
      {props.children}
    </AudioPlayerContext.Provider>
  );
}

import React, {
  createContext,
  PropsWithChildren,
  useMemo,
  useState,
} from "react";
import { AudioPlayerListItem } from "~/features/audio/types";

type AudioPlayerContextType = {
  isPlaying: boolean;
  audioList: AudioPlayerListItem[];
  clearPriorAudioList: boolean;
  playIndex: number | undefined;
  volume: number;
  addToQueue: (item: AudioPlayerListItem) => void;
  clearQueue: () => void;
  startPlay: (list: AudioPlayerListItem[], index: number) => void;
  setPlaying: (state?: boolean) => void;
  syncQueue: (list: AudioPlayerListItem[]) => void;
  volumeChange: (level: number) => void;
};

export const AudioPlayerContext = createContext<AudioPlayerContextType>(
  {} as AudioPlayerContextType
);

export default function AudioPlayerProvider(props: PropsWithChildren<any>) {
  const [audioList, setAudioList] = useState<AudioPlayerListItem[]>([]);
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

  const startPlay = (list: AudioPlayerListItem[], index: number = 0) => {
    setAudioList(list);
    setPlayIndex(index);
    setClearPriorAudioList(true);
  };

  const addToQueue = (item: AudioPlayerListItem) => {
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

  const syncQueue = (list: AudioPlayerListItem[]) => {
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
    ]
  );

  return (
    <AudioPlayerContext.Provider value={values}>
      {props.children}
    </AudioPlayerContext.Provider>
  );
}

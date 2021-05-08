import React, { useContext } from "react";
import { AudioPlayerItem } from "~/components/AudioPlayer/types";
import { AudioPlayerAction } from "./actions/audioPlayerActions";

export enum REPEAT_MODE {
  DISABLE = "disable",
  REPEAT = "repeat",
  REPEAT_SINGLE = "repeat-one",
}

export interface AudioPlayerState {
  audioRef: HTMLAudioElement | null;
  currentAudio?: AudioPlayerItem;
  queue: AudioPlayerItem[];
  playIndex?: number;
  isPlaying: boolean;
  repeat: REPEAT_MODE;
  currentTime?: number;
  volume: number;
}

export const defaultState: AudioPlayerState = {
  audioRef: null,
  currentAudio: undefined,
  queue: [],
  playIndex: undefined,
  isPlaying: false,
  repeat: REPEAT_MODE.DISABLE,
  currentTime: undefined,
  volume: 25,
};

type AudioPlayerContextType = {
  state: AudioPlayerState;
  dispatch: React.Dispatch<AudioPlayerAction>;
};

export const AudioPlayerContext = React.createContext<AudioPlayerContextType>(
  {} as AudioPlayerContextType
);

export const useAudioPlayer = () => {
  const context = useContext(AudioPlayerContext);
  if (!context) throw new Error("Cannot find AudioPlayerContext.");
  return context;
};

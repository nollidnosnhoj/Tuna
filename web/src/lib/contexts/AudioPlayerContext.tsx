import React, { useContext } from "react";
import { AudioPlayerAction } from "./actions/audioPlayerActions";
import { REPEAT_MODE } from "./types";
import { AudioPlayerItem } from "~/lib/contexts/types";

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

import create from "zustand";

type AudioPlayerState = {
  audioRef: HTMLAudioElement | null;
  playIndex?: number;
  isPlaying: boolean;
  currentTime?: number;
  volume: number;
  setAudioRef: (ref: HTMLAudioElement | null) => void;
  setCurrentTime: (time: number) => void;
  setIsPlaying: (isPlaying: boolean) => void;
  setVolume: (volume: number) => void;
  togglePlaying: () => void;
};

export const useAudioPlayer = create<AudioPlayerState>((set) => ({
  audioRef: null,
  playIndex: 0,
  isPlaying: false,
  currentTime: undefined,
  volume: 25,
  setAudioRef: (ref) =>
    set({
      audioRef: ref,
    }),
  setCurrentTime: (time) =>
    set({
      currentTime: time,
    }),
  setIsPlaying: (isPlaying) =>
    set({
      isPlaying: isPlaying,
    }),
  setVolume: (volume) =>
    set({
      volume: volume,
    }),
  togglePlaying: () =>
    set((state) => ({
      isPlaying: !state.isPlaying,
    })),
}));

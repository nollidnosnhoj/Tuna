import create from "zustand";

export enum REPEAT_MODE {
  DISABLE = "disable",
  REPEAT = "repeat",
  REPEAT_ONE = "repeat_one",
}

export type AudioPlayerItem = {
  queueId: string;
  audioId: string;
  title: string;
  artist: string;
  artistId: string;
  duration: number;
  cover: string;
  source?: string;
  privateKey?: string;
  related: boolean;
};

type AudioPlayerState = {
  audioRef: HTMLAudioElement | null;
  queue: AudioPlayerItem[];
  playIndex: number;
  isPlaying: boolean;
  repeat: REPEAT_MODE;
  currentTime?: number;
  volume: number;
  addToQueue: (audios: AudioPlayerItem[]) => void;
  clearQueue: () => void;
  playNext: () => void;
  playPrevious: () => void;
  removeFromQueueByAudioId: (id: string) => void;
  removeFromQueueByPlayIndex: (index: number) => void;
  setAudioRef: (ref: HTMLAudioElement | null) => void;
  setCurrentTime: (time: number) => void;
  setIsPlaying: (isPlaying: boolean) => void;
  setNewQueue: (queue: AudioPlayerItem[], defaultIndex?: number) => void;
  setPlayIndex: (index: number) => void;
  setRepeatMode: (mode: REPEAT_MODE) => void;
  setVolume: (volume: number) => void;
  togglePlaying: () => void;
};

export const useAudioPlayer = create<AudioPlayerState>((set, get) => ({
  audioRef: null,
  queue: [],
  playIndex: 0,
  isPlaying: false,
  repeat: REPEAT_MODE.DISABLE,
  currentTime: undefined,
  volume: 25,
  addToQueue: (audios) =>
    set((state) => ({
      queue: [...state.queue, ...audios],
    })),
  clearQueue: () =>
    set({
      queue: [],
      playIndex: 0,
    }),
  playNext: () => {
    const { playIndex, queue, repeat } = get();
    if (playIndex === undefined) return set({});
    let newIndex = playIndex + 1;
    if (newIndex > queue.length - 1) {
      newIndex = repeat === REPEAT_MODE.REPEAT ? 0 : queue.length - 1;
    }
    return set({
      isPlaying: true,
      playIndex: newIndex,
    });
  },
  playPrevious: () => {
    const { playIndex, queue } = get();
    if (playIndex === undefined) return set({});
    const newIndex = Math.max(0, Math.min(queue.length - 1, playIndex - 1));
    return set({
      isPlaying: true,
      playIndex: newIndex,
    });
  },
  removeFromQueueByAudioId: (id) => {
    const { queue, playIndex } = get();
    const newState = {};
    const filtered = queue.filter((x) => x.audioId !== id);
    if (queue[playIndex]?.audioId === id) {
      Object.assign(newState, {
        isPlaying: false,
        playIndex: 0,
      });
    } else {
      const newPlayIndex = filtered.findIndex(
        (x) => x.queueId === queue[playIndex]?.queueId
      );

      Object.assign(newState, {
        playIndex: newPlayIndex,
      });
    }
    return set({
      ...newState,
      queue: filtered,
    });
  },
  removeFromQueueByPlayIndex: (index) => {
    const { queue, playIndex } = get();
    let newPlayIndex = playIndex;
    if (playIndex !== undefined && index < playIndex) {
      newPlayIndex = playIndex - 1;
    }
    const newQueue = [...queue].filter((_, i) => i !== index);

    if (newQueue.length === 0) {
      newPlayIndex = 0;
    }
    return set({
      queue: newQueue,
      playIndex: newPlayIndex,
    });
  },
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
  setNewQueue: (queue, defaultIndex = 0) => {
    if (queue.length === 0) return set({});
    return set({
      queue: queue,
      playIndex: defaultIndex,
      isPlaying: true,
    });
  },
  setPlayIndex: (index) => {
    return set((state) => ({
      isPlaying: true,
      playIndex: Math.max(0, Math.min(index, state.queue.length - 1)),
    }));
  },
  setRepeatMode: (mode) =>
    set({
      repeat: mode,
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

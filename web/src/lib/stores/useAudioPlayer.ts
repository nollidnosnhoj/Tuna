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
  source: string;
  related: boolean;
};

type AudioPlayerState = {
  audioRef: HTMLAudioElement | null;
  queue: AudioPlayerItem[];
  currentAudio?: AudioPlayerItem;
  playIndex?: number;
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
      currentAudio: undefined,
    }),
  playNext: () => {
    const { playIndex, queue, repeat } = get();
    let newIndex = (playIndex ?? 0) + 1;
    if (newIndex > queue.length - 1) {
      newIndex = repeat === REPEAT_MODE.REPEAT ? 0 : queue.length - 1;
    }
    return set((state) => ({
      isPlaying: true,
      playIndex: newIndex,
      currentAudio: state.queue[newIndex],
    }));
  },
  playPrevious: () => {
    const { playIndex, queue } = get();
    const newIndex = Math.max(
      0,
      Math.min(queue.length - 1, (playIndex ?? queue.length - 1) - 1)
    );
    return set((state) => ({
      isPlaying: true,
      playIndex: newIndex,
      currentAudio: state.queue[newIndex],
    }));
  },
  removeFromQueueByAudioId: (id) => {
    const { queue, currentAudio } = get();
    const newQueue = queue.filter((x) => x.audioId !== id);
    if (currentAudio?.audioId === id) {
      return set({
        queue: newQueue,
        playIndex: undefined,
        isPlaying: false,
        currentAudio: undefined,
      });
    }
    const newPlayIndex = newQueue.findIndex(
      (x) => x.queueId === currentAudio?.queueId
    );
    return set({
      queue: newQueue,
      playIndex: newPlayIndex,
      currentAudio: newQueue[newPlayIndex],
    });
  },
  removeFromQueueByPlayIndex: (index) => {
    const { queue, currentAudio } = get();
    const currentAudioQueueId = currentAudio?.queueId;

    const newQueue = [...queue];
    newQueue.splice(index, 1);

    if (currentAudioQueueId !== undefined) {
      const newPlayIndex = newQueue.findIndex(
        (x) => x.queueId === currentAudioQueueId
      );

      if (newPlayIndex > -1) {
        return set({
          queue: newQueue,
          playIndex: newPlayIndex,
          currentAudio: newQueue[newPlayIndex],
        });
      }
    }

    return set({
      queue: newQueue,
      isPlaying: false,
      playIndex: undefined,
      currentAudio: undefined,
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
    if (queue.length > 0) {
      return set({
        queue: queue,
        currentAudio: queue[defaultIndex],
        playIndex: defaultIndex,
        isPlaying: true,
      });
    }
  },
  setPlayIndex: (index) => {
    const { queue } = get();
    const newPlayIndex = Math.max(0, Math.min(index, queue.length - 1));
    return set({
      isPlaying: true,
      playIndex: newPlayIndex,
      currentAudio: queue[newPlayIndex],
    });
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

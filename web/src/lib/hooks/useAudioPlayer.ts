import create, { StateCreator, SetState, GetState, StoreApi } from "zustand";

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
  currentAudio?: AudioPlayerItem;
  queue: AudioPlayerItem[];
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

const updateCurrentAudioMiddleware =
  <T extends AudioPlayerState>(config: StateCreator<T>) =>
  (set: SetState<T>, get: GetState<T>, api: StoreApi<T>): T =>
    config(
      (args) => {
        set(args);
        const { playIndex, queue } = get();
        set({
          currentAudio: playIndex === undefined ? undefined : queue[playIndex],
        });
      },
      get,
      api
    );

export const useAudioPlayer = create<AudioPlayerState>(
  updateCurrentAudioMiddleware((set) => ({
    audioRef: null,
    currentAudio: undefined,
    queue: [],
    playIndex: undefined,
    isPlaying: false,
    repeat: REPEAT_MODE.DISABLE,
    currentTime: undefined,
    volume: 25,
    addToQueue: (audios) =>
      set((state) => ({
        ...state,
        queue: [...state.queue, ...audios],
      })),
    clearQueue: () =>
      set((state) => ({
        ...state,
        queue: [],
        playIndex: undefined,
        currentAudio: undefined,
      })),
    playNext: () =>
      set((state) => {
        const { playIndex, queue, repeat } = state;
        if (playIndex === undefined) return state;
        let newIndex = playIndex + 1;
        if (newIndex > queue.length - 1) {
          newIndex = repeat === REPEAT_MODE.REPEAT ? 0 : queue.length - 1;
        }
        return {
          ...state,
          isPlaying: true,
          playIndex: newIndex,
        };
      }),
    playPrevious: () =>
      set((state) => {
        const { playIndex, queue } = state;
        if (playIndex === undefined) return state;
        const newIndex = Math.max(0, Math.min(queue.length - 1, playIndex - 1));
        return {
          ...state,
          isPlaying: true,
          playIndex: newIndex,
        };
      }),
    removeFromQueueByAudioId: (id) =>
      set((state) => {
        const { queue, currentAudio } = state;
        const newState = state;
        const filtered = queue.filter((x) => x.audioId !== id);
        if (currentAudio?.audioId === id) {
          Object.assign(newState, {
            currentAudio: undefined,
            isPlaying: false,
            playIndex: undefined,
          });
        } else {
          const newPlayIndex = filtered.findIndex(
            (x) => x.queueId === currentAudio?.queueId
          );

          Object.assign(newState, {
            playIndex: newPlayIndex,
          });
        }

        return {
          ...newState,
          queue: filtered,
        };
      }),
    removeFromQueueByPlayIndex: (index) =>
      set((state) => {
        const { queue, playIndex } = state;
        let newPlayIndex = playIndex;
        if (playIndex !== undefined && index < playIndex) {
          newPlayIndex = playIndex - 1;
        }
        const newQueue = [...queue].filter((_, i) => i !== index);

        if (newQueue.length === 0) {
          newPlayIndex = undefined;
        }

        return {
          ...state,
          queue: newQueue,
          playIndex: newPlayIndex,
        };
      }),
    setAudioRef: (ref) =>
      set((state) => ({
        ...state,
        audioRef: ref,
      })),
    setCurrentTime: (time) =>
      set((state) => ({
        ...state,
        currentTime: time,
      })),
    setIsPlaying: (isPlaying) =>
      set((state) => ({
        ...state,
        isPlaying: isPlaying,
      })),
    setNewQueue: (queue, defaultIndex) =>
      set((state) => {
        if (queue.length === 0) return state;
        return {
          ...state,
          queue: queue,
          playIndex: defaultIndex,
          isPlaying: true,
        };
      }),
    setPlayIndex: (index) =>
      set((state) => ({
        ...state,
        isPlaying: true,
        playIndex: Math.max(0, Math.min(index, state.queue.length - 1)),
      })),
    setRepeatMode: (mode) =>
      set((state) => ({
        ...state,
        repeat: mode,
      })),
    setVolume: (volume) =>
      set((state) => ({
        ...state,
        volume: volume,
      })),
    togglePlaying: () =>
      set((state) => ({
        ...state,
        isPlaying: !state.isPlaying,
      })),
  }))
);

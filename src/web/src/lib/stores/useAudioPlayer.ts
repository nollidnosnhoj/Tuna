import create from "zustand";
import { v4 as uuidv4 } from "uuid";
import { AudioView } from "~/features/audio/api/types";
import { ID } from "../types";

export enum REPEAT_MODE {
  DISABLE = "disable",
  REPEAT = "repeat",
  REPEAT_ONE = "repeat_one",
}

export type AudioQueueItem = {
  queueId: string;
  audioId: ID;
  title: string;
  artist: string;
  artistId: ID;
  duration: number;
  cover: string;
  source: string;
  related: boolean;
};

type IAudioPlayerSlice = {
  audioRef: HTMLAudioElement | null;
  isPlaying: boolean;
  currentTime?: number;
  volume: number;
  context: string;
  queue: AudioQueueItem[];
  current?: AudioQueueItem;
  playIndex: number;
  repeat: REPEAT_MODE;
  setAudioRef: (ref: HTMLAudioElement | null) => void;
  setCurrentTime: (time: number) => void;
  setIsPlaying: (isPlaying: boolean) => void;
  setVolume: (volume: number) => void;
  togglePlaying: () => void;
  addToQueue: (context: string, audios: AudioView[]) => void;
  clearQueue: (context: string) => void;
  playNext: () => void;
  playPrevious: () => void;
  removeIndexFromQueue: (context: string, index: number) => void;
  removeAudioIdFromQueue: (audioId: ID) => void;
  setNewQueue: (
    context: string,
    queue: AudioView[],
    defaultIndex?: number
  ) => void;
  setPlayIndex: (index: number) => void;
  setRepeatMode: (mode: REPEAT_MODE) => void;
};

export const useAudioPlayer = create<IAudioPlayerSlice>((set, get) => ({
  audioRef: null,
  playIndex: -1,
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
  context: "",
  queue: [],
  repeat: REPEAT_MODE.DISABLE,
  addToQueue: (context, audios) => {
    const { context: currentContext, queue } = get();
    if (context === currentContext) {
      set({
        queue: [...queue, ...mapAudiosForAudioQueue(audios)],
      });
    }
  },
  clearQueue: (context?: string) => {
    const { context: currentContext, queue, current } = get();
    if (context && context !== currentContext) return;
    if (queue.length <= 1) return;
    set({
      queue: current ? [current] : [],
      playIndex: 0,
    });
  },
  playNext: () => {
    const { playIndex, queue, repeat } = get();
    let newIndex = playIndex + 1;
    if (newIndex > queue.length - 1) {
      newIndex = repeat === REPEAT_MODE.REPEAT ? 0 : queue.length - 1;
    }
    set((state) => ({
      isPlaying: true,
      playIndex: newIndex,
      current: state.queue[newIndex],
    }));
  },
  playPrevious: () => {
    const { playIndex, queue } = get();
    const newIndex = Math.max(0, Math.min(queue.length - 1, playIndex - 1));
    set((state) => ({
      isPlaying: true,
      playIndex: newIndex,
      current: state.queue[newIndex],
    }));
  },
  removeIndexFromQueue: (context, index) => {
    const { queue, playIndex, context: currentContext } = get();

    // Ignore removal if the context does not match
    if (currentContext !== context) {
      return;
    }

    // There should always be one item [except initially]
    if (queue.length <= 1) {
      return;
    }

    // Can't remove audio from queue when it's currently playing
    if (playIndex === index) {
      return;
    }

    // Remove from queue
    const newQueue = [...queue];
    newQueue.splice(index, 1);

    // Find the new play index after removal
    let newIdx = playIndex;
    if (playIndex > index) {
      newIdx = playIndex - 1;
    }

    set({
      queue: newQueue,
      playIndex: newIdx,
      current: newQueue[newIdx],
    });
  },
  removeAudioIdFromQueue: (audioId) => {
    const { queue, current } = get();
    const filteredQueue = queue.filter((x) => x.audioId !== audioId);
    const newIndex =
      current?.audioId === audioId
        ? 0
        : queue.findIndex((x) => x.queueId === current?.queueId);
    set({ queue: filteredQueue, playIndex: newIndex });
  },
  setNewQueue: (context, queue, defaultIndex = 0) => {
    if (queue.length > 0) {
      const items = mapAudiosForAudioQueue(queue);
      set({
        context,
        queue: items,
        current: items[defaultIndex],
        playIndex: defaultIndex,
        isPlaying: true,
      });
    }
  },
  setPlayIndex: (index) => {
    const { queue, playIndex } = get();
    if (playIndex === index) return;
    const newPlayIndex = Math.max(0, Math.min(index, queue.length - 1));
    set({
      playIndex: newPlayIndex,
      current: queue[newPlayIndex],
      isPlaying: true,
    });
  },
  setRepeatMode: (mode) =>
    set({
      repeat: mode,
    }),
}));

function mapAudiosForAudioQueue(
  audios: AudioView[],
  isRelatedAudio = false
): AudioQueueItem[] {
  return audios.map((audio) => ({
    queueId: uuidv4(),
    audioId: audio.id,
    title: audio.title,
    artist: audio.user.userName,
    artistId: audio.user.id,
    cover: audio.picture ?? "",
    duration: audio.duration,
    source: audio.audio,
    related: isRelatedAudio,
  }));
}

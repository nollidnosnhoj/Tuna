import create from "zustand";
import { v4 as uuidv4 } from "uuid";
import { AudioView } from "~/features/audio/api/types";
import { ID } from "../types";
import { useAudioPlayer as audioPlayerStore } from "./useAudioPlayer";

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

interface UseAudioQueueState {
  context: string;
  queue: AudioQueueItem[];
  current?: AudioQueueItem;
  playIndex: number;
  repeat: REPEAT_MODE;
  addToQueue: (context: string, audios: AudioView[]) => Promise<void>;
  clearQueue: (context: string) => void;
  playNext: () => void;
  playPrevious: () => void;
  removeIndexFromQueue: (context: string, index: number) => Promise<void>;
  removeAudioIdFromQueue: (audioId: ID) => Promise<void>;
  setNewQueue: (
    context: string,
    queue: AudioView[],
    defaultIndex?: number
  ) => Promise<void>;
  setPlayIndex: (index: number) => void;
  setRepeatMode: (mode: REPEAT_MODE) => void;
}

export const useAudioQueue = create<UseAudioQueueState>((set, get) => ({
  context: "",
  queue: [],
  playIndex: -1,
  repeat: REPEAT_MODE.DISABLE,
  addToQueue: (context, audios) => {
    return new Promise<void>((resolve) => {
      const { context: currentContext, queue } = get();
      if (context === currentContext) {
        set({
          queue: [...queue, ...mapAudiosForAudioQueue(audios)],
        });
      }
      resolve();
      return;
    });
  },
  clearQueue: (context?: string) => {
    const { context: currentContext, queue, current } = get();
    if (context && context !== currentContext) return;
    if (queue.length <= 1) return;
    return set({
      queue: current ? [current] : [],
      playIndex: 0,
    });
  },
  playNext: () => {
    const { playIndex, queue, repeat } = get();
    let newIndex = (playIndex ?? 0) + 1;
    if (newIndex > queue.length - 1) {
      newIndex = repeat === REPEAT_MODE.REPEAT ? 0 : queue.length - 1;
    }
    set((state) => ({
      playIndex: newIndex,
      current: state.queue[newIndex],
    }));
    return audioPlayerStore.setState({ isPlaying: true });
  },
  playPrevious: () => {
    const { playIndex, queue } = get();
    const newIndex = Math.max(0, Math.min(queue.length - 1, playIndex - 1));
    set((state) => ({
      playIndex: newIndex,
      current: state.queue[newIndex],
    }));
    return audioPlayerStore.setState({ isPlaying: true });
  },
  removeIndexFromQueue: (context, index) => {
    return new Promise<void>((resolve) => {
      const { queue, playIndex, context: currentContext } = get();

      // Ignore removal if the context does not match
      if (currentContext !== context) {
        resolve();
        return;
      }

      // There should always be one item [except initially]
      if (queue.length <= 1) {
        resolve();
        return;
      }

      // Remove from queue
      const newQueue = [...queue];
      newQueue.splice(index, 1);

      // Find the new play index after removal
      let newIdx = playIndex;
      if (playIndex === index) {
        newIdx = Math.max(0, Math.min(playIndex, newQueue.length - 1));
      } else if (playIndex > index) {
        newIdx = playIndex - 1;
      }

      set({
        queue: newQueue,
        playIndex: newIdx,
        current: newQueue[newIdx],
      });

      resolve();
      return;
    });
  },
  removeAudioIdFromQueue: (audioId) => {
    return new Promise<void>((resolve) => {
      const { queue, current } = get();
      const filteredQueue = queue.filter((x) => x.audioId !== audioId);
      const newIndex =
        current?.audioId === audioId
          ? 0
          : queue.findIndex((x) => x.queueId === current?.queueId);
      set({ queue: filteredQueue, playIndex: newIndex });
      return resolve();
    });
  },
  setNewQueue: (context, queue, defaultIndex = 0) => {
    return new Promise<void>((resolve) => {
      if (queue.length > 0) {
        const items = mapAudiosForAudioQueue(queue);
        set({
          context,
          queue: items,
          current: items[defaultIndex],
          playIndex: defaultIndex,
        });
        audioPlayerStore.setState({ isPlaying: true });
      }
      resolve();
      return;
    });
  },
  setPlayIndex: (index) => {
    const { queue, playIndex } = get();
    if (playIndex === index) return;
    const newPlayIndex = Math.max(0, Math.min(index, queue.length - 1));
    set({
      playIndex: newPlayIndex,
      current: queue[newPlayIndex],
    });
    return audioPlayerStore.setState({ isPlaying: true });
  },
  setRepeatMode: (mode) =>
    set({
      repeat: mode,
    }),
}));

export function mapAudiosForAudioQueue(
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

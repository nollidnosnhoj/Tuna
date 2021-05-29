import create from "zustand";
import { useAudioPlayer as audioPlayerStore } from "./useAudioPlayer";

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

interface UseAudioQueueState {
  queue: AudioPlayerItem[];
  current?: AudioPlayerItem;
  playIndex?: number;
  repeat: REPEAT_MODE;
  addToQueue: (audios: AudioPlayerItem[]) => void;
  clearQueue: (audioId?: string) => void;
  playNext: () => void;
  playPrevious: () => void;
  removeFromQueue: (index: number) => void;
  setNewQueue: (queue: AudioPlayerItem[], defaultIndex?: number) => void;
  setPlayIndex: (index: number) => void;
  setRepeatMode: (mode: REPEAT_MODE) => void;
}

export const useAudioQueue = create<UseAudioQueueState>((set, get) => ({
  queue: [],
  playIndex: undefined,
  repeat: REPEAT_MODE.DISABLE,
  addToQueue: (audios) =>
    set((state) => ({
      queue: [...state.queue, ...audios],
    })),
  clearQueue: (audioId?: string) => {
    if (audioId) {
      const { queue, current } = get();
      // Create new queue without the specified audio.
      const newQueue = queue.filter((x) => x.audioId !== audioId);

      // If the current audio is the one removed from queue, then just end the audio player.
      if (current?.audioId === audioId) {
        set({
          queue: newQueue,
          playIndex: undefined,
          current: undefined,
        });
        return audioPlayerStore.setState({ isPlaying: false });
      }

      // Find the new index of the current playing audio
      const newPlayIndex = newQueue.findIndex(
        (x) => x.queueId === current?.queueId
      );

      return set({
        queue: newQueue,
        playIndex: newPlayIndex,
        current: newQueue[newPlayIndex],
      });
    }

    set({
      queue: [],
      playIndex: undefined,
      current: undefined,
    });

    return audioPlayerStore.setState({ isPlaying: false });
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
    const newIndex = Math.max(
      0,
      Math.min(queue.length - 1, (playIndex ?? queue.length - 1) - 1)
    );
    set((state) => ({
      playIndex: newIndex,
      current: state.queue[newIndex],
    }));
    return audioPlayerStore.setState({ isPlaying: true });
  },
  removeFromQueue: (index) => {
    const { queue, playIndex } = get();

    const newQueue = [...queue];
    newQueue.splice(index, 1);

    let newIdx: number | undefined = playIndex;

    if (newQueue.length === 0 || playIndex === undefined) {
      newIdx = undefined;
    } else if (playIndex === index) {
      newIdx = Math.max(
        0,
        Math.min(playIndex ?? newQueue.length - 1, newQueue.length - 1)
      );
    } else if (playIndex > index) {
      newIdx = playIndex - 1;
    }

    set({
      queue: newQueue,
      playIndex: newIdx,
      current: newIdx === undefined ? undefined : newQueue[newIdx],
    });

    return audioPlayerStore.setState({ isPlaying: newQueue.length > 0 });
  },
  setNewQueue: (queue, defaultIndex = 0) => {
    if (queue.length > 0) {
      set({
        queue: queue,
        current: queue[defaultIndex],
        playIndex: defaultIndex,
      });
      return audioPlayerStore.setState({ isPlaying: true });
    }
  },
  setPlayIndex: (index) => {
    const { queue } = get();
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

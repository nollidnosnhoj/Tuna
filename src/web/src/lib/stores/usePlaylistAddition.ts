import create from "zustand";

type UsePlaylistAdditionState = {
  playlistIds: number[];
  onAddIds: (ids: number[]) => void;
  onClear: () => void;
};

export const usePlaylistAdditions = create<UsePlaylistAdditionState>((set) => ({
  playlistIds: [],
  onAddIds: (ids) =>
    set((state) => ({
      ...state,
      playlistIds: [...state.playlistIds, ...ids],
    })),
  onClear: () =>
    set((state) => ({
      ...state,
      playlistIds: [],
    })),
}));

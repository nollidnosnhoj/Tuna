import create from "zustand";
import { AudioId, AudioView } from "~/features/audio/api/types";

type UseAddToPlaylistState = {
  open: boolean;
  duplicateOpen: boolean;
  selectedIds: AudioId[];
  duplicateIds: AudioId[];
  defaultPlaylistTitle: string;
  openDialog: (audios: AudioView[]) => void;
  addDups: (ids: AudioId[]) => void;
  closeDialog: () => void;
};

export const useAddToPlaylist = create<UseAddToPlaylistState>((set) => ({
  open: false,
  duplicateOpen: false,
  selectedIds: [],
  duplicateIds: [],
  defaultPlaylistTitle: "",
  addDups: (ids) =>
    set((state) => ({
      ...state,
      duplicateOpen: ids.length > 0,
      duplicateIds: [...ids],
    })),
  openDialog: (audios) =>
    set((state) => ({
      ...state,
      open: audios.length > 0,
      selectedIds: audios.map((a) => a.id),
      defaultPlaylistTitle: audios[0].title,
    })),
  closeDialog: () =>
    set((state) => ({
      ...state,
      open: false,
      duplicateOpen: false,
      duplicateIds: [],
      selectedIds: [],
      defaultPlaylistTitle: "",
    })),
}));

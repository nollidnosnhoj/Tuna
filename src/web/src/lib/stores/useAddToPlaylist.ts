import create from "zustand";
import { AudioData } from "~/features/audio/types";

type UseAddToPlaylistState = {
  open: boolean;
  duplicateOpen: boolean;
  selectedIds: string[];
  duplicateIds: string[];
  defaultPlaylistTitle: string;
  openDialog: (audios: AudioData[]) => void;
  addDups: (ids: string[]) => void;
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

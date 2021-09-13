import create from "zustand";
import { AudioView } from "~/features/audio/api/types";
import { ID } from "../types";

type UseAddToPlaylistState = {
  open: boolean;
  duplicateOpen: boolean;
  selectedIds: ID[];
  duplicateIds: ID[];
  defaultPlaylistTitle: string;
  openDialog: (audios: AudioView[]) => void;
  addDups: (ids: ID[]) => void;
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

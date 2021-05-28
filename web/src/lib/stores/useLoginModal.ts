import create from "zustand";

type UseLoginModalState = {
  modalState: "login" | "register";
  open: boolean;
  onOpen: (modalState: "login" | "register") => void;
  onClose: () => void;
};

export const useLoginModal = create<UseLoginModalState>((set) => ({
  modalState: "login",
  open: false,
  onOpen: (modalState) =>
    set((state) => ({
      ...state,
      modalState: modalState,
      open: true,
    })),
  onClose: () =>
    set((state) => ({
      ...state,
      open: false,
    })),
}));

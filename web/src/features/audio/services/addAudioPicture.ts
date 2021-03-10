import api from "~/utils/api";

export const addAudioPicture = async (id: number, data: string) =>
  await api.patch<{ image: string }>(`audios/${id}/picture`, { data });
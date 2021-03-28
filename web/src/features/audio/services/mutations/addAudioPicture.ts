import api from "~/utils/api";

export async function addAudioPicture(id: number, data: string) {
  return await api.patch<{ image: string; }>(`audios/${id}/picture`, { data });
}
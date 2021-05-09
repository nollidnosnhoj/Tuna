import api from "~/lib/api";

type UploadResponse = {
  audioId: string;
  uploadUrl: string;
};

const getDurationFromAudio = (file: File): Promise<number> => {
  return new Promise<number>((resolve, reject) => {
    const audio = new Audio();
    audio.src = window.URL.createObjectURL(file);
    audio.onloadedmetadata = () => {
      resolve(audio.duration);
    };
    audio.onerror = () => {
      reject("Could not load metadata from audio.");
    };
  });
};

export const getS3PresignedUrl = async (
  file: File
): Promise<UploadResponse> => {
  const duration = await getDurationFromAudio(file);
  const { data } = await api.post<UploadResponse>("upload", {
    fileName: file.name,
    fileSize: file.size,
    duration: duration,
  });
  return data;
};

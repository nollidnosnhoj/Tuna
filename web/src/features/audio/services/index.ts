import api from "~/lib/api";

type UploadResponse = {
  uploadId: string;
  uploadUrl: string;
};

export const getDurationFromAudio = (file: File): Promise<number> => {
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
  const { data } = await api.post<UploadResponse>("upload", {
    fileName: file.name,
    fileSize: file.size,
  });
  return data;
};

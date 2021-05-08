import axios from "axios";
import api from "~/lib/api";

type UploadResponse = {
  audioId: string;
  uploadUrl: string;
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

export const uploadAudioToS3 = (
  s3Url: string,
  userId: string,
  file: File,
  progressCallback: (value: number) => void
): Promise<void> => {
  return new Promise<void>((resolve, reject) => {
    axios
      .put(s3Url, file, {
        headers: {
          "Content-Type": file.type,
          "x-amz-meta-userId": `${userId}`,
          "x-amz-meta-originalFilename": `${file.name}`,
        },
        onUploadProgress: (evt) => {
          const currentProgress = (evt.loaded / evt.total) * 100;
          progressCallback(currentProgress);
        },
      })
      .then(() => {
        resolve();
      })
      .catch(() => {
        reject("Unable to upload audio.");
      });
  });
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

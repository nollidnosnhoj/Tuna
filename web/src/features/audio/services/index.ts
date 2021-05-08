import axios from 'axios'
import { ErrorResponse } from '~/lib/types';
import api from '~/lib/api'
import { isAxiosError } from '~/utils/http';

type UploadResponse = {
  audioId: string;
  uploadUrl: string;
}

export const getS3PresignedUrl = (file: File) => {
  return new Promise<UploadResponse>(async (resolve, reject) => {
    try {
      const duration = await getDurationFromAudio(file);

      const { data } = await api.post<UploadResponse>("upload", {
        fileName: file.name,
        fileSize: file.size,
        duration: duration,
      });

      resolve(data);
    } catch (err) {
      const errorMessage = "Unable to upload audio.";
      if (isAxiosError<ErrorResponse>(err)) {
        reject(err.response?.data.message ?? errorMessage);
      } else {
        reject(errorMessage);
      }
    }
  });
}

export const uploadAudioToS3 = (s3Url: string, userId: string, file: File, progressCallback: (value: number) => void) => {
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
}

export const getDurationFromAudio = (file: File) => {
  return new Promise<number>((resolve, reject) => {
    const audio = new Audio();
    audio.src = window.URL.createObjectURL(file);
    audio.onloadedmetadata = () => {
      resolve(audio.duration);
    };
    audio.onerror = () => {
      reject("Could not load metadata from audio.")
    }
  });
}
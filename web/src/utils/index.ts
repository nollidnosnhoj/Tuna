import { ReactJkMusicPlayerAudioListProps } from "react-jinke-music-player";
import slugify from "slugify";
import api from '~/utils/api'
import { Audio } from "~/features/audio/types";

export const validationMessages = {
  required: function (field: string) {
    return `${field} is required.`;
  },
  min: function (field: string, min: number) {
    return `${field} must be at least ${min} characters long.`;
  },
  max: function (field: string, max: number) {
    return `${field} must be no more than ${max} characters long.`;
  },
};

export function taggify(value: string) {
  return slugify(value, {
    replacement: '-',
    lower: true,
    strict: true
  });
}

export function objectToFormData(values: object): FormData {
  var formData = new FormData();

  Object.entries(values).forEach(([key, value]) => {
    if (value) {
      if (Array.isArray(value)) {
        value.forEach((val) => formData.append(key, val));
      } else if (value instanceof File) {
        formData.append(key, value);
      } else {
        formData.append(key, value.toString());
      }
    }
  });

  return formData;
}

export function mapToAudioListProps(audio: Audio): ReactJkMusicPlayerAudioListProps {
  return {
    audioId: audio.id,
    name: audio.title,
    singer: audio.user.username,
    cover: `https://audiochan-public.s3.amazonaws.com/${audio.picture}`,
    duration: audio.duration,
    musicSrc: () => {
      return new Promise<string>((resolve, reject) => {
        api.get<{ url: string}>(`/audios/${audio.id}/url`)
          .then(({ data }) => {
            resolve(data.url)
          })
          .catch(err => {
            reject(err);
          })
      })
    }
  }
}
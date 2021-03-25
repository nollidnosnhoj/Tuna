import slugify from "slugify";
import { v4 as uuidv4 } from 'uuid'
import { Audio, AudioPlayerItem } from "~/features/audio/types";

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

export function mapToAudioListForPlayer(audios: Audio[], isRelatedAudio: boolean = false): AudioPlayerItem[] {
  return audios.map((audio) => ({
    queueId: uuidv4(),
    audioId: audio.id,
    title: audio.title,
    artist: audio.user.username,
    cover: !!audio.picture ? `https://audiochan-public.s3.amazonaws.com/${audio.picture}` : '',
    duration: audio.duration,
    source: audio.audioUrl,
    related: isRelatedAudio
  }))
}
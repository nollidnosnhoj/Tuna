import slugify from "slugify";
import { Audio, AudioDetail, AudioPlayerItem } from "~/features/audio/types";

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

export function mapToAudioListForPlayer(audios: Audio[]): AudioPlayerItem[] {
  return audios.map(audio => ({
    audioId: audio.id,
    title: audio.title,
    artist: audio.user.username,
    cover: !!audio.picture ? `https://audiochan-public.s3.amazonaws.com/${audio.picture}` : '',
    duration: audio.duration,
    source: audio.audioUrl
  }))
}

export function mapSingleAudioForPlayer(audio: AudioDetail, relatedAudios: Audio[] = []): AudioPlayerItem[] {
  let list: AudioPlayerItem[] = []

  list.push({
    audioId: audio.id,
    title: audio.title,
    artist: audio.user.username,
    cover: !!audio.picture ? `https://audiochan-public.s3.amazonaws.com/${audio.picture}` : '',
    duration: audio.duration,
    source: audio.audioUrl
  });

  if (relatedAudios.length > 0) {
    list = [...list, ...relatedAudios.map(a => ({
      audioId: a.id,
      title: a.title,
      artist: a.user.username,
      cover: !!a.picture ? `https://audiochan-public.s3.amazonaws.com/${audio.picture}` : '',
      duration: a.duration,
      source: a.audioUrl
    }))]
  }

  return list
}
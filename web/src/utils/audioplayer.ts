import { v4 as uuidv4 } from "uuid";
import { AudioDetail, Audio } from "~/features/audio/types";
import { AudioPlayerItem } from "~/lib/contexts/types";

export function mapAudioForAudioQueue(audio: AudioDetail): AudioPlayerItem[] {
  return [
    {
      queueId: uuidv4(),
      audioId: audio.id,
      title: audio.title,
      artist: audio.author.username,
      artistId: audio.author.id,
      cover: audio.picture
        ? `https://audiochan.s3.amazonaws.com/${audio.picture}`
        : "",
      duration: audio.duration,
      privateKey: audio.privateKey,
      source: audio.audioUrl,
      related: false,
    },
  ];
}

export function mapAudiosForAudioQueue(
  audios: Audio[],
  isRelatedAudio = false
): AudioPlayerItem[] {
  return audios.map((audio) => ({
    queueId: uuidv4(),
    audioId: audio.id,
    title: audio.title,
    artist: audio.author.username,
    artistId: audio.author.id,
    cover: audio.picture
      ? `https://audiochan.s3.amazonaws.com/${audio.picture}`
      : "",
    duration: audio.duration,
    source: audio.audioUrl,
    related: isRelatedAudio,
  }));
}

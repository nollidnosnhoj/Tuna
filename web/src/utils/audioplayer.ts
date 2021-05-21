import { v4 as uuidv4 } from "uuid";
import { AudioData } from "~/features/audio/types";
import { AudioPlayerItem } from "~/features/audio/contexts/types";

export function mapAudioForAudioQueue(audio: AudioData): AudioPlayerItem[] {
  return [
    {
      queueId: uuidv4(),
      audioId: audio.id,
      title: audio.title,
      artist: audio.author.username,
      artistId: audio.author.id,
      cover: audio.picture ?? "",
      duration: audio.duration,
      source: audio.audioUrl,
      related: false,
    },
  ];
}

export function mapAudiosForAudioQueue(
  audios: AudioData[],
  isRelatedAudio = false
): AudioPlayerItem[] {
  return audios.map((audio) => ({
    queueId: uuidv4(),
    audioId: audio.id,
    title: audio.title,
    artist: audio.author.username,
    artistId: audio.author.id,
    cover: audio.picture ?? "",
    duration: audio.duration,
    source: audio.audioUrl,
    related: isRelatedAudio,
  }));
}

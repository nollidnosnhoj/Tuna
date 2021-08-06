import { v4 as uuidv4 } from "uuid";
import { AudioView } from "~/features/audio/api/types";
import { AudioPlayerItem } from "~/lib/stores/useAudioQueue";

export function mapAudioForAudioQueue(audio: AudioView): AudioPlayerItem[] {
  return [
    {
      queueId: uuidv4(),
      audioId: audio.id,
      title: audio.title,
      artist: audio.user.username,
      artistId: audio.user.id,
      cover: audio.picture ?? "",
      duration: audio.duration,
      source: audio.audioUrl,
      related: false,
    },
  ];
}

export function mapAudiosForAudioQueue(
  audios: AudioView[],
  isRelatedAudio = false
): AudioPlayerItem[] {
  return audios.map((audio) => ({
    queueId: uuidv4(),
    audioId: audio.id,
    title: audio.title,
    artist: audio.user.username,
    artistId: audio.user.id,
    cover: audio.picture ?? "",
    duration: audio.duration,
    source: audio.audioUrl,
    related: isRelatedAudio,
  }));
}

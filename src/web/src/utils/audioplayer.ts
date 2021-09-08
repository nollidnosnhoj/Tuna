import { v4 as uuidv4 } from "uuid";
import { AudioView } from "~/features/audio/api/types";
import { AudioQueueItem } from "~/lib/stores/useAudioQueue";

export function mapAudioForAudioQueue(audio: AudioView): AudioQueueItem[] {
  return [
    {
      queueId: uuidv4(),
      audioId: audio.id,
      title: audio.title,
      artist: audio.user.userName,
      artistId: audio.user.id,
      cover: audio.picture ?? "",
      duration: audio.duration,
      source: audio.audio,
      related: false,
    },
  ];
}

export function mapAudiosForAudioQueue(
  audios: AudioView[],
  isRelatedAudio = false
): AudioQueueItem[] {
  return audios.map((audio) => ({
    queueId: uuidv4(),
    audioId: audio.id,
    title: audio.title,
    artist: audio.user.userName,
    artistId: audio.user.id,
    cover: audio.picture ?? "",
    duration: audio.duration,
    source: audio.audio,
    related: isRelatedAudio,
  }));
}

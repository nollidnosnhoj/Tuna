export type AudioPlayerItem = {
  queueId: string;
  audioId: number;
  title: string;
  artist: string;
  artistId: string;
  duration: number;
  cover: string;
  source?: string;
  privateKey?: string
  related: boolean;
}
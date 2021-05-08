export enum REPEAT_MODE {
  DISABLE = 'disable',
  REPEAT = 'repeat',
  REPEAT_ONE = 'repeat_one'
}

export type AudioPlayerItem = {
  queueId: string;
  audioId: string;
  title: string;
  artist: string;
  artistId: string;
  duration: number;
  cover: string;
  source?: string;
  privateKey?: string
  related: boolean;
}
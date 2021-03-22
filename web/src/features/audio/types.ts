import { ReactJkMusicPlayerAudioInfo, ReactJkMusicPlayerAudioListProps } from "react-jinke-music-player"
import { Creator } from "~/lib/types"

export type AudioSearchType = 'audios' | 'user' | 'feed'

export type Publicity = 'unlisted' | 'public' | 'private'

export type Audio = {
  id: number;
  title: string;
  publicity: Publicity;
  duration: number;
  picture: string;
  created: string;
  user: Creator;
}

export type AudioDetail = {
  id: number;
  title: string;
  description: string;
  publicity: Publicity;
  privateKey?: string;
  tags: string[];
  duration: number;
  fileSize: number;
  fileExt: string;
  url: string;
  picture: string;
  created: string;
  updated?: string;
  user: Creator;
}

export interface AudioRequest {
  title: string;
  description?: string;
  tags: string[];
  publicity: Publicity;
};

export interface CreateAudioRequest extends AudioRequest {
  uploadId: string;
  fileName: string;
  duration: number;
  fileSize: number;
}

export interface AudioPlayerListItem extends ReactJkMusicPlayerAudioListProps {
  audioId?: number;
}

export interface AudioPlayerItemInfo extends ReactJkMusicPlayerAudioInfo {
  audioId?: number;
}
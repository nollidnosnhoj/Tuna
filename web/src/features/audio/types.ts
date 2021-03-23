import { ReactJkMusicPlayerAudioInfo, ReactJkMusicPlayerAudioListProps } from "react-jinke-music-player"
import { Creator } from "~/lib/types"

export type AudioSearchType = 'audios' | 'user' | 'feed'

export type Visibility = 'unlisted' | 'public' | 'private'

export type Audio = {
  id: number;
  title: string;
  visibility: Visibility;
  duration: number;
  picture: string;
  audioUrl: string;
  created: string;
  user: Creator;
}

export type AudioDetail = {
  id: number;
  title: string;
  description: string;
  visibility: Visibility;
  privateKey?: string;
  tags: string[];
  duration: number;
  fileSize: number;
  fileExt: string;
  audioUrl: string;
  picture: string;
  created: string;
  updated?: string;
  user: Creator;
}

export interface AudioRequest {
  title?: string;
  description?: string;
  tags: string[];
  visibility: Visibility;
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
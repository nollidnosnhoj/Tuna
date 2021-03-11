import { ReactJkMusicPlayerAudioInfo, ReactJkMusicPlayerAudioListProps } from "react-jinke-music-player"
import { Creator, Genre } from "~/lib/types"

export type AudioSearchType = 'audios' | 'favorites' | 'user' | 'feed'

export type Audio = {
  id: number;
  title: string;
  isPublic: boolean;
  duration: number;
  picture: string;
  favoriteCount: number;
  isFavorited: boolean;
  created: string;
  genre?: Genre;
  user: Creator;
}

export type AudioDetail = {
  id: number;
  title: string;
  description: string;
  isPublic: boolean;
  tags: string[];
  duration: number;
  fileSize: number;
  fileExt: string;
  url: string;
  picture: string;
  favoriteCount: number;
  isFavorited: boolean;
  created: string;
  updated?: string;
  genre?: Genre;
  user: Creator;
}

export interface AudioRequest {
  title: string;
  description?: string;
  tags: string[];
  isPublic: boolean;
  genre?: string;
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
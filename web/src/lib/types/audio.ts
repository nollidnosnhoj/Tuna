import { Creator } from "~/lib/types"
import { GenreDto } from "~/lib/types/genre"

export type AudioSearchType = 'audios' | 'favorites' | 'user' | 'feed'

export type AudioDetail = {
  id: string;
  title: string;
  description: string;
  isPublic: boolean;
  isLoop: boolean;
  tags: string[];
  duration: number;
  fileSize: number;
  fileExt: string;
  url: string;
  pictureUrl: string;
  favoriteCount: number;
  isFavorited: boolean;
  created: string;
  updated?: string;
  genre: GenreDto,
  user: Creator;
}

export type AudioListItem = {
  id: string;
  title: string;
  isPublic: boolean;
  favoriteCount: number;
  isFavorited: boolean;
  genre: string;
  created: string;
  updated?: string;
  user: Creator;
}

export interface EditAudioRequest {
  title: string;
  description?: string;
  tags?: string[];
  isPublic: boolean;
  genre: string;
  image?: File;
};

export interface UploadAudioRequest extends EditAudioRequest {
  file: File,
  image?: File,
  acceptTerms: boolean
}
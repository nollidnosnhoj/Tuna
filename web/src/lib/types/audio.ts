import { Creator } from "~/lib/types"
import { GenreDto } from "~/lib/types/genre"

export type AudioSearchType = 'audios' | 'favorites' | 'user' | 'feed'

export type AudioDetail = {
  id: string;
  title: string;
  description: string;
  isPublic: boolean;
  tags: string[];
  duration: number;
  fileSize: number;
  fileExt: string;
  url: string;
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

export interface AudioRequest {
  title?: string;
  description?: string;
  tags?: string[];
  isPublic?: boolean;
  genre?: string;
};
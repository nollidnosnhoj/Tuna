import { MetaAuthor } from "~/lib/types"

export type AudioSearchType = 'audios' | 'user' | 'feed'

export type Visibility = 'unlisted' | 'public' | 'private'

export type Audio = {
  id: string;
  title: string;
  isPublic: boolean;
  duration: number;
  picture?: string;
  uploaded: string;
  audioUrl: string;
  author: MetaAuthor;
}

export type AudioDetail = {
  id: string;
  title: string;
  description?: string;
  isPublic: boolean;
  privateKey?: string;
  tags: string[];
  duration: number;
  fileSize: number;
  fileExt: string;
  picture?: string;
  uploaded: string;
  lastModified?: string;
  audioUrl: string;
  author: MetaAuthor;
}

export interface AudioRequest {
  title: string;
  description?: string;
  tags: string[];
  isPublic?: boolean;
};

export interface CreateAudioRequest extends AudioRequest {
  audioId: string;
}
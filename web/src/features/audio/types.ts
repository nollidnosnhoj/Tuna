import { MetaAuthor } from "~/lib/types"

export type AudioSearchType = 'audios' | 'user' | 'feed'

export type Visibility = 'unlisted' | 'public' | 'private'

export type Audio = {
  id: string;
  title: string;
  visibility: string;
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
  visibility: string;
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
  visibility: string;
};

export interface CreateAudioRequest extends AudioRequest {
  uploadId: string;
  fileName: string;
  duration: number;
  fileSize: number;
}
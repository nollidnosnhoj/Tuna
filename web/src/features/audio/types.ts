import { MetaAuthor } from "~/lib/types"

export type AudioSearchType = 'audios' | 'user' | 'feed'

export type Visibility = 'unlisted' | 'public' | 'private'

export type Audio = {
  id: number;
  title: string;
  visibility: Visibility;
  duration: number;
  picture?: string;
  uploaded: string;
  audioUrl: string;
  author: MetaAuthor;
}

export type AudioDetail = {
  id: number;
  title: string;
  description?: string;
  visibility: Visibility;
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

export type AudioPlayerItem = {
  queueId: string;
  audioId: number;
  title: string;
  artist: string;
  duration: number;
  cover: string;
  source?: string;
  privateKey?: string
  related: boolean;
}
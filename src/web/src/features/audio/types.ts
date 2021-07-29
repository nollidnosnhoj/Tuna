import { MetaAuthor } from "~/lib/types";

export type AudioId = string;

export enum Visibility {
  Public = "public",
  Unlisted = "unlisted",
  Private = "private",
}

export interface AudioData {
  id: string;
  title: string;
  visibility: Visibility;
  duration: number;
  picture?: string;
  created: string;
  audioUrl: string;
  user: MetaAuthor;
}

export interface AudioDetailData extends AudioData {
  description?: string;
  tags: string[];
  size: number;
  lastModified?: string;
}

export interface AudioRequest {
  title: string;
  description?: string;
  tags: string[];
  visibility: Visibility;
}

export interface CreateAudioRequest extends AudioRequest {
  uploadId: string;
  fileName: string;
  fileSize: number;
  duration: number;
}

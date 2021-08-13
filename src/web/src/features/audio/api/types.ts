import { MetaAuthor } from "~/lib/types";

export type AudioId = number;

export enum Visibility {
  Public = "public",
  Unlisted = "unlisted",
  Private = "private",
}

export interface AudioView {
  id: AudioId;
  title: string;
  slug: string;
  description: string;
  visibility: Visibility;
  secret?: string;
  tags: string[];
  duration: number;
  size: number;
  picture?: string;
  created: string;
  lastModified?: string;
  audioUrl: string;
  user: MetaAuthor;
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
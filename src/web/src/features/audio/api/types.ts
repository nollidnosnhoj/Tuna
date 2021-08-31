import { MetaAuthor } from "~/lib/types";

export type AudioId = number;

export interface AudioView {
  id: AudioId;
  title: string;
  slug: string;
  description: string;
  tags: string[];
  duration: number;
  size: number;
  picture?: string;
  isFavorited?: boolean;
  created: string;
  lastModified?: string;
  audioUrl: string;
  user: MetaAuthor;
}

export interface AudioRequest {
  title: string;
  description?: string;
  tags: string[];
}

export interface CreateAudioRequest extends AudioRequest {
  uploadId: string;
  fileName: string;
  fileSize: number;
  duration: number;
}

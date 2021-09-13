import { ID, MetaAuthor } from "~/lib/types";

export interface AudioView {
  id: ID;
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
  audio: string;
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

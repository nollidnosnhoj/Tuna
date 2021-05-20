import { MetaAuthor } from "~/lib/types";

export interface AudioData {
  id: string;
  title: string;
  isPublic: boolean;
  duration: number;
  picture?: string;
  uploaded: string;
  audioUrl: string;
  author: MetaAuthor;
}

export interface AudioDetailData extends AudioData {
  description?: string;
  tags: string[];
  fileSize: number;
  fileExt: string;
  uploaded: string;
  lastModified?: string;
}

export interface AudioRequest {
  title: string;
  description?: string;
  tags: string[];
  isPublic?: boolean;
}

export interface CreateAudioRequest extends AudioRequest {
  uploadId: string;
  fileName: string;
  fileSize: number;
  duration: number;
  contentType: string;
}

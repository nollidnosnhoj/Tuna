import { MetaAuthor } from "~/lib/types";
import { AudioData, Visibility } from "../audio/types";

export interface Playlist {
  id: string;
  title: string;
  description: string;
  picture?: string;
  visibility: Visibility;
  tags: string[];
  audios: AudioData[];
  user: MetaAuthor;
}

export interface PlaylistRequest {
  title: string;
  description?: string;
  tags: string[];
  visibility: Visibility;
}

export interface CreatePlaylistRequest extends PlaylistRequest {
  audioIds: string[];
}

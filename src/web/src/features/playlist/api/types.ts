import { MetaAuthor } from "~/lib/types";
import { AudioId, AudioView } from "../../audio/api/types";

export type PlaylistId = number;
export type PlaylistAudioId = number;

export interface Playlist {
  id: PlaylistId;
  title: string;
  slug: string;
  description: string;
  picture?: string;
  tags: string[];
  audios: AudioView[];
  user: MetaAuthor;
}

export interface PlaylistRequest {
  title: string;
  description?: string;
  tags: string[];
}

export interface CreatePlaylistRequest extends PlaylistRequest {
  audioIds: AudioId[];
}

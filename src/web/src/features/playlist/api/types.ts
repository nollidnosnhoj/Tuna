import { ID, MetaAuthor } from "~/lib/types";
import { AudioView } from "../../audio/api/types";

export interface Playlist {
  id: ID;
  title: string;
  description: string;
  picture?: string;
  tags: string[];
  audios: AudioView[];
  user: MetaAuthor;
}

export interface PlaylistAudio {
  id: number;
  audio: AudioView;
}

export interface PlaylistRequest {
  title: string;
  description?: string;
  tags: string[];
}

export interface CreatePlaylistRequest extends PlaylistRequest {
  audioIds: ID[];
}

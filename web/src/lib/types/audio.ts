export type AudioSearchType = 'audios' | 'favorites' | 'user'
export type Audio = {
  id: string;
  title: string;
  description: string;
  isPublic: boolean;
  tags: string[];
  duration: number;
  fileSize: number;
  fileExt: string;
  url: string;
  favoriteCount: number;
  isFavorited: boolean;
  created: string;
  updated?: string;
  user: Creator;
}

export type AudioListItem = {
  id: string;
  title: string;
  isPublic: boolean;
  favoriteCount: number;
  isFavorited: boolean;
  created: string;
  updated?: string;
  user: Creator;
}

export type Creator = {
  id: number;
  username: string;
  displayName: string;
}

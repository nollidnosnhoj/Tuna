export type CurrentUser = {
  id: number;
  username: string;
  email: string;
  roles?: string[];
}

export type Profile = {
  id: number;
  username: string;
  aboutMe: string;
  audioCount: number;
  isFollowing?: boolean;
  followerCount: number;
  followingCount: number;
  picture: string;
}
export type CurrentUser = {
  id: string;
  username: string;
  email: string;
  roles?: string[];
};

export type Profile = {
  id: string;
  username: string;
  aboutMe: string;
  audioCount: number;
  isFollowing?: boolean;
  followerCount: number;
  followingCount: number;
  picture: string;
};

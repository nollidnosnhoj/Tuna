export type CurrentUser = {
  id: number;
  username: string;
  email: string;
  roles?: string[];
};

export type Profile = {
  id: number;
  username: string;
  audioCount: number;
  followerCount: number;
  followingCount: number;
  picture: string;
};

export type Profile = {
  id: number;
  username: string;
  aboutMe: string;
  audioCount: number;
  isFollowing: boolean;
  followerCount: number;
  followingCount: number;
  avatarUrl?: string;
}
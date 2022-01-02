import { gql } from '@apollo/client';
import * as Apollo from '@apollo/client';
export type Maybe<T> = T | null;
export type InputMaybe<T> = Maybe<T>;
export type Exact<T extends { [key: string]: unknown }> = { [K in keyof T]: T[K] };
export type MakeOptional<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]?: Maybe<T[SubKey]> };
export type MakeMaybe<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]: Maybe<T[SubKey]> };
const defaultOptions =  {}
/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
  ID: string;
  String: string;
  Boolean: boolean;
  Int: number;
  Float: number;
  /** The `DateTime` scalar represents an ISO-8601 compliant date time type. */
  DateTime: any;
  /** The built-in `Decimal` scalar type. */
  Decimal: any;
  /** The `Long` scalar type represents non-fractional signed whole 64-bit numeric values. Long can represent values between -(2^63) and 2^63 - 1. */
  Long: any;
};

export enum ApplyPolicy {
  AfterResolver = 'AFTER_RESOLVER',
  BeforeResolver = 'BEFORE_RESOLVER'
}

export type Audio = Node & {
  __typename?: 'Audio';
  created: Scalars['DateTime'];
  description?: Maybe<Scalars['String']>;
  duration: Scalars['Decimal'];
  favorited?: Maybe<Array<Maybe<User>>>;
  id: Scalars['ID'];
  isFavorited?: Maybe<Scalars['Boolean']>;
  mp3?: Maybe<Scalars['String']>;
  picture?: Maybe<Scalars['String']>;
  size: Scalars['Long'];
  slug: Scalars['String'];
  tags: Array<Scalars['String']>;
  title: Scalars['String'];
  user?: Maybe<User>;
};

export type AudioNotFound = Error & {
  __typename?: 'AudioNotFound';
  audioId: Scalars['Long'];
  code: Scalars['String'];
  message: Scalars['String'];
};

/** A connection to a list of items. */
export type AudiosByTagsConnection = {
  __typename?: 'AudiosByTagsConnection';
  /** A list of edges. */
  edges?: Maybe<Array<AudiosByTagsEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<Audio>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type AudiosByTagsEdge = {
  __typename?: 'AudiosByTagsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String'];
  /** The item at the end of the edge. */
  node: Audio;
};

/** A connection to a list of items. */
export type AudiosByUsernameConnection = {
  __typename?: 'AudiosByUsernameConnection';
  /** A list of edges. */
  edges?: Maybe<Array<AudiosByUsernameEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<Audio>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type AudiosByUsernameEdge = {
  __typename?: 'AudiosByUsernameEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String'];
  /** The item at the end of the edge. */
  node: Audio;
};

export type CreateAudioError = Unauthorized | UploadDoesNotExist | ValidationError;

export type CreateAudioInput = {
  description: Scalars['String'];
  duration: Scalars['Decimal'];
  fileName: Scalars['String'];
  fileSize: Scalars['Long'];
  tags: Array<Scalars['String']>;
  title: Scalars['String'];
  uploadId: Scalars['String'];
};

export type CreateAudioPayload = {
  __typename?: 'CreateAudioPayload';
  audio?: Maybe<Audio>;
  errors?: Maybe<Array<CreateAudioError>>;
};

export type EmailTaken = Error & {
  __typename?: 'EmailTaken';
  code: Scalars['String'];
  email: Scalars['String'];
  message: Scalars['String'];
};

export type Error = {
  code: Scalars['String'];
  message: Scalars['String'];
};

export type FavoriteAudioError = AudioNotFound;

export type FavoriteAudioInput = {
  id: Scalars['ID'];
};

export type FavoriteAudioPayload = {
  __typename?: 'FavoriteAudioPayload';
  errors?: Maybe<Array<FavoriteAudioError>>;
  message?: Maybe<Scalars['String']>;
};

/** A connection to a list of items. */
export type FavoriteAudiosByUserNameConnection = {
  __typename?: 'FavoriteAudiosByUserNameConnection';
  /** A list of edges. */
  edges?: Maybe<Array<FavoriteAudiosByUserNameEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<Audio>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type FavoriteAudiosByUserNameEdge = {
  __typename?: 'FavoriteAudiosByUserNameEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String'];
  /** The item at the end of the edge. */
  node: Audio;
};

export type FollowUserError = Forbidden | UserNotFound;

export type FollowUserInput = {
  targetUserId: Scalars['ID'];
};

export type FollowUserPayload = {
  __typename?: 'FollowUserPayload';
  errors?: Maybe<Array<FollowUserError>>;
  message?: Maybe<Scalars['String']>;
};

/** A connection to a list of items. */
export type FollowersConnection = {
  __typename?: 'FollowersConnection';
  /** A list of edges. */
  edges?: Maybe<Array<FollowersEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<User>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type FollowersEdge = {
  __typename?: 'FollowersEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String'];
  /** The item at the end of the edge. */
  node: User;
};

/** A connection to a list of items. */
export type FollowingsConnection = {
  __typename?: 'FollowingsConnection';
  /** A list of edges. */
  edges?: Maybe<Array<FollowingsEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<User>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type FollowingsEdge = {
  __typename?: 'FollowingsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String'];
  /** The item at the end of the edge. */
  node: User;
};

export type Forbidden = Error & {
  __typename?: 'Forbidden';
  code: Scalars['String'];
  message: Scalars['String'];
};

export type GenerateUploadLinkError = ValidationError;

export type GenerateUploadLinkInput = {
  fileName: Scalars['String'];
  filesize: Scalars['Long'];
};

export type GenerateUploadLinkPayload = {
  __typename?: 'GenerateUploadLinkPayload';
  errors?: Maybe<Array<GenerateUploadLinkError>>;
  response?: Maybe<GenerateUploadLinkResponse>;
};

export type GenerateUploadLinkResponse = {
  __typename?: 'GenerateUploadLinkResponse';
  uploadId: Scalars['String'];
  uploadUrl: Scalars['String'];
};

export type ImageUploadResponse = {
  __typename?: 'ImageUploadResponse';
  url: Scalars['String'];
};

export type LoginError = SignInError;

export type LoginInput = {
  login: Scalars['String'];
  password: Scalars['String'];
};

export type LoginPayload = {
  __typename?: 'LoginPayload';
  errors?: Maybe<Array<LoginError>>;
  user?: Maybe<User>;
};

export type LogoutPayload = {
  __typename?: 'LogoutPayload';
  message?: Maybe<Scalars['String']>;
};

export type Mutation = {
  __typename?: 'Mutation';
  createAudio: CreateAudioPayload;
  favoriteAudio: FavoriteAudioPayload;
  followUser: FollowUserPayload;
  generateUploadLink: GenerateUploadLinkPayload;
  login: LoginPayload;
  logout: LogoutPayload;
  register: RegisterPayload;
  removeAudio: RemoveAudioPayload;
  removeAudioPicture: RemoveAudioPicturePayload;
  removeUserPicture: RemoveUserPicturePayload;
  unfavoriteAudio: UnfavoriteAudioPayload;
  unfollowUser: UnfollowUserPayload;
  updateAudio: UpdateAudioPayload;
  updateAudioPicture: UpdateAudioPicturePayload;
  updatePassword: UpdatePasswordPayload;
  updateProfile: UpdateProfilePayload;
  updateUser: UpdateUserPayload;
  updateUserPicture: UpdateUserPicturePayload;
};


export type MutationCreateAudioArgs = {
  input: CreateAudioInput;
};


export type MutationFavoriteAudioArgs = {
  input: FavoriteAudioInput;
};


export type MutationFollowUserArgs = {
  input: FollowUserInput;
};


export type MutationGenerateUploadLinkArgs = {
  input: GenerateUploadLinkInput;
};


export type MutationLoginArgs = {
  input: LoginInput;
};


export type MutationRegisterArgs = {
  input: RegisterInput;
};


export type MutationRemoveAudioArgs = {
  input: RemoveAudioInput;
};


export type MutationRemoveAudioPictureArgs = {
  input: RemoveAudioPictureInput;
};


export type MutationUnfavoriteAudioArgs = {
  input: UnfavoriteAudioInput;
};


export type MutationUnfollowUserArgs = {
  input: UnfollowUserInput;
};


export type MutationUpdateAudioArgs = {
  input: UpdateAudioInput;
};


export type MutationUpdateAudioPictureArgs = {
  input: UpdateAudioPictureInput;
};


export type MutationUpdatePasswordArgs = {
  input: UpdatePasswordInput;
};


export type MutationUpdateProfileArgs = {
  input: UpdateProfileInput;
};


export type MutationUpdateUserArgs = {
  input: UpdateUserInput;
};


export type MutationUpdateUserPictureArgs = {
  input: UpdateUserPictureInput;
};

/** The node interface is implemented by entities that have a global unique identifier. */
export type Node = {
  id: Scalars['ID'];
};

/** Information about pagination in a connection. */
export type PageInfo = {
  __typename?: 'PageInfo';
  /** When paginating forwards, the cursor to continue. */
  endCursor?: Maybe<Scalars['String']>;
  /** Indicates whether more edges exist following the set defined by the clients arguments. */
  hasNextPage: Scalars['Boolean'];
  /** Indicates whether more edges exist prior the set defined by the clients arguments. */
  hasPreviousPage: Scalars['Boolean'];
  /** When paginating backwards, the cursor to continue. */
  startCursor?: Maybe<Scalars['String']>;
};

export type Query = {
  __typename?: 'Query';
  audioBySlug?: Maybe<Audio>;
  audiosByTags?: Maybe<AudiosByTagsConnection>;
  audiosByUsername?: Maybe<AudiosByUsernameConnection>;
  favoriteAudiosByUserName?: Maybe<FavoriteAudiosByUserNameConnection>;
  followers?: Maybe<FollowersConnection>;
  followings?: Maybe<FollowingsConnection>;
  me: User;
  /** Fetches an object given its ID. */
  node?: Maybe<Node>;
  /** Lookup nodes by a list of IDs. */
  nodes: Array<Maybe<Node>>;
  userByName: User;
  yourAudios?: Maybe<YourAudiosConnection>;
  yourFavoriteAudios?: Maybe<YourFavoriteAudiosConnection>;
  yourFollowers?: Maybe<YourFollowersConnection>;
  yourFollowings?: Maybe<YourFollowingsConnection>;
};


export type QueryAudioBySlugArgs = {
  slug: Scalars['String'];
};


export type QueryAudiosByTagsArgs = {
  after?: InputMaybe<Scalars['String']>;
  before?: InputMaybe<Scalars['String']>;
  first?: InputMaybe<Scalars['Int']>;
  last?: InputMaybe<Scalars['Int']>;
  tags: Array<Scalars['String']>;
};


export type QueryAudiosByUsernameArgs = {
  after?: InputMaybe<Scalars['String']>;
  before?: InputMaybe<Scalars['String']>;
  first?: InputMaybe<Scalars['Int']>;
  last?: InputMaybe<Scalars['Int']>;
  userName: Scalars['String'];
};


export type QueryFavoriteAudiosByUserNameArgs = {
  after?: InputMaybe<Scalars['String']>;
  before?: InputMaybe<Scalars['String']>;
  first?: InputMaybe<Scalars['Int']>;
  last?: InputMaybe<Scalars['Int']>;
  userName: Scalars['String'];
};


export type QueryFollowersArgs = {
  after?: InputMaybe<Scalars['String']>;
  before?: InputMaybe<Scalars['String']>;
  first?: InputMaybe<Scalars['Int']>;
  last?: InputMaybe<Scalars['Int']>;
  userName: Scalars['String'];
};


export type QueryFollowingsArgs = {
  after?: InputMaybe<Scalars['String']>;
  before?: InputMaybe<Scalars['String']>;
  first?: InputMaybe<Scalars['Int']>;
  last?: InputMaybe<Scalars['Int']>;
  userName: Scalars['String'];
};


export type QueryNodeArgs = {
  id: Scalars['ID'];
};


export type QueryNodesArgs = {
  ids: Array<Scalars['ID']>;
};


export type QueryUserByNameArgs = {
  userName: Scalars['String'];
};


export type QueryYourAudiosArgs = {
  after?: InputMaybe<Scalars['String']>;
  before?: InputMaybe<Scalars['String']>;
  first?: InputMaybe<Scalars['Int']>;
  last?: InputMaybe<Scalars['Int']>;
};


export type QueryYourFavoriteAudiosArgs = {
  after?: InputMaybe<Scalars['String']>;
  before?: InputMaybe<Scalars['String']>;
  first?: InputMaybe<Scalars['Int']>;
  last?: InputMaybe<Scalars['Int']>;
};


export type QueryYourFollowersArgs = {
  after?: InputMaybe<Scalars['String']>;
  before?: InputMaybe<Scalars['String']>;
  first?: InputMaybe<Scalars['Int']>;
  last?: InputMaybe<Scalars['Int']>;
};


export type QueryYourFollowingsArgs = {
  after?: InputMaybe<Scalars['String']>;
  before?: InputMaybe<Scalars['String']>;
  first?: InputMaybe<Scalars['Int']>;
  last?: InputMaybe<Scalars['Int']>;
};

export type RegisterError = EmailTaken | UsernameTaken | ValidationError;

export type RegisterInput = {
  email: Scalars['String'];
  password: Scalars['String'];
  userName: Scalars['String'];
};

export type RegisterPayload = {
  __typename?: 'RegisterPayload';
  errors?: Maybe<Array<RegisterError>>;
  message?: Maybe<Scalars['String']>;
};

export type RemoveAudioError = AudioNotFound | Forbidden;

export type RemoveAudioInput = {
  id: Scalars['ID'];
};

export type RemoveAudioPayload = {
  __typename?: 'RemoveAudioPayload';
  errors?: Maybe<Array<RemoveAudioError>>;
  message?: Maybe<Scalars['String']>;
};

export type RemoveAudioPictureError = AudioNotFound | Forbidden;

export type RemoveAudioPictureInput = {
  id: Scalars['ID'];
};

export type RemoveAudioPicturePayload = {
  __typename?: 'RemoveAudioPicturePayload';
  errors?: Maybe<Array<RemoveAudioPictureError>>;
  message?: Maybe<Scalars['String']>;
};

export type RemoveUserPictureError = Forbidden;

export type RemoveUserPicturePayload = {
  __typename?: 'RemoveUserPicturePayload';
  errors?: Maybe<Array<RemoveUserPictureError>>;
  message?: Maybe<Scalars['String']>;
};

export type SignInError = Error & {
  __typename?: 'SignInError';
  code: Scalars['String'];
  message: Scalars['String'];
};

export type Unauthorized = Error & {
  __typename?: 'Unauthorized';
  code: Scalars['String'];
  message: Scalars['String'];
};

export type UnfavoriteAudioError = AudioNotFound;

export type UnfavoriteAudioInput = {
  id: Scalars['ID'];
};

export type UnfavoriteAudioPayload = {
  __typename?: 'UnfavoriteAudioPayload';
  errors?: Maybe<Array<UnfavoriteAudioError>>;
  message?: Maybe<Scalars['String']>;
};

export type UnfollowUserError = Forbidden | UserNotFound;

export type UnfollowUserInput = {
  targetUserId: Scalars['ID'];
};

export type UnfollowUserPayload = {
  __typename?: 'UnfollowUserPayload';
  errors?: Maybe<Array<UnfollowUserError>>;
  message?: Maybe<Scalars['String']>;
};

export type UnmatchedPassword = Error & {
  __typename?: 'UnmatchedPassword';
  code: Scalars['String'];
  message: Scalars['String'];
};

export type UpdateAudioError = AudioNotFound | Forbidden | ValidationError;

export type UpdateAudioInput = {
  description?: InputMaybe<Scalars['String']>;
  id: Scalars['ID'];
  tags?: InputMaybe<Array<Scalars['String']>>;
  title?: InputMaybe<Scalars['String']>;
};

export type UpdateAudioPayload = {
  __typename?: 'UpdateAudioPayload';
  audio?: Maybe<Audio>;
  errors?: Maybe<Array<UpdateAudioError>>;
};

export type UpdateAudioPictureError = AudioNotFound | Forbidden | ValidationError;

export type UpdateAudioPictureInput = {
  data: Scalars['String'];
  id: Scalars['ID'];
};

export type UpdateAudioPicturePayload = {
  __typename?: 'UpdateAudioPicturePayload';
  errors?: Maybe<Array<UpdateAudioPictureError>>;
  response?: Maybe<ImageUploadResponse>;
};

export type UpdatePasswordError = Forbidden | UnmatchedPassword | ValidationError;

export type UpdatePasswordInput = {
  currentPassword: Scalars['String'];
  newPassword: Scalars['String'];
};

export type UpdatePasswordPayload = {
  __typename?: 'UpdatePasswordPayload';
  errors?: Maybe<Array<UpdatePasswordError>>;
  message?: Maybe<Scalars['String']>;
};

export type UpdateProfileError = ValidationError;

export type UpdateProfileInput = {
  about?: InputMaybe<Scalars['String']>;
  displayName?: InputMaybe<Scalars['String']>;
  website?: InputMaybe<Scalars['String']>;
};

export type UpdateProfilePayload = {
  __typename?: 'UpdateProfilePayload';
  errors?: Maybe<Array<UpdateProfileError>>;
  user?: Maybe<User>;
};

export type UpdateUserError = EmailTaken | Forbidden | UsernameTaken | ValidationError;

export type UpdateUserInput = {
  email?: InputMaybe<Scalars['String']>;
  username?: InputMaybe<Scalars['String']>;
};

export type UpdateUserPayload = {
  __typename?: 'UpdateUserPayload';
  errors?: Maybe<Array<UpdateUserError>>;
  user?: Maybe<User>;
};

export type UpdateUserPictureError = Forbidden | ValidationError;

export type UpdateUserPictureInput = {
  data: Scalars['String'];
};

export type UpdateUserPicturePayload = {
  __typename?: 'UpdateUserPicturePayload';
  errors?: Maybe<Array<UpdateUserPictureError>>;
  response?: Maybe<ImageUploadResponse>;
};

export type UploadDoesNotExist = Error & {
  __typename?: 'UploadDoesNotExist';
  code: Scalars['String'];
  message: Scalars['String'];
  uploadId: Scalars['String'];
};

export type User = Node & {
  __typename?: 'User';
  audios?: Maybe<Array<Maybe<Audio>>>;
  email?: Maybe<Scalars['String']>;
  favoriteAudios?: Maybe<Array<Maybe<Audio>>>;
  followers?: Maybe<Array<Maybe<User>>>;
  followings?: Maybe<Array<Maybe<User>>>;
  id: Scalars['ID'];
  isFollowed?: Maybe<Scalars['Boolean']>;
  picture?: Maybe<Scalars['String']>;
  userName: Scalars['String'];
};

export type UserNotFound = Error & {
  __typename?: 'UserNotFound';
  code: Scalars['String'];
  message: Scalars['String'];
  userId: Scalars['Long'];
};

export type UsernameTaken = Error & {
  __typename?: 'UsernameTaken';
  code: Scalars['String'];
  message: Scalars['String'];
  username: Scalars['String'];
};

export type ValidationError = Error & {
  __typename?: 'ValidationError';
  code: Scalars['String'];
  failures: Array<ValidationPropertyError>;
  message: Scalars['String'];
};

export type ValidationPropertyError = {
  __typename?: 'ValidationPropertyError';
  message: Scalars['String'];
  property: Scalars['String'];
};

/** A connection to a list of items. */
export type YourAudiosConnection = {
  __typename?: 'YourAudiosConnection';
  /** A list of edges. */
  edges?: Maybe<Array<YourAudiosEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<Audio>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type YourAudiosEdge = {
  __typename?: 'YourAudiosEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String'];
  /** The item at the end of the edge. */
  node: Audio;
};

/** A connection to a list of items. */
export type YourFavoriteAudiosConnection = {
  __typename?: 'YourFavoriteAudiosConnection';
  /** A list of edges. */
  edges?: Maybe<Array<YourFavoriteAudiosEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<Audio>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type YourFavoriteAudiosEdge = {
  __typename?: 'YourFavoriteAudiosEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String'];
  /** The item at the end of the edge. */
  node: Audio;
};

/** A connection to a list of items. */
export type YourFollowersConnection = {
  __typename?: 'YourFollowersConnection';
  /** A list of edges. */
  edges?: Maybe<Array<YourFollowersEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<User>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type YourFollowersEdge = {
  __typename?: 'YourFollowersEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String'];
  /** The item at the end of the edge. */
  node: User;
};

/** A connection to a list of items. */
export type YourFollowingsConnection = {
  __typename?: 'YourFollowingsConnection';
  /** A list of edges. */
  edges?: Maybe<Array<YourFollowingsEdge>>;
  /** A flattened list of the nodes. */
  nodes?: Maybe<Array<User>>;
  /** Information to aid in pagination. */
  pageInfo: PageInfo;
};

/** An edge in a connection. */
export type YourFollowingsEdge = {
  __typename?: 'YourFollowingsEdge';
  /** A cursor for use in pagination. */
  cursor: Scalars['String'];
  /** The item at the end of the edge. */
  node: User;
};

export type AudioFragment = { __typename?: 'Audio', id: string, slug: string, title: string, description?: string | null | undefined, created: any, tags: Array<string>, picture?: string | null | undefined, duration: any, size: any, mp3?: string | null | undefined, user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined };

export type ProfileFragment = { __typename?: 'User', isFollowed?: boolean | null | undefined, id: string, userName: string, picture?: string | null | undefined };

export type UserFragment = { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined };

export type CreateAudioMutationVariables = Exact<{
  input: CreateAudioInput;
}>;


export type CreateAudioMutation = { __typename?: 'Mutation', createAudio: { __typename?: 'CreateAudioPayload', audio?: { __typename?: 'Audio', id: string, slug: string, title: string, description?: string | null | undefined, created: any, tags: Array<string>, picture?: string | null | undefined, duration: any, size: any, mp3?: string | null | undefined, user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } | null | undefined } };

export type UpdateAudioMutationVariables = Exact<{
  input: UpdateAudioInput;
}>;


export type UpdateAudioMutation = { __typename?: 'Mutation', updateAudio: { __typename?: 'UpdateAudioPayload', audio?: { __typename?: 'Audio', id: string, slug: string, title: string, description?: string | null | undefined, created: any, tags: Array<string>, picture?: string | null | undefined, duration: any, size: any, mp3?: string | null | undefined, user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } | null | undefined } };

export type UpdateAudioPictureMutationVariables = Exact<{
  input: UpdateAudioPictureInput;
}>;


export type UpdateAudioPictureMutation = { __typename?: 'Mutation', updateAudioPicture: { __typename?: 'UpdateAudioPicturePayload', response?: { __typename?: 'ImageUploadResponse', url: string } | null | undefined } };

export type RemoveAudioMutationVariables = Exact<{
  input: RemoveAudioInput;
}>;


export type RemoveAudioMutation = { __typename?: 'Mutation', removeAudio: { __typename?: 'RemoveAudioPayload', message?: string | null | undefined } };

export type RemoveAudioPictureMutationVariables = Exact<{
  input: RemoveAudioPictureInput;
}>;


export type RemoveAudioPictureMutation = { __typename?: 'Mutation', removeAudioPicture: { __typename?: 'RemoveAudioPicturePayload', message?: string | null | undefined } };

export type GenerateUploadLinkMutationVariables = Exact<{
  input: GenerateUploadLinkInput;
}>;


export type GenerateUploadLinkMutation = { __typename?: 'Mutation', generateUploadLink: { __typename?: 'GenerateUploadLinkPayload', response?: { __typename?: 'GenerateUploadLinkResponse', uploadId: string, uploadUrl: string } | null | undefined } };

export type FavoriteAudioMutationVariables = Exact<{
  input: FavoriteAudioInput;
}>;


export type FavoriteAudioMutation = { __typename?: 'Mutation', favoriteAudio: { __typename?: 'FavoriteAudioPayload', message?: string | null | undefined } };

export type UnfavoriteAudioMutationVariables = Exact<{
  input: UnfavoriteAudioInput;
}>;


export type UnfavoriteAudioMutation = { __typename?: 'Mutation', unfavoriteAudio: { __typename?: 'UnfavoriteAudioPayload', message?: string | null | undefined } };

export type LoginMutationVariables = Exact<{
  input: LoginInput;
}>;


export type LoginMutation = { __typename?: 'Mutation', login: { __typename?: 'LoginPayload', user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } };

export type LogoutMutationVariables = Exact<{ [key: string]: never; }>;


export type LogoutMutation = { __typename?: 'Mutation', logout: { __typename?: 'LogoutPayload', message?: string | null | undefined } };

export type RegisterUserMutationVariables = Exact<{
  input: RegisterInput;
}>;


export type RegisterUserMutation = { __typename?: 'Mutation', register: { __typename?: 'RegisterPayload', message?: string | null | undefined } };

export type UpdateProfileMutationVariables = Exact<{
  input: UpdateProfileInput;
}>;


export type UpdateProfileMutation = { __typename?: 'Mutation', updateProfile: { __typename?: 'UpdateProfilePayload', user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } };

export type UpdateUserPictureMutationVariables = Exact<{
  input: UpdateUserPictureInput;
}>;


export type UpdateUserPictureMutation = { __typename?: 'Mutation', updateUserPicture: { __typename?: 'UpdateUserPicturePayload', response?: { __typename?: 'ImageUploadResponse', url: string } | null | undefined } };

export type RemoveUserPictureMutationVariables = Exact<{ [key: string]: never; }>;


export type RemoveUserPictureMutation = { __typename?: 'Mutation', removeUserPicture: { __typename?: 'RemoveUserPicturePayload', message?: string | null | undefined } };

export type UpdateUserMutationVariables = Exact<{
  input: UpdateUserInput;
}>;


export type UpdateUserMutation = { __typename?: 'Mutation', updateUser: { __typename?: 'UpdateUserPayload', user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } };

export type UpdatePasswordMutationVariables = Exact<{
  input: UpdatePasswordInput;
}>;


export type UpdatePasswordMutation = { __typename?: 'Mutation', updatePassword: { __typename?: 'UpdatePasswordPayload', message?: string | null | undefined } };

export type FollowUserMutationVariables = Exact<{
  input: FollowUserInput;
}>;


export type FollowUserMutation = { __typename?: 'Mutation', followUser: { __typename?: 'FollowUserPayload', message?: string | null | undefined } };

export type UnfollowUserMutationVariables = Exact<{
  input: UnfollowUserInput;
}>;


export type UnfollowUserMutation = { __typename?: 'Mutation', unfollowUser: { __typename?: 'UnfollowUserPayload', message?: string | null | undefined } };

export type GetAudioBySlugQueryVariables = Exact<{
  slug: Scalars['String'];
}>;


export type GetAudioBySlugQuery = { __typename?: 'Query', audioBySlug?: { __typename?: 'Audio', isFavorited?: boolean | null | undefined, id: string, slug: string, title: string, description?: string | null | undefined, created: any, tags: Array<string>, picture?: string | null | undefined, duration: any, size: any, mp3?: string | null | undefined, favorited?: Array<{ __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined> | null | undefined, user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } | null | undefined };

export type GetUserAudiosQueryVariables = Exact<{
  userName: Scalars['String'];
  pageSize?: InputMaybe<Scalars['Int']>;
  cursor?: InputMaybe<Scalars['String']>;
}>;


export type GetUserAudiosQuery = { __typename?: 'Query', audiosByUsername?: { __typename?: 'AudiosByUsernameConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, endCursor?: string | null | undefined }, edges?: Array<{ __typename?: 'AudiosByUsernameEdge', cursor: string, node: { __typename?: 'Audio', id: string, slug: string, title: string, description?: string | null | undefined, created: any, tags: Array<string>, picture?: string | null | undefined, duration: any, size: any, mp3?: string | null | undefined, user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } }> | null | undefined } | null | undefined };

export type GetUserFavoriteAudiosQueryVariables = Exact<{
  userName: Scalars['String'];
  pageSize?: InputMaybe<Scalars['Int']>;
  cursor?: InputMaybe<Scalars['String']>;
}>;


export type GetUserFavoriteAudiosQuery = { __typename?: 'Query', favoriteAudiosByUserName?: { __typename?: 'FavoriteAudiosByUserNameConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, endCursor?: string | null | undefined }, edges?: Array<{ __typename?: 'FavoriteAudiosByUserNameEdge', cursor: string, node: { __typename?: 'Audio', id: string, slug: string, title: string, description?: string | null | undefined, created: any, tags: Array<string>, picture?: string | null | undefined, duration: any, size: any, mp3?: string | null | undefined, user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } }> | null | undefined } | null | undefined };

export type GetAudiosByTagsQueryVariables = Exact<{
  tags: Array<Scalars['String']> | Scalars['String'];
  pageSize?: InputMaybe<Scalars['Int']>;
  cursor?: InputMaybe<Scalars['String']>;
}>;


export type GetAudiosByTagsQuery = { __typename?: 'Query', audiosByTags?: { __typename?: 'AudiosByTagsConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, endCursor?: string | null | undefined }, edges?: Array<{ __typename?: 'AudiosByTagsEdge', cursor: string, node: { __typename?: 'Audio', id: string, slug: string, title: string, description?: string | null | undefined, created: any, tags: Array<string>, picture?: string | null | undefined, duration: any, size: any, mp3?: string | null | undefined, user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } }> | null | undefined } | null | undefined };

export type GetYourAudiosQueryVariables = Exact<{
  pageSize?: InputMaybe<Scalars['Int']>;
  cursor?: InputMaybe<Scalars['String']>;
}>;


export type GetYourAudiosQuery = { __typename?: 'Query', yourAudios?: { __typename?: 'YourAudiosConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, endCursor?: string | null | undefined }, edges?: Array<{ __typename?: 'YourAudiosEdge', cursor: string, node: { __typename?: 'Audio', id: string, slug: string, title: string, description?: string | null | undefined, created: any, tags: Array<string>, picture?: string | null | undefined, duration: any, size: any, mp3?: string | null | undefined, user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } }> | null | undefined } | null | undefined };

export type GetYourFavoriteAudiosQueryVariables = Exact<{
  pageSize?: InputMaybe<Scalars['Int']>;
  cursor?: InputMaybe<Scalars['String']>;
}>;


export type GetYourFavoriteAudiosQuery = { __typename?: 'Query', yourFavoriteAudios?: { __typename?: 'YourFavoriteAudiosConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, endCursor?: string | null | undefined }, edges?: Array<{ __typename?: 'YourFavoriteAudiosEdge', cursor: string, node: { __typename?: 'Audio', id: string, slug: string, title: string, description?: string | null | undefined, created: any, tags: Array<string>, picture?: string | null | undefined, duration: any, size: any, mp3?: string | null | undefined, user?: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } | null | undefined } }> | null | undefined } | null | undefined };

export type GetCurrentUserQueryVariables = Exact<{ [key: string]: never; }>;


export type GetCurrentUserQuery = { __typename?: 'Query', me: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } };

export type GetProfileQueryVariables = Exact<{
  userName: Scalars['String'];
}>;


export type GetProfileQuery = { __typename?: 'Query', userByName: { __typename?: 'User', isFollowed?: boolean | null | undefined, id: string, userName: string, picture?: string | null | undefined } };

export type GetUserFollowingsQueryVariables = Exact<{
  userName: Scalars['String'];
  pageSize?: InputMaybe<Scalars['Int']>;
  cursor?: InputMaybe<Scalars['String']>;
}>;


export type GetUserFollowingsQuery = { __typename?: 'Query', followings?: { __typename?: 'FollowingsConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, endCursor?: string | null | undefined }, edges?: Array<{ __typename?: 'FollowingsEdge', cursor: string, node: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } }> | null | undefined } | null | undefined };

export type GetUserFollowersQueryVariables = Exact<{
  userName: Scalars['String'];
  pageSize?: InputMaybe<Scalars['Int']>;
  cursor?: InputMaybe<Scalars['String']>;
}>;


export type GetUserFollowersQuery = { __typename?: 'Query', followers?: { __typename?: 'FollowersConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, endCursor?: string | null | undefined }, edges?: Array<{ __typename?: 'FollowersEdge', cursor: string, node: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } }> | null | undefined } | null | undefined };

export type GetYourFollowingsQueryVariables = Exact<{
  pageSize?: InputMaybe<Scalars['Int']>;
  cursor?: InputMaybe<Scalars['String']>;
}>;


export type GetYourFollowingsQuery = { __typename?: 'Query', yourFollowings?: { __typename?: 'YourFollowingsConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, endCursor?: string | null | undefined }, edges?: Array<{ __typename?: 'YourFollowingsEdge', cursor: string, node: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } }> | null | undefined } | null | undefined };

export type GetYourFollowersQueryVariables = Exact<{
  pageSize?: InputMaybe<Scalars['Int']>;
  cursor?: InputMaybe<Scalars['String']>;
}>;


export type GetYourFollowersQuery = { __typename?: 'Query', yourFollowers?: { __typename?: 'YourFollowersConnection', pageInfo: { __typename?: 'PageInfo', hasNextPage: boolean, endCursor?: string | null | undefined }, edges?: Array<{ __typename?: 'YourFollowersEdge', cursor: string, node: { __typename?: 'User', id: string, userName: string, picture?: string | null | undefined } }> | null | undefined } | null | undefined };

export const UserFragmentDoc = gql`
    fragment User on User {
  id
  userName
  picture
}
    `;
export const AudioFragmentDoc = gql`
    fragment Audio on Audio {
  id
  slug
  title
  description
  created
  tags
  picture
  duration
  size
  mp3
  user {
    ...User
  }
}
    ${UserFragmentDoc}`;
export const ProfileFragmentDoc = gql`
    fragment Profile on User {
  ...User
  isFollowed @authorize
}
    ${UserFragmentDoc}`;
export const CreateAudioDocument = gql`
    mutation CreateAudio($input: CreateAudioInput!) {
  createAudio(input: $input) {
    audio {
      ...Audio
    }
  }
}
    ${AudioFragmentDoc}`;
export type CreateAudioMutationFn = Apollo.MutationFunction<CreateAudioMutation, CreateAudioMutationVariables>;

/**
 * __useCreateAudioMutation__
 *
 * To run a mutation, you first call `useCreateAudioMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useCreateAudioMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [createAudioMutation, { data, loading, error }] = useCreateAudioMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useCreateAudioMutation(baseOptions?: Apollo.MutationHookOptions<CreateAudioMutation, CreateAudioMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<CreateAudioMutation, CreateAudioMutationVariables>(CreateAudioDocument, options);
      }
export type CreateAudioMutationHookResult = ReturnType<typeof useCreateAudioMutation>;
export type CreateAudioMutationResult = Apollo.MutationResult<CreateAudioMutation>;
export type CreateAudioMutationOptions = Apollo.BaseMutationOptions<CreateAudioMutation, CreateAudioMutationVariables>;
export const UpdateAudioDocument = gql`
    mutation UpdateAudio($input: UpdateAudioInput!) {
  updateAudio(input: $input) {
    audio {
      ...Audio
    }
  }
}
    ${AudioFragmentDoc}`;
export type UpdateAudioMutationFn = Apollo.MutationFunction<UpdateAudioMutation, UpdateAudioMutationVariables>;

/**
 * __useUpdateAudioMutation__
 *
 * To run a mutation, you first call `useUpdateAudioMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useUpdateAudioMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [updateAudioMutation, { data, loading, error }] = useUpdateAudioMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useUpdateAudioMutation(baseOptions?: Apollo.MutationHookOptions<UpdateAudioMutation, UpdateAudioMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<UpdateAudioMutation, UpdateAudioMutationVariables>(UpdateAudioDocument, options);
      }
export type UpdateAudioMutationHookResult = ReturnType<typeof useUpdateAudioMutation>;
export type UpdateAudioMutationResult = Apollo.MutationResult<UpdateAudioMutation>;
export type UpdateAudioMutationOptions = Apollo.BaseMutationOptions<UpdateAudioMutation, UpdateAudioMutationVariables>;
export const UpdateAudioPictureDocument = gql`
    mutation UpdateAudioPicture($input: UpdateAudioPictureInput!) {
  updateAudioPicture(input: $input) {
    response {
      url
    }
  }
}
    `;
export type UpdateAudioPictureMutationFn = Apollo.MutationFunction<UpdateAudioPictureMutation, UpdateAudioPictureMutationVariables>;

/**
 * __useUpdateAudioPictureMutation__
 *
 * To run a mutation, you first call `useUpdateAudioPictureMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useUpdateAudioPictureMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [updateAudioPictureMutation, { data, loading, error }] = useUpdateAudioPictureMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useUpdateAudioPictureMutation(baseOptions?: Apollo.MutationHookOptions<UpdateAudioPictureMutation, UpdateAudioPictureMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<UpdateAudioPictureMutation, UpdateAudioPictureMutationVariables>(UpdateAudioPictureDocument, options);
      }
export type UpdateAudioPictureMutationHookResult = ReturnType<typeof useUpdateAudioPictureMutation>;
export type UpdateAudioPictureMutationResult = Apollo.MutationResult<UpdateAudioPictureMutation>;
export type UpdateAudioPictureMutationOptions = Apollo.BaseMutationOptions<UpdateAudioPictureMutation, UpdateAudioPictureMutationVariables>;
export const RemoveAudioDocument = gql`
    mutation RemoveAudio($input: RemoveAudioInput!) {
  removeAudio(input: $input) {
    message
  }
}
    `;
export type RemoveAudioMutationFn = Apollo.MutationFunction<RemoveAudioMutation, RemoveAudioMutationVariables>;

/**
 * __useRemoveAudioMutation__
 *
 * To run a mutation, you first call `useRemoveAudioMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useRemoveAudioMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [removeAudioMutation, { data, loading, error }] = useRemoveAudioMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useRemoveAudioMutation(baseOptions?: Apollo.MutationHookOptions<RemoveAudioMutation, RemoveAudioMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<RemoveAudioMutation, RemoveAudioMutationVariables>(RemoveAudioDocument, options);
      }
export type RemoveAudioMutationHookResult = ReturnType<typeof useRemoveAudioMutation>;
export type RemoveAudioMutationResult = Apollo.MutationResult<RemoveAudioMutation>;
export type RemoveAudioMutationOptions = Apollo.BaseMutationOptions<RemoveAudioMutation, RemoveAudioMutationVariables>;
export const RemoveAudioPictureDocument = gql`
    mutation RemoveAudioPicture($input: RemoveAudioPictureInput!) {
  removeAudioPicture(input: $input) {
    message
  }
}
    `;
export type RemoveAudioPictureMutationFn = Apollo.MutationFunction<RemoveAudioPictureMutation, RemoveAudioPictureMutationVariables>;

/**
 * __useRemoveAudioPictureMutation__
 *
 * To run a mutation, you first call `useRemoveAudioPictureMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useRemoveAudioPictureMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [removeAudioPictureMutation, { data, loading, error }] = useRemoveAudioPictureMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useRemoveAudioPictureMutation(baseOptions?: Apollo.MutationHookOptions<RemoveAudioPictureMutation, RemoveAudioPictureMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<RemoveAudioPictureMutation, RemoveAudioPictureMutationVariables>(RemoveAudioPictureDocument, options);
      }
export type RemoveAudioPictureMutationHookResult = ReturnType<typeof useRemoveAudioPictureMutation>;
export type RemoveAudioPictureMutationResult = Apollo.MutationResult<RemoveAudioPictureMutation>;
export type RemoveAudioPictureMutationOptions = Apollo.BaseMutationOptions<RemoveAudioPictureMutation, RemoveAudioPictureMutationVariables>;
export const GenerateUploadLinkDocument = gql`
    mutation GenerateUploadLink($input: GenerateUploadLinkInput!) {
  generateUploadLink(input: $input) {
    response {
      uploadId
      uploadUrl
    }
  }
}
    `;
export type GenerateUploadLinkMutationFn = Apollo.MutationFunction<GenerateUploadLinkMutation, GenerateUploadLinkMutationVariables>;

/**
 * __useGenerateUploadLinkMutation__
 *
 * To run a mutation, you first call `useGenerateUploadLinkMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useGenerateUploadLinkMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [generateUploadLinkMutation, { data, loading, error }] = useGenerateUploadLinkMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useGenerateUploadLinkMutation(baseOptions?: Apollo.MutationHookOptions<GenerateUploadLinkMutation, GenerateUploadLinkMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<GenerateUploadLinkMutation, GenerateUploadLinkMutationVariables>(GenerateUploadLinkDocument, options);
      }
export type GenerateUploadLinkMutationHookResult = ReturnType<typeof useGenerateUploadLinkMutation>;
export type GenerateUploadLinkMutationResult = Apollo.MutationResult<GenerateUploadLinkMutation>;
export type GenerateUploadLinkMutationOptions = Apollo.BaseMutationOptions<GenerateUploadLinkMutation, GenerateUploadLinkMutationVariables>;
export const FavoriteAudioDocument = gql`
    mutation FavoriteAudio($input: FavoriteAudioInput!) {
  favoriteAudio(input: $input) {
    message
  }
}
    `;
export type FavoriteAudioMutationFn = Apollo.MutationFunction<FavoriteAudioMutation, FavoriteAudioMutationVariables>;

/**
 * __useFavoriteAudioMutation__
 *
 * To run a mutation, you first call `useFavoriteAudioMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useFavoriteAudioMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [favoriteAudioMutation, { data, loading, error }] = useFavoriteAudioMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useFavoriteAudioMutation(baseOptions?: Apollo.MutationHookOptions<FavoriteAudioMutation, FavoriteAudioMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<FavoriteAudioMutation, FavoriteAudioMutationVariables>(FavoriteAudioDocument, options);
      }
export type FavoriteAudioMutationHookResult = ReturnType<typeof useFavoriteAudioMutation>;
export type FavoriteAudioMutationResult = Apollo.MutationResult<FavoriteAudioMutation>;
export type FavoriteAudioMutationOptions = Apollo.BaseMutationOptions<FavoriteAudioMutation, FavoriteAudioMutationVariables>;
export const UnfavoriteAudioDocument = gql`
    mutation UnfavoriteAudio($input: UnfavoriteAudioInput!) {
  unfavoriteAudio(input: $input) {
    message
  }
}
    `;
export type UnfavoriteAudioMutationFn = Apollo.MutationFunction<UnfavoriteAudioMutation, UnfavoriteAudioMutationVariables>;

/**
 * __useUnfavoriteAudioMutation__
 *
 * To run a mutation, you first call `useUnfavoriteAudioMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useUnfavoriteAudioMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [unfavoriteAudioMutation, { data, loading, error }] = useUnfavoriteAudioMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useUnfavoriteAudioMutation(baseOptions?: Apollo.MutationHookOptions<UnfavoriteAudioMutation, UnfavoriteAudioMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<UnfavoriteAudioMutation, UnfavoriteAudioMutationVariables>(UnfavoriteAudioDocument, options);
      }
export type UnfavoriteAudioMutationHookResult = ReturnType<typeof useUnfavoriteAudioMutation>;
export type UnfavoriteAudioMutationResult = Apollo.MutationResult<UnfavoriteAudioMutation>;
export type UnfavoriteAudioMutationOptions = Apollo.BaseMutationOptions<UnfavoriteAudioMutation, UnfavoriteAudioMutationVariables>;
export const LoginDocument = gql`
    mutation Login($input: LoginInput!) {
  login(input: $input) {
    user {
      ...User
    }
  }
}
    ${UserFragmentDoc}`;
export type LoginMutationFn = Apollo.MutationFunction<LoginMutation, LoginMutationVariables>;

/**
 * __useLoginMutation__
 *
 * To run a mutation, you first call `useLoginMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useLoginMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [loginMutation, { data, loading, error }] = useLoginMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useLoginMutation(baseOptions?: Apollo.MutationHookOptions<LoginMutation, LoginMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<LoginMutation, LoginMutationVariables>(LoginDocument, options);
      }
export type LoginMutationHookResult = ReturnType<typeof useLoginMutation>;
export type LoginMutationResult = Apollo.MutationResult<LoginMutation>;
export type LoginMutationOptions = Apollo.BaseMutationOptions<LoginMutation, LoginMutationVariables>;
export const LogoutDocument = gql`
    mutation Logout {
  logout {
    message
  }
}
    `;
export type LogoutMutationFn = Apollo.MutationFunction<LogoutMutation, LogoutMutationVariables>;

/**
 * __useLogoutMutation__
 *
 * To run a mutation, you first call `useLogoutMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useLogoutMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [logoutMutation, { data, loading, error }] = useLogoutMutation({
 *   variables: {
 *   },
 * });
 */
export function useLogoutMutation(baseOptions?: Apollo.MutationHookOptions<LogoutMutation, LogoutMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<LogoutMutation, LogoutMutationVariables>(LogoutDocument, options);
      }
export type LogoutMutationHookResult = ReturnType<typeof useLogoutMutation>;
export type LogoutMutationResult = Apollo.MutationResult<LogoutMutation>;
export type LogoutMutationOptions = Apollo.BaseMutationOptions<LogoutMutation, LogoutMutationVariables>;
export const RegisterUserDocument = gql`
    mutation RegisterUser($input: RegisterInput!) {
  register(input: $input) {
    message
  }
}
    `;
export type RegisterUserMutationFn = Apollo.MutationFunction<RegisterUserMutation, RegisterUserMutationVariables>;

/**
 * __useRegisterUserMutation__
 *
 * To run a mutation, you first call `useRegisterUserMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useRegisterUserMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [registerUserMutation, { data, loading, error }] = useRegisterUserMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useRegisterUserMutation(baseOptions?: Apollo.MutationHookOptions<RegisterUserMutation, RegisterUserMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<RegisterUserMutation, RegisterUserMutationVariables>(RegisterUserDocument, options);
      }
export type RegisterUserMutationHookResult = ReturnType<typeof useRegisterUserMutation>;
export type RegisterUserMutationResult = Apollo.MutationResult<RegisterUserMutation>;
export type RegisterUserMutationOptions = Apollo.BaseMutationOptions<RegisterUserMutation, RegisterUserMutationVariables>;
export const UpdateProfileDocument = gql`
    mutation UpdateProfile($input: UpdateProfileInput!) {
  updateProfile(input: $input) {
    user {
      ...User
    }
  }
}
    ${UserFragmentDoc}`;
export type UpdateProfileMutationFn = Apollo.MutationFunction<UpdateProfileMutation, UpdateProfileMutationVariables>;

/**
 * __useUpdateProfileMutation__
 *
 * To run a mutation, you first call `useUpdateProfileMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useUpdateProfileMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [updateProfileMutation, { data, loading, error }] = useUpdateProfileMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useUpdateProfileMutation(baseOptions?: Apollo.MutationHookOptions<UpdateProfileMutation, UpdateProfileMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<UpdateProfileMutation, UpdateProfileMutationVariables>(UpdateProfileDocument, options);
      }
export type UpdateProfileMutationHookResult = ReturnType<typeof useUpdateProfileMutation>;
export type UpdateProfileMutationResult = Apollo.MutationResult<UpdateProfileMutation>;
export type UpdateProfileMutationOptions = Apollo.BaseMutationOptions<UpdateProfileMutation, UpdateProfileMutationVariables>;
export const UpdateUserPictureDocument = gql`
    mutation UpdateUserPicture($input: UpdateUserPictureInput!) {
  updateUserPicture(input: $input) {
    response {
      url
    }
  }
}
    `;
export type UpdateUserPictureMutationFn = Apollo.MutationFunction<UpdateUserPictureMutation, UpdateUserPictureMutationVariables>;

/**
 * __useUpdateUserPictureMutation__
 *
 * To run a mutation, you first call `useUpdateUserPictureMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useUpdateUserPictureMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [updateUserPictureMutation, { data, loading, error }] = useUpdateUserPictureMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useUpdateUserPictureMutation(baseOptions?: Apollo.MutationHookOptions<UpdateUserPictureMutation, UpdateUserPictureMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<UpdateUserPictureMutation, UpdateUserPictureMutationVariables>(UpdateUserPictureDocument, options);
      }
export type UpdateUserPictureMutationHookResult = ReturnType<typeof useUpdateUserPictureMutation>;
export type UpdateUserPictureMutationResult = Apollo.MutationResult<UpdateUserPictureMutation>;
export type UpdateUserPictureMutationOptions = Apollo.BaseMutationOptions<UpdateUserPictureMutation, UpdateUserPictureMutationVariables>;
export const RemoveUserPictureDocument = gql`
    mutation RemoveUserPicture {
  removeUserPicture {
    message
  }
}
    `;
export type RemoveUserPictureMutationFn = Apollo.MutationFunction<RemoveUserPictureMutation, RemoveUserPictureMutationVariables>;

/**
 * __useRemoveUserPictureMutation__
 *
 * To run a mutation, you first call `useRemoveUserPictureMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useRemoveUserPictureMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [removeUserPictureMutation, { data, loading, error }] = useRemoveUserPictureMutation({
 *   variables: {
 *   },
 * });
 */
export function useRemoveUserPictureMutation(baseOptions?: Apollo.MutationHookOptions<RemoveUserPictureMutation, RemoveUserPictureMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<RemoveUserPictureMutation, RemoveUserPictureMutationVariables>(RemoveUserPictureDocument, options);
      }
export type RemoveUserPictureMutationHookResult = ReturnType<typeof useRemoveUserPictureMutation>;
export type RemoveUserPictureMutationResult = Apollo.MutationResult<RemoveUserPictureMutation>;
export type RemoveUserPictureMutationOptions = Apollo.BaseMutationOptions<RemoveUserPictureMutation, RemoveUserPictureMutationVariables>;
export const UpdateUserDocument = gql`
    mutation UpdateUser($input: UpdateUserInput!) {
  updateUser(input: $input) {
    user {
      ...User
    }
  }
}
    ${UserFragmentDoc}`;
export type UpdateUserMutationFn = Apollo.MutationFunction<UpdateUserMutation, UpdateUserMutationVariables>;

/**
 * __useUpdateUserMutation__
 *
 * To run a mutation, you first call `useUpdateUserMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useUpdateUserMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [updateUserMutation, { data, loading, error }] = useUpdateUserMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useUpdateUserMutation(baseOptions?: Apollo.MutationHookOptions<UpdateUserMutation, UpdateUserMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<UpdateUserMutation, UpdateUserMutationVariables>(UpdateUserDocument, options);
      }
export type UpdateUserMutationHookResult = ReturnType<typeof useUpdateUserMutation>;
export type UpdateUserMutationResult = Apollo.MutationResult<UpdateUserMutation>;
export type UpdateUserMutationOptions = Apollo.BaseMutationOptions<UpdateUserMutation, UpdateUserMutationVariables>;
export const UpdatePasswordDocument = gql`
    mutation UpdatePassword($input: UpdatePasswordInput!) {
  updatePassword(input: $input) {
    message
  }
}
    `;
export type UpdatePasswordMutationFn = Apollo.MutationFunction<UpdatePasswordMutation, UpdatePasswordMutationVariables>;

/**
 * __useUpdatePasswordMutation__
 *
 * To run a mutation, you first call `useUpdatePasswordMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useUpdatePasswordMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [updatePasswordMutation, { data, loading, error }] = useUpdatePasswordMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useUpdatePasswordMutation(baseOptions?: Apollo.MutationHookOptions<UpdatePasswordMutation, UpdatePasswordMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<UpdatePasswordMutation, UpdatePasswordMutationVariables>(UpdatePasswordDocument, options);
      }
export type UpdatePasswordMutationHookResult = ReturnType<typeof useUpdatePasswordMutation>;
export type UpdatePasswordMutationResult = Apollo.MutationResult<UpdatePasswordMutation>;
export type UpdatePasswordMutationOptions = Apollo.BaseMutationOptions<UpdatePasswordMutation, UpdatePasswordMutationVariables>;
export const FollowUserDocument = gql`
    mutation FollowUser($input: FollowUserInput!) {
  followUser(input: $input) {
    message
  }
}
    `;
export type FollowUserMutationFn = Apollo.MutationFunction<FollowUserMutation, FollowUserMutationVariables>;

/**
 * __useFollowUserMutation__
 *
 * To run a mutation, you first call `useFollowUserMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useFollowUserMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [followUserMutation, { data, loading, error }] = useFollowUserMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useFollowUserMutation(baseOptions?: Apollo.MutationHookOptions<FollowUserMutation, FollowUserMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<FollowUserMutation, FollowUserMutationVariables>(FollowUserDocument, options);
      }
export type FollowUserMutationHookResult = ReturnType<typeof useFollowUserMutation>;
export type FollowUserMutationResult = Apollo.MutationResult<FollowUserMutation>;
export type FollowUserMutationOptions = Apollo.BaseMutationOptions<FollowUserMutation, FollowUserMutationVariables>;
export const UnfollowUserDocument = gql`
    mutation UnfollowUser($input: UnfollowUserInput!) {
  unfollowUser(input: $input) {
    message
  }
}
    `;
export type UnfollowUserMutationFn = Apollo.MutationFunction<UnfollowUserMutation, UnfollowUserMutationVariables>;

/**
 * __useUnfollowUserMutation__
 *
 * To run a mutation, you first call `useUnfollowUserMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useUnfollowUserMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [unfollowUserMutation, { data, loading, error }] = useUnfollowUserMutation({
 *   variables: {
 *      input: // value for 'input'
 *   },
 * });
 */
export function useUnfollowUserMutation(baseOptions?: Apollo.MutationHookOptions<UnfollowUserMutation, UnfollowUserMutationVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useMutation<UnfollowUserMutation, UnfollowUserMutationVariables>(UnfollowUserDocument, options);
      }
export type UnfollowUserMutationHookResult = ReturnType<typeof useUnfollowUserMutation>;
export type UnfollowUserMutationResult = Apollo.MutationResult<UnfollowUserMutation>;
export type UnfollowUserMutationOptions = Apollo.BaseMutationOptions<UnfollowUserMutation, UnfollowUserMutationVariables>;
export const GetAudioBySlugDocument = gql`
    query GetAudioBySlug($slug: String!) {
  audioBySlug(slug: $slug) {
    ...Audio
    isFavorited @authorize
    favorited {
      ...User
    }
  }
}
    ${AudioFragmentDoc}
${UserFragmentDoc}`;

/**
 * __useGetAudioBySlugQuery__
 *
 * To run a query within a React component, call `useGetAudioBySlugQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetAudioBySlugQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetAudioBySlugQuery({
 *   variables: {
 *      slug: // value for 'slug'
 *   },
 * });
 */
export function useGetAudioBySlugQuery(baseOptions: Apollo.QueryHookOptions<GetAudioBySlugQuery, GetAudioBySlugQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetAudioBySlugQuery, GetAudioBySlugQueryVariables>(GetAudioBySlugDocument, options);
      }
export function useGetAudioBySlugLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetAudioBySlugQuery, GetAudioBySlugQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetAudioBySlugQuery, GetAudioBySlugQueryVariables>(GetAudioBySlugDocument, options);
        }
export type GetAudioBySlugQueryHookResult = ReturnType<typeof useGetAudioBySlugQuery>;
export type GetAudioBySlugLazyQueryHookResult = ReturnType<typeof useGetAudioBySlugLazyQuery>;
export type GetAudioBySlugQueryResult = Apollo.QueryResult<GetAudioBySlugQuery, GetAudioBySlugQueryVariables>;
export const GetUserAudiosDocument = gql`
    query GetUserAudios($userName: String!, $pageSize: Int, $cursor: String) {
  audiosByUsername(userName: $userName, first: $pageSize, after: $cursor) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      cursor
      node {
        ...Audio
      }
    }
  }
}
    ${AudioFragmentDoc}`;

/**
 * __useGetUserAudiosQuery__
 *
 * To run a query within a React component, call `useGetUserAudiosQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetUserAudiosQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetUserAudiosQuery({
 *   variables: {
 *      userName: // value for 'userName'
 *      pageSize: // value for 'pageSize'
 *      cursor: // value for 'cursor'
 *   },
 * });
 */
export function useGetUserAudiosQuery(baseOptions: Apollo.QueryHookOptions<GetUserAudiosQuery, GetUserAudiosQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetUserAudiosQuery, GetUserAudiosQueryVariables>(GetUserAudiosDocument, options);
      }
export function useGetUserAudiosLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetUserAudiosQuery, GetUserAudiosQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetUserAudiosQuery, GetUserAudiosQueryVariables>(GetUserAudiosDocument, options);
        }
export type GetUserAudiosQueryHookResult = ReturnType<typeof useGetUserAudiosQuery>;
export type GetUserAudiosLazyQueryHookResult = ReturnType<typeof useGetUserAudiosLazyQuery>;
export type GetUserAudiosQueryResult = Apollo.QueryResult<GetUserAudiosQuery, GetUserAudiosQueryVariables>;
export const GetUserFavoriteAudiosDocument = gql`
    query GetUserFavoriteAudios($userName: String!, $pageSize: Int, $cursor: String) {
  favoriteAudiosByUserName(userName: $userName, first: $pageSize, after: $cursor) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      cursor
      node {
        ...Audio
      }
    }
  }
}
    ${AudioFragmentDoc}`;

/**
 * __useGetUserFavoriteAudiosQuery__
 *
 * To run a query within a React component, call `useGetUserFavoriteAudiosQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetUserFavoriteAudiosQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetUserFavoriteAudiosQuery({
 *   variables: {
 *      userName: // value for 'userName'
 *      pageSize: // value for 'pageSize'
 *      cursor: // value for 'cursor'
 *   },
 * });
 */
export function useGetUserFavoriteAudiosQuery(baseOptions: Apollo.QueryHookOptions<GetUserFavoriteAudiosQuery, GetUserFavoriteAudiosQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetUserFavoriteAudiosQuery, GetUserFavoriteAudiosQueryVariables>(GetUserFavoriteAudiosDocument, options);
      }
export function useGetUserFavoriteAudiosLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetUserFavoriteAudiosQuery, GetUserFavoriteAudiosQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetUserFavoriteAudiosQuery, GetUserFavoriteAudiosQueryVariables>(GetUserFavoriteAudiosDocument, options);
        }
export type GetUserFavoriteAudiosQueryHookResult = ReturnType<typeof useGetUserFavoriteAudiosQuery>;
export type GetUserFavoriteAudiosLazyQueryHookResult = ReturnType<typeof useGetUserFavoriteAudiosLazyQuery>;
export type GetUserFavoriteAudiosQueryResult = Apollo.QueryResult<GetUserFavoriteAudiosQuery, GetUserFavoriteAudiosQueryVariables>;
export const GetAudiosByTagsDocument = gql`
    query GetAudiosByTags($tags: [String!]!, $pageSize: Int, $cursor: String) {
  audiosByTags(tags: $tags, first: $pageSize, after: $cursor) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      cursor
      node {
        ...Audio
      }
    }
  }
}
    ${AudioFragmentDoc}`;

/**
 * __useGetAudiosByTagsQuery__
 *
 * To run a query within a React component, call `useGetAudiosByTagsQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetAudiosByTagsQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetAudiosByTagsQuery({
 *   variables: {
 *      tags: // value for 'tags'
 *      pageSize: // value for 'pageSize'
 *      cursor: // value for 'cursor'
 *   },
 * });
 */
export function useGetAudiosByTagsQuery(baseOptions: Apollo.QueryHookOptions<GetAudiosByTagsQuery, GetAudiosByTagsQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetAudiosByTagsQuery, GetAudiosByTagsQueryVariables>(GetAudiosByTagsDocument, options);
      }
export function useGetAudiosByTagsLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetAudiosByTagsQuery, GetAudiosByTagsQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetAudiosByTagsQuery, GetAudiosByTagsQueryVariables>(GetAudiosByTagsDocument, options);
        }
export type GetAudiosByTagsQueryHookResult = ReturnType<typeof useGetAudiosByTagsQuery>;
export type GetAudiosByTagsLazyQueryHookResult = ReturnType<typeof useGetAudiosByTagsLazyQuery>;
export type GetAudiosByTagsQueryResult = Apollo.QueryResult<GetAudiosByTagsQuery, GetAudiosByTagsQueryVariables>;
export const GetYourAudiosDocument = gql`
    query GetYourAudios($pageSize: Int, $cursor: String) {
  yourAudios(first: $pageSize, after: $cursor) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      cursor
      node {
        ...Audio
      }
    }
  }
}
    ${AudioFragmentDoc}`;

/**
 * __useGetYourAudiosQuery__
 *
 * To run a query within a React component, call `useGetYourAudiosQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetYourAudiosQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetYourAudiosQuery({
 *   variables: {
 *      pageSize: // value for 'pageSize'
 *      cursor: // value for 'cursor'
 *   },
 * });
 */
export function useGetYourAudiosQuery(baseOptions?: Apollo.QueryHookOptions<GetYourAudiosQuery, GetYourAudiosQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetYourAudiosQuery, GetYourAudiosQueryVariables>(GetYourAudiosDocument, options);
      }
export function useGetYourAudiosLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetYourAudiosQuery, GetYourAudiosQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetYourAudiosQuery, GetYourAudiosQueryVariables>(GetYourAudiosDocument, options);
        }
export type GetYourAudiosQueryHookResult = ReturnType<typeof useGetYourAudiosQuery>;
export type GetYourAudiosLazyQueryHookResult = ReturnType<typeof useGetYourAudiosLazyQuery>;
export type GetYourAudiosQueryResult = Apollo.QueryResult<GetYourAudiosQuery, GetYourAudiosQueryVariables>;
export const GetYourFavoriteAudiosDocument = gql`
    query GetYourFavoriteAudios($pageSize: Int, $cursor: String) {
  yourFavoriteAudios(first: $pageSize, after: $cursor) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      cursor
      node {
        ...Audio
      }
    }
  }
}
    ${AudioFragmentDoc}`;

/**
 * __useGetYourFavoriteAudiosQuery__
 *
 * To run a query within a React component, call `useGetYourFavoriteAudiosQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetYourFavoriteAudiosQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetYourFavoriteAudiosQuery({
 *   variables: {
 *      pageSize: // value for 'pageSize'
 *      cursor: // value for 'cursor'
 *   },
 * });
 */
export function useGetYourFavoriteAudiosQuery(baseOptions?: Apollo.QueryHookOptions<GetYourFavoriteAudiosQuery, GetYourFavoriteAudiosQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetYourFavoriteAudiosQuery, GetYourFavoriteAudiosQueryVariables>(GetYourFavoriteAudiosDocument, options);
      }
export function useGetYourFavoriteAudiosLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetYourFavoriteAudiosQuery, GetYourFavoriteAudiosQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetYourFavoriteAudiosQuery, GetYourFavoriteAudiosQueryVariables>(GetYourFavoriteAudiosDocument, options);
        }
export type GetYourFavoriteAudiosQueryHookResult = ReturnType<typeof useGetYourFavoriteAudiosQuery>;
export type GetYourFavoriteAudiosLazyQueryHookResult = ReturnType<typeof useGetYourFavoriteAudiosLazyQuery>;
export type GetYourFavoriteAudiosQueryResult = Apollo.QueryResult<GetYourFavoriteAudiosQuery, GetYourFavoriteAudiosQueryVariables>;
export const GetCurrentUserDocument = gql`
    query GetCurrentUser {
  me {
    ...User
  }
}
    ${UserFragmentDoc}`;

/**
 * __useGetCurrentUserQuery__
 *
 * To run a query within a React component, call `useGetCurrentUserQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetCurrentUserQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetCurrentUserQuery({
 *   variables: {
 *   },
 * });
 */
export function useGetCurrentUserQuery(baseOptions?: Apollo.QueryHookOptions<GetCurrentUserQuery, GetCurrentUserQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetCurrentUserQuery, GetCurrentUserQueryVariables>(GetCurrentUserDocument, options);
      }
export function useGetCurrentUserLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetCurrentUserQuery, GetCurrentUserQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetCurrentUserQuery, GetCurrentUserQueryVariables>(GetCurrentUserDocument, options);
        }
export type GetCurrentUserQueryHookResult = ReturnType<typeof useGetCurrentUserQuery>;
export type GetCurrentUserLazyQueryHookResult = ReturnType<typeof useGetCurrentUserLazyQuery>;
export type GetCurrentUserQueryResult = Apollo.QueryResult<GetCurrentUserQuery, GetCurrentUserQueryVariables>;
export const GetProfileDocument = gql`
    query GetProfile($userName: String!) {
  userByName(userName: $userName) {
    ...Profile
  }
}
    ${ProfileFragmentDoc}`;

/**
 * __useGetProfileQuery__
 *
 * To run a query within a React component, call `useGetProfileQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetProfileQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetProfileQuery({
 *   variables: {
 *      userName: // value for 'userName'
 *   },
 * });
 */
export function useGetProfileQuery(baseOptions: Apollo.QueryHookOptions<GetProfileQuery, GetProfileQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetProfileQuery, GetProfileQueryVariables>(GetProfileDocument, options);
      }
export function useGetProfileLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetProfileQuery, GetProfileQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetProfileQuery, GetProfileQueryVariables>(GetProfileDocument, options);
        }
export type GetProfileQueryHookResult = ReturnType<typeof useGetProfileQuery>;
export type GetProfileLazyQueryHookResult = ReturnType<typeof useGetProfileLazyQuery>;
export type GetProfileQueryResult = Apollo.QueryResult<GetProfileQuery, GetProfileQueryVariables>;
export const GetUserFollowingsDocument = gql`
    query GetUserFollowings($userName: String!, $pageSize: Int, $cursor: String) {
  followings(userName: $userName, first: $pageSize, after: $cursor) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      cursor
      node {
        ...User
      }
    }
  }
}
    ${UserFragmentDoc}`;

/**
 * __useGetUserFollowingsQuery__
 *
 * To run a query within a React component, call `useGetUserFollowingsQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetUserFollowingsQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetUserFollowingsQuery({
 *   variables: {
 *      userName: // value for 'userName'
 *      pageSize: // value for 'pageSize'
 *      cursor: // value for 'cursor'
 *   },
 * });
 */
export function useGetUserFollowingsQuery(baseOptions: Apollo.QueryHookOptions<GetUserFollowingsQuery, GetUserFollowingsQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetUserFollowingsQuery, GetUserFollowingsQueryVariables>(GetUserFollowingsDocument, options);
      }
export function useGetUserFollowingsLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetUserFollowingsQuery, GetUserFollowingsQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetUserFollowingsQuery, GetUserFollowingsQueryVariables>(GetUserFollowingsDocument, options);
        }
export type GetUserFollowingsQueryHookResult = ReturnType<typeof useGetUserFollowingsQuery>;
export type GetUserFollowingsLazyQueryHookResult = ReturnType<typeof useGetUserFollowingsLazyQuery>;
export type GetUserFollowingsQueryResult = Apollo.QueryResult<GetUserFollowingsQuery, GetUserFollowingsQueryVariables>;
export const GetUserFollowersDocument = gql`
    query GetUserFollowers($userName: String!, $pageSize: Int, $cursor: String) {
  followers(userName: $userName, first: $pageSize, after: $cursor) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      cursor
      node {
        ...User
      }
    }
  }
}
    ${UserFragmentDoc}`;

/**
 * __useGetUserFollowersQuery__
 *
 * To run a query within a React component, call `useGetUserFollowersQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetUserFollowersQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetUserFollowersQuery({
 *   variables: {
 *      userName: // value for 'userName'
 *      pageSize: // value for 'pageSize'
 *      cursor: // value for 'cursor'
 *   },
 * });
 */
export function useGetUserFollowersQuery(baseOptions: Apollo.QueryHookOptions<GetUserFollowersQuery, GetUserFollowersQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetUserFollowersQuery, GetUserFollowersQueryVariables>(GetUserFollowersDocument, options);
      }
export function useGetUserFollowersLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetUserFollowersQuery, GetUserFollowersQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetUserFollowersQuery, GetUserFollowersQueryVariables>(GetUserFollowersDocument, options);
        }
export type GetUserFollowersQueryHookResult = ReturnType<typeof useGetUserFollowersQuery>;
export type GetUserFollowersLazyQueryHookResult = ReturnType<typeof useGetUserFollowersLazyQuery>;
export type GetUserFollowersQueryResult = Apollo.QueryResult<GetUserFollowersQuery, GetUserFollowersQueryVariables>;
export const GetYourFollowingsDocument = gql`
    query GetYourFollowings($pageSize: Int, $cursor: String) {
  yourFollowings(first: $pageSize, after: $cursor) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      cursor
      node {
        ...User
      }
    }
  }
}
    ${UserFragmentDoc}`;

/**
 * __useGetYourFollowingsQuery__
 *
 * To run a query within a React component, call `useGetYourFollowingsQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetYourFollowingsQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetYourFollowingsQuery({
 *   variables: {
 *      pageSize: // value for 'pageSize'
 *      cursor: // value for 'cursor'
 *   },
 * });
 */
export function useGetYourFollowingsQuery(baseOptions?: Apollo.QueryHookOptions<GetYourFollowingsQuery, GetYourFollowingsQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetYourFollowingsQuery, GetYourFollowingsQueryVariables>(GetYourFollowingsDocument, options);
      }
export function useGetYourFollowingsLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetYourFollowingsQuery, GetYourFollowingsQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetYourFollowingsQuery, GetYourFollowingsQueryVariables>(GetYourFollowingsDocument, options);
        }
export type GetYourFollowingsQueryHookResult = ReturnType<typeof useGetYourFollowingsQuery>;
export type GetYourFollowingsLazyQueryHookResult = ReturnType<typeof useGetYourFollowingsLazyQuery>;
export type GetYourFollowingsQueryResult = Apollo.QueryResult<GetYourFollowingsQuery, GetYourFollowingsQueryVariables>;
export const GetYourFollowersDocument = gql`
    query GetYourFollowers($pageSize: Int, $cursor: String) {
  yourFollowers(first: $pageSize, after: $cursor) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      cursor
      node {
        ...User
      }
    }
  }
}
    ${UserFragmentDoc}`;

/**
 * __useGetYourFollowersQuery__
 *
 * To run a query within a React component, call `useGetYourFollowersQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetYourFollowersQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetYourFollowersQuery({
 *   variables: {
 *      pageSize: // value for 'pageSize'
 *      cursor: // value for 'cursor'
 *   },
 * });
 */
export function useGetYourFollowersQuery(baseOptions?: Apollo.QueryHookOptions<GetYourFollowersQuery, GetYourFollowersQueryVariables>) {
        const options = {...defaultOptions, ...baseOptions}
        return Apollo.useQuery<GetYourFollowersQuery, GetYourFollowersQueryVariables>(GetYourFollowersDocument, options);
      }
export function useGetYourFollowersLazyQuery(baseOptions?: Apollo.LazyQueryHookOptions<GetYourFollowersQuery, GetYourFollowersQueryVariables>) {
          const options = {...defaultOptions, ...baseOptions}
          return Apollo.useLazyQuery<GetYourFollowersQuery, GetYourFollowersQueryVariables>(GetYourFollowersDocument, options);
        }
export type GetYourFollowersQueryHookResult = ReturnType<typeof useGetYourFollowersQuery>;
export type GetYourFollowersLazyQueryHookResult = ReturnType<typeof useGetYourFollowersLazyQuery>;
export type GetYourFollowersQueryResult = Apollo.QueryResult<GetYourFollowersQuery, GetYourFollowersQueryVariables>;
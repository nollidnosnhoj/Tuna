export type ErrorResponse = {
  title: string;
  message: string;
  errors?: { [key: string]: string[] }
}

export type AudioRequest = {
  title?: string;
  description?: string;
  tags?: string[];
  isPublic?: boolean;
};
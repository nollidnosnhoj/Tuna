import {
  array,
  boolean,
  string,
  object,
  SchemaOf,
} from 'yup'
import { validationMessages } from '~/utils'
import { AudioRequest } from './types';

export const editAudioSchema: SchemaOf<AudioRequest> = object().shape({
  title: string()
    .required(validationMessages.required("Title"))
    .max(30, validationMessages.max("Title", 30))
    .defined(),
  description: string()
    .max(500, validationMessages.max("Description", 500))
    .ensure()
    .defined(),
  tags: array(string())
    .max(10, validationMessages.max("Tags", 10))
    .ensure()
    .defined(),
  genre: string()
    .ensure()
    .defined(),
  isPublic: boolean()
    .defined()
}).defined();

export const uploadAudioSchema: SchemaOf<AudioRequest> = object().shape({
  title: string()
    .max(30, validationMessages.max("Title", 30))
    .ensure()
    .defined(),
  description: string()
    .max(500, validationMessages.max("Description", 500))
    .ensure()
    .defined(),
  tags: array(string())
    .max(10, validationMessages.max("Tags", 10))
    .ensure()
    .defined(),
  genre: string()
    .ensure()
    .defined(),
  isPublic: boolean()
    .defined()
}).defined();
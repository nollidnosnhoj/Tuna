import {
  array,
  boolean,
  string,
  object,
  SchemaOf,
  mixed
} from 'yup'
import CONSTANTS from '~/constants/'
import { validationMessages } from '~/utils'
import { EditAudioRequest, UploadAudioRequest } from './types/audio';

const { 
  usernameMinLength,
  usernameMaxLength,
  usernameAllowedChars,
  passwordRequiresDigit: passwordRequireDigit, 
  passwordRequiresLowercase: passwordRequireLowercase, 
  passwordRequiresNonAlphanumeric: passwordRequireNonAlphanumeric, 
  passwordRequiresUppercase: passwordRequireUppercase, 
  passwordMinimumLength: passwordMinLength } = CONSTANTS.IDENTITY_OPTIONS;

export const usernameRule = (label: string, isRequired: boolean = true) => {
  let schema = string();

  schema = isRequired && schema.required();

  schema = usernameMinLength
    && schema.min(usernameMinLength, validationMessages.min(label, usernameMinLength));

  schema = usernameMaxLength 
    && schema.max(usernameMaxLength, validationMessages.max(label, usernameMaxLength));

  schema = schema.test(
    'allowedCharacters',
    "Username can only contain uppercase, lowercase, numbers, hyphens, or underscores.",
    (value) => {
      for(const char of value) {
        if (usernameAllowedChars.indexOf(char) == -1) {
          return false;
        }
      }
      return true;
    }
  );

  return schema;
}

export const passwordRule = (label: string, isRequired: boolean = true) => {
  let schema = string();

  schema = isRequired && schema.required();

  schema = passwordMinLength 
    && schema.min(passwordMinLength, validationMessages.min(label, passwordMinLength));

  schema = passwordRequireDigit
    && schema.matches(/^[0-9]+$/, "Password must contain one digit.");

  schema = passwordRequireLowercase
    && schema.matches(/^[a-z]+$/, "Password must contain one lowercase character.");

  schema = passwordRequireUppercase
    && schema.matches(/^[A-Z]+$/, "Password must contain one uppercase character.");

  schema = passwordRequireNonAlphanumeric
    && schema.matches(/^[^a-zA-Z\d]+$/, "Password must contain one non-alphanumeric character.");

  return schema;
}

export const editAudioSchema: SchemaOf<EditAudioRequest> = object().shape({
  title: string()
    .required(validationMessages.required("Title"))
    .max(30, validationMessages.max("Title", 30))
    .defined(),
  description: string()
    .max(500, validationMessages.max("Description", 500))
    .defined(),
  tags: array(string())
    .max(10, validationMessages.max("Tags", 10))
    .ensure()
    .defined(),
  genre: string()
    .required(validationMessages.required("Genre"))
    .defined(),
  isPublic: boolean()
    .defined()
}).defined();

export const uploadAudioSchema: SchemaOf<UploadAudioRequest> = object().shape({
  file: mixed()
    .required(validationMessages.required("File"))
    .defined(),
  title: string()
    .required(validationMessages.required("Title"))
    .max(30, validationMessages.max("Title", 30))
    .defined(),
  description: string()
    .max(500, validationMessages.max("Description", 500))
    .defined(),
  tags: array(string())
    .max(10, validationMessages.max("Tags", 10))
    .ensure()
    .defined(),
  genre: string()
    .required(validationMessages.required("Genre"))
    .defined(),
  isPublic: boolean()
    .defined(),
  acceptTerms: boolean()
    .required()
    .oneOf([true], "You must accept terms of service.")
    .defined()
}).defined();
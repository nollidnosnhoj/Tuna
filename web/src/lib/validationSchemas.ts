import * as yup from 'yup'
import CONSTANTS from '~/constants/'
import { validationMessages } from '~/utils'
import { formatFileSize } from '~/utils/format';

const { 
  usernameMinLength,
  usernameMaxLength,
  usernameAllowedChars,
  passwordRequireDigit, 
  passwordRequireLowercase, 
  passwordRequireNonAlphanumeric, 
  passwordRequireUppercase, 
  passwordMinLength } = CONSTANTS.IDENTITY_SETTINGS;

export const usernameRule = (label: string, isRequired: boolean = true) => {
  let rule = yup.string();

  rule = isRequired && rule.required();

  rule = usernameMinLength
    && rule.min(usernameMinLength, validationMessages.min(label, usernameMinLength));

  rule = usernameMaxLength 
    && rule.max(usernameMaxLength, validationMessages.max(label, usernameMaxLength));

  rule = rule.test(
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

  return rule;
}

export const passwordRule = (label: string, isRequired: boolean = true) => {
  let rule = yup.string();

  rule = isRequired && rule.required();

  rule = passwordMinLength 
    && rule.min(passwordMinLength, validationMessages.min(label, passwordMinLength));

  rule = passwordRequireDigit
    && rule.matches(/^[0-9]+$/, "Password must contain one digit.");

  rule = passwordRequireLowercase
    && rule.matches(/^[a-z]+$/, "Password must contain one lowercase character.");

  rule = passwordRequireUppercase
    && rule.matches(/^[A-Z]+$/, "Password must contain one uppercase character.");

  rule = passwordRequireNonAlphanumeric
    && rule.matches(/^[^a-zA-Z\d]+$/, "Password must contain one non-alphanumeric character.");

  return rule;
}

export const audioSchema = (type: "create" | "edit") => {
  const main = yup.object().shape({
    title: yup.string().required(validationMessages.required("Title")).max(30),
    description: yup.string().max(500, validationMessages.max("Description", 500)),
    tags: yup.array(yup.string()).max(10, validationMessages.max("Tags", 10)).ensure(),
    genre: yup.string().required(),
    isPublic: yup.boolean()
  });

  if (type === 'create') {
    const { accept, maxSize } = CONSTANTS.UPLOAD_RULES;

    const uploadSchema = yup.object().shape({
      file: yup
        .mixed()
        .required()
        .test({
          name: 'maxSize',
          message: `File cannot be greater than 2GB.`,
          test: (value) => value != null && value.size <= maxSize
        })
        .test({
          name: 'accept',
          message: 'File has an invalid content-type.',
          test: (value) => value != null && accept.includes(value.type)
        }),
      acceptTerms: yup
        .boolean()
        .required()
        .oneOf([true], "You must accept terms of service."),
    });

    return main.concat(uploadSchema);
  }

  return main;
}
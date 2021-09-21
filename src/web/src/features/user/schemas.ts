import { z, ZodType } from "zod";
import SETTINGS from "~/lib/config";
import { validationMessages } from "~/utils";

const {
  usernameMinLength,
  usernameMaxLength,
  usernameAllowedChars,
  passwordRequiresDigit: passwordRequireDigit,
  passwordRequiresLowercase: passwordRequireLowercase,
  passwordRequiresNonAlphanumeric: passwordRequireNonAlphanumeric,
  passwordRequiresUppercase: passwordRequireUppercase,
  passwordMinimumLength: passwordMinLength,
} = SETTINGS.IDENTITY;

export const usernameRule = (label: string): ZodType<string> => {
  return z.string().superRefine((arg, ctx) => {
    for (const char of arg) {
      if (usernameAllowedChars.indexOf(char) == -1) {
        ctx.addIssue({
          code: "custom",
          message:
            "Username can only contain lowercase, numbers, hyphens, or underscores.",
        });
        break;
      }
    }

    if (usernameMinLength && arg.length < usernameMinLength) {
      ctx.addIssue({
        code: "too_small",
        minimum: usernameMinLength,
        inclusive: false,
        type: "string",
        message: validationMessages.min(label, usernameMinLength),
      });
    }

    if (usernameMaxLength && arg.length > usernameMaxLength) {
      ctx.addIssue({
        code: "too_big",
        inclusive: false,
        maximum: usernameMaxLength,
        type: "string",
        message: validationMessages.max(label, usernameMaxLength),
      });
    }
  });
};

export const passwordRule = (label: string): ZodType<string> => {
  return z.string().superRefine((arg, ctx) => {
    if (passwordMinLength && arg.length < passwordMinLength) {
      ctx.addIssue({
        code: "too_small",
        inclusive: false,
        minimum: passwordMinLength,
        type: "string",
        message: validationMessages.min(label, passwordMinLength),
      });
    }

    if (passwordRequireDigit && !/[\d]+/.test(arg)) {
      ctx.addIssue({
        code: "invalid_string",
        validation: "regex",
        message: "Password must contain one digit.",
      });
    }

    if (passwordRequireUppercase && !/[a-z]+/.test(arg)) {
      ctx.addIssue({
        code: "invalid_string",
        validation: "regex",
        message: "Password must contain one uppercase character.",
      });
    }

    if (passwordRequireLowercase && !/[a-z]+/.test(arg)) {
      ctx.addIssue({
        code: "invalid_string",
        validation: "regex",
        message: "Password must contain one lowercase character.",
      });
    }

    if (passwordRequireNonAlphanumeric && !/[^a-zA-Z\d]+/.test(arg)) {
      ctx.addIssue({
        code: "invalid_string",
        validation: "regex",
        message: "Password must contain one non-alphanumeric character.",
      });
    }
  });
};

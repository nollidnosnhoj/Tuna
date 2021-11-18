import { z, ZodType } from "zod";
import SETTINGS from "~/lib/config";

const DefaultUsernameRules = SETTINGS.IDENTITY.usernameRules;
const DefaultPasswordRules = SETTINGS.IDENTITY.passwordRules;

export type UsernameRulesType = Partial<typeof DefaultUsernameRules>;
export type PasswordRulesType = Partial<typeof DefaultPasswordRules>;

export const usernameRule = (
  label: string,
  rules: UsernameRulesType = DefaultUsernameRules
): ZodType<string> => {
  const { allowedCharacters, maxLength, minLength } = rules;

  if (maxLength && maxLength < 0) throw Error("maxLength cannot be negative.");
  if (minLength && minLength < 0) throw Error("minLength cannot be negative.");

  return z.string().superRefine((arg, ctx) => {
    if (allowedCharacters) {
      for (const char of arg) {
        if (allowedCharacters.indexOf(char) == -1) {
          ctx.addIssue({
            code: "custom",
            message:
              "Username can only contain lowercase, numbers, hyphens, or underscores.",
          });
          break;
        }
      }
    }

    if (minLength && arg.length < minLength) {
      ctx.addIssue({
        code: "too_small",
        minimum: minLength,
        inclusive: false,
        type: "string",
      });
    }

    if (maxLength && arg.length > maxLength) {
      ctx.addIssue({
        code: "too_big",
        inclusive: false,
        maximum: maxLength,
        type: "string",
      });
    }
  });
};

export const passwordRule = (
  label: string,
  rules: PasswordRulesType = DefaultPasswordRules
): ZodType<string> => {
  const {
    minLength,
    requiresDigit,
    requiresLowercase,
    requiresNonAlphanumeric,
    requiresUppercase,
  } = rules;

  if (minLength && minLength < 0) throw Error("minLength cannot be negative.");

  return z.string().superRefine((arg, ctx) => {
    if (minLength && arg.length < minLength) {
      ctx.addIssue({
        code: "too_small",
        inclusive: false,
        minimum: minLength,
        type: "string",
      });
    }

    if (requiresDigit && !/[\d]+/.test(arg)) {
      ctx.addIssue({
        code: "invalid_string",
        validation: "regex",
        message: "Password must contain one digit.",
      });
    }

    if (requiresUppercase && !/[A-Z]+/.test(arg)) {
      ctx.addIssue({
        code: "invalid_string",
        validation: "regex",
        message: "Password must contain one uppercase character.",
      });
    }

    if (requiresLowercase && !/[a-z]+/.test(arg)) {
      ctx.addIssue({
        code: "invalid_string",
        validation: "regex",
        message: "Password must contain one lowercase character.",
      });
    }

    if (requiresNonAlphanumeric && !/[^a-zA-Z\d]+/.test(arg)) {
      ctx.addIssue({
        code: "invalid_string",
        validation: "regex",
        message: "Password must contain one non-alphanumeric character.",
      });
    }
  });
};

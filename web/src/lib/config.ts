export default {
  IS_PRODUCTION: process.env.NEXT_PUBLIC_IS_PRODUCTION,
  BACKEND_API: process.env.NEXT_PUBLIC_BACKEND_API,
  IDENTITY: {
    usernameMinLength: 3,
    usernameMaxLength: 20,
    usernameAllowedChars: "abcdefghijklmnopqrstuvwxyz-_",
    passwordMinimumLength: 6,
    passwordRequiresDigit: true,
    passwordRequiresLowercase: true,
    passwordRequiresUppercase: true,
    passwordRequiresNonAlphanumeric: false,
  },
  UPLOAD: {
    AUDIO: {
      accept: ["audio/mp3", "audio/mpeg"],
      maxSize: 262144000,
    },
    IMAGE: {
      accept: ["image/jpeg", "image/png", "image/gif"],
      maxSize: 2097152,
    },
  },
};

export default {
  BACKEND_API: process.env.NEXT_PUBLIC_BACKEND_API,
  FRONTEND_URL: process.env.NEXT_PUBLIC_FRONTEND_URL,
  IDENTITY: {
    usernameRules: {
      minLength: 3,
      maxLength: 20,
      allowedCharacters: "0123456789abcdefghijklmnopqrstuvwxyz-_",
    },
    passwordRules: {
      minLength: 6,
      requiresDigit: true,
      requiresUppercase: true,
      requiresLowercase: true,
      requiresNonAlphanumeric: false,
    },
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

export default {
  API_URL: 'http://localhost:5000/',
  IDENTITY_OPTIONS: {
    usernameMinLength: 3,
    usernameMaxLength: 20,
    usernameAllowedChars: "abcdefghijklmnopqrstuvwxyz-_",
    passwordMinimumLength: 6,
    passwordRequiresDigit: true,
    passwordRequiresLowercase: true,
    passwordRequiresUppercase: true,
    passwordRequiresNonAlphanumeric: false
  },
  UPLOAD_RULES: {
    accept: [
      "audio/mpeg",
      "audio/x-mpeg",
      "audio/mp3",
      "audio/x-mp3",
      "audio/mpeg3",
      "audio/x-mpeg3",
      "audio/mpg",
      "audio/x-mpg",
      "audio/x-mpegaudio",
    ],
    maxSize: 2000000000
  }
}
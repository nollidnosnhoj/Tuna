export default {
  API_URL: 'http://localhost:5000/',
  IDENTITY_SETTINGS: {
    usernameMinLength: 3,
    usernameMaxLength: 20,
    usernameAllowedChars: "abcdefghijklmnopqrstuvwxyz-_",
    passwordMinLength: 6,
    passwordRequireDigit: true,
    passwordRequireLowercase: true,
    passwordRequireUppercase: true,
    passwordRequireNonAlphanumeric: false
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
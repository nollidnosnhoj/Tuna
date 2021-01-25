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
    accept: ['.mp3', '.aac', '.ogg'],
    maxSize: 2000000000
  }
}
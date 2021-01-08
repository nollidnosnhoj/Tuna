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
  }
}
import { artistSignUpValidationSchema } from "./ArtistSignUpForm";

describe("registration validation schema", () => {
  it("successfully validates", () => {
    const result = artistSignUpValidationSchema.safeParse({
      username: "testuser",
      password: "Password1!",
      confirmPassword: "Password1!",
      email: "testuser@example.com",
    });
    expect(result.success).toBeTruthy();
  });
});

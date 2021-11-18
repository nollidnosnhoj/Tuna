import { userSignUpValidationSchema } from "./UserSignUpForm";

describe("registration validation schema", () => {
  it("successfully validates", () => {
    const result = userSignUpValidationSchema.safeParse({
      username: "testuser",
      password: "Password1!",
      confirmPassword: "Password1!",
      email: "testuser@example.com",
    });
    expect(result.success).toBeTruthy();
  });
});

import { registrationValidationSchema } from "./RegisterForm";

describe("registration validation schema", () => {
  it("successfully validates", () => {
    const result = registrationValidationSchema.safeParse({
      username: "testuser",
      password: "Password1!",
      confirmPassword: "Password1!",
      email: "testuser@example.com",
    });
    expect(result.success).toBeTruthy();
  });
});

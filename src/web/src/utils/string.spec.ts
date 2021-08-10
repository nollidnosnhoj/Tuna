import { getIdAndSlugFromSlug } from "./string";

describe("getIdAndSlugFromSlug", () => {
  it("Successfully obtain id", () => {
    const idSlug = "12345";
    const [id, slug] = getIdAndSlugFromSlug(idSlug);
    expect(id).toBe(12345);
    expect(slug).toBe("");
  });

  it("Successfully obtain id and slug", () => {
    const idSlug = "12345-this-is-a-slug";
    const [id, slug] = getIdAndSlugFromSlug(idSlug);
    expect(id).toBe(12345);
    expect(slug).toBe("this-is-a-slug");
  });

  it("Should obtain invalid id", () => {
    const idSlug = "this-is-a-slug";
    const [id, slug] = getIdAndSlugFromSlug(idSlug);
    expect(id).toBeFalsy();
    expect(slug).toBeFalsy();
  });
});

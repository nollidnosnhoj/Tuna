import slugify from "slugify";

export function taggify(value: string): string {
  return slugify(value, {
    replacement: "-",
    lower: true,
    strict: true,
  });
}

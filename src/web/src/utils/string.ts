import slugify from "slugify";

export function taggify(value: string): string {
  return slugify(value, {
    replacement: "-",
    lower: true,
    strict: true,
  });
}

export function getIdAndSlugFromSlug(slug: string): [number, string] {
  let id = parseInt(slug, 10);
  if (isNaN(id)) return [0, ""];
  const splits = slug.split("-");
  const idString = splits[0];
  const slugString = splits.slice(1).join("-");
  id = parseInt(idString, 10);
  const valid = !isNaN(id);
  return [valid ? id : 0, valid ? slugString : ""];
}

import slugify from "slugify";
import { IdSlug } from "~/lib/types";

export function taggify(value: string): string {
  return slugify(value, {
    replacement: "-",
    lower: true,
    strict: true,
  });
}

export function getIdAndSlugFromSlug(slug: IdSlug): [number, string] {
  // Return the id if the input is a number
  if (typeof slug === "number") return [slug, ""];

  // Check to see if the string is a number
  let id = parseInt(slug, 10);
  if (isNaN(id)) return [0, ""];

  // Parse the slug
  const splits = slug.split("-");
  const idString = splits[0];
  const slugString = splits.slice(1).join("-");
  id = parseInt(idString, 10);
  const valid = !isNaN(id);
  return [valid ? id : 0, valid ? slugString : ""];
}

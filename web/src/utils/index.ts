import slugify from "slugify";

export const validationMessages = {
  required: function (field: string) {
    return `${field} is required.`;
  },
  min: function (field: string, min: number) {
    return `${field} must be at least ${min} characters long.`;
  },
  max: function (field: string, max: number) {
    return `${field} must be no more than ${max} characters long.`;
  },
};

export function taggify(value: string) {
  return slugify(value, {
    replacement: '-',
    lower: true,
    strict: true
  });
}

export function getThumbnailUrl(url: string, size: 'lg' | 'md' | 'sm' = 'lg') {
  if (size === 'lg') return url;

  let pixels: number;
  switch(size) {
    case 'md':
      pixels = 200;
      break;
    case 'sm':
      pixels = 100;
      break;
  }

  const [filePath, fileExt] = url.split('.');
  var thumbnailUrl = filePath + `-${pixels}`;
  return thumbnailUrl + fileExt;
}
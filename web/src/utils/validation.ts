export const validationMessages = {
  required: function (field: string): string {
    return `${field} is required.`;
  },
  min: (field: string, min: number): string => {
    return `${field} must be at least ${min} characters long.`;
  },
  max: function (field: string, max: number): string {
    return `${field} must be no more than ${max} characters long.`;
  },
};

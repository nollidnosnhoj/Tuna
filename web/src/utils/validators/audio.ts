import * as yup from 'yup'

export const audioSchema = () => {
  const schema = yup.object().shape({
    title: yup.string().required().max(30),
    description: yup.string().max(500),
    tags: yup.array(yup.string()).max(10).ensure(),
    isPublic: yup.boolean(),
  });

  return schema
}
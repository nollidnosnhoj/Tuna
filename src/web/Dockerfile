FROM mhart/alpine-node:14

ENV PORT 3000
ENV NODE_ENV="production"

RUN apk add git

WORKDIR /usr/app
COPY . .

RUN yarn install --production
RUN yarn build

EXPOSE 3000
CMD ["yarn", "start"]
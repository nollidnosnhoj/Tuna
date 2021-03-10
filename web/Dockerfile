FROM mhart/alpine-node:14

# setup environment variables
ENV PORT 3000
ENV NODE_ENV="production"

RUN apk add git

# move project into app folder
WORKDIR /usr/src/app
COPY . /usr/src/app

# install packages and build project
RUN yarn install --production
RUN yarn build

# start server
EXPOSE 3000
CMD ["yarn", "start"]
# Audiochan [Name WIP]

Audiochan is a website that allow users to upload their music and share to people. This also could be a repository for content creators to search for audios to use on their work, as long as they give credit where credit is due.

This is a personal project I have been working on for a while to help me understand how to develop a full-stack web application. If there is any suggestions or advice, you can contact me or create an issue expressing your feedback. I would love to hear them so I can improve as a developer.

The stack includes ASP.NET Core as the backend, and Next.js as the frontend framework. I am also using PostgreSQL for data persistence, and Amazon S3 for cloud storage. I am also structuring my backend to be a hybrid between Vertical Slicing and Clean Architecture, although I am not sure how I feel about clean architecture.

## Demos

- [Demo #1](https://www.youtube.com/watch?v=XFWvhNB-YW4). Demonstrates uploading and audio player.

## Branches

- `staging` - Pull request from and to this branch
- `production` - Do not touch this! This branch goes live.

## TODOS

- Dockerize
- Make frontend more mobile-responsive

## Tech Stack

### Backend

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Serilog
- Swagger
- FluentValidation
- Mediatr
- ASP.NET Core Identity
- Imagesharp (thumbnail creation)
- Amazon S3 (media storage)

### Frontend

- Next.js
- Chakra UI
- Formik

## Contributing

TBA

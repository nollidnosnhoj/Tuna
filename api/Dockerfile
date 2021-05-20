#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

# Copy csproj files to its project folders
COPY *.sln .
COPY src/Audiochan/*.csproj Audiochan/
COPY src/Audiochan.Core/*.csproj Audiochan.Core/
COPY src/Audiochan.Infrastructure/*.csproj Audiochan.Infrastructure/
COPY tests/Audiochan.IntegrationTests/*.csproj Audiochan.IntegrationTests/
COPY tests/Audiochan.UnitTests/*.csproj Audiochan.UnitTests

RUN dotnet restore

COPY src/. .
COPY tests/ .

WORKDIR /src/Audiochan
RUN dotnet build "./Audiochan.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Audiochan.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Audiochan.dll"]
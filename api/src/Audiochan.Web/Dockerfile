#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Audiochan.Web/Audiochan.Web.csproj", "src/Audiochan.Web/"]
COPY ["src/Audiochan.Infrastructure/Audiochan.Infrastructure.csproj", "src/Audiochan.Infrastructure/"]
COPY ["src/Audiochan.Core/Audiochan.Core.csproj", "src/Audiochan.Core/"]
RUN dotnet restore "src/Audiochan.Web/Audiochan.Web.csproj"
COPY . .
WORKDIR "/src/src/Audiochan.Web"
RUN dotnet build "Audiochan.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Audiochan.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Audiochan.Web.dll"]
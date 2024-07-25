# Установка временной зоны в Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8081
EXPOSE 10001

ENV TZ=Europe/Moscow

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TelegramPoster.Background/TelegramPoster.Background.csproj", "TelegramPoster.Background/"]
COPY ["TelegramPoster.Persistence/TelegramPoster.Persistence.csproj", "TelegramPoster.Persistence/"]
COPY ["TelegramPoster.Application/TelegramPoster.Application.csproj", "TelegramPoster.Application/"]
COPY ["TelegramPoster.Auth/TelegramPoster.Auth.csproj", "TelegramPoster.Auth/"]
COPY ["TelegramPoster.Domain/TelegramPoster.Domain.csproj", "TelegramPoster.Domain/"]
RUN dotnet restore "./TelegramPoster.Background/TelegramPoster.Background.csproj"
COPY . .
WORKDIR "/src/TelegramPoster.Background"
RUN dotnet build "./TelegramPoster.Background.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TelegramPoster.Background.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY /TelegramPoster.Background/appsettings.json .
ENTRYPOINT ["dotnet", "TelegramPoster.Background.dll"]

#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 10000

ENV TZ=Europe/Moscow

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TelegramPoster.Api/TelegramPoster.Api.csproj", "TelegramPoster.Api/"]
COPY ["TelegramPoster.Application/TelegramPoster.Application.csproj", "TelegramPoster.Application/"]
COPY ["TelegramPoster.Auth/TelegramPoster.Auth.csproj", "TelegramPoster.Auth/"]
COPY ["TelegramPoster.Domain/TelegramPoster.Domain.csproj", "TelegramPoster.Domain/"]
COPY ["TelegramPoster.Persistence/TelegramPoster.Persistence.csproj", "TelegramPoster.Persistence/"]
COPY ["Utility/Utility.csproj", "Utility/"]
RUN dotnet restore "./TelegramPoster.Api/TelegramPoster.Api.csproj"
COPY . .
WORKDIR "/src/TelegramPoster.Api"
RUN dotnet build "./TelegramPoster.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TelegramPoster.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY /TelegramPoster.Api/appsettings.json .
ENTRYPOINT ["dotnet", "TelegramPoster.Api.dll"]

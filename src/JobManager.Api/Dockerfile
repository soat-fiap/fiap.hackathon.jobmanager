﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./*.sln /
COPY src ./src

# Restoring just the api project will cause a chain reaction that will resolve the nuget dependencies from all class libraries.
RUN dotnet restore src/JobManager.Api

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG API_VERSION
RUN dotnet publish "src/JobManager.Api/JobManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobManager.Api.dll"]


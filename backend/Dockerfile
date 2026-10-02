# syntax=docker/dockerfile:1

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first (better layer caching): copy only the project files the API needs.
COPY src/ProjectTracker.Api/ProjectTracker.Api.csproj            src/ProjectTracker.Api/
COPY src/ProjectTracker.Application/ProjectTracker.Application.csproj  src/ProjectTracker.Application/
COPY src/ProjectTracker.Domain/ProjectTracker.Domain.csproj      src/ProjectTracker.Domain/
COPY src/ProjectTracker.Infrastructure/ProjectTracker.Infrastructure.csproj src/ProjectTracker.Infrastructure/
COPY src/ProjectTracker.Shared/ProjectTracker.Shared.csproj      src/ProjectTracker.Shared/
RUN dotnet restore src/ProjectTracker.Api/ProjectTracker.Api.csproj

# Then copy the rest of the source and publish.
COPY src/ src/
RUN dotnet publish src/ProjectTracker.Api/ProjectTracker.Api.csproj -c Release -o /app/publish --no-restore

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Uploaded files live here; mount a volume to persist them across container restarts.
RUN mkdir -p /app/App_Data/uploads

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProjectTracker.Api.dll"]

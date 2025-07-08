# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

# Copy project files for dependency restore (excluding unit tests)
COPY NuGet.config ./
COPY MathMax.Administration.WebApi/*.csproj ./MathMax.Administration.WebApi/
COPY MathMax.Administration/*.csproj ./MathMax.Administration/
COPY MathMax.EventSourcing.Api/*.csproj ./MathMax.EventSourcing.Api/
COPY MathMax.EventSourcing.Database/*.csproj ./MathMax.EventSourcing.Database/
COPY MathMax.EventSourcing.Infrastructure/*.csproj ./MathMax.EventSourcing.Infrastructure/
COPY MathMax.EventSourcing/*.csproj ./MathMax.EventSourcing/
COPY MathMax.WebApi/*.csproj ./MathMax.WebApi/

# Clear NuGet cache and restore dependencies for the main project
RUN dotnet nuget locals all --clear
RUN dotnet restore MathMax.Administration.WebApi/MathMax.Administration.WebApi.csproj

# Copy source code and build (excluding unit tests via .dockerignore)
COPY . ./
RUN dotnet publish MathMax.Administration.WebApi/MathMax.Administration.WebApi.csproj \
    -c Development -o out --no-self-contained

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Install security updates and create non-root user
RUN apk update && apk upgrade && apk add --no-cache curl && \
    adduser -D appuser && chown -R appuser:appuser /app
USER appuser

# Copy published app
COPY --from=build --chown=appuser:appuser /app/out ./

# Configure environment
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose ports
EXPOSE 80 443

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

# Set the entrypoint
ENTRYPOINT ["dotnet", "MathMax.Administration.WebApi.dll"]

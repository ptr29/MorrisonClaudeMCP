# MorrisonClaudeMCP Server Dockerfile
# Multi-stage build for optimized container size

# ============================================================================
# Build Stage - SDK image for compilation
# ============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["KateMorrisonMCP.sln", "./"]
COPY ["src/KateMorrisonMCP.Server/KateMorrisonMCP.Server.csproj", "src/KateMorrisonMCP.Server/"]
COPY ["src/KateMorrisonMCP.Tools/KateMorrisonMCP.Tools.csproj", "src/KateMorrisonMCP.Tools/"]
COPY ["src/KateMorrisonMCP.Data/KateMorrisonMCP.Data.csproj", "src/KateMorrisonMCP.Data/"]
COPY ["src/KateMorrisonMCP.Ingestion/KateMorrisonMCP.Ingestion.csproj", "src/KateMorrisonMCP.Ingestion/"]
COPY ["src/KateMorrisonMCP.Migration/KateMorrisonMCP.Migration.csproj", "src/KateMorrisonMCP.Migration/"]
COPY ["tests/KateMorrisonMCP.Tests/KateMorrisonMCP.Tests.csproj", "tests/KateMorrisonMCP.Tests/"]

# Restore dependencies
RUN dotnet restore "KateMorrisonMCP.sln"

# Copy all source code
COPY . .

# Build the Server project
WORKDIR /src/src/KateMorrisonMCP.Server
RUN dotnet build "KateMorrisonMCP.Server.csproj" -c Release -o /app/build

# ============================================================================
# Publish Stage - Create optimized release build
# ============================================================================
FROM build AS publish
RUN dotnet publish "KateMorrisonMCP.Server.csproj" -c Release -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ============================================================================
# Ingestion Tool Build (optional, for data loading)
# ============================================================================
FROM build AS publish-ingestion
WORKDIR /src/src/KateMorrisonMCP.Ingestion
RUN dotnet publish "KateMorrisonMCP.Ingestion.csproj" -c Release -o /app/publish-ingestion \
    --no-restore \
    /p:UseAppHost=false

# ============================================================================
# Runtime Stage - ASP.NET Core runtime image (required for HTTP/SSE support)
# ============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install SQLite (usually included, but explicit for clarity)
RUN apt-get update && apt-get install -y \
    sqlite3 \
    && rm -rf /var/lib/apt/lists/*

# Copy published server application
COPY --from=publish /app/publish .

# Copy ingestion tool (optional)
COPY --from=publish-ingestion /app/publish-ingestion ./ingestion/

# Create data directory for database and source files
RUN mkdir -p /app/data /app/data/Background

# Copy database and seed data
COPY ["data/canonical_facts.db", "/app/data/"]
COPY ["data/create_schema.sql", "/app/data/"]
COPY ["data/canonical_facts_data.sql", "/app/data/"]

# Copy background markdown files (if they exist)
# Commented out due to file permission issues - database is already populated
# COPY ["Background/", "/app/data/Background/"] 2>/dev/null || true

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DatabasePath=/app/data/canonical_facts.db
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Optional: Set MCP_PORT to enable HTTP/SSE mode (default is stdio)
# ENV MCP_PORT=8080
# ENV MCP_API_KEY=your-api-key-here

# Set permissions
RUN chmod -R 755 /app && \
    chmod 666 /app/data/canonical_facts.db

# Port exposure (only needed for HTTP/SSE mode)
# Uncomment if running in HTTP mode with MCP_PORT set
EXPOSE 8080

# Health check (verify database is accessible)
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD sqlite3 /app/data/canonical_facts.db "SELECT 1;" || exit 1

# Set working directory
WORKDIR /app

# Entry point - Run the MCP server
ENTRYPOINT ["dotnet", "KateMorrisonMCP.Server.dll"]

# Labels for metadata
LABEL maintainer="Kate Morrison MCP"
LABEL description="Model Context Protocol server for Kate Morrison universe facts"
LABEL version="1.0"

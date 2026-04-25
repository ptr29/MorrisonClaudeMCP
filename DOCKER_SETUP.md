# MorrisonClaudeMCP Docker Setup Guide

This guide explains how to build and run the MorrisonClaudeMCP server in a Docker container.

## Quick Start

### 1. Build the Container

```bash
# Build using Docker
docker build -t morrison-mcp:latest .

# Or build using docker-compose
docker-compose build
```

### 2. Run the MCP Server

```bash
# Run using docker-compose (recommended)
docker-compose up -d mcp-server

# Or run using Docker directly
docker run -it \
  -v $(pwd)/data:/app/data \
  morrison-mcp:latest
```

### 3. Test the Server

```bash
# Check logs
docker-compose logs -f mcp-server

# Verify database is accessible
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "SELECT COUNT(*) FROM characters;"

# Health check
docker-compose ps
```

## Container Architecture

The container uses a **multi-stage build**:

1. **Build Stage**: Uses .NET 8.0 SDK to compile the application
2. **Publish Stage**: Creates optimized release builds
3. **Runtime Stage**: Minimal .NET 8.0 runtime image with compiled binaries

**Final Image Size**: ~200-250MB

## Volume Mounts

### Required Volume
- `./data:/app/data` - **Required** for database persistence

### Optional Volumes
- `./Background:/app/data/Background:ro` - Source markdown files for data ingestion (read-only)

## Environment Variables

Configure the container using these environment variables:

| Variable | Default | Description |
|----------|---------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Runtime environment |
| `DatabasePath` | `/app/data/canonical_facts.db` | SQLite database file path |
| `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT` | `false` | Enable globalization support |
| `Logging__LogLevel__Default` | `Information` | Default log level |
| `Logging__LogLevel__Microsoft` | `Warning` | Microsoft framework log level |

## Using with Claude Desktop

### Method 1: Docker Compose (Recommended)

Update your Claude Desktop MCP configuration (`claude_desktop_config.json`):

```json
{
  "mcpServers": {
    "morrison-facts": {
      "command": "docker",
      "args": [
        "compose",
        "-f",
        "/Users/paulrobak/Documents/GitHub/MorrisonClaudeMCP/docker-compose.yml",
        "run",
        "--rm",
        "mcp-server"
      ]
    }
  }
}
```

### Method 2: Direct Docker Run

```json
{
  "mcpServers": {
    "morrison-facts": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "-i",
        "-v",
        "/Users/paulrobak/Documents/GitHub/MorrisonClaudeMCP/data:/app/data:ro",
        "morrison-mcp:latest"
      ]
    }
  }
}
```

### Method 3: Docker Exec into Running Container

1. Start container in background:
```bash
docker-compose up -d mcp-server
```

2. Configure Claude Desktop:
```json
{
  "mcpServers": {
    "morrison-facts": {
      "command": "docker",
      "args": [
        "exec",
        "-i",
        "morrison-mcp-server",
        "dotnet",
        "KateMorrisonMCP.Server.dll"
      ]
    }
  }
}
```

## Data Ingestion

To populate or update the database from Background markdown files:

```bash
# Run the ingestion tool
docker-compose --profile tools run --rm ingestion

# Or run manually
docker run --rm \
  -v $(pwd)/data:/app/data \
  -v $(pwd)/Background:/app/data/Background:ro \
  morrison-mcp:latest \
  dotnet /app/ingestion/KateMorrisonMCP.Ingestion.dll
```

## Database Management

### Backup Database

```bash
# Create backup
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db ".backup /app/data/canonical_facts_backup_$(date +%Y%m%d).db"

# Or copy from host
cp data/canonical_facts.db data/canonical_facts_backup_$(date +%Y%m%d).db
```

### Restore Database

```bash
# Stop container
docker-compose down

# Restore from backup
cp data/canonical_facts_backup_YYYYMMDD.db data/canonical_facts.db

# Restart container
docker-compose up -d
```

### Initialize Fresh Database

```bash
# Stop container
docker-compose down

# Remove existing database
rm data/canonical_facts.db

# Recreate from schema and data files
docker run --rm \
  -v $(pwd)/data:/app/data \
  morrison-mcp:latest \
  sh -c "sqlite3 /app/data/canonical_facts.db < /app/data/create_schema.sql && \
         sqlite3 /app/data/canonical_facts.db < /app/data/canonical_facts_data.sql"

# Restart container
docker-compose up -d
```

## Troubleshooting

### Container Won't Start

**Check logs:**
```bash
docker-compose logs mcp-server
```

**Common issues:**
- Database file permissions: `chmod 666 data/canonical_facts.db`
- Missing data directory: `mkdir -p data`
- Port conflicts: Check if another process is using the same resources

### Database Locked

If you see "database is locked" errors:

```bash
# Stop all containers
docker-compose down

# Remove WAL files
rm -f data/canonical_facts.db-shm data/canonical_facts.db-wal

# Restart
docker-compose up -d
```

### Health Check Failing

```bash
# Check database integrity
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "PRAGMA integrity_check;"

# Verify file permissions
docker exec morrison-mcp-server ls -la /app/data/
```

### MCP Communication Issues

For stdio-based MCP servers, ensure:
1. Container runs with `-i` (interactive) flag
2. Container runs with `-t` (TTY) flag for docker-compose
3. `stdin_open: true` and `tty: true` in docker-compose.yml

## Performance Tuning

### Resource Limits

Adjust in `docker-compose.yml`:

```yaml
deploy:
  resources:
    limits:
      cpus: '2.0'      # Increase for better performance
      memory: 1G       # Increase for larger datasets
    reservations:
      cpus: '0.5'
      memory: 256M
```

### SQLite Optimization

For better performance, mount database on SSD and use:

```bash
# Add to database initialization
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;"
```

## Building for Different Architectures

### Multi-Architecture Build

```bash
# Enable BuildKit
export DOCKER_BUILDKIT=1

# Build for linux/amd64 and linux/arm64
docker buildx build --platform linux/amd64,linux/arm64 -t morrison-mcp:latest .
```

### Push to Registry

```bash
# Tag for registry
docker tag morrison-mcp:latest your-registry.com/morrison-mcp:latest

# Push
docker push your-registry.com/morrison-mcp:latest
```

## Security Considerations

1. **Database Permissions**: Container runs with limited permissions
2. **Read-Only Mounts**: Background files are mounted read-only
3. **Network Isolation**: Container doesn't expose ports externally
4. **Resource Limits**: Prevents container from consuming excessive resources

## Advanced Configuration

### Custom appsettings.json

Mount custom configuration:

```bash
docker run -it \
  -v $(pwd)/data:/app/data \
  -v $(pwd)/appsettings.Production.json:/app/appsettings.json:ro \
  morrison-mcp:latest
```

### Enable Debug Logging

```bash
docker run -it \
  -v $(pwd)/data:/app/data \
  -e Logging__LogLevel__Default=Debug \
  morrison-mcp:latest
```

### Use Named Volumes

Instead of bind mounts, use Docker named volumes for better performance:

```yaml
volumes:
  mcp-data:
    driver: local

services:
  mcp-server:
    volumes:
      - mcp-data:/app/data
```

## Cleaning Up

```bash
# Stop and remove containers
docker-compose down

# Remove containers and volumes
docker-compose down -v

# Remove images
docker rmi morrison-mcp:latest

# Clean up build cache
docker builder prune
```

## Next Steps

1. **Verify the build**: Run `docker-compose build` to test the Dockerfile
2. **Test locally**: Start with `docker-compose up` and check logs
3. **Configure Claude Desktop**: Add MCP server configuration
4. **Test MCP tools**: Ask Claude to use the tools to retrieve character information
5. **Set up data ingestion**: Run ingestion tool to populate database from markdown files

## Support

For issues or questions:
- Check container logs: `docker-compose logs -f`
- Verify database: `docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db ".tables"`
- Review MCP connection in Claude Desktop logs

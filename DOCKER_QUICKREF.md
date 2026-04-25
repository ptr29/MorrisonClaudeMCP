# Docker Quick Reference - MorrisonClaudeMCP

## Quick Commands

### Build & Run
```bash
# Build image
./build-container.sh build

# Run with docker-compose
docker-compose up -d

# Run standalone
docker run -it -v $(pwd)/data:/app/data morrison-mcp:latest
```

### Common Operations
```bash
# View logs
docker-compose logs -f mcp-server

# Stop container
docker-compose down

# Restart container
docker-compose restart mcp-server

# Shell into container
docker exec -it morrison-mcp-server /bin/bash

# Check health
docker-compose ps
```

### Database Operations
```bash
# Query database
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "SELECT * FROM characters LIMIT 5;"

# Character count
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "SELECT COUNT(*) FROM characters;"

# Backup database
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db ".backup /app/data/backup_$(date +%Y%m%d).db"

# Database integrity check
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "PRAGMA integrity_check;"
```

### Data Ingestion
```bash
# Run ingestion tool
docker-compose --profile tools run --rm ingestion

# Manual ingestion
docker run --rm \
  -v $(pwd)/data:/app/data \
  -v $(pwd)/Background:/app/data/Background:ro \
  morrison-mcp:latest \
  dotnet /app/ingestion/KateMorrisonMCP.Ingestion.dll
```

### Troubleshooting
```bash
# Check container logs (last 100 lines)
docker-compose logs --tail=100 mcp-server

# Follow logs in real-time
docker-compose logs -f mcp-server

# Inspect container
docker inspect morrison-mcp-server

# Check resource usage
docker stats morrison-mcp-server

# List all MCP containers
docker ps -a --filter name=morrison
```

### Maintenance
```bash
# Rebuild without cache
docker-compose build --no-cache

# Prune unused images
docker image prune

# Remove all stopped containers
docker container prune

# Full cleanup
./build-container.sh cleanup
```

## Claude Desktop Integration

### Configuration File Location
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`

### Configuration Example
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

## Testing MCP Tools

Once running, test in Claude Desktop:

1. **Get Character Info**:
   ```
   Can you tell me about Kate Morrison using the morrison-facts tool?
   ```

2. **Check Relationships**:
   ```
   What is the relationship between Kate and Paul?
   ```

3. **Get Location Details**:
   ```
   Describe Paul's house layout
   ```

4. **Verify Schedule**:
   ```
   What is Kate's weekly schedule?
   ```

5. **Check Character Negatives**:
   ```
   What are some things Kate doesn't do?
   ```

## Common Issues & Solutions

### Database Locked
```bash
docker-compose down
rm -f data/*.db-shm data/*.db-wal
docker-compose up -d
```

### Permission Denied
```bash
chmod 666 data/canonical_facts.db
```

### Out of Disk Space
```bash
docker system prune -a
docker volume prune
```

### Container Won't Start
```bash
# Check logs
docker-compose logs mcp-server

# Rebuild
docker-compose build --no-cache
docker-compose up -d
```

### MCP Not Connecting
1. Check Claude Desktop logs
2. Verify container is running: `docker ps`
3. Test database access: `docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "SELECT 1;"`
4. Restart Claude Desktop

## Resource Management

### Check Disk Usage
```bash
docker system df
```

### Set Memory Limits
Edit `docker-compose.yml`:
```yaml
deploy:
  resources:
    limits:
      memory: 512M
```

### Check Memory Usage
```bash
docker stats --no-stream morrison-mcp-server
```

## Development Workflow

### 1. Make Code Changes
```bash
# Edit source files in src/
```

### 2. Rebuild Container
```bash
./build-container.sh build
```

### 3. Test Changes
```bash
./build-container.sh test
```

### 4. Run Updated Container
```bash
docker-compose down
docker-compose up -d
```

### 5. Verify in Claude Desktop
```
# Ask Claude to use the MCP tools
```

## Backup & Restore

### Backup Everything
```bash
# Stop container
docker-compose down

# Backup data directory
tar -czf morrison-backup-$(date +%Y%m%d).tar.gz data/

# Restart
docker-compose up -d
```

### Restore from Backup
```bash
docker-compose down
tar -xzf morrison-backup-YYYYMMDD.tar.gz
docker-compose up -d
```

## Performance Tuning

### Enable WAL Mode (Better Concurrency)
```bash
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "PRAGMA journal_mode=WAL;"
```

### Optimize Database
```bash
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "VACUUM;"
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "ANALYZE;"
```

### Check Query Performance
```bash
docker exec -it morrison-mcp-server sqlite3 /app/data/canonical_facts.db
# Then in SQLite:
.timer on
SELECT * FROM characters WHERE preferred_name = 'Kate';
```

## Useful One-Liners

```bash
# Get character count
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db "SELECT COUNT(*) FROM characters;"

# List all tables
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db ".tables"

# Get database schema
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db ".schema characters"

# Export table to CSV
docker exec morrison-mcp-server sqlite3 /app/data/canonical_facts.db <<< ".mode csv
.output /tmp/characters.csv
SELECT * FROM characters;"

# Find container IP
docker inspect -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' morrison-mcp-server

# Container size
docker ps -s --filter name=morrison-mcp-server
```

## Environment Variables

Override in docker-compose.yml or command line:

```bash
docker run -it \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e Logging__LogLevel__Default=Debug \
  -e DatabasePath=/app/data/canonical_facts.db \
  -v $(pwd)/data:/app/data \
  morrison-mcp:latest
```

## Multi-Platform Builds

### Build for ARM64 (Apple Silicon)
```bash
docker buildx build --platform linux/arm64 -t morrison-mcp:arm64 .
```

### Build for AMD64 (Intel/AMD)
```bash
docker buildx build --platform linux/amd64 -t morrison-mcp:amd64 .
```

### Build Both
```bash
docker buildx build --platform linux/amd64,linux/arm64 -t morrison-mcp:latest .
```

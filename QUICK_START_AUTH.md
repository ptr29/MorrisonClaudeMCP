# MCP Server Authentication - Quick Start

## Overview

The Morrison MCP server now supports two transport modes:

1. **Stdio Mode** (default) - For local Claude Desktop, no authentication
2. **HTTP/SSE Mode** - For containerized deployment with X-API-Key authentication

## Quick Start

### Local Development (No Auth)

```bash
# Build and run locally
dotnet run --project src/KateMorrisonMCP.Server

# Or use container in stdio mode
docker compose up mcp-server
```

### Container with Authentication

```bash
# Generate a secure API key
API_KEY=$(openssl rand -base64 32)
echo "Your API key: $API_KEY"

# Run in HTTP mode
docker run -d \
  -p 8080:8080 \
  -v $(pwd)/data:/app/data \
  -e MCP_PORT=8080 \
  -e MCP_API_KEY="$API_KEY" \
  morrison-mcp:latest

# Or use docker-compose
docker compose --profile http up mcp-server-http
```

## Testing Authentication

### Health Check (No Auth Required)
```bash
curl http://localhost:8080/health
```

### Initialize MCP (Auth Required)
```bash
# Without auth - fails with 401
curl -X POST http://localhost:8080/sse \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"initialize","params":{"protocolVersion":"2024-11-05"},"id":1}'

# With auth - succeeds
curl -X POST http://localhost:8080/sse \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key-here" \
  -d '{"jsonrpc":"2.0","method":"initialize","params":{"protocolVersion":"2024-11-05"},"id":1}'
```

### Call a Tool (Auth Required)
```bash
curl -X POST http://localhost:8080/sse \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key-here" \
  -d '{
    "jsonrpc": "2.0",
    "method": "tools/call",
    "params": {
      "name": "get_character_facts",
      "arguments": {"character_name": "Kate"}
    },
    "id": 2
  }'
```

## Claude Desktop Integration

### Local Mode (Stdio)
```json
{
  "mcpServers": {
    "kate-morrison-facts": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/path/to/MorrisonClaudeMCP/src/KateMorrisonMCP.Server"
      ]
    }
  }
}
```

### Remote Mode (HTTP with Auth)
```json
{
  "mcpServers": {
    "kate-morrison-facts-remote": {
      "url": "http://localhost:8080/sse",
      "headers": {
        "X-API-Key": "your-api-key-here"
      },
      "transport": "http"
    }
  }
}
```

## Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `MCP_PORT` | none | Port for HTTP mode (e.g., 8080). If not set, uses stdio mode |
| `MCP_API_KEY` | none | API key for authentication. If not set with MCP_PORT, auth is disabled (not recommended) |
| `DATABASE_PATH` | `./data/canonical_facts.db` | Path to SQLite database |

## Security Notes

- **Local mode**: No auth needed, server only accessible to spawning process
- **HTTP mode without MCP_API_KEY**: Server warns but allows unauthenticated access
- **Production**: Always set MCP_API_KEY and use HTTPS reverse proxy (nginx/Caddy)
- **API Key**: Use 32+ character random keys, rotate regularly

## Test Results

```bash
# Stdio mode test
✓ Server starts in stdio mode
✓ Initialize request works
✓ Tool calls work

# HTTP mode test
✓ Server starts on port 8080
✓ Health endpoint accessible without auth
✓ SSE endpoint requires X-API-Key header
✓ Invalid/missing key returns 401
✓ Valid key allows MCP requests
✓ Tool calls work with authentication
```

## Next Steps

1. See [AUTHENTICATION.md](AUTHENTICATION.md) for detailed documentation
2. Update `docker-compose.yml` with your API key
3. Configure Claude Desktop with remote HTTP access
4. Consider adding HTTPS reverse proxy for production

## Troubleshooting

**Server won't start**: Check if port 8080 is already in use
**401 errors**: Verify X-API-Key header matches MCP_API_KEY env var
**No response**: Check firewall settings and container logs

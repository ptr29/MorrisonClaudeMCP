# MCP Server Authentication Guide

The Morrison MCP server supports two transport modes:

1. **Stdio Mode** (default) - No authentication, for local Claude Desktop integration
2. **HTTP/SSE Mode** - With X-API-Key authentication, for containerized deployments

## Transport Modes

### Stdio Mode (Local)

Default mode when `MCP_PORT` is not set. Used for local Claude Desktop integration via stdio.

**No authentication required** - security by process isolation.

```bash
# Run locally
dotnet run

# Or via Docker (stdio mode)
docker compose up mcp-server
```

### HTTP/SSE Mode (Container)

Enabled when `MCP_PORT` environment variable is set. Provides HTTP endpoints with optional authentication.

**Authentication via X-API-Key header** when `MCP_API_KEY` is set.

```bash
# Set environment variables
export MCP_PORT=8080
export MCP_API_KEY=my-secret-key-12345

# Run server
dotnet run
```

## Container Deployment with Authentication

### Docker Compose (Recommended)

Use the HTTP profile:

```bash
# Start server in HTTP mode
docker compose --profile http up mcp-server-http
```

**Important:** Change the default API key in `docker-compose.yml`:

```yaml
environment:
  - MCP_PORT=8080
  - MCP_API_KEY=your-secure-random-api-key-here  # CHANGE THIS!
```

### Docker Run

```bash
docker run -d \
  -p 8080:8080 \
  -v $(pwd)/data:/app/data \
  -e MCP_PORT=8080 \
  -e MCP_API_KEY=your-secure-api-key \
  morrison-mcp:latest
```

### Generate Secure API Key

```bash
# Linux/macOS - generate random 32-character key
openssl rand -base64 32

# Or use a UUID
uuidgen
```

## Using HTTP Mode with Claude Desktop

Claude Desktop can connect to remote MCP servers via HTTP. Add to your Claude Desktop config:

**macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`

```json
{
  "mcpServers": {
    "kate-morrison-facts-remote": {
      "url": "http://localhost:8080/sse",
      "headers": {
        "X-API-Key": "your-secure-api-key"
      },
      "transport": "http"
    }
  }
}
```

## API Endpoints

### POST /sse

Main MCP endpoint for sending JSON-RPC requests.

**Authentication:** X-API-Key header required (when `MCP_API_KEY` is set)

**Request:**
```bash
curl -X POST http://localhost:8080/sse \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-secure-api-key" \
  -d '{"jsonrpc":"2.0","method":"initialize","params":{"protocolVersion":"2024-11-05"},"id":1}'
```

**Response:**
```json
{
  "jsonrpc": "2.0",
  "result": {
    "protocolVersion": "2024-11-05",
    "capabilities": {
      "tools": {}
    },
    "serverInfo": {
      "name": "kate-morrison-canonical-facts",
      "version": "1.0.0"
    }
  },
  "id": 1
}
```

### GET /health

Health check endpoint (no authentication required).

```bash
curl http://localhost:8080/health
```

**Response:**
```json
{
  "status": "healthy",
  "server": "kate-morrison-canonical-facts",
  "version": "1.0.0",
  "transport": "http/sse",
  "authEnabled": true
}
```

### GET /

Server information (no authentication required).

```bash
curl http://localhost:8080/
```

## Security Considerations

### For Local Development (Stdio Mode)
- No authentication needed
- Server only accessible to spawning process
- Claude Desktop spawns server as subprocess

### For Container Deployment (HTTP Mode)
- **Always set MCP_API_KEY** in production
- Use strong, random API keys (32+ characters)
- Rotate keys regularly
- Use environment variables, never hardcode keys
- Consider using HTTPS/TLS reverse proxy (nginx, Caddy) for production

### Example Production Setup

Use a reverse proxy with TLS:

```yaml
# docker-compose.yml
services:
  mcp-server-http:
    # ... MCP server config
    expose:
      - "8080"
    networks:
      - internal

  caddy:
    image: caddy:latest
    ports:
      - "443:443"
    volumes:
      - ./Caddyfile:/etc/caddy/Caddyfile
    networks:
      - internal

networks:
  internal:
```

```
# Caddyfile
mcp.yourdomain.com {
  reverse_proxy mcp-server-http:8080
}
```

## Environment Variables Reference

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `MCP_PORT` | No | none | Port to listen on (enables HTTP mode) |
| `MCP_API_KEY` | No | none | API key for X-API-Key authentication |
| `DATABASE_PATH` | No | `./data/canonical_facts.db` | Path to SQLite database |

## Testing Authentication

### Valid Request (Authenticated)
```bash
curl -X POST http://localhost:8080/sse \
  -H "X-API-Key: your-secure-api-key" \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"tools/list","id":1}'
```

### Invalid Request (Unauthorized)
```bash
curl -X POST http://localhost:8080/sse \
  -H "X-API-Key: wrong-key" \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"tools/list","id":1}'
```

**Response:**
```
HTTP/1.1 401 Unauthorized
{
  "error": "Unauthorized - Invalid or missing X-API-Key header"
}
```

## Troubleshooting

### Server starts but Claude Desktop can't connect
- Check firewall settings
- Verify port is accessible: `curl http://localhost:8080/health`
- Check API key matches in both server and Claude config
- Check server logs for authentication failures

### 401 Unauthorized errors
- Verify X-API-Key header is set correctly
- Check for typos in API key
- Ensure no extra whitespace in key
- Verify environment variable is set: `docker exec container env | grep MCP_API_KEY`

### Server won't start in HTTP mode
- Check port is not already in use: `lsof -i :8080`
- Verify MCP_PORT is a valid number
- Check container logs: `docker logs morrison-mcp-server-http`

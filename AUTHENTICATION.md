# MCP Server Authentication Guide

The Morrison MCP server supports two transport modes:

1. **Stdio Mode** (default) — No authentication, for local Claude Desktop integration
2. **HTTP/SSE Mode** — With dual authentication for containerized deployments

## Transport Modes

### Stdio Mode (Local)

Default mode when `MCP_PORT` is not set. Used for local Claude Desktop integration via stdio.

**No authentication required** — security by process isolation.

```bash
# Run locally
dotnet run

# Or via Docker (stdio mode)
docker compose up mcp-server
```

### HTTP/SSE Mode (Container)

Enabled when `MCP_PORT` environment variable is set. Uses two authentication schemes:

| Endpoint | Scheme | Header |
|---|---|---|
| `/sse` (MCP protocol) | OAuth Bearer | `Authorization: Bearer <token>` |
| `/` (server info) | OAuth Bearer | `Authorization: Bearer <token>` |
| `/health` (health check) | X-API-Key | `X-API-Key: <key>` |
| `/.well-known/oauth-protected-resource` | None (public) | — |

## Environment Variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `MCP_PORT` | No | none | Port to listen on (enables HTTP mode) |
| `OAUTH_TOKEN` | Yes* | none | Bearer token for `/sse` and `/` |
| `OAUTH_RESOURCE_URL` | No | `https://mcp-morrison.thresholdwater.com` | Resource URL for OAuth metadata |
| `AUTH_DISABLED` | No | `false` | Set to `true` to disable Bearer auth |
| `MCP_API_KEY` | No | none | X-API-Key for `/health` endpoint |
| `DATABASE_PATH` | No | `./data/canonical_facts.db` | Path to SQLite database |

*Required in HTTP mode unless `AUTH_DISABLED=true`

## Container Deployment

### Docker Compose (Recommended)

```bash
# Start server in HTTP mode
docker compose --profile http up mcp-server-http
```

**Important:** Change the default tokens in `docker-compose.yml`:

```yaml
environment:
  - MCP_PORT=8080
  - OAUTH_TOKEN=your-secure-oauth-token-here   # CHANGE THIS!
  - MCP_API_KEY=your-secure-api-key-here       # CHANGE THIS!
```

### Generate Secure Tokens

```bash
# Generate a random 32-byte base64 token
openssl rand -base64 32
```

## Using HTTP Mode with Claude Desktop

Add to your Claude Desktop config:

**macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`

```json
{
  "mcpServers": {
    "kate-morrison-facts-remote": {
      "url": "http://localhost:8080/sse",
      "headers": {
        "Authorization": "Bearer your-oauth-token"
      },
      "transport": "http"
    }
  }
}
```

## API Endpoints

### POST /sse — MCP Protocol (Bearer required)

```bash
# Unauthorized (no token)
curl -X POST http://localhost:8080/sse \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"tools/list","id":1}'
# → 401 Unauthorized, WWW-Authenticate: Bearer realm="MCP", resource_metadata="..."

# Authorized
curl -X POST http://localhost:8080/sse \
  -H "Authorization: Bearer your-oauth-token" \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"tools/list","id":1}'
```

### GET /health — Health Check (X-API-Key required)

```bash
# Unauthorized (no key)
curl http://localhost:8080/health
# → 401 Unauthorized

# Authorized
curl http://localhost:8080/health \
  -H "X-API-Key: your-api-key"
```

### GET /.well-known/oauth-protected-resource — Public

```bash
curl http://localhost:8080/.well-known/oauth-protected-resource
```

Response:
```json
{
  "resource": "https://mcp-morrison.thresholdwater.com",
  "authorization_servers": ["https://auth.thresholdwater.com"]
}
```

## Security Considerations

- **Always set `OAUTH_TOKEN`** — the server refuses to start in HTTP mode without it (unless `AUTH_DISABLED=true`)
- **Always set `MCP_API_KEY`** — without it, `/health` is publicly accessible
- Use strong, random tokens (32+ bytes)
- Use HTTPS/TLS reverse proxy (nginx, Caddy) in production

## Troubleshooting

### Server won't start — "OAUTH_TOKEN must be set"
Set `OAUTH_TOKEN` in your environment, or set `AUTH_DISABLED=true` to disable Bearer auth.

### 401 on /sse
Verify `Authorization: Bearer <token>` header is set and the token matches `OAUTH_TOKEN`.

### 401 on /health
Verify `X-API-Key: <key>` header is set and the key matches `MCP_API_KEY`.

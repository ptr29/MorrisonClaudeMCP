# Claude Desktop MCP Server Setup Guide

## Overview

This guide explains how to configure Claude Desktop to use the Kate Morrison Canonical Facts MCP server, enabling Claude to query canonical facts about characters, locations, schedules, and timeline events during writing sessions.

## Prerequisites

- Claude Desktop installed
- .NET 8.0 SDK installed
- MCP server built successfully (`dotnet build` in the Server project)
- Database initialized with sample data

## Configuration Steps

### 1. Locate Claude Desktop Configuration File

The configuration file location depends on your operating system:

**macOS:**
```
~/Library/Application Support/Claude/claude_desktop_config.json
```

**Windows:**
```
%APPDATA%\Claude\claude_desktop_config.json
```

**Linux:**
```
~/.config/Claude/claude_desktop_config.json
```

### 2. Edit Configuration File

Open the `claude_desktop_config.json` file and add the MCP server configuration:

```json
{
  "mcpServers": {
    "kate-morrison-facts": {
      "command": "/usr/local/share/dotnet/dotnet",
      "args": [
        "run",
        "--project",
        "/Users/paulrobak/Documents/GitHub/MorrisonClaudeMCP/src/KateMorrisonMCP.Server/KateMorrisonMCP.Server.csproj"
      ],
      "env": {
        "DATABASE_PATH": "/Users/paulrobak/Documents/GitHub/MorrisonClaudeMCP/data/canonical_facts.db"
      }
    }
  }
}
```

**Important:**
- Use the **full path** to the dotnet executable: `/usr/local/share/dotnet/dotnet` (find yours with `which dotnet`)
- Replace the `--project` path to point to your `KateMorrisonMCP.Server.csproj` file
- Replace the `DATABASE_PATH` to point to your `canonical_facts.db` file

**Note:** Claude Desktop doesn't use your shell's PATH, so you must specify the full path to `dotnet`. Common locations:
- macOS: `/usr/local/share/dotnet/dotnet` or `/opt/homebrew/bin/dotnet`
- Linux: `/usr/bin/dotnet` or `/usr/local/bin/dotnet`
- Windows: `C:\Program Files\dotnet\dotnet.exe`

### 3. Restart Claude Desktop

After saving the configuration file, completely restart Claude Desktop:
1. Quit Claude Desktop (not just close the window)
2. Relaunch Claude Desktop

### 4. Verify MCP Server is Loaded

1. Open a new conversation in Claude Desktop
2. Look for the MCP tools indicator (usually a small icon or menu)
3. You should see 6 available tools:
   - `check_negative` - Check if a behavior violates character negatives
   - `get_schedule` - Get character schedule information
   - `get_character_facts` - Retrieve character biographical/physical data
   - `get_location_layout` - Get room-by-room layout of a location
   - `verify_timeline` - Verify timeline events and dates
   - `check_relationship` - Check relationship status between characters

## Testing the Integration

### Test 1: Check Kate's Negatives

Ask Claude:
```
Does Kate go to the gym?
```

**Expected:** Claude should use the `check_negative` tool and respond that Kate does NOT go to gyms (absolute negative).

### Test 2: Check Paul's Schedule

Ask Claude:
```
What time does Paul start work in the morning?
```

**Expected:** Claude should use the `get_schedule` tool and respond that Paul starts at 7:00 AM with Mumbai team calls, NOT 9 AM.

### Test 3: Get Paul's House Layout

Ask Claude:
```
What's the layout of Paul's house in Skokie?
```

**Expected:** Claude should use the `get_location_layout` tool and return details about 14 rooms across Main Floor and Lower Level.

### Test 4: Character Facts

Ask Claude:
```
How tall is Kate Morrison?
```

**Expected:** Claude should use the `get_character_facts` tool and respond that Kate is 5'6" (66 inches).

### Test 5: Timeline Verification

Ask Claude:
```
When did Kate and Paul first meet?
```

**Expected:** Claude should use the `verify_timeline` or `check_relationship` tool and respond with September 6, 2025.

## Troubleshooting

### Server Not Appearing in Claude Desktop

**Symptoms:** No MCP tools visible in Claude Desktop, or error "Failed to spawn process: No such file or directory"

**Solutions:**
1. **Use full path to dotnet** - Claude Desktop doesn't use your shell's PATH. Find your dotnet path:
   ```bash
   which dotnet
   ```
   Then use that full path in the `command` field (e.g., `/usr/local/share/dotnet/dotnet`)

2. Check that the paths in `claude_desktop_config.json` are correct and absolute (not relative)
3. Verify the server compiles: `dotnet build src/KateMorrisonMCP.Server`
4. Check Claude Desktop logs (usually in the same directory as config file)
5. Ensure .NET 8.0 SDK is installed: `dotnet --version`

### Server Starts But Tools Don't Work

**Symptoms:** Tools appear but return errors when used

**Solutions:**
1. Verify database exists at the path specified in `DATABASE_PATH`
2. Check database has data: `sqlite3 path/to/canonical_facts.db "SELECT COUNT(*) FROM characters;"`
3. Run health check manually: `dotnet run --project src/KateMorrisonMCP.Server`
4. Check server logs for errors

### Database Path Issues

**Symptoms:** Server starts but reports database not found

**Solutions:**
1. Use absolute paths, not relative paths
2. Expand `~` to full home directory path
3. Verify file permissions (database file must be readable)
4. On Windows, use forward slashes or escaped backslashes in JSON: `C:/path/to/db` or `C:\\path\\to\\db`

### Permission Errors

**Symptoms:** Access denied or permission errors

**Solutions:**
1. Ensure database file has read/write permissions
2. Check parent directory is accessible
3. On macOS, grant Claude Desktop necessary permissions in System Preferences > Privacy & Security

## Advanced Configuration

### Custom Database Location

To use a different database location, update the `DATABASE_PATH` environment variable:

```json
"env": {
  "DATABASE_PATH": "/custom/path/to/canonical_facts.db"
}
```

### Logging Level

To enable debug logging for troubleshooting:

```json
"env": {
  "DATABASE_PATH": "/path/to/canonical_facts.db",
  "Logging__LogLevel__Default": "Debug"
}
```

### Using Published Build

Instead of `dotnet run`, you can publish the server and run the executable directly:

```bash
dotnet publish src/KateMorrisonMCP.Server -c Release -o ./publish
```

Then update the configuration:

```json
{
  "mcpServers": {
    "kate-morrison-facts": {
      "command": "/Users/paulrobak/Documents/GitHub/MorrisonClaudeMCP/publish/KateMorrisonMCP.Server",
      "env": {
        "DATABASE_PATH": "/Users/paulrobak/Documents/GitHub/MorrisonClaudeMCP/data/canonical_facts.db"
      }
    }
  }
}
```

## Health Check

The server performs a health check on startup and logs the results:

```
info: Program[0]
      MCP Canonical Facts Server starting...
info: Program[0]
      Database path: /path/to/canonical_facts.db
info: KateMorrisonMCP.Server.McpServer[0]
      Starting MCP Canonical Facts Server...
info: KateMorrisonMCP.Server.McpServer[0]
      Health check passed:
      ✓ Database connected, 2 characters found
      ✓ All required tables present
      ✓ Foreign keys enabled
```

If health check fails, the server will not start and will log the error.

## Sample Queries for Testing

Here are useful queries to test each MCP tool:

### check_negative
- "Can Kate eat pasta?" (should detect gluten negative)
- "Does Kate run on treadmills?" (should detect treadmill negative with <15°F exception)
- "Does Paul go to the gym?" (should return no violation)

### get_schedule
- "What's Paul's morning routine?"
- "When does Paul have his Mumbai calls?"
- "What time does Paul finish work?"

### get_character_facts
- "Tell me about Kate Morrison's physical appearance"
- "What does Paul do for work?"
- "How old is Kate?"

### get_location_layout
- "Describe Paul's home office setup"
- "What rooms are on the lower level of Paul's house?"
- "How big is the recreation room?"

### verify_timeline
- "What happened on September 6, 2025?"
- "When did Kate and Paul meet?"

### check_relationship
- "What's the relationship between Kate and Paul?"
- "How did Kate and Paul meet?"

## Next Steps

Once the MCP server is working with Claude Desktop:

1. **Add More Data:** Use the migration tool to import data from the 19 markdown files
2. **Expand Tools:** Implement the remaining 5 post-MVP tools
3. **Write Tests:** Create unit and integration tests for reliability
4. **Document Canonical Facts:** Add more characters, locations, and timeline events

## Support

For issues or questions:
- Check server logs in Claude Desktop log directory
- Verify database schema: `sqlite3 canonical_facts.db ".schema"`
- Test server manually: `dotnet run --project src/KateMorrisonMCP.Server`
- Review MCP protocol documentation: https://spec.modelcontextprotocol.io/

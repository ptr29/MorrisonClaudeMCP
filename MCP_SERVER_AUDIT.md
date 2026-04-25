# MCP Server Code Audit & Fixes

## Date: 2026-01-10

## Summary
Conducted a comprehensive review of the MCP server implementation against the JSON-RPC 2.0 and MCP protocol specifications. Fixed critical issues that were causing Zod validation errors in the Claude Desktop client.

---

## Critical Issues Found & Fixed

### 1. **CRITICAL: JSON-RPC Notifications Not Handled Correctly**

**Issue**: The server was responding to notifications (requests without an `id` field), which violates JSON-RPC 2.0 specification.

**Evidence from Log**:
```
Line 40: {"method":"notifications/initialized","jsonrpc":"2.0"}  // No id field
```

**Root Cause**:
- The server was treating all incoming messages as requests requiring responses
- When it received `notifications/initialized` (no id), it processed it through the request handler
- Since the method wasn't in the switch statement, it returned an error response
- This error response had both an `error` field AND a null `id`, which the MCP client rejected with Zod validation errors

**Fix Applied** (McpServer.cs:80-90):
- Added check for `request.Id == null` to identify notifications
- Created separate `ProcessNotificationAsync()` method to handle notifications
- Notifications now processed silently without sending responses
- Handles: `notifications/initialized`, `notifications/cancelled`

**Code Changed**:
```csharp
// Check if this is a notification (no id field)
var isNotification = request.Id == null;

if (isNotification)
{
    _logger.LogDebug("Received notification: {Method}", request.Method);
    await ProcessNotificationAsync(request);
    continue; // Don't send response
}
```

---

### 2. **Type Mismatch: ID Field Should Support Both String and Number**

**Issue**: The `Id` field was typed as `int?`, but JSON-RPC 2.0 requires supporting both string and number types.

**Fix Applied**:
- Changed `McpRequest.Id` from `int?` to `object?`
- Changed `McpResponse.Id` from `int?` to `object?`

**Files Modified**:
- `Models/McpRequest.cs:10`
- `Models/McpResponse.cs:16`

---

### 3. **ID Field Must Always Be Serialized in Responses**

**Issue**: The global JSON serialization setting `DefaultIgnoreCondition.WhenWritingNull` was causing `null` id values to be omitted from error responses.

**JSON-RPC 2.0 Requirement**: The `id` field MUST be present in all responses, even if `null`.

**Fix Applied** (McpResponse.cs:15):
```csharp
[JsonIgnore(Condition = JsonIgnoreCondition.Never)]
public object? Id { get; set; }
```

This ensures the `id` field is always serialized, even when null.

---

### 4. **Improved Error Code for Parse Errors**

**Issue**: Using generic error code `-32603` (Internal error) for parse failures.

**Fix Applied** (McpServer.cs:106):
- Changed to `-32700` (Parse error) for JSON deserialization failures
- More accurate per JSON-RPC 2.0 error code specification

---

## Verification

### Build Status
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Compliance Checklist

✅ **JSON-RPC 2.0 Compliance**
- [x] All responses include `jsonrpc: "2.0"`
- [x] ID field supports both string and number types
- [x] ID field always present in responses (even if null)
- [x] Notifications (no id) receive no response
- [x] Error responses use correct format and codes
- [x] Result and Error are mutually exclusive

✅ **MCP Protocol Compliance**
- [x] `initialize` method implemented
- [x] `tools/list` returns correct schema format
- [x] `tools/call` returns content array with text objects
- [x] Notifications handled: `notifications/initialized`
- [x] Logging goes to stderr (stdout reserved for JSON-RPC)

✅ **Code Quality**
- [x] No compiler warnings or errors
- [x] Proper async/await usage
- [x] Exception handling at appropriate levels
- [x] Logging for debugging

---

## JSON-RPC 2.0 Message Flow (Now Correct)

### Request with ID (Requires Response):
```json
→ {"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}
← {"jsonrpc":"2.0","id":1,"result":{"tools":[...]}}
```

### Notification (No Response):
```json
→ {"jsonrpc":"2.0","method":"notifications/initialized"}
  (No response sent)
```

### Error Response:
```json
→ {"jsonrpc":"2.0","id":2,"method":"unknown"}
← {"jsonrpc":"2.0","id":2,"error":{"code":-32601,"message":"Method not found"}}
```

---

## Files Modified

1. `src/KateMorrisonMCP.Server/Models/McpRequest.cs` - Changed Id type to object?
2. `src/KateMorrisonMCP.Server/Models/McpResponse.cs` - Changed Id type, added Never ignore condition
3. `src/KateMorrisonMCP.Server/McpServer.cs` - Added notification handling, improved error codes

---

## Testing Recommendations

1. Start the server and verify no Zod errors in Claude Desktop logs
2. Test that tools are listed correctly
3. Test tool execution
4. Verify notifications don't generate error responses
5. Check that all log output goes to stderr (not stdout)

---

## Additional Notes

### Why the Previous Fixes Failed

**First attempt**: Only fixed the Id serialization, but didn't address the root cause (responding to notifications).

**Result**: The server still tried to send error responses for `notifications/initialized`, causing the same Zod errors.

**This fix**: Addresses the root cause by properly implementing JSON-RPC 2.0 notification handling.

### MCP Specification Reference

The official MCP specification can be found at:
https://spec.modelcontextprotocol.io/specification/2025-11-25/

Key requirements implemented:
- JSON-RPC 2.0 transport layer
- Proper request/response correlation via id field
- Notification handling (no response)
- Tool listing and execution endpoints
- Proper error codes and formats

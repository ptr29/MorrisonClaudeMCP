# MCP Server Test Suite

Comprehensive test suite for the Kate Morrison MCP server covering protocol compliance, semantic matching, database operations, and tool execution.

## Quick Start

```bash
# Run all tests
dotnet test

# Run with coverage (accurate metrics, excludes integration layer)
../run-tests-with-coverage.sh

# Run with detailed output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "FullyQualifiedName~NegativeRepositoryTests"

# Watch mode (auto-run on file changes)
dotnet watch test
```

📊 **[View Detailed Coverage Report](../COVERAGE_SUMMARY.md)**

## Test Organization

```
tests/KateMorrisonMCP.Tests/
├── CheckNegativeToolTests.cs       # Check negative behaviors (10 tests)
├── GetScheduleToolTests.cs         # Get character schedules (7 tests)
├── GetCharacterFactsToolTests.cs   # Get character details (9 tests)
├── GetLocationLayoutToolTests.cs   # Get location layouts (8 tests)
├── VerifyTimelineToolTests.cs      # Verify timeline events (8 tests)
├── CheckRelationshipToolTests.cs   # Check relationships (10 tests)
├── NegativeRepositoryTests.cs      # Semantic matching logic (10 tests)
├── DatabaseContextTests.cs         # Database operations (8 tests)
├── McpProtocolTests.cs            # JSON-RPC 2.0 compliance (13 tests)
├── IntegrationTests.cs            # Full workflow tests (5 tests)
├── TestHelpers.cs                 # Shared test utilities
└── TEST_SUITE_SUMMARY.md          # Detailed test documentation
```

## Current Status

✅ **87 Tests Passing**
❌ **0 Tests Failing**
📊 **81%+ Testable Code Coverage**
⏱️ **~180ms Total Runtime**

## What's Tested

### Critical Path Coverage
- ✅ Semantic matching for negative behaviors ("goes to gym" → "Does NOT go to gyms")
- ✅ Verb normalization (goes→go, eats→eat, runs→run)
- ✅ Database column mapping (snake_case ↔ PascalCase)
- ✅ JSON-RPC 2.0 protocol compliance
- ✅ Notification vs request handling
- ✅ Error handling and validation
- ✅ Tool registration and execution

### Bugs Verified as Fixed
1. Notification handling (don't respond to notifications)
2. Verb conjugation normalization
3. Database column name mapping
4. ID field serialization (always present, even if null)
5. Missing argument handling

## Adding New Tests

### 1. Create Test Class
```csharp
public class MyNewTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly string _testDbPath;

    public MyNewTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
    }

    public async ValueTask DisposeAsync()
    {
        if (File.Exists(_testDbPath))
            File.Delete(_testDbPath);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task MyTest()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        // ... seed data

        // Act
        // ... execute code

        // Assert
        Assert.True(result);
    }
}
```

### 2. Use Test Helpers
```csharp
// Create schema
await TestHelpers.CreateTestSchemaAsync(_db);

// Each test gets its own isolated database
// Automatic cleanup via IAsyncDisposable
```

### 3. Follow Naming Convention
- Test class: `{ComponentName}Tests`
- Test method: `{MethodName}_{Scenario}_{ExpectedResult}`
- Example: `FindMatchingNegative_VerbConjugation_GoesVsGo`

## Test Categories

Run specific categories:

```bash
# Semantic matching tests
dotnet test --filter "FullyQualifiedName~NegativeRepository"

# Tool execution tests
dotnet test --filter "FullyQualifiedName~CheckNegativeTool"

# Protocol compliance tests
dotnet test --filter "FullyQualifiedName~McpProtocol"

# Integration tests
dotnet test --filter "FullyQualifiedName~Integration"

# Database tests
dotnet test --filter "FullyQualifiedName~DatabaseContext"
```

## Troubleshooting

### Tests Fail to Create Database
**Problem:** SQLite file locks or permission issues
**Solution:** Tests use temp directory with unique GUIDs - check /tmp permissions

### Column Mapping Errors
**Problem:** Dapper can't map database columns
**Solution:** Verify CharacterNegative model has both PascalCase and snake_case properties

### Test Timeout
**Problem:** Database operations hanging
**Solution:** Ensure test database is cleaned up (check IAsyncDisposable implementation)

## CI/CD Integration

### GitHub Actions Example
```yaml
name: Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build
      - run: dotnet test --no-build --verbosity normal
```

## Performance

| Metric | Value |
|--------|-------|
| Total tests | 87 |
| Average runtime | 180ms |
| Slowest test | ~20ms |
| Memory usage | <100MB |
| Test isolation | 100% (unique DBs) |
| Coverage | 81%+ (testable code) |

## Dependencies

- **xUnit 2.5.3** - Test framework
- **Moq 4.20.72** - Mocking (for future use)
- **Microsoft.NET.Test.Sdk 17.8.0** - Test runner
- **SQLite** - In-memory test databases

## Best Practices

1. ✅ **Test isolation** - Each test gets its own database
2. ✅ **Arrange-Act-Assert** - Clear three-phase structure
3. ✅ **Descriptive names** - Test names describe scenario and expectation
4. ✅ **Fast execution** - All tests complete in <1 second
5. ✅ **No flaky tests** - 100% reliable, deterministic results
6. ✅ **Cleanup** - IAsyncDisposable handles cleanup automatically

## Resources

- [xUnit Documentation](https://xunit.net/)
- [MCP Specification](https://spec.modelcontextprotocol.io/)
- [JSON-RPC 2.0 Spec](https://www.jsonrpc.org/specification)
- [Test Suite Summary](./TEST_SUITE_SUMMARY.md) - Detailed test documentation

## Maintenance

### When Adding Features
1. Write tests first (TDD)
2. Test both success and error cases
3. Add integration test for full workflow
4. Update TEST_SUITE_SUMMARY.md

### When Fixing Bugs
1. Write failing test that reproduces bug
2. Fix the code
3. Verify test passes
4. Document in TEST_SUITE_SUMMARY.md

## Questions?

Check [TEST_SUITE_SUMMARY.md](./TEST_SUITE_SUMMARY.md) for detailed test documentation and coverage analysis.

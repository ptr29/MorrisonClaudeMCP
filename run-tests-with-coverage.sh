#!/bin/bash
# Run tests with coverage, excluding non-testable code (Server integration layer)
# This gives accurate coverage of testable business logic (Tools + Data repositories)

dotnet test tests/KateMorrisonMCP.Tests/KateMorrisonMCP.Tests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  /p:Exclude="[KateMorrisonMCP.Server]*" \
  --verbosity normal

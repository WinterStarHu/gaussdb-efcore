## Why

The `EFCore.GaussDB.FunctionalTests` suite is currently expensive to investigate because a full run produces a large volume of failures without clearly separating basic connectivity/setup problems from expected database-compatibility failures. This change defines a repeatable workflow for running the suite against Docker-based openGauss and producing a failure log that highlights connection-related issues as actionable while treating database-specific incompatibilities as informational.

## What Changes

- Define a standard workflow for running `test/EFCore.GaussDB.FunctionalTests` against a local Docker openGauss instance.
- Define the expected connection override and smoke-check steps needed before a full suite run.
- Define result capture requirements so each failed test case is recorded with its primary failure reason.
- Define failure triage rules that distinguish connection/authentication/setup failures from database capability or compatibility failures.
- Define the expected output artifacts for investigation, including raw test logs and a summarized failure report.

## Capabilities

### New Capabilities
- `functionaltests-docker-triage`: Run the functional test suite against Docker openGauss with a repeatable setup process and produce a categorized failure report that separates connection failures from database-specific incompatibilities.

### Modified Capabilities

## Impact

- Affects local test execution for `test/EFCore.GaussDB.FunctionalTests`.
- Depends on Docker Desktop or equivalent Docker runtime plus an openGauss container reachable from the host.
- Depends on test-time connection override behavior in `config.json` and `TestEnvironment`.
- Produces investigation artifacts such as full logs and summarized failure classification reports for developers.

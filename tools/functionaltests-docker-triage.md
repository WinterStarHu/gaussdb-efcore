# EFCore.GaussDB.FunctionalTests Docker Triage

Use this workflow when `EFCore.GaussDB.FunctionalTests` needs to run against the local Docker openGauss fallback database and the goal is to separate connection/setup failures from expected database compatibility failures.

## Required Docker state

- Container name: `gaussdb-efcore-opengauss`
- Host port mapping: `127.0.0.1:5432 -> container:5432`
- Test user: `npgsql_tests`
- Expected connection override:
  `Test__GaussDB__DefaultConnection=Server=localhost;Username=npgsql_tests;Password=NpGsql@123;Port=5432;Database=postgres`

`config.json` and `TestEnvironment` still contain the older fallback password `npgsql_tests`. If the environment override is missing or applied incorrectly, the suite silently falls back to the wrong credentials and can fail with `28P01` or eventually lock the account.

## Smoke check

Run the workflow with `-SmokeOnly` when you only need to confirm the environment before a long suite run:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File D:\code\gaussdb-efcore\tools\Run-FunctionalTestsDocker.ps1 -SmokeOnly
```

The smoke check does three gated checks in this order:

1. Verifies the target container exists and is currently `Up`.
2. Verifies host TCP reachability to `127.0.0.1:5432`.
3. Uses a tiny .NET probe on the Windows host to authenticate with the effective `Test__GaussDB__DefaultConnection`.

If any of those checks fail, treat the result as an actionable connection/setup problem and stop there instead of starting the full suite.

## Full suite command

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File D:\code\gaussdb-efcore\tools\Run-FunctionalTestsDocker.ps1
```

The runner sets:

- `DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1`
- `DOTNET_CLI_HOME=D:\code\.dotnet`
- `Test__GaussDB__DefaultConnection=Server=localhost;Username=npgsql_tests;Password=NpGsql@123;Port=5432;Database=postgres`
- `BlameHangTimeout=2m` by default so hung test hosts are terminated instead of blocking the workflow indefinitely

Then it runs:

```powershell
dotnet test D:\code\gaussdb-efcore\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj --no-build -v:q --results-directory <run-dir> --logger "console;verbosity=quiet" --logger "trx;LogFileName=functionaltests.trx" --blame-hang --blame-hang-timeout 2m --blame-hang-dump-type none
```

Use `-EnableDiag` only when the standard artifacts are insufficient, because the extra VSTest diagnostic log adds noticeable overhead on this suite.

## Output artifacts

Each run creates a timestamped directory under `artifacts\functionaltests-docker\`.

Expected files:

- `run-metadata.json`: effective non-secret run configuration
- `probe-build.stdout.log` and `probe-build.stderr.log`
- `probe.stdout.log` and `probe.stderr.log`
- `dotnet-test.stdout.log` and `dotnet-test.stderr.log`
- `functionaltests.trx`
- `failed-tests.csv`
- `failure-summary.md`
- `failure-summary.json`
- `workflow-summary.json`

Optional when `-EnableDiag` is used:

- `dotnet-test.diag.log`

## Failure categories

- `actionable-connection`
  Use for container down, host port unreachable, transport failure, authentication failure, or account lock.
- `informational-compatibility`
  Use for openGauss feature gaps such as unsupported `EXTENSION`, missing extension control files, missing relations after incompatible initialization, and missing PostgreSQL functions or operators.
- `informational-other`
  Use when the failure is not obviously connection-related and does not match the current compatibility patterns.

For the current investigation only `actionable-connection` failures should block follow-up work. Compatibility failures are expected because openGauss is only an interim fallback target here.

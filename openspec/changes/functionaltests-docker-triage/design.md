## Context

`test/EFCore.GaussDB.FunctionalTests` is a large suite with many environment-sensitive setup steps. In the current workflow, the suite is pointed at a Docker-hosted openGauss instance and produces a very large number of failures. The primary investigation need is not to make openGauss feature-complete, but to separate infrastructure failures that block meaningful execution from expected compatibility failures caused by using an interim database target.

Key constraints:
- The target database is a Docker-hosted openGauss instance reachable from the Windows host.
- The suite reads configuration from `config.json`, `config.test.json`, and environment variables, so connection overrides must be explicit and repeatable.
- The suite is too large for ad hoc console-driven investigation; raw output becomes noisy and slow to interpret.
- Database-specific failures such as missing extensions, unsupported functions, or schema incompatibilities are expected and should be logged, not treated as blockers.

## Goals / Non-Goals

**Goals:**
- Define a repeatable test execution workflow for Docker-based openGauss.
- Ensure the workflow verifies host-to-container connectivity before running the full suite.
- Capture suite failures in durable log artifacts rather than relying on console output alone.
- Classify failures into actionable connection/setup problems versus informational database compatibility problems.
- Provide a clear basis for future automation or tooling without requiring provider feature changes now.

**Non-Goals:**
- Make openGauss pass PostgreSQL-specific functional tests.
- Implement missing extensions or compatibility shims in openGauss.
- Fix every failing functional test.
- Redesign the EF Core provider or change test semantics.

## Decisions

### Decision: Treat connectivity as a gated prerequisite
Before a full suite run, the workflow should validate that:
- Docker is running and the target container is up.
- Host port `5432` is reachable.
- The configured test user can authenticate from the host.
- A small connection-focused smoke test succeeds.

Rationale:
- This isolates environment/setup failures early.
- It prevents multi-minute full-suite runs that only prove the test harness cannot connect.

Alternatives considered:
- Run the full suite directly and inspect the first failure.
  Rejected because the suite emits too much noise and can mask the true first actionable issue.

### Decision: Prefer durable result artifacts over interactive console review
The workflow should produce:
- Raw full-suite output
- Optional structured test result output
- A summarized failure classification log

Rationale:
- The suite is large enough that console output alone is not a workable investigation surface.
- Durable artifacts allow iterative triage without rerunning immediately.

Alternatives considered:
- Rely only on console output and manual scrolling.
  Rejected because it is slow, fragile, and hard to compare across runs.

### Decision: Use first-order failure categories
Failures should be triaged into two top-level classes:
- Connection/setup failures: host/port unreachable, authentication failures, account lock, container not running, base connectivity errors.
- Database compatibility failures: missing extensions, missing relations caused by incompatible setup, unsupported SQL/functions/operators, feature gaps.

Rationale:
- This matches the current goal: only connectivity failures require immediate action.
- It keeps the investigation focused and avoids spending time “fixing” expected incompatibilities.

Alternatives considered:
- Fine-grained classification from the start.
  Rejected because it adds complexity before the basic triage boundary is stable.

### Decision: Keep the workflow explicit about connection override precedence
The design must account for the fact that local config files may specify one connection string while environment variables specify another.

Rationale:
- Previous runs showed that misapplied overrides can silently revert to `config.json`, causing authentication failures and account lockouts.
- Documenting precedence is necessary for repeatability.

Alternatives considered:
- Assume environment variables always win in practice.
  Rejected because incorrect invocation patterns can still cause confusion during investigation.

## Risks / Trade-offs

- [Large suite output still consumes time] → Mitigation: require smoke-check gating before full runs and prefer artifact-based review.
- [Interim database may fail during initialization scripts, making many tests noisy] → Mitigation: classify those failures as compatibility issues rather than blockers.
- [Credentials or override precedence may be misapplied again] → Mitigation: explicitly validate effective connectivity before the full run.
- [Docker behavior on the Windows host may differ across shells or invocation styles] → Mitigation: standardize on a single invocation style and validate container state before running tests.

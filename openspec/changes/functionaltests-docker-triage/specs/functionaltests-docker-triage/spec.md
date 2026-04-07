## ADDED Requirements

### Requirement: Docker-based functional test runs SHALL verify connectivity before a full suite run
The test investigation workflow SHALL perform a connectivity gate before running the full `EFCore.GaussDB.FunctionalTests` suite against Docker-hosted openGauss. The gate SHALL confirm container availability, host port reachability, and successful host-side authentication with the configured test connection.

#### Scenario: Connectivity gate passes
- **WHEN** the target openGauss container is running, port `5432` is reachable from the host, and the configured test user can authenticate from the host
- **THEN** the workflow SHALL allow a full functional test run to proceed

#### Scenario: Connectivity gate fails
- **WHEN** the container is not running, the host port is unreachable, or authentication fails
- **THEN** the workflow SHALL classify the issue as a connection/setup failure and SHALL not require a full suite run before reporting it

### Requirement: Functional test investigation SHALL produce durable failure artifacts
The workflow SHALL persist full test execution artifacts for later triage rather than relying solely on interactive console review.

#### Scenario: Full suite run completes or is interrupted
- **WHEN** a functional test run produces output
- **THEN** the workflow SHALL preserve raw logs or result artifacts that can be reviewed after the run

#### Scenario: Failure review is needed
- **WHEN** developers need to understand why test cases failed
- **THEN** the workflow SHALL provide enough stored output to map each failing test case to a primary failure reason

### Requirement: Failure triage SHALL distinguish connection failures from database compatibility failures
The workflow SHALL classify failures into connection/setup failures and database compatibility failures, and SHALL treat only connection/setup failures as immediately actionable blockers for this investigation.

#### Scenario: Connection-related failure is encountered
- **WHEN** a test failure is caused by unreachable host/port, authentication failure, account lock, missing container availability, or equivalent connectivity issues
- **THEN** the workflow SHALL record that failure as actionable and requiring follow-up

#### Scenario: Database compatibility failure is encountered
- **WHEN** a test failure is caused by missing extensions, unsupported SQL features, missing relations caused by incompatible initialization, unsupported operators, or other database-specific behavior gaps
- **THEN** the workflow SHALL record that failure in the output log and SHALL mark it as informational for the current investigation

## 1. Establish the Docker test entry path

- [x] 1.1 Document the required Docker container state, host port mapping, and test connection override inputs for `EFCore.GaussDB.FunctionalTests`
- [x] 1.2 Define and verify a smoke-check sequence that confirms container availability, host reachability, and successful authentication before a full suite run

## 2. Capture full-suite artifacts

- [x] 2.1 Define a repeatable command path for running the full functional test suite with the Docker openGauss connection override applied
- [x] 2.2 Configure the run workflow to persist raw logs and any structured test result output for later analysis

## 3. Classify failures

- [x] 3.1 Define the failure categories used by the investigation workflow, including actionable connection/setup failures and informational database compatibility failures
- [x] 3.2 Produce a summarized report format that maps each failed test case to its primary failure reason and category

## 4. Validate the workflow

- [x] 4.1 Run the workflow end to end against the current Docker openGauss environment and confirm that connection failures are surfaced separately from compatibility failures
- [x] 4.2 Review the resulting report and confirm it is sufficient for future investigations without requiring immediate database-compatibility fixes

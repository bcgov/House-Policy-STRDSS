# MOHSTR Test Automation Framework

Playwright + TypeScript end-to-end test automation for the STR (Short-Term Rental) Data Portal.

## Framework Overview

- End-to-end UI validation with Playwright Test Runner
- Environment-based runtime configuration with dotenv
- Browser matrix support: Chromium, Firefox, WebKit
- Feature-based spec organization under tests/
- Tag-based execution with @smoke and @regression
- HTML reporting and Playwright artifacts

## Configuration Model

Environment is selected using TEST_ENV in playwright.config.ts.

Supported values:
- dev
- test
- uat (default)

Resolution order used at runtime:
1. Config/config.<TEST_ENV>.env (mandatory)
2. Config/secrets.<TEST_ENV>.env (preferred)
3. Config/secrets.env (fallback if env-specific secrets file is missing)

Required variables:
- BASE_URL
- IDIR_USERNAME
- IDIR_PASSWORD
- BCEID_USERNAME (or BUSINESS_BCEID_USERNAME)
- BCEID_PASSWORD (or BUSINESS_BCEID_PASSWORD)

## Setup

1. Install dependencies

```bash
npm install
```

2. Install Playwright browsers (first-time setup)

```bash
npx playwright install
```

3. Configure environment files in Config/

Use one of the following approaches:

- Preferred (environment-specific):
	- Config/config.dev.env + Config/secrets.dev.env
	- Config/config.test.env + Config/secrets.test.env
	- Config/config.uat.env + Config/secrets.uat.env
- Fallback:
	- Config/secrets.env

Populate credentials and URL:

```env
BASE_URL=https://<portal-url>

# IDIR credentials
IDIR_USERNAME=<value>
IDIR_PASSWORD=<value>

# BCeID credentials
BCEID_USERNAME=<value>
BCEID_PASSWORD=<value>
```

## Run Tests

### Default and environment-specific

```bash
npm test
npm run test:dev
npm run test:test
npm run test:uat
```

### Tag-based

```bash
npm run test:smoke
npm run test:regression
npm run test:smoke:chromium
npm run test:regression:chromium
```

### IDIR landing-page suite

```bash
npm run test:idir
npm run test:idir:headed
npm run test:idir:dev
npm run test:idir:test
npm run test:idir:uat
```

### Targeted spec execution examples

```bash
npx playwright test tests/send-notices-of-non-compliance.spec.ts
npx playwright test tests/send-notices-of-non-compliance.spec.ts --project=chromium
npx playwright test tests/send-notices-of-non-compliance.spec.ts --grep "AC5"
npx playwright test tests/send-notices-of-non-compliance.spec.ts --grep "AC5" --headed --project=chromium
```

### Run against a specific environment manually

PowerShell:

```powershell
$env:TEST_ENV="test"; npx playwright test
```

Bash/macOS/Linux:

```bash
TEST_ENV=test npx playwright test
```

## Reporting and Artifacts

Open HTML report:

```bash
npm run report
```

Common output locations:
- playwright-report/ (HTML report)
- test-results/ (screenshots, traces, error-context.md on failures)
- test-output/ (local logs/artifacts)

## Notes

- Authentication helpers are centralized in tests/support/auth.ts.
- Suites are guarded with test.skip when required auth/env variables are missing.
- Prefer role-based locators and state-based waits (expect / expect.poll) over fixed sleep delays.

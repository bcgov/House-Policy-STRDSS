# MOHSTR Test Automation Framework User Guide

## 1. Purpose
This guide explains the project architecture, Playwright setup, environment configuration, test execution, reporting, and maintenance practices for the MOHSTR test automation framework.

---

## 2. Framework Overview
This project is a Playwright + TypeScript automation suite for STR (Short-Term Rental) Data Portal acceptance and regression scenarios.

Core characteristics:
- End-to-end UI validation with Playwright Test Runner
- Environment-based runtime configuration using dotenv
- Browser matrix support: Chromium, Firefox, WebKit (all enabled by default)
- Functional test organization by feature area
- Tag-based test selection (`@smoke`, `@regression`)
- HTML execution reporting

---

## 3. Repository Architecture

Top-level structure:
```
package.json                  npm scripts and dependencies
playwright.config.ts          Playwright configuration and environment loading
Config/                       Environment config files and local secrets files
tests/                        Feature-based spec files
tests/support/auth.ts         Centralized authentication helpers
test-data/                    Local CSV files used in upload scenarios (gitignored)
test-results/                 Playwright run artifacts (screenshots, traces)
playwright-report/            HTML report output
test-output/                  Local execution logs and artifacts
```

### Current test modules under tests/

| Spec File | Identity Provider | Feature Area |
|---|---|---|
| idir-landing-page.spec.ts | IDIR | IDIR portal landing page |
| bceid-landing-page.spec.ts | Business BCeID | BCeID portal landing page |
| download-listing-data.spec.ts | IDIR | Download listing data |
| upload-str-data.spec.ts | IDIR | Upload STR data (CSV) |
| user-management-access-list.spec.ts | IDIR | User management / access list |
| manage-jurisdiction.spec.ts | IDIR | Manage jurisdiction |
| manage-platforms-add-platform.spec.ts | IDIR | Add platform |
| manage-platforms-add-subsidiary-platform.spec.ts | IDIR | Add subsidiary platform |
| manage-platforms-edit-platform.spec.ts | IDIR | Edit platform |
| manage-platforms-edit-subsidiary-platform.spec.ts | IDIR | Edit subsidiary platform |
| view-aggregated-listings.spec.ts | IDIR | View aggregated listings |
| view-individual-listings.spec.ts | IDIR | View individual listings |
| deny-access-to-system.spec.ts | IDIR + Business BCeID | Access control / denied access validation |
| send-notice-of-takedown-without-dss-listing.spec.ts | Business BCeID | Notice of takedown without DSS listing |
| send-notices-of-non-compliance-with-invalid-email.spec.ts | Business BCeID | Non-compliance notice validation with invalid email |
| send-notices-of-non-compliance.spec.ts | Business BCeID | Send Notices of Non-Compliance |
| send-takedown-request-without-dss-listing.spec.ts | Business BCeID | Send takedown request without DSS listing |
| send-takedown-requests.spec.ts | Business BCeID | Send takedown requests |

### 3.1 Git ignore policy (important)

The project `.gitignore` excludes local runtime and sensitive files. Key entries:

- `Config/secrets.env`, `Config/secrets.dev.env`, `Config/secrets.test.env`, `Config/secrets.uat.env`
- `test-results/`, `playwright-report/`, `blob-report/`, `test-output/`, `test-output.log`
- `test-data/`

Implication:
- Any files you place in `test-data/` are local-only and are not committed by default.
- Secret files under `Config/` are local-only and must never be committed.

---

## 4. Configuration Model

### 4.1 Environment selection
The framework selects the target environment using the `TEST_ENV` variable in `playwright.config.ts`.

Supported values:
- `dev`
- `test`
- `uat` (default if TEST_ENV is not set)

Resolution logic:
1. Load `Config/config.<TEST_ENV>.env` (mandatory — throws if missing)
2. Load `Config/secrets.<TEST_ENV>.env` (preferred)
3. Fall back to `Config/secrets.env` if env-specific secrets file is missing

### 4.2 Base URL
`BASE_URL` is read from the env config file for the selected environment:
- `Config/config.dev.env`
- `Config/config.test.env`
- `Config/config.uat.env`

### 4.3 Credentials
Two identity provider types are supported:

**IDIR (Ministry staff):**
- `IDIR_USERNAME`
- `IDIR_PASSWORD`

**Business BCeID (Local Government users):**
- `BCEID_USERNAME` (also accepted as `BUSINESS_BCEID_USERNAME`)
- `BCEID_PASSWORD` (also accepted as `BUSINESS_BCEID_PASSWORD`)

Additional variables used by specific suites:
- `DENIED_BCEID_USERNAME`
- `DENIED_BCEID_PASSWORD`
- `DENIED_IDIR_USERNAME`
- `DENIED_IDIR_PASSWORD`
- `LG_SENDER_EMAIL`

All credentials are loaded from the env-specific secrets file or the fallback `Config/secrets.env`.

---

## 5. Setup Instructions

### 5.1 Prerequisites
- Node.js LTS
- npm
- Access to the STR Data Portal environment (dev / test / uat)
- Valid IDIR credentials for IDIR-based specs
- Valid Business BCeID credentials for BCeID-based specs

### 5.2 Install project dependencies
Run from project root:
```
npm install
```

### 5.3 Install Playwright browsers (first time)
```
npx playwright install
```

### 5.4 Create secrets file

Option A — environment-specific (recommended):
- `Config/secrets.dev.env`
- `Config/secrets.test.env`
- `Config/secrets.uat.env`

Option B — shared fallback:
```
Use Config/secrets.env
```

Note:
- The repository does not include `.example` secrets templates.
- Secrets files are gitignored and are intended for local use only.

Populate with the relevant credentials:
```
# IDIR credentials (for IDIR-based specs)
IDIR_USERNAME=<value>
IDIR_PASSWORD=<value>

# BCeID credentials (for BCeID-based specs)
BCEID_USERNAME=<value>
BCEID_PASSWORD=<value>

# Optional: denied-access users (for deny-access-to-system.spec.ts)
DENIED_BCEID_USERNAME=<value>
DENIED_BCEID_PASSWORD=<value>
DENIED_IDIR_USERNAME=<value>
DENIED_IDIR_PASSWORD=<value>

# Optional: sender email used by notice/takedown tests
LG_SENDER_EMAIL=<value>
```

`BASE_URL` is read from the selected `Config/config.<TEST_ENV>.env` file.

---

## 6. How to Run Tests

### 6.1 Run all tests (default UAT environment)
```
npm test
```

### 6.2 Run by environment
```
npm run test:dev
npm run test:test
npm run test:uat
```

### 6.3 Run by tag
```
npm run test:smoke               # all @smoke tests, all browsers
npm run test:regression          # all @regression tests, all browsers
npm run test:smoke:chromium      # @smoke tests, Chromium only
npm run test:regression:chromium # @regression tests, Chromium only
```

### 6.4 Run IDIR landing page suite
```
npm run test:idir                # Chromium headless
npm run test:idir:headed         # Chromium headed
npm run test:idir:dev            # dev environment
npm run test:idir:test           # test environment
npm run test:idir:uat            # uat environment
```

### 6.5 Run a specific spec
```
npx playwright test tests/send-notices-of-non-compliance.spec.ts
```

### 6.6 Run a specific spec with a specific browser
```
npx playwright test tests/send-notices-of-non-compliance.spec.ts --project=chromium
```

### 6.7 Run a specific AC (test) within a spec using --grep
```
npx playwright test tests/send-notices-of-non-compliance.spec.ts --grep "AC5"
```

### 6.8 Run in headed mode (visible browser)
```
npx playwright test tests/send-notices-of-non-compliance.spec.ts --headed
npx playwright test tests/send-notices-of-non-compliance.spec.ts --grep "AC5" --headed --project=chromium
```

### 6.9 Run against a specific environment
PowerShell:
```
$env:TEST_ENV="test"; npx playwright test tests/send-notices-of-non-compliance.spec.ts
```
Bash/macOS/Linux:
```
TEST_ENV=test npx playwright test tests/send-notices-of-non-compliance.spec.ts
```

### 6.10 Open HTML report
```
npm run report
```

---

## 7. Test Design Patterns

Common patterns used across all specs:

- **Feature-oriented describe blocks** — each spec covers one feature, tagged with `@regression` and optionally `@smoke`
- **AC/step-based JSDoc header** — every spec file opens with a structured comment block documenting the feature, Jira link, test case summary, and numbered steps per AC
- **Guard clauses via test.skip** — specs skip gracefully when required env vars (`BASE_URL`, credentials) are missing rather than failing
- **Reusable per-spec helper functions** — login, navigation, form interactions, and assertions are extracted to named async functions within the spec file
- **Role-based locators with resilient fallbacks** — `getByRole` is preferred; multiple candidate selectors are tried in order where the DOM is ambiguous
- **`expect.poll`** for async state changes — used instead of arbitrary `waitForTimeout` when waiting for button enable/disable state
- **`test.setTimeout`** — set to `240_000ms` (4 minutes) for specs with multi-step flows
- **console.log step tracing** — key steps are logged with `📝` / `✅` / `⚠️` prefixes to aid debugging without requiring trace viewer

---

## 8. Authentication Flow

Authentication is centralized in `tests/support/auth.ts`. Specs import helpers from this module and wrap them in a local thin function.

### 8.1 Exported API from auth.ts

| Export | Type | Purpose |
|---|---|---|
| `loginAsIdir(page, baseUrl?)` | async function | Authenticates using IDIR SSO |
| `loginAsBceid(page, baseUrl?)` | async function | Authenticates using Business BCeID SSO |
| `hasIdirAuthConfig()` | function → boolean | Returns true if IDIR credentials are configured |
| `hasBceidAuthConfig()` | function → boolean | Returns true if BCeID credentials are configured |
| `IDIR_AUTH_ENV_MESSAGE` | string constant | Skip message for missing IDIR config |
| `BCEID_AUTH_ENV_MESSAGE` | string constant | Skip message for missing BCeID config |

### 8.2 IDIR login flow
1. Navigate to `BASE_URL`
2. If portal heading is already visible — skip login (already authenticated)
3. Wait for "Authenticate with:" heading
4. Click IDIR identity provider link/button
5. If portal heading appears immediately — skip credential entry (SSO session reused)
6. Fill username and password fields
7. Click Continue
8. Assert portal heading is visible

### 8.3 Business BCeID login flow
Same pattern as IDIR but clicks the "Business BCeID" identity provider and uses `BCEID_USERNAME` / `BCEID_PASSWORD`.

### 8.4 Usage pattern in specs
```typescript
import {
  BCEID_AUTH_ENV_MESSAGE,
  hasBceidAuthConfig,
  loginAsBceid as loginAsBceidShared,
} from './support/auth';

const APP_URL = process.env.BASE_URL ?? '';

async function loginAsBceid(page: Page): Promise<void> {
  await loginAsBceidShared(page, APP_URL);
}

// Inside test.describe:
test.skip(!APP_URL, 'Set BASE_URL environment variable before running this suite.');
test.skip(!hasBceidAuthConfig(), BCEID_AUTH_ENV_MESSAGE);
```

---

## 9. Send Notices of Non-Compliance Spec

**File:** `tests/send-notices-of-non-compliance.spec.ts`  
**Identity Provider:** Business BCeID (Local Government user)  
**Jira:** DSS-678  
**Tags:** `@regression`, `@smoke` (AC1 only)

### Required environment variables
```
BASE_URL=<portal-url>
BCEID_USERNAME=<lg-user>
BCEID_PASSWORD=<lg-password>
```

### Test cases

| Test | Description |
|---|---|
| AC1 `@smoke` | Send Notices button disabled → enable on listing selection |
| AC2 | Review button disabled until mandatory fields filled |
| AC3 | Cancel on bulk-compliance-notice page returns to listings |
| AC4 | Invalid email keeps Review disabled; valid email enables it |
| AC5 | Full review/cancel/re-review/submit flow + Last Action verification |
| AC6 | Unchecking all listing checkboxes in form disables Review button |

### Key helper functions in this spec

| Function | Purpose |
|---|---|
| `navigateToListingsPage(page)` | Navigates from home to the listings grid; returns page state |
| `getSendNoticesButton(page)` | Locates Send Notices button by ID (`#send_delisting_notice_btn`) with role fallback |
| `getRecentlyReportedToggle(page)` | Locates the Recently Reported toggle with 7 candidate strategies |
| `selectListingByIndex(page, index)` | Clicks the checkbox of a listing row by zero-based index |
| `openNoticeDetailsForm(page)` | Clicks Send Notices button and waits for bulk-compliance-notice page |
| `getReviewButton(page)` | Returns the Review button locator |
| `fillLGEmailField(page, email)` | Fills the LG email input with resilient locator strategy |
| `fillMandatoryFields(page, lgEmail)` | Fills all mandatory fields on the bulk-compliance-notice page |
| `getEmailValidationError(page)` | Returns the text of an email validation error if visible |

### Important DOM observations
- After clicking Review, a `role="dialog"` named "Send Notice of Non-Compliance" opens. Its title is a plain `div`, not a heading element — use `getByRole('dialog', { name: /Send Notice of Non-Compliance/i })` to locate it
- The bulk-compliance-notice page uses `role="heading" level=2` with text "Send Notices of Non-Compliance"
- The Send Notices button has a stable ID: `#send_delisting_notice_btn`

---

## 10. Data and File-Driven Testing

The framework supports file-driven tests using CSV input files under `test-data/`.

Current files:
- Monthly CSVs using reporting-period naming: `January 2026.csv` through `December 2026.csv`
- Used by `upload-str-data.spec.ts` for STR data upload scenarios

Recommended practice:
- Keep test data deterministic and environment-safe
- Do not store personally identifiable data in local test files
- `test-data/` is ignored by git in this repository; treat it as local execution data

---

## 11. Reporting and Artifacts

Outputs generated per run:

| Output | Location | Contents |
|---|---|---|
| HTML report | `playwright-report/index.html` | Full test results with step logs |
| Test artifacts | `test-results/` | Screenshots, traces, error-context.md on failure |
| Local logs | `test-output/` | Run-time console output |

### Viewing failures
When a test fails, Playwright writes an `error-context.md` file under `test-results/<test-name>/` containing:
- Error details and call log
- Full page snapshot (ARIA tree of the page at point of failure)
- Relevant source code excerpt

To open the HTML report:
```
npm run report
```

---

## 12. Troubleshooting

### 12.1 Tests skipped — missing credentials
**Symptom:** All tests in a suite show as skipped  
**Cause:** `BASE_URL`, `IDIR_USERNAME`/`IDIR_PASSWORD`, or `BCEID_USERNAME`/`BCEID_PASSWORD` not set  
**Fix:** Populate the correct secrets file for the active `TEST_ENV`

### 12.2 BCeID specs skipped
**Symptom:** BCeID specs (bceid-landing-page, send-notices-of-non-compliance) are skipped  
**Fix:** Ensure `BCEID_USERNAME` and `BCEID_PASSWORD` are set in the active secrets file  
Note: `BUSINESS_BCEID_USERNAME` / `BUSINESS_BCEID_PASSWORD` are also accepted as aliases

### 12.3 Invalid TEST_ENV value
**Symptom:** `playwright.config.ts` throws on startup  
**Fix:** `TEST_ENV` must be one of: `dev`, `test`, `uat`

### 12.4 Config file not found
**Symptom:** Error: `Environment config file not found: Config/config.<env>.env`  
**Fix:** Ensure the correct `Config/config.<TEST_ENV>.env` file exists

### 12.5 Selector / locator failures after UI changes
If tests start failing due to DOM changes:
- Prefer `getByRole` selectors over CSS classes
- For elements with stable IDs (e.g. `#send_delisting_notice_btn`), use `page.locator('#id')` as primary with role-based fallback
- Scope locators inside dialogs using `dialog.getByRole(...)` rather than `page.getByRole(...).first()`
- Use error-context.md page snapshot to identify the actual element structure

### 12.6 Dialog title not found as heading
Some modal dialogs in this portal render their title as a plain `div`, not a heading element.  
**Fix:** Use `page.getByRole('dialog', { name: /title/i })` instead of `page.getByRole('heading', { name: /title/i })` for dialog detection.

### 12.7 Timeout on slow environments
**Fix:** Increase `test.setTimeout` at the describe level (current default: `240_000ms`). For individual assertions, increase the `timeout` option on `expect(...).toBeVisible({ timeout: ... })`.

### 12.8 Deny-access tests fail early
**Symptom:** `deny-access-to-system.spec.ts` fails due to missing denied-user credentials  
**Fix:** Set `DENIED_BCEID_USERNAME`, `DENIED_BCEID_PASSWORD`, `DENIED_IDIR_USERNAME`, and `DENIED_IDIR_PASSWORD` in the active secrets file

### 12.9 Notice/takedown tests fail on sender email validation
**Symptom:** Notice/takedown suites fail during form validation for sender email  
**Fix:** Set `LG_SENDER_EMAIL` in the active secrets file

---

## 13. Adding a New Test Spec

Suggested workflow:

1. Create a new `.spec.ts` file under `tests/` named after the feature
2. Add a JSDoc header block with:
   - Feature name and Jira link
   - `@FeatureTag` for describe block
   - Test Case Summary (Given/When/Then)
   - AC-numbered steps with validation checkpoints
3. Add env guards at the top of `test.describe`:
   ```typescript
   test.skip(!APP_URL, 'Set BASE_URL ...');
   test.skip(!hasBceidAuthConfig(), BCEID_AUTH_ENV_MESSAGE); // or hasIdirAuthConfig
   ```
4. Import and wrap the relevant auth helper (`loginAsIdir` or `loginAsBceid`)
5. Extract reusable logic into named async helper functions within the spec
6. Tag the describe block with `@regression` and mark smoke tests with `@smoke`
7. Set `test.setTimeout(240_000)` for multi-step flows
8. Run in Chromium first (`--project=chromium`), then validate across all browsers

---

## 14. Proposed BCeID Extension Placeholder (Recommended Next Step)
To support future requirements where some specs run with IDIR and BCeID:
- Add env vars:
  - BCeID_USERNAME
  - BCeID_PASSWORD
- Introduce shared auth utility for provider-aware login selection
- Add an auth user matrix selector (for example via AUTH_USER_TYPES)
- Parameterize selected specs to run for one or both user types

This keeps current tests stable while enabling incremental adoption.

## 15. Quick Start Checklist
- Install dependencies
- Configure BASE_URL and IDIR credentials
- Run a smoke spec in Chromium
- Open HTML report and verify clean pass
- Run target suite for the selected environment

## 16. Tagging Strategy (Implemented)
Tagging conventions:
- @smoke: critical user journeys and fast validation checks
- @regression: full functional validation scope

Execution commands:
- npm run test:smoke
- npm run test:regression
- npm run test:smoke:chromium
- npm run test:regression:chromium

Implementation approach in this repository:
- test.describe blocks are tagged with @regression
- Representative critical tests are tagged with @smoke
- Teams can add @smoke to newly created high-priority scenarios while leaving broader cases under @regression

---
Document owner: Kaushik Mandal (NTT DATA)
Project: MOHSTR Test Automation Framework

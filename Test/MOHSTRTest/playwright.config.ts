import { defineConfig, devices } from '@playwright/test';
import dotenv from 'dotenv';
import path from 'path';
import fs from 'fs';

/**
 * Multi-environment support.
 *
 * Set the TEST_ENV variable to target a specific environment:
 *   TEST_ENV=dev   → Config/config.dev.env   + Config/secrets.dev.env
 *   TEST_ENV=test  → Config/config.test.env  + Config/secrets.test.env
 *   TEST_ENV=uat   → Config/config.uat.env   + Config/secrets.uat.env  (default)
 *
 * Usage examples:
 *   $env:TEST_ENV="test" ; npx playwright test --project=chromium   (PowerShell)
 *   TEST_ENV=test npx playwright test --project=chromium             (Bash/macOS/Linux)
 *   npm run test:test                                                (npm script shorthand)
 *
 * Secrets files (secrets.dev.env, secrets.test.env, secrets.uat.env) must be created
 * locally from the .example templates and must NOT be committed to source control.
 * The legacy secrets.env is used as a fallback when an env-specific secrets file
 * does not exist yet.
 */
const TEST_ENV = (process.env.TEST_ENV ?? 'uat').toLowerCase();
const VALID_ENVS = ['dev', 'test', 'uat'];
if (!VALID_ENVS.includes(TEST_ENV)) {
  throw new Error(
    `Invalid TEST_ENV="${TEST_ENV}". Allowed values: ${VALID_ENVS.join(', ')}`
  );
}

const configFile  = path.resolve(__dirname, 'Config', `config.${TEST_ENV}.env`);
const secretsFile = path.resolve(__dirname, 'Config', `secrets.${TEST_ENV}.env`);
const legacySecretsFile = path.resolve(__dirname, 'Config', 'secrets.env');

if (!fs.existsSync(configFile)) {
  throw new Error(`Environment config file not found: ${configFile}`);
}

dotenv.config({ path: configFile });

// Load env-specific secrets; fall back to the legacy shared secrets.env
if (fs.existsSync(secretsFile)) {
  dotenv.config({ path: secretsFile });
} else {
  console.warn(
    `[playwright.config] secrets.${TEST_ENV}.env not found — falling back to secrets.env. ` +
    `Copy Config/secrets.${TEST_ENV}.env.example to Config/secrets.${TEST_ENV}.env and fill in credentials.`
  );
  dotenv.config({ path: legacySecretsFile });
}

/**
 * See https://playwright.dev/docs/test-configuration.
 */
export default defineConfig({
  testDir: './tests',
  /* Run tests in files in parallel */
  fullyParallel: true,
  /* Fail the build on CI if you accidentally left test.only in the source code. */
  forbidOnly: !!process.env.CI,
  /* Retry on CI only */
  retries: process.env.CI ? 2 : 0,
  /* Opt out of parallel tests on CI. */
  workers: process.env.CI ? 1 : undefined,
  /* Reporter to use. See https://playwright.dev/docs/test-reporters */
  reporter: 'html',
  /* Shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
  use: {
    /* Base URL to use in actions like `await page.goto('')`. */
    // baseURL: 'http://localhost:3000',

    /* Collect trace when retrying the failed test. See https://playwright.dev/docs/trace-viewer */
    trace: 'on-first-retry',
  },

  /* Configure projects for major browsers */
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },

    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },

    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },

    /* Test against mobile viewports. */
    // {
    //   name: 'Mobile Chrome',
    //   use: { ...devices['Pixel 5'] },
    // },
    // {
    //   name: 'Mobile Safari',
    //   use: { ...devices['iPhone 12'] },
    // },

    /* Test against branded browsers. */
    // {
    //   name: 'Microsoft Edge',
    //   use: { ...devices['Desktop Edge'], channel: 'msedge' },
    // },
    // {
    //   name: 'Google Chrome',
    //   use: { ...devices['Desktop Chrome'], channel: 'chrome' },
    // },
  ],

  /* Run your local dev server before starting the tests */
  // webServer: {
  //   command: 'npm run start',
  //   url: 'http://localhost:3000',
  //   reuseExistingServer: !process.env.CI,
  // },
});

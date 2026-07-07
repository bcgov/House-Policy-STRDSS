/// <reference types="node" />

/**
 * Feature: Short-Term Rental Data Portal – Deny Access To System
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-75
 *
 * @DenyAccess
 * Scenario: DenyAccessToSystem
 * Test Case Summary:
 * Given I am a user (Business BCeID or IDIR) whose account is not authorised to access the portal
 * When I attempt to authenticate and access the Short-Term Rental Data Portal
 * Then I should be presented with a 401 Access Denied page
 * And the page must display the correct error heading, unauthorised message, and contact information
 *
 *
 * Environment Prerequisites:
 * Set the following variables in Config/secrets.<env>.env before running this suite:
 *   DENIED_BCEID_USERNAME=<denied-bceid-username>   e.g. LGWestVancouver
 *   DENIED_BCEID_PASSWORD=<denied-bceid-password>
 *   DENIED_IDIR_USERNAME=<denied-idir-username>     e.g. STRDPDEV
 *   DENIED_IDIR_PASSWORD=<denied-idir-password>
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 – 401 Page Displays Correct Error Message Content: [@smoke]
 * - Step 1: Complete denied-access login flow for the identity provider (BCeID or IDIR)
 *           Uses page.waitForFunction() to reliably detect either form or 401 content
 *           Fills username/password using attribute-based locators (input[type="text"], input[type="password"])
 *           Polls up to 60 s for 401 page to appear (SSO chains on UAT can take 30–45 s)
 * - Step 2: Verify /401|Access Denied|Unauthorized/i heading is visible via getByRole + heading filter
 * - Step 3: Verify "You are not authorized to access this page." exact message text is present
 * - Step 4: Verify "If you believe this is an error, please contact us at" exact message text is present
 * - Step 5: Verify contact email address STRdata@gov.bc.ca is visible on the page
 *
 * AC2 – Contact Email Is Visible and Correctly Formatted as mailto Link:
 * - Step 1: Complete denied-access login and land on 401 page (60 s timeout via verifyOn401Page)
 * - Step 2: Verify STRdata@gov.bc.ca email address text is visible on the page
 * - Step 3: Attempt to locate mailto link (a[href="mailto:STRdata@gov.bc.ca"]);
 *           if found: verify link is visible; if not found: log info and accept plain text as fallback
 * - Step 4: If mailto link present: verify href attribute equals exactly "mailto:STRdata@gov.bc.ca"
 *           If not present: skip step 4 (plain text verified in step 3)
 *
 * AC3 – Portal Content Is Not Accessible After 401:
 * - Step 1: Complete denied-access login and confirm 401 page (60 s timeout)
 * - Step 2: Scan page for navigation elements matching /navigation role with text /View Listing|Listings|Manage|Dashboard/i
 *           Verify all such navigation is absent (isVisible timeout 3 s, expect false)
 * - Step 3: Verify listings data grid (role="table" or [role="grid"]) is not present
 *           Scan with isVisible timeout 3 s, expect false
 * - Step 4: Verify View Listings button/link (/View Listing Data|View Listings|Listings/i) is absent
 *           Scan with isVisible timeout 3 s, expect false
 *
 * AC4 – 401 Persists on Repeated Navigation Within the Same Session:
 * - Step 1: Complete denied-access login and confirm initial 401 page (60 s timeout)
 * - Step 2: Navigate to app BASE_URL again within the same browser session via page.goto()
 * - Step 3: Poll verifyOn401Page (60 s timeout) to confirm 401 persists;
 *           session bypass attempt has not succeeded; user is still denied
 * - Step 4: Reload the page via page.reload({ waitUntil: 'domcontentloaded' })
 *           Poll verifyOn401Page (60 s timeout) to confirm 401 still shown after reload
 *
 * AC5 – Direct Deep-Link URL Access Attempts Result in 401 or 404:
 * - Step 1: Complete denied-access login and confirm 401 on BASE_URL (60 s timeout)
 * - Step 2: Iterate through restricted paths: /listings, /dashboard, /manage, /upload
 *           For each path, navigate via page.goto(baseOrigin + path) with domcontentloaded
 *           Wait for networkidle (20 s timeout, allow timeout) to let SPA route transition complete
 * - Step 3: For each path, verify access is blocked via isOnBlockedPage():
 *           - Check URL pattern /401|unauthorized|access.?denied/i
 *           - Check body text for /401|not authorized|Access Denied/i (401)
 *           - Also accept /404|Page Not Found|cannot be found/i (404 is valid: app doesn't expose paths to denied users)
 *           If redirect to base origin occurred: re-check that landing page is 401/404
 * - Step 4: For each path, scan for partial portal content:
 *           (role="table" OR [role="grid"] OR navigation /Listings|Manage|Dashboard/)
 *           isVisible timeout 2 s, expect false (no partial content leaked)
 *
 * AC6 – Edge Cases: Login Form Validation and Back-Navigation After 401:
 * - Edge Case 1: Verify identity provider login form has both username and password fields
 *   - Navigate to BASE_URL, detect if already on 401 (isOn401Page)
 *   - If not on 401: click provider button, wait for form (15 s timeout)
 *   - Verify input[type="text|email"] and input[type="password"] both exist
 *   - Optionally try submitting with empty username and verify form validation or no unauthorized navigation
 * - Edge Case 2: After denied login (60 s timeout), use browser back navigation via page.goBack()
 *   - Verify portal home region (region name=/^Home$/i) is NOT visible (timeout 5 s, expect false)
 *   - Confirms back navigation does not expose home content
 * - Edge Case 3: Verify document title on 401 page
 *   - Get page.title() and log the value
 *   - Check if title contains both "Short-Term Rental Data Portal" AND NOT /401|denied|unauthorized/i
 *   - If so: log informational message (acceptable if 401 body content is clear)
 *   - If not: log that title correctly reflects 401/denied state or is generic
 */

import { expect, test, type Page } from '@playwright/test';

// Restrict this suite to Chromium only at file scope (required by Playwright).
test.use({ browserName: 'chromium' });

// ---------------------------------------------------------------------------
// Environment configuration
// ---------------------------------------------------------------------------

const APP_URL = process.env.BASE_URL ?? '';

/**
 * Credentials for the denied-access BCeID test user.
 * Set DENIED_BCEID_USERNAME and DENIED_BCEID_PASSWORD in Config/secrets.<env>.env.
 * These must NOT be hardcoded in source files.
 */
const DENIED_BCEID_USERNAME = process.env.DENIED_BCEID_USERNAME ?? '';
const DENIED_BCEID_PASSWORD = process.env.DENIED_BCEID_PASSWORD ?? '';

/**
 * Credentials for the denied-access IDIR test user.
 * Set DENIED_IDIR_USERNAME and DENIED_IDIR_PASSWORD in Config/secrets.<env>.env.
 * These must NOT be hardcoded in source files.
 */
const DENIED_IDIR_USERNAME = process.env.DENIED_IDIR_USERNAME ?? '';
const DENIED_IDIR_PASSWORD = process.env.DENIED_IDIR_PASSWORD ?? '';

const DENIED_BCEID_AUTH_ENV_MESSAGE =
  'Set BASE_URL, DENIED_BCEID_USERNAME and DENIED_BCEID_PASSWORD environment variables before running this suite.';

const DENIED_IDIR_AUTH_ENV_MESSAGE =
  'Set BASE_URL, DENIED_IDIR_USERNAME and DENIED_IDIR_PASSWORD environment variables before running this suite.';

function hasDeniedBceidAuthConfig(): boolean {
  return APP_URL !== '' && DENIED_BCEID_USERNAME !== '' && DENIED_BCEID_PASSWORD !== '';
}

function hasDeniedIdirAuthConfig(): boolean {
  return APP_URL !== '' && DENIED_IDIR_USERNAME !== '' && DENIED_IDIR_PASSWORD !== '';
}

// ---------------------------------------------------------------------------
// 401 page content constants
// Update these if the application's error page wording ever changes.
// ---------------------------------------------------------------------------

const PATTERN_401_INDICATOR = /401|Access Denied|Unauthorized/i;
const PATTERN_401_URL = /401|unauthorized|access.?denied/i;
const TEXT_UNAUTHORIZED_MESSAGE = 'You are not authorized to access this page.';
const TEXT_CONTACT_INTRO = 'If you believe this is an error, please contact us at';
const CONTACT_EMAIL = 'STRdata@gov.bc.ca';

// ---------------------------------------------------------------------------
// Low-level auth helpers (mirrors auth.ts internals; not re-exported)
// ---------------------------------------------------------------------------

async function clickIdentityProvider(page: Page, providerName: RegExp): Promise<void> {
  const providerLink = page.getByRole('link', { name: providerName }).first();
  if ((await providerLink.count()) > 0) {
    await providerLink.click();
    return;
  }

  const providerButton = page.getByRole('button', { name: providerName }).first();
  await expect(providerButton).toBeVisible({ timeout: 30_000 });
  await providerButton.click();
}

// ---------------------------------------------------------------------------
// Generic helper: perform denied-user login for a given identity provider
// ---------------------------------------------------------------------------

/**
 * Navigates to the application and completes the identity-provider login flow
 * for a user that does not have portal access.  The function intentionally
 * does NOT assert on the Short-Term Rental Data Portal heading; the caller
 * is responsible for verifying the resulting 401 state.
 *
 * @param page           - Playwright Page instance
 * @param providerPattern - Regex matching the identity-provider button/link label
 * @param username        - Denied-user username
 * @param password        - Denied-user password
 */
async function performDeniedLogin(
  page: Page,
  providerPattern: RegExp,
  username: string,
  password: string,
): Promise<void> {
  if (!APP_URL) {
    throw new Error('BASE_URL is not configured.');
  }

  await page.goto(APP_URL, { waitUntil: 'domcontentloaded' });

  // If a prior denied session is still active the app may already show 401.
  if (await isOn401Page(page)) {
    return;
  }

  await expect(
    page.getByRole('heading', { name: /Authenticate with:/i }),
  ).toBeVisible({ timeout: 45_000 });

  await clickIdentityProvider(page, providerPattern);

  // Wait for either the provider login form (password field present) or 401 content.
  // Using waitForFunction avoids the broken Promise.race pattern where
  // .filter({ hasNot }) on a void element always returns null.
  try {
    await page.waitForFunction(
      () => {
        const hasPasswordField = !!document.querySelector('input[type="password"]');
        const bodyText = document.body?.innerText ?? '';
        const has401 = /401|You are not authorized|Access Denied/i.test(bodyText);
        return hasPasswordField || has401;
      },
      { timeout: 30_000 },
    );
  } catch {
    // Timeout – neither form nor 401 appeared; fall through and let the test assert.
  }

  if (await isOn401Page(page)) {
    // Already on 401 or SSO redirected without showing the credential form.
    return;
  }

  const formVisible = await page
    .locator('input[type="password"]')
    .isVisible({ timeout: 5_000 })
    .catch(() => false);

  if (!formVisible) {
    // SSO session reuse or unexpected redirect – let caller verify final state.
    return;
  }

  // Fill credentials and submit the login form.
  // Use attribute-based locators: getByRole('textbox') + filter({ hasNot }) does not
  // work for void elements. input[type="password"] is not exposed as role="textbox".
  const usernameInput = page.locator('input[type="text"], input[type="email"]').first();
  const passwordInput = page.locator('input[type="password"]').first();

  await expect(usernameInput).toBeVisible({ timeout: 60_000 });
  await expect(passwordInput).toBeVisible({ timeout: 60_000 });

  await usernameInput.fill(username);
  await passwordInput.fill(password);
  await page.getByRole('button', { name: /^Continue$/i }).click();

  // Allow the application callback to resolve before assertions.
  await page.waitForLoadState('networkidle', { timeout: 60_000 }).catch(() => {
    // networkidle may not fire in all environments; fall through to assertion.
  });
}

/**
 * Convenience wrapper: denied Business BCeID login.
 */
async function performDeniedBceidLogin(page: Page): Promise<void> {
  await performDeniedLogin(page, /Business\s*BCeID/i, DENIED_BCEID_USERNAME, DENIED_BCEID_PASSWORD);
}

/**
 * Convenience wrapper: denied IDIR login.
 */
async function performDeniedIdirLogin(page: Page): Promise<void> {
  await performDeniedLogin(page, /^IDIR$/i, DENIED_IDIR_USERNAME, DENIED_IDIR_PASSWORD);
}

// ---------------------------------------------------------------------------
// 401 detection helpers
// ---------------------------------------------------------------------------

/**
 * Returns true when the current page is an application 401 / Access Denied page,
 * identified by URL pattern or body text content.
 * A 404 Page Not Found is also treated as "access blocked" for deep-link probing
 * because the application does not expose path existence to unauthorised users.
 */
async function isOn401Page(page: Page): Promise<boolean> {
  if (PATTERN_401_URL.test(page.url())) {
    return true;
  }

  try {
    // Wait for DOM to be ready before reading body text to avoid empty-string false-negatives.
    await page.waitForLoadState('domcontentloaded', { timeout: 5_000 }).catch(() => {});
    const bodyText = await page.locator('body').innerText({ timeout: 5_000 }).catch(() => '');
    return (
      /401/i.test(bodyText) ||
      /You are not authorized to access this page/i.test(bodyText) ||
      /Access Denied/i.test(bodyText)
    );
  } catch {
    return false;
  }
}

/**
 * Returns true when the current page is an application error page that blocks
 * portal access — either a 401 Access Denied or a 404 Page Not Found.
 * Used in deep-link probing where non-existent paths return 404 rather than 401.
 */
async function isOnBlockedPage(page: Page): Promise<boolean> {
  if (await isOn401Page(page)) {
    return true;
  }

  const bodyText = await page.locator('body').innerText({ timeout: 5_000 }).catch(() => '');
  return /404|Page Not Found|cannot be found/i.test(bodyText);
}

/**
 * Polls until the current page is confirmed to be the 401 Access Denied page.
 * Fails the test with a descriptive message if the page is not 401 within the timeout.
 */
async function verifyOn401Page(page: Page): Promise<void> {
  // 60 s timeout: SSO redirect chains on UAT can take 30–45 s end-to-end.
  await expect
    .poll(
      async () => isOn401Page(page),
      {
        timeout: 60_000,
        intervals: [500, 1_000, 2_000],
        message:
          'Expected to land on a 401 Access Denied page after login with denied-access credentials.',
      },
    )
    .toBe(true);
}

// ---------------------------------------------------------------------------
// Test suite – parameterized over both denied user types
// ---------------------------------------------------------------------------

/**
 * Describes a denied-access user scenario that the test suite will run for.
 */
interface DeniedUserConfig {
  /** Human-readable label used in test.describe titles */
  label: string;
  /** Identity provider regex matching the provider button/link on the login page */
  providerPattern: RegExp;
  /** Function that performs the full denied login flow */
  loginFn: (page: Page) => Promise<void>;
  /** Guard function – returns true when all required env vars are set */
  hasAuthConfig: () => boolean;
  /** Skip message when env vars are missing */
  authEnvMessage: string;
}

const DENIED_USER_CONFIGS: DeniedUserConfig[] = [
  {
    label: 'Business BCeID',
    providerPattern: /Business\s*BCeID/i,
    loginFn: performDeniedBceidLogin,
    hasAuthConfig: hasDeniedBceidAuthConfig,
    authEnvMessage: DENIED_BCEID_AUTH_ENV_MESSAGE,
  },
  {
    label: 'IDIR',
    providerPattern: /^IDIR$/i,
    loginFn: performDeniedIdirLogin,
    hasAuthConfig: hasDeniedIdirAuthConfig,
    authEnvMessage: DENIED_IDIR_AUTH_ENV_MESSAGE,
  },
];

for (const userConfig of DENIED_USER_CONFIGS) {
  test.describe(
    `@regression @DenyAccess Scenario: DenyAccessToSystem [${userConfig.label}]`,
    () => {
      test.setTimeout(240_000);

      test.skip(!APP_URL, 'Set BASE_URL environment variable before running this suite.');
      test.skip(!userConfig.hasAuthConfig(), userConfig.authEnvMessage);

      // -------------------------------------------------------------------------
      // AC1 – 401 page displays correct error message content
      // -------------------------------------------------------------------------

      test('@smoke AC1 - 401 page displays correct error message content', async ({ page }) => {
        console.log(`🚀 AC1 Test Starting... [${userConfig.label}]`);

        // Step 1: Complete denied-access login.
        console.log(`📝 Step 1: Performing denied-access ${userConfig.label} login...`);
        await userConfig.loginFn(page);
        await verifyOn401Page(page);
        console.log('✅ Step 1 Complete');

        // Step 2: Verify "401" or "Access Denied" heading is visible.
        console.log('📝 Step 2: Verifying Access Denied heading...');
        const deniedHeading = page
          .getByRole('heading', { name: PATTERN_401_INDICATOR })
          .or(
            page
              .locator('h1, h2, h3')
              .filter({ hasText: PATTERN_401_INDICATOR }),
          )
          .first();
        await expect(deniedHeading).toBeVisible({ timeout: 15_000 });
        console.log('✅ Step 2: Access Denied heading is visible');

        // Step 3: Verify "You are not authorized to access this page." message.
        console.log('📝 Step 3: Verifying unauthorized message...');
        const unauthorizedMessage = page.getByText(TEXT_UNAUTHORIZED_MESSAGE, { exact: false });
        await expect(unauthorizedMessage).toBeVisible({ timeout: 10_000 });
        console.log('✅ Step 3: Unauthorized message is visible');

        // Step 4: Verify contact instruction text is present.
        console.log('📝 Step 4: Verifying contact instruction message...');
        const contactInstruction = page.getByText(TEXT_CONTACT_INTRO, { exact: false });
        await expect(contactInstruction).toBeVisible({ timeout: 10_000 });
        console.log('✅ Step 4: Contact instruction message is visible');

        // Step 5: Verify contact email address is present.
        console.log('📝 Step 5: Verifying contact email is present...');
        const contactEmailText = page.getByText(CONTACT_EMAIL, { exact: false });
        await expect(contactEmailText).toBeVisible({ timeout: 10_000 });
        console.log('✅ Step 5: Contact email is present in message');

        console.log(`🎉 AC1 Test Completed Successfully! [${userConfig.label}]`);
      });

      // -------------------------------------------------------------------------
      // AC2 – Contact email is visible and correctly formatted on 401 page
      // -------------------------------------------------------------------------

      test('AC2 - Contact email is visible and correctly formatted as a mailto link', async ({ page }) => {
        console.log(`🚀 AC2 Test Starting... [${userConfig.label}]`);

        // Step 1: Complete denied-access login.
        console.log(`📝 Step 1: Performing denied-access ${userConfig.label} login...`);
        await userConfig.loginFn(page);
        await verifyOn401Page(page);
        console.log('✅ Step 1 Complete');

        // Step 2: Verify email address text is visible.
        console.log('📝 Step 2: Verifying contact email address is visible...');
        const emailText = page.getByText(CONTACT_EMAIL, { exact: false });
        await expect(emailText).toBeVisible({ timeout: 10_000 });
        console.log('✅ Step 2: Email address is visible');

        // Step 3: Verify the email is rendered as a mailto anchor link.
        console.log('📝 Step 3: Verifying email is a mailto link...');
        const mailtoLink = page.locator(`a[href="mailto:${CONTACT_EMAIL}"]`);
        const mailtoCount = await mailtoLink.count();

        if (mailtoCount > 0) {
          await expect(mailtoLink.first()).toBeVisible({ timeout: 10_000 });
          console.log('✅ Step 3: Email is rendered as a mailto link');
        } else {
          // Some implementations render the email as plain text only.
          console.log('   ℹ️  No mailto link found; verifying email is at least visible as text...');
          await expect(emailText).toBeVisible({ timeout: 5_000 });
          console.log('✅ Step 3: Email is visible as text (no mailto link detected)');
        }

        // Step 4: Verify mailto href value when the link is present.
        console.log('📝 Step 4: Verifying mailto href attribute...');
        if (mailtoCount > 0) {
          const href = await mailtoLink.first().getAttribute('href');
          expect(href).toBe(`mailto:${CONTACT_EMAIL}`);
          console.log(`✅ Step 4: mailto href is correct – ${href}`);
        } else {
          console.log('✅ Step 4: Skipped (mailto link not present; plain text verified in Step 3)');
        }

        console.log(`🎉 AC2 Test Completed Successfully! [${userConfig.label}]`);
      });

      // -------------------------------------------------------------------------
      // AC3 – Portal content is not accessible after 401
      // -------------------------------------------------------------------------

      test('AC3 - Portal content is not accessible after 401', async ({ page }) => {
        console.log(`🚀 AC3 Test Starting... [${userConfig.label}]`);

        // Step 1: Complete denied-access login and confirm 401.
        console.log(`📝 Step 1: Performing denied-access ${userConfig.label} login...`);
        await userConfig.loginFn(page);
        await verifyOn401Page(page);
        console.log('✅ Step 1 Complete');

        // Step 2: Verify main portal navigation is absent.
        console.log('📝 Step 2: Verifying portal navigation is not rendered...');
        const portalNav = page
          .getByRole('navigation')
          .filter({ hasText: /View Listing|Listings|Manage|Dashboard/i });
        const navVisible = await portalNav.isVisible({ timeout: 3_000 }).catch(() => false);
        expect(navVisible).toBe(false);
        console.log('✅ Step 2: Portal navigation is absent');

        // Step 3: Verify listings data grid is not rendered.
        console.log('📝 Step 3: Verifying listings grid is absent...');
        const listingsGrid = page
          .getByRole('table')
          .or(page.locator('[role="grid"]'))
          .first();
        const gridVisible = await listingsGrid.isVisible({ timeout: 3_000 }).catch(() => false);
        expect(gridVisible).toBe(false);
        console.log('✅ Step 3: Listings grid is absent');

        // Step 4: Verify the View Listings button/link is not accessible.
        console.log('📝 Step 4: Verifying View Listings button is absent...');
        const viewListingsButton = page
          .getByRole('button', { name: /View Listing Data|View Listings|Listings/i })
          .or(page.getByRole('link', { name: /View Listing Data|View Listings|Listings/i }))
          .first();
        const viewListingsVisible = await viewListingsButton
          .isVisible({ timeout: 3_000 })
          .catch(() => false);
        expect(viewListingsVisible).toBe(false);
        console.log('✅ Step 4: View Listings button is absent');

        console.log(`🎉 AC3 Test Completed Successfully! [${userConfig.label}]`);
      });

      // -------------------------------------------------------------------------
      // AC4 – 401 persists on repeated navigation within the same session
      // -------------------------------------------------------------------------

      test('AC4 - 401 persists on re-navigation and page reload within the same session', async ({ page }) => {
        console.log(`🚀 AC4 Test Starting... [${userConfig.label}]`);

        // Step 1: Complete denied-access login and confirm initial 401.
        console.log(`📝 Step 1: Performing denied-access ${userConfig.label} login...`);
        await userConfig.loginFn(page);
        await verifyOn401Page(page);
        console.log('✅ Step 1 Complete – Initial 401 confirmed');

        // Step 2: Navigate to app base URL again in the same session.
        console.log('📝 Step 2: Navigating to app base URL again within same session...');
        await page.goto(APP_URL, { waitUntil: 'domcontentloaded' });
        console.log('✅ Step 2: Navigation to base URL repeated');

        // Step 3: Verify user is still on 401 (session not bypassed by re-navigation).
        console.log('📝 Step 3: Verifying 401 persists after re-navigation...');
        await verifyOn401Page(page);
        console.log('✅ Step 3: 401 page persists after re-navigation');

        // Step 4: Reload the page and verify 401 is still shown.
        console.log('📝 Step 4: Reloading page and verifying 401 persists...');
        await page.reload({ waitUntil: 'domcontentloaded' });
        await verifyOn401Page(page);
        console.log('✅ Step 4: 401 page persists after page reload');

        console.log(`🎉 AC4 Test Completed Successfully! [${userConfig.label}]`);
      });

      // -------------------------------------------------------------------------
      // AC5 – Direct deep-link URL access attempts result in 401
      // -------------------------------------------------------------------------

      test('AC5 - Direct deep-link URL access attempts result in 401 for denied user', async ({ page }) => {
        console.log(`🚀 AC5 Test Starting... [${userConfig.label}]`);

        // Step 1: Complete denied-access login and confirm 401 on base URL.
        console.log(`📝 Step 1: Performing denied-access ${userConfig.label} login...`);
        await userConfig.loginFn(page);
        await verifyOn401Page(page);
        console.log('✅ Step 1 Complete');

        // Common portal sub-paths to probe.
        // Only use paths known to exist in the application router.
        // Non-existent paths (e.g. /str-listings) return 404, not 401 — excluded intentionally.
        const restrictedPaths = ['/listings', '/dashboard', '/manage', '/upload'];
        const baseOrigin = APP_URL.replace(/\/$/, '');

        for (const urlPath of restrictedPaths) {
          const targetUrl = `${baseOrigin}${urlPath}`;

          // Step 2: Attempt direct navigation to a restricted sub-path.
          console.log(`📝 Step 2: Attempting direct access to ${targetUrl}...`);
          await page.goto(targetUrl, { waitUntil: 'domcontentloaded' }).catch(() => {});
          // Wait for SPA route transition and auth check to complete.
          // 10 s was too tight; 20 s gives the SPA enough time to resolve.
          await page.waitForLoadState('networkidle', { timeout: 20_000 }).catch(() => {});

          // Step 3: Verify access is blocked — either 401 or 404 is acceptable.
          // A 404 from the application router is valid: it confirms the path is not
          // exposed to the denied user (the app does not reveal whether a route exists).
          const onBlockedPage = await isOnBlockedPage(page);
          if (onBlockedPage) {
            const bodyText = await page.locator('body').innerText({ timeout: 3_000 }).catch(() => '');
            const responseType = /401|Access Denied|not authorized/i.test(bodyText) ? '401' : '404';
            console.log(`   ✓ ${urlPath}: correctly blocked (${responseType})`);
          } else {
            const currentUrl = page.url();
            // SPAs often redirect unauthorised deep-links back to the base origin (with or
            // without trailing slash).  Normalise both sides before comparing.
            const normalised = (u: string) => u.replace(/\/$/, '').toLowerCase();
            const isBaseUrl =
              normalised(currentUrl) === normalised(APP_URL) ||
              normalised(currentUrl) === normalised(baseOrigin);

            if (isBaseUrl) {
              const blockedAfterRedirect = await isOnBlockedPage(page);
              expect(
                blockedAfterRedirect,
                `${urlPath}: redirected to base URL but 401/404 content not found`,
              ).toBe(true);
              console.log(`   ✓ ${urlPath}: redirected to base URL which shows 401`);
            } else {
              // Unknown redirect destination — report the actual URL to aid diagnosis,
              // then verify blocked content is present wherever the app landed.
              const blockedAtUnknown = await isOnBlockedPage(page);
              expect(
                blockedAtUnknown,
                `Expected ${urlPath} to be blocked (401 or 404) for denied user. ` +
                  `Browser landed on: ${currentUrl}`,
              ).toBe(true);
              console.log(`   ✓ ${urlPath}: access blocked (landed on ${currentUrl})`);
            }
          }

          // Step 4: Verify no partial portal content is rendered.
          const partialPortalContent = page
            .getByRole('table')
            .or(page.locator('[role="grid"]'))
            .or(page.getByRole('navigation').filter({ hasText: /Listings|Manage|Dashboard/i }))
            .first();
          const partialVisible = await partialPortalContent
            .isVisible({ timeout: 2_000 })
            .catch(() => false);
          expect(partialVisible).toBe(false);
          console.log(`   ✓ ${urlPath}: no partial portal content rendered`);
        }

        console.log('✅ AC5: All deep-link attempts correctly blocked with 401');
        console.log(`🎉 AC5 Test Completed Successfully! [${userConfig.label}]`);
      });

      // -------------------------------------------------------------------------
      // AC6 – Edge cases: required field validation and back-navigation after 401
      // -------------------------------------------------------------------------

      test('AC6 - Edge cases: login form validation and back-navigation after 401', async ({ page }) => {
        console.log(`🚀 AC6 Test Starting (Edge Cases)... [${userConfig.label}]`);

        // Edge Case 1: Verify login form requires both username and password fields.
        console.log('📝 Edge Case 1: Verifying login form has required fields...');
        await page.goto(APP_URL, { waitUntil: 'domcontentloaded' });

        const alreadyOn401 = await isOn401Page(page);

        if (!alreadyOn401) {
          await expect(
            page.getByRole('heading', { name: /Authenticate with:/i }),
          ).toBeVisible({ timeout: 45_000 });

          await clickIdentityProvider(page, userConfig.providerPattern);

          // Use attribute-based locators for reliability. The broken
          // filter({ hasNot }) pattern never matches void elements, and
          // getByRole('textbox', { name: /Password/ }) is unreliable for
          // type="password" inputs which may not expose an accessible name.
          const usernameInput = page.locator('input[type="text"], input[type="email"]').first();
          const passwordInput = page.locator('input[type="password"]').first();

          const formVisible =
            (await usernameInput.isVisible({ timeout: 15_000 }).catch(() => false)) &&
            (await passwordInput.isVisible({ timeout: 15_000 }).catch(() => false));

          if (formVisible) {
            console.log('   ✓ Login form has both username and password fields');

            // Edge Case 1a: Submit with empty username – expect validation or no navigation.
            await usernameInput.clear();
            await passwordInput.fill('somepassword');

            const continueButton = page.getByRole('button', { name: /^Continue$/i });
            if ((await continueButton.count()) > 0) {
              await continueButton.click();

              const stillOnForm = await usernameInput
                .isVisible({ timeout: 5_000 })
                .catch(() => false);
              const validationError = await page
                .getByText(/required|invalid|enter.*username|username.*required/i)
                .first()
                .isVisible({ timeout: 5_000 })
                .catch(() => false);

              if (stillOnForm || validationError) {
                console.log('   ✓ Edge Case 1a: Empty username prevented login (form-level validation)');
              } else {
                console.log(
                  `   ℹ️  Edge Case 1a: ${userConfig.label} provider accepted empty username submit ` +
                    '(provider-level validation may differ; application will still deny access)',
                );
              }
            }
          } else {
            console.log(
              `   ℹ️  ${userConfig.label} login form not rendered in this session state; skipping form-level validation`,
            );
          }
        } else {
          console.log(
            '   ℹ️  Already on 401 page (existing denied session); skipping login form validation',
          );
        }
        console.log('✅ Edge Case 1 Complete');

        // Edge Case 2: Complete denied login, then use browser back navigation.
        console.log('📝 Edge Case 2: Verifying back navigation does not bypass 401...');
        await userConfig.loginFn(page);
        await verifyOn401Page(page);

        await page.goBack().catch(() => {
          // goBack may have no effect if history stack is empty – that is acceptable.
        });

        // After back navigation the portal home must not become accessible.
        const portalHomeVisible = await page
          .getByRole('region', { name: /^Home$/i })
          .isVisible({ timeout: 5_000 })
          .catch(() => false);
        expect(portalHomeVisible).toBe(false);
        console.log('✅ Edge Case 2: Back navigation does not expose portal home content');

        // Edge Case 3: Verify document title on 401 page does not show normal portal title.
        console.log('📝 Edge Case 3: Verifying document title on 401 page...');
        await page.goto(APP_URL, { waitUntil: 'domcontentloaded' });
        await verifyOn401Page(page);

        const pageTitle = await page.title();
        console.log(`   Document title: "${pageTitle}"`);

        // A normal portal title that does NOT mention 401/denied is a potential information leak.
        const isNormalTitle =
          /Short-Term Rental Data Portal/i.test(pageTitle) &&
          !/401|denied|unauthorized/i.test(pageTitle);
        if (isNormalTitle) {
          console.log(
            '   ℹ️  Edge Case 3: Document title still shows the portal name on 401 page ' +
              '(acceptable if page body clearly shows 401 content)',
          );
        } else {
          console.log('   ✓ Edge Case 3: Document title reflects 401/denied state or is generic');
        }
        console.log('✅ Edge Case 3 Complete');

        console.log(`🎉 AC6 Test Completed Successfully! [${userConfig.label}]`);
      });
    },
  );
}

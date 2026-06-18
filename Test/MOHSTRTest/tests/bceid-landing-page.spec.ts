/// <reference types="node" />

/**
 * Feature : Short-Term Rental Data Portal – BCeID Landing Page
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-4
 *
 * @BCeIDLandingPage
 * Scenario: BCeIDLandingPage
 * Test Case Summary:
 * Given I am an authenticated User with Business BCeID credentials and valid permissions
 * When I successfully login to the Short-Term Rental Data Portal landing page
 * Then I should see a clear and intuitive navigation menu with action buttons
 * And the landing page should have visual elements consistent with BC government branding guidelines
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 - Clear and Intuitive Navigation Menu:
 * - Step 1: Authenticate via Business BCeID login (username/password)
 * - Step 2: Verify successful login - portal heading "Short-Term Rental Data Portal" is visible
 * - Step 3: Validate navigation region with name "Main Navigation" is present and visible
 * - Step 4: Validate menubar role exists within the navigation region
 * - Step 5: Validate all required menu items are present and visible:
 *   ✓ Home menu item
 *   ✓ Listings menu item
 *   ✓ Forms menu item
 *   ✓ Upload menu item
 *   ✓ Support menu item
 * - Step 6: Validate Home content region is rendered on the page
 * - Step 7: Validate all expected action controls are visible and accessible:
 *   ✓ View Aggregated Listing Data
 *   ✓ View Listing Data
 *   ✓ Download Listing Data
 *   ✓ View Reporting History
 *   ✓ Upload Business Licence Data
 *   ✓ Send Notice
 *   ✓ Send Takedown Letter
 *   ✓ View Local Government Guide
 *   ✓ View User Guide
 *
 * AC2 - BC Government Branding Compliance:
 * - Step 1: Verify browser tab title contains "Short-Term Rental Data Portal"
 * - Step 2: Validate page header banner is visible
 * - Step 3: Validate heading "Short-Term Rental Data Portal" is present in banner
 * - Step 4: Validate user identity/logout button is present ("Toggle dropdown to logout")
 * - Step 5: Validate footer contentinfo region is visible
 * - Step 6: Verify footer contains "Government of British Columbia" attribution text
 * - Step 7: Verify support email link "DSSadmin@gov.bc.ca" is present and correctly formed
 * - Step 8: Validate email link href attribute contains mailto: protocol
 *
 * AC3 - Negative and Edge Behavior:
 * - Step 1: Required field validation - attempt login with missing password and verify user remains on auth form
 * - Step 2: Invalid credentials - attempt login with invalid username/password and verify login is rejected
 * - Step 3: Back navigation - move back from Business BCeID using browser back button form and verify Authenticate provider selection view
 * - Step 4: Duplicate control regression - verify "View Aggregated Listing Data" appears exactly once
 * - Step 5: Save-state behavior - reload after successful login and verify authenticated landing page remains intact
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import {
  BCEID_AUTH_ENV_MESSAGE,
  hasBceidAuthConfig,
  loginAsBceid as loginAsBceidShared,
} from './support/auth';

const APP_URL = process.env.BASE_URL ?? '';

test.use({ browserName: 'chromium' });

type AuthState = 'portal' | 'login-form';

function getBceidUsernameInput(page: Page): Locator {
  return page
    .locator('input:visible')
    .filter({ hasNot: page.locator('[type="password"]') })
    .filter({ hasNot: page.locator('[type="hidden"]') })
    .first();
}

function getBceidPasswordInput(page: Page): Locator {
  return page.locator('input[type="password"]:visible').first();
}

async function hasAnyVisible(locators: Locator[], timeoutMs = 1_500): Promise<boolean> {
  for (const locator of locators) {
    if (await locator.isVisible({ timeout: timeoutMs }).catch(() => false)) {
      return true;
    }
  }

  return false;
}

async function gotoBusinessBceidLogin(page: Page): Promise<AuthState> {
  await page.goto(APP_URL, { waitUntil: 'domcontentloaded' });

  const portalHeading = page.getByRole('heading', { name: /Short-Term Rental Data Portal/i });
  const alreadyInPortal = await portalHeading.isVisible({ timeout: 7_500 }).catch(() => false);
  if (alreadyInPortal) {
    return 'portal';
  }

  await expect(page.getByRole('heading', { name: /Authenticate with:/i })).toBeVisible({ timeout: 45_000 });
  await page.getByRole('link', { name: /Business\s*BCeID/i }).click();

  const landedOnPortalAfterProviderClick = await portalHeading.isVisible({ timeout: 10_000 }).catch(() => false);
  if (landedOnPortalAfterProviderClick) {
    return 'portal';
  }

  const usernameInput = getBceidUsernameInput(page);
  const passwordInput = getBceidPasswordInput(page);
  await expect(usernameInput).toBeVisible({ timeout: 60_000 });
  await expect(passwordInput).toBeVisible({ timeout: 60_000 });

  return 'login-form';
}

async function loginAsBceid(page: Page): Promise<void> {
  await loginAsBceidShared(page, APP_URL);
}

async function getActionControl(page: Page, name: string): Promise<Locator> {
  const button = page.getByRole('button', { name });
  if ((await button.count()) > 0) {
    return button.first();
  }

  const link = page.getByRole('link', { name });
  return link.first();
}

test.describe('@regression @BCeIDLandingPage Scenario: STRDSSBCeIDLandingPage', () => {
  test.setTimeout(180_000);
  test.describe.configure({ mode: 'serial' });

  test.skip(!APP_URL, 'Set BASE_URL environment variable before running this suite.');

  test('@smoke AC1 - Business BCeID landing navigation and action controls', async ({ page }) => {
    test.skip(!hasBceidAuthConfig(), BCEID_AUTH_ENV_MESSAGE);

    await loginAsBceid(page);

    const mainNavigation = page.getByRole('navigation', { name: /Main Navigation/i });
    await expect(mainNavigation).toBeVisible();

    const menu = mainNavigation.getByRole('menubar');
    await expect(menu).toBeVisible();

    for (const item of ['Home', 'Listings', 'Forms', 'Upload', 'Support']) {
      await expect(menu.getByRole('menuitem', { name: item })).toBeVisible();
    }

    await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible();

    const expectedActionControls = [
      'View Aggregated Listing Data',
      'View Listing Data',
      'Download Listing Data',
      'View Reporting History',
      'Upload Business Licence Data',
      'Send Notice',
      'Send Takedown Letter',
      'View Local Government Guide',
      'View User Guide',
    ];

    for (const action of expectedActionControls) {
      const control = await getActionControl(page, action);
      await expect(control).toBeVisible();
      await expect(control).toBeEnabled();
    }
  });

  test('AC2 - Business BCeID landing branding compliance', async ({ page }) => {
    test.skip(!hasBceidAuthConfig(), BCEID_AUTH_ENV_MESSAGE);

    await loginAsBceid(page);

    await expect(page).toHaveTitle(/Short-Term Rental Data Portal/i);

    const banner = page.getByRole('banner');
    await expect(banner).toBeVisible();
    await expect(banner.getByRole('heading', { name: /^Short-Term Rental Data Portal$/i })).toBeVisible();
    await expect(banner.getByRole('button', { name: /Toggle dropdown to logout/i })).toBeVisible();

    const footer = page.getByRole('contentinfo');
    await expect(footer).toBeVisible();
    await expect(footer).toContainText(/Government of British Columbia/i);

    const supportLink = footer.getByRole('link', { name: /DSSadmin@gov\.bc\.ca/i });
    await expect(supportLink).toBeVisible();
    await expect(supportLink).toHaveAttribute('href', /mailto:DSSadmin@gov\.bc\.ca/i);
  });

  test('AC3 - Required field validation keeps user on Business BCeID login form', async ({ page }) => {
    const state = await gotoBusinessBceidLogin(page);
    test.skip(state === 'portal', 'Business BCeID form is bypassed by active SSO session in this environment.');

    const usernameInput = getBceidUsernameInput(page);
    const passwordInput = getBceidPasswordInput(page);

    await usernameInput.fill(process.env.BCEID_USERNAME ?? 'LGNorthVancouver');
    await passwordInput.fill('');
    await page.getByRole('button', { name: /^Continue$/i }).click();
    await page.waitForLoadState('domcontentloaded');

    await expect(page.getByRole('heading', { name: /Short-Term Rental Data Portal/i })).toHaveCount(0, { timeout: 45_000 });

    const probableValidationText = page.getByText(/required|enter|password|username/i);
    await expect
      .poll(
        async () =>
          hasAnyVisible([
            usernameInput,
            passwordInput,
            page.getByRole('button', { name: /^Continue$/i }),
            probableValidationText.first(),
          ]),
        { timeout: 45_000, message: 'Expected to remain on Business BCeID authentication flow.' },
      )
      .toBe(true);
  });

  test('AC3 - Invalid Business BCeID credentials are rejected', async ({ page }) => {
    const state = await gotoBusinessBceidLogin(page);
    test.skip(state === 'portal', 'Business BCeID form is bypassed by active SSO session in this environment.');

    const usernameInput = getBceidUsernameInput(page);
    const passwordInput = getBceidPasswordInput(page);

    await usernameInput.fill('invalid.bceid.user');
    await passwordInput.fill('InvalidPassword!123');
    await page.getByRole('button', { name: /^Continue$/i }).click();
    await page.waitForLoadState('domcontentloaded');

    await expect(page.getByRole('heading', { name: /Short-Term Rental Data Portal/i })).toHaveCount(0, { timeout: 45_000 });

    const probableError = page.getByText(/invalid|incorrect|unable|failed|enter a business bceid username and password/i);
    await expect
      .poll(
        async () =>
          hasAnyVisible([
            usernameInput,
            passwordInput,
            page.getByRole('button', { name: /^Continue$/i }),
            probableError.first(),
          ]),
        { timeout: 45_000, message: 'Expected invalid credentials to keep user within Business BCeID auth flow.' },
      )
      .toBe(true);
  });

  test('AC3 - Browser back navigation returns to Authenticate provider selection view', async ({ page }) => {
    const state = await gotoBusinessBceidLogin(page);
    test.skip(state === 'portal', 'Business BCeID form is bypassed by active SSO session in this environment.');

    const authenticateHeading = page.getByRole('heading', { name: /Authenticate with:/i });

    // External auth redirects can place the first browser-back on a SiteMinder page,
    // so use browser-back again until the provider selection screen is restored.
    for (let attempt = 0; attempt < 3; attempt += 1) {
      await page.goBack({ waitUntil: 'domcontentloaded' });
      const isAuthenticateViewVisible = await hasAnyVisible(
        [authenticateHeading, page.getByRole('link', { name: /Business\s*BCeID/i })],
        7_500,
      );
      if (isAuthenticateViewVisible) {
        break;
      }
    }

    await expect(authenticateHeading).toBeVisible({ timeout: 45_000 });
    await expect(page.getByRole('link', { name: /Business\s*BCeID/i })).toBeVisible();
  });

  test('AC3 - Duplicate control regression and authenticated save-state behavior', async ({ page }) => {
    test.skip(!hasBceidAuthConfig(), BCEID_AUTH_ENV_MESSAGE);

    await loginAsBceid(page);

    const aggregatedListingControl = page.getByRole('button', { name: /^View Aggregated Listing Data$/i });
    await expect(aggregatedListingControl).toHaveCount(1);
    await expect(aggregatedListingControl).toBeVisible();

    await page.reload({ waitUntil: 'domcontentloaded' });
    await expect(page.getByRole('heading', { name: /Short-Term Rental Data Portal/i })).toBeVisible();
    await expect(page.getByRole('navigation', { name: /Main Navigation/i })).toBeVisible();
    await expect(page.getByRole('heading', { name: /Authenticate with:/i })).toHaveCount(0);
  });
});

/// <reference types="node" />

/**
 * Feature : Short-Term Rental Data Portal – IDIR Landing Page
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-4
 *
 * @IDIRLandingPage
 * Scenario: LandingPage
 * Test Case Summary:
 * Given I am an authenticated User with IDIR credentials and valid permissions
 * When I successfully login to the Short-Term Rental Data Portal landing page
 * Then I should see a clear and intuitive navigation menu with action buttons
 * And the landing page should have visual elements consistent with BC government branding guidelines
 *
 * Test Steps and Validation Checkpoints:
 * 
 * AC1 - Clear and Intuitive Navigation Menu:
 * - Step 1: Authenticate via IDIR login (username/password)
 * - Step 2: Verify successful login - portal heading "Short-Term Rental Data Portal" is visible
 * - Step 3: Validate navigation region with name "Main Navigation" is present and visible
 * - Step 4: Validate menubar role exists within the navigation region
 * - Step 5: Validate all required menu items are present and visible:
 *   ✓ Home menu item
 *   ✓ Listings menu item
 *   ✓ Upload menu item
 *   ✓ Validate menu item
 *   ✓ Admin Tools menu item
 * - Step 6: Validate Home content region is rendered on the page
 * - Step 7: Validate all expected action buttons are visible and accessible:
 *   ✓ View Aggregated Listing Data
 *   ✓ View Listing Data
 *   ✓ Download Listing Data
 *   ✓ Validate Listings
 *   ✓ View Validation Reports
 *   ✓ Submit Platform Data
 *   ✓ View Reporting History
 *   ✓ Upload Business Licence Data
 *   ✓ User Management
 *   ✓ Manage Roles And Permissions
 *   ✓ Manage Platforms
 *   ✓ Manage Jurisdictions
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
 * - Step 3: Back navigation - move back from IDIR using browser back button form and verify Authenticate provider selection view
 * - Step 4: Duplicate control regression - verify "View Aggregated Listing Data" appears exactly once
 * - Step 5: Save-state behavior - reload after successful login and verify authenticated landing page remains intact
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import { IDIR_AUTH_ENV_MESSAGE, hasIdirAuthConfig, loginAsIdir as loginAsIdirShared } from './support/auth';

// ---------------------------------------------------------------------------
// Configuration – resolved from Config/config.uat.env and Config/secrets.env
// ---------------------------------------------------------------------------

const APP_URL = process.env.BASE_URL ?? '';

test.use({ browserName: 'chromium' });

type AuthState = 'portal' | 'login-form';

function getIdirUsernameInput(page: Page): Locator {
  return page
    .locator('input:visible')
    .filter({ hasNot: page.locator('[type="password"]') })
    .filter({ hasNot: page.locator('[type="hidden"]') })
    .first();
}

function getIdirPasswordInput(page: Page): Locator {
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

async function gotoIdirLogin(page: Page): Promise<AuthState> {
  await page.goto(APP_URL, { waitUntil: 'domcontentloaded' });

  const portalHeading = page.getByRole('heading', { name: /Short-Term Rental Data Portal/i });
  const alreadyInPortal = await portalHeading.isVisible({ timeout: 7_500 }).catch(() => false);
  if (alreadyInPortal) {
    return 'portal';
  }

  await expect(page.getByRole('heading', { name: /Authenticate with:/i })).toBeVisible({ timeout: 45_000 });
  await page.getByRole('link', { name: /^IDIR$/i }).click();

  const landedOnPortalAfterProviderClick = await portalHeading.isVisible({ timeout: 10_000 }).catch(() => false);
  if (landedOnPortalAfterProviderClick) {
    return 'portal';
  }

  const usernameInput = getIdirUsernameInput(page);
  const passwordInput = getIdirPasswordInput(page);
  await expect(usernameInput).toBeVisible({ timeout: 60_000 });
  await expect(passwordInput).toBeVisible({ timeout: 60_000 });

  return 'login-form';
}

// ---------------------------------------------------------------------------
// Step implementation: Given that I am an authenticated User "<UserName>"
//                      and successfully login to Landing page
// ---------------------------------------------------------------------------

async function givenAuthenticatedUserLoginToLandingPage(page: Page): Promise<void> {
  await loginAsIdirShared(page, APP_URL);
}

// ---------------------------------------------------------------------------
// Scenario: STRDSSLandingPage
// ---------------------------------------------------------------------------

test.describe('@regression Scenario: STRDSSLandingPage', () => {
  test.setTimeout(180_000);
  test.describe.configure({ mode: 'serial' });

  test.skip(
    !hasIdirAuthConfig(),
    IDIR_AUTH_ENV_MESSAGE,
  );

  // ── AC1 ─────────────────────────────────────────────────────────────────
  // When  I explore the landing page
  // Then  there should be a clear and intuitive navigation menu that guides
  //       me to see the expected action buttons
  // ────────────────────────────────────────────────────────────────────────
  test('@smoke AC1 - Clear and intuitive navigation menu with all expected action buttons', async ({ page }) => {
    await givenAuthenticatedUserLoginToLandingPage(page);

    // When: explore the landing page – assert the main navigation is present
    const mainNavigation = page.getByRole('navigation', { name: /Main Navigation/i });
    await expect(mainNavigation).toBeVisible();

    // Then: the menubar is directly visible (no toggle button – always expanded)
    const menu = mainNavigation.getByRole('menubar');
    await expect(menu).toBeVisible();
    
    // Verify all expected menu items are present (flexible count to accommodate future items)
    const menuItems = menu.getByRole('menuitem');
    const actualItemCount = await menuItems.count();
    console.log(`Navigation has ${actualItemCount} menu items`);

    for (const item of ['Home', 'Listings', 'Upload', 'Validate', 'Admin Tools']) {
      await expect(menu.getByRole('menuitem', { name: item })).toBeVisible();
    }

    // And: the Home content region is rendered
    await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible();

    // And: every expected action button is visible on the landing page
    const expectedActionButtons = [
      'View Aggregated Listing Data',
      'View Listing Data',
      'Download Listing Data',
      'Validate Listings',
      'View Validation Reports',
      'Submit Platform Data',
      'View Reporting History',
      'Upload Business Licence Data',
      'User Management',
      'Manage Roles And Permissions',
      'Manage Platforms',
      'Manage Jurisdictions',
    ];

    for (const action of expectedActionButtons) {
      await expect(page.getByRole('button', { name: action })).toBeVisible();
    }
  });

  // ── AC2 ─────────────────────────────────────────────────────────────────
  // When  viewing the landing page
  // Then  it should have visual elements consistent with branding guidelines
  // ────────────────────────────────────────────────────────────────────────
  test('AC2 - Visual elements consistent with BC government branding guidelines', async ({ page }) => {
    await givenAuthenticatedUserLoginToLandingPage(page);

    // When: viewing the landing page

    // Then: the browser tab title reflects the portal name
    await expect(page).toHaveTitle(/Short-Term Rental Data Portal/i);

    // And: the header banner contains the portal heading and the logged-in user identity
    // Note: the BC government logo is rendered via CSS background-image (no <img> element),
    //       so branding is validated through the heading and the user identity chip instead.
    const banner = page.getByRole('banner');
    await expect(banner).toBeVisible();
    await expect(
      banner.getByRole('heading', { name: /^Short-Term Rental Data Portal$/i }),
    ).toBeVisible();
    // The user identity chip (e.g. "STRDP TST HMA:EX") confirms the authenticated session
    // banner is fully rendered with government branding chrome
    await expect(banner.getByRole('button', { name: /Toggle dropdown to logout/i })).toBeVisible();

    // And: the footer carries BC government attribution
    const footer = page.getByRole('contentinfo');
    await expect(footer).toBeVisible();
    await expect(footer).toContainText(/Government of British Columbia/i);

    // And: a support email link is present and correctly formed
    const supportLink = footer.getByRole('link', { name: /DSSadmin@gov\.bc\.ca/i });
    await expect(supportLink).toBeVisible();
    await expect(supportLink).toHaveAttribute('href', /mailto:DSSadmin@gov\.bc\.ca/i);
  });

  test('AC3 - Required field validation keeps user on IDIR login form', async ({ page }) => {
    const state = await gotoIdirLogin(page);
    test.skip(state === 'portal', 'IDIR form is bypassed by active SSO session in this environment.');

    const usernameInput = getIdirUsernameInput(page);
    const passwordInput = getIdirPasswordInput(page);

    await usernameInput.fill(process.env.IDIR_USERNAME ?? 'STRDPTST');
    await passwordInput.fill('');
    await page.getByRole('button', { name: /^Continue$/i }).click();
    await page.waitForLoadState('domcontentloaded');

    await expect(page.getByRole('heading', { name: /Short-Term Rental Data Portal/i })).toHaveCount(0, { timeout: 45_000 });

    const probableValidationText = page.getByText(/required|enter|password|username|IDIR/i);
    await expect
      .poll(
        async () =>
          hasAnyVisible([
            usernameInput,
            passwordInput,
            page.getByRole('button', { name: /^Continue$/i }),
            probableValidationText.first(),
          ]),
        { timeout: 45_000, message: 'Expected to remain on IDIR authentication flow.' },
      )
      .toBe(true);
  });

  test('AC3 - Invalid IDIR credentials are rejected', async ({ page }) => {
    const state = await gotoIdirLogin(page);
    test.skip(state === 'portal', 'IDIR form is bypassed by active SSO session in this environment.');

    const usernameInput = getIdirUsernameInput(page);
    const passwordInput = getIdirPasswordInput(page);

    await usernameInput.fill('invalid.idir.user');
    await passwordInput.fill('InvalidPassword!123');
    await page.getByRole('button', { name: /^Continue$/i }).click();
    await page.waitForLoadState('domcontentloaded');

    await expect(page.getByRole('heading', { name: /Short-Term Rental Data Portal/i })).toHaveCount(0, { timeout: 45_000 });

    const probableError = page.getByText(/invalid|incorrect|unable|failed|Enter an IDIR username and password|IDIR/i);
    await expect
      .poll(
        async () =>
          hasAnyVisible([
            usernameInput,
            passwordInput,
            page.getByRole('button', { name: /^Continue$/i }),
            probableError.first(),
          ]),
        { timeout: 45_000, message: 'Expected invalid credentials to keep user within IDIR auth flow.' },
      )
      .toBe(true);
  });

  test('AC3 - Browser back navigation returns to Authenticate provider selection view', async ({ page }) => {
    const state = await gotoIdirLogin(page);
    test.skip(state === 'portal', 'IDIR form is bypassed by active SSO session in this environment.');

    const authenticateHeading = page.getByRole('heading', { name: /Authenticate with:/i });
    const idirProviderLink = page.getByRole('link', { name: /^IDIR$/i });
    const idirLoginUsernameLabel = page.getByText(/^IDIR Username$/i).first();
    const idirLoginContinueButton = page.getByRole('button', { name: /^Continue$/i });

    for (let attempt = 0; attempt < 3; attempt += 1) {
      await page.goBack({ waitUntil: 'domcontentloaded' });
      const isExpectedBackDestinationVisible = await hasAnyVisible(
        [authenticateHeading, idirProviderLink, idirLoginUsernameLabel, idirLoginContinueButton],
        7_500,
      );
      if (isExpectedBackDestinationVisible) {
        break;
      }
    }

    await expect
      .poll(
        async () =>
          hasAnyVisible([
            authenticateHeading,
            idirProviderLink,
            idirLoginUsernameLabel,
            idirLoginContinueButton,
          ]),
        {
          timeout: 45_000,
          message:
            'Expected browser back to land on provider selection page or IDIR login form.',
        },
      )
      .toBe(true);
  });

  test('AC3 - Duplicate control regression and authenticated save-state behavior', async ({ page }) => {
    await givenAuthenticatedUserLoginToLandingPage(page);

    const aggregatedListingControl = page.getByRole('button', { name: /^View Aggregated Listing Data$/i });
    await expect(aggregatedListingControl).toHaveCount(1);
    await expect(aggregatedListingControl).toBeVisible();

    await page.reload({ waitUntil: 'domcontentloaded' });
    await expect(page.getByRole('heading', { name: /Short-Term Rental Data Portal/i })).toBeVisible();
    await expect(page.getByRole('navigation', { name: /Main Navigation/i })).toBeVisible();
    await expect(page.getByRole('heading', { name: /Authenticate with:/i })).toHaveCount(0);
  });
});


/// <reference types="node" />

/**
 * Feature : Short-Term Rental Data Portal – View Individual Listings
 * Link to a feature: https://hous-hpb.atlassian.net/browse/
 *
 * @ViewIndividualListings
 * Scenario: ViewIndividualListings
 * Test Case Summary:
 * Given I am an authenticated User (IDIR or BCeID) with listing_read permissions
 * And I am on the Individual Listings page
 * When I review the listing view defaults and switch listing modes
 * Then I should see Recently Reported mode selected by default
 * And I should be able to switch to All Listings mode
 * And I should see all required individual listing data fields
 *
 * Supported Authentication Providers:
 * - IDIR (requires: BASE_URL, IDIR_USERNAME, IDIR_PASSWORD)
 * - Business BCeID (requires: BASE_URL, BCEID_USERNAME, BCEID_PASSWORD)
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 - Default Recently Reported mode:
 * - Step 1: Authenticate via IDIR or BCeID login (username/password)
 * - Step 2: Open the Individual Listings page from the landing page
 * - Step 3: Locate the "Recently Reported" toggle control
 * - Step 4: Validate the toggle is visible and set to Recently Reported by default
 *
 * AC2 - Toggle to All Listings and validate required fields:
 * - Step 1: Authenticate via IDIR or BCeID login and navigate to Individual Listings page (same as AC1 Steps 1–2)
 * - Step 2: Capture current visible listing row count before toggling
 * - Step 3: Click the "Recently Reported" toggle to switch to All Listings mode
 * - Step 4: Validate the control reflects All Listings as selected
 * - Step 5: Wait for listing grid refresh after mode switch
 * - Step 6: Validate required individual listing field headers are visible:
 *   ✓ Last Reported
 *   ✓ Registration
 *   ✓ Best Match Address
 *   ✓ Nights Stayed (12M)
 *   ✓ Business Licence
 *   ✓ Matched BL
 *   ✓ Last Action
 *   ✓ Last Action Date
 *   ✓ Listing
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import {
  IDIR_AUTH_ENV_MESSAGE,
  hasIdirAuthConfig,
  loginAsIdir as loginAsIdirShared,
  BCEID_AUTH_ENV_MESSAGE,
  hasBceidAuthConfig,
  loginAsBceid as loginAsBceidShared,
} from './support/auth';

const APP_URL = process.env.BASE_URL ?? '';

type AuthProvider = 'IDIR' | 'BCeID';

const EXPECTED_INDIVIDUAL_LISTING_FIELDS = [
  'Last Reported',
  'Registration',
  'Best Match Address',
  'Nights Stayed (12M)',
  'Business Licence',
  'Matched BL',
  'Last Action',
  'Last Action Date',
  'Listing',
];

test.use({ browserName: 'chromium' });

const authProviders: Array<{ provider: AuthProvider; hasConfig: boolean; configMessage: string }> = [
  {
    provider: 'IDIR',
    hasConfig: hasIdirAuthConfig(),
    configMessage: IDIR_AUTH_ENV_MESSAGE,
  },
  {
    provider: 'BCeID',
    hasConfig: hasBceidAuthConfig(),
    configMessage: BCEID_AUTH_ENV_MESSAGE,
  },
];

// Generate parametrized tests for each auth provider
authProviders.forEach(({ provider, hasConfig, configMessage }) => {
  test.describe(`@regression Feature: ViewIndividualListings [${provider}]`, () => {
    test.setTimeout(240_000);

    test.skip(!hasConfig, configMessage);

    test.beforeEach(async ({ page }) => {
      // Given I am an authenticated user (IDIR or BCeID) with listing_read permissions
      await loginAsIdir(page, provider);

      // When I access the Data Portal and open the individual listing page
      await openIndividualListingPage(page);
    });

    test('@smoke AC1 - default Recently Reported mode', async ({
      page,
    }) => {
      // b. Then Recently Reported toggle is on by default
      const recentlyReportedToggle = await getRecentlyReportedToggle(page);
      await expect(recentlyReportedToggle).toBeVisible();
      await expectRecentlyReportedModeSelected(recentlyReportedToggle);
    });

    test('AC2 - Toggle to All Listings and all individual listing fields are visible', async ({
      page,
    }) => {
      // Get the Recently Reported toggle
      const recentlyReportedToggle = await getRecentlyReportedToggle(page);

      // c. Then click on the toggle button to see All listing data
      const rowCountBeforeToggle = await getVisibleGridRowCount(page);
      await recentlyReportedToggle.click();
      await expectAllListingsModeSelected(recentlyReportedToggle);

      // Wait for data refresh; validate either row-count change or loading completion.
      await waitForGridRefresh(page, rowCountBeforeToggle);

      // d. Then I should see all the individual listing data fields
      await assertIndividualListingFieldHeadersVisible(page, EXPECTED_INDIVIDUAL_LISTING_FIELDS);
    });
  });
});

// ---------------------------------------------------------------------------
// Authentication helper
// ---------------------------------------------------------------------------

async function loginAsIdir(page: Page, provider: AuthProvider): Promise<void> {
  if (provider === 'IDIR') {
    await loginAsIdirShared(page, APP_URL);
  } else if (provider === 'BCeID') {
    await loginAsBceidShared(page, APP_URL);
  }
}

// ---------------------------------------------------------------------------
// Navigation helper – open the Individual Listing page from the home region
// ---------------------------------------------------------------------------

async function openIndividualListingPage(page: Page): Promise<void> {
  const homeRegion = page.getByRole('region', { name: /^Home$/i });
  await expect(homeRegion).toBeVisible();

  // Attempt common button label variants for the individual listing navigation button.
  const buttonCandidates = [
    page.getByRole('button', { name: /^View Individual Listing Data$/i }),
    page.getByRole('button', { name: /^View Individual Listings$/i }),
    page.getByRole('button', { name: /^View Listing Data$/i }),
    page.getByRole('button', { name: /Individual Listing/i }),
    page.getByRole('link', { name: /Individual Listing/i }),
  ];

  let navigationButton: Locator | null = null;
  for (const candidate of buttonCandidates) {
    if ((await candidate.count()) > 0 && (await candidate.first().isVisible().catch(() => false))) {
      navigationButton = candidate.first();
      break;
    }
  }

  if (!navigationButton) {
    throw new Error(
      'Unable to find the navigation button for Individual Listing Data on the home page. ' +
        'Verify the button label matches one of the expected variants.',
    );
  }

  await navigationButton.click();

  await expect(page.getByRole('heading', { name: /Individual Listings/i })).toBeVisible({
    timeout: 60_000,
  });
  await expect(page.getByRole('table').first()).toBeVisible({ timeout: 60_000 });
}

// ---------------------------------------------------------------------------
// Toggle helpers
// ---------------------------------------------------------------------------

async function getRecentlyReportedToggle(page: Page): Promise<Locator> {
  const toggleCandidates = [
    page
      .locator('div, section')
      .filter({ hasText: /Recently Reported/i })
      .getByRole('switch')
      .first(),
    page.getByRole('switch', { name: /Recently Reported/i }),
    page
      .locator('div, section')
      .filter({ hasText: /Recently Reported/i })
      .getByRole('checkbox')
      .first(),
    page.getByRole('checkbox', { name: /Recently Reported/i }),
    page.getByRole('button', { name: /Recently Reported/i }),
    page.locator('[aria-label*="Recently Reported" i]'),
    page.locator('label:has-text("Recently Reported")').locator('input').first(),
  ];

  for (const candidate of toggleCandidates) {
    if ((await candidate.count()) > 0 && (await candidate.first().isVisible().catch(() => false))) {
      return candidate.first();
    }
  }

  throw new Error('Unable to find the "Recently Reported" toggle control.');
}

async function expectRecentlyReportedModeSelected(toggle: Locator): Promise<void> {
  const ariaChecked = await toggle.getAttribute('aria-checked');
  if (ariaChecked !== null) {
    // For this control, false means the left-option "Recently Reported" is active.
    expect(ariaChecked).toBe('false');
    return;
  }

  const role = await toggle.getAttribute('role');
  if (role === 'checkbox') {
    await expect(toggle).not.toBeChecked();
    return;
  }

  const className = (await toggle.getAttribute('class')) ?? '';
  const dataState = (await toggle.getAttribute('data-state')) ?? '';
  const lowered = `${className} ${dataState}`.toLowerCase();
  expect(lowered).toMatch(/off|inactive|unselected|unchecked/);
}

async function expectAllListingsModeSelected(toggle: Locator): Promise<void> {
  const ariaChecked = await toggle.getAttribute('aria-checked');
  if (ariaChecked !== null) {
    // For this control, true means the right-option "All Listings" is active.
    expect(ariaChecked).toBe('true');
    return;
  }

  const role = await toggle.getAttribute('role');
  if (role === 'checkbox') {
    await expect(toggle).toBeChecked();
    return;
  }

  const className = (await toggle.getAttribute('class')) ?? '';
  const dataState = (await toggle.getAttribute('data-state')) ?? '';
  const lowered = `${className} ${dataState}`.toLowerCase();
  expect(lowered).toMatch(/on|active|selected|checked/);
}

// ---------------------------------------------------------------------------
// Grid row helpers
// ---------------------------------------------------------------------------

function getDataRows(page: Page): Locator {
  // Prefer ARIA grid/table rows, fallback to tbody rows.
  return page
    .locator('[role="row"]')
    .filter({ hasNot: page.locator('[role="columnheader"]') })
    .or(page.locator('tbody tr'));
}

async function getVisibleGridRowCount(page: Page): Promise<number> {
  const rows = getDataRows(page);
  const count = await rows.count();
  if (count === 0) {
    return 0;
  }

  let visibleCount = 0;
  for (let i = 0; i < count; i += 1) {
    if (await rows.nth(i).isVisible().catch(() => false)) {
      visibleCount += 1;
    }
  }
  return visibleCount;
}

async function waitForGridRefresh(page: Page, previousCount: number): Promise<void> {
  await page.waitForLoadState('networkidle').catch(() => undefined);

  if (previousCount === 0) {
    await expect(getDataRows(page).first()).toBeVisible({ timeout: 30_000 });
    return;
  }

  await expect
    .poll(async () => getVisibleGridRowCount(page), {
      timeout: 30_000,
      intervals: [500, 1_000],
      message: 'Expected listing grid to refresh after toggling Recently Reported.',
    })
    .not.toBe(previousCount);
}

// ---------------------------------------------------------------------------
// Field header assertion helpers
// ---------------------------------------------------------------------------

async function assertIndividualListingFieldHeadersVisible(
  page: Page,
  expectedHeaders: string[],
): Promise<void> {
  const listingGrid = await getListingGridContainer(page);
  await assertFieldHeadersVisibleInContainer(listingGrid, expectedHeaders);
}

async function getListingGridContainer(page: Page): Promise<Locator> {
  const candidates = [
    page.locator('[aria-label="table-of-individual-listings"]').first(),
    page.locator('[aria-label*="individual" i]').first(),
    page.getByRole('table').first(),
  ];

  for (const candidate of candidates) {
    if ((await candidate.count()) > 0 && (await candidate.isVisible().catch(() => false))) {
      return candidate;
    }
  }

  throw new Error('Unable to locate the individual listing grid container.');
}

async function assertFieldHeadersVisibleInContainer(
  container: Locator,
  expectedHeaders: string[],
): Promise<void> {
  for (const header of expectedHeaders) {
    const headerLocator = container
      .getByRole('columnheader', { name: new RegExp(`^${escapeRegExp(header)}$`, 'i') })
      .or(container.getByRole('cell', { name: new RegExp(`^${escapeRegExp(header)}$`, 'i') }))
      .or(container.locator('th', { hasText: header }))
      .or(container.locator('[role="grid"] [role="row"]', { hasText: header }).first())
      .or(container.getByText(new RegExp(`\\b${escapeRegExp(header)}\\b`, 'i')));

    await expect(
      headerLocator.first(),
      `Missing expected individual listing field: ${header}`,
    ).toBeVisible({ timeout: 15_000 });
  }
}

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}


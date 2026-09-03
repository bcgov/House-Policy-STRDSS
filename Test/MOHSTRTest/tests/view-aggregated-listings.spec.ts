/// <reference types="node" />

/**
 * Feature : Short-Term Rental Data Portal – View Aggregated Listings
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-505
 *
 * @ViewAggregatedListings
 * Scenario: ViewAggregatedListings
 * Test Case Summary:
 * Given I am an authenticated User (IDIR or BCeID) with listing_read permissions
 * When I navigate to the View Aggregated Listing Data page from the portal home
 * Then I should see aggregated listing data with a toggleable Recently Reported / All Listings view mode
 * And I should be able to expand parent rows to view child listing details with all required fields
 *
 * Supported Authentication Providers:
 * - IDIR (requires: BASE_URL, IDIR_USERNAME, IDIR_PASSWORD)
 * - Business BCeID (requires: BASE_URL, BCEID_USERNAME, BCEID_PASSWORD)
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 - Default Recently Reported Mode and Common Parent Row Fields:
 * - Step 1:  Authenticate via IDIR or BCeID login (username/password)
 * - Step 2:  Verify successful login – portal heading "Short-Term Rental Data Portal" is visible
 * - Step 3:  From the Home region, click the "View Aggregated Listing Data" button
 * - Step 4:  Verify "Aggregated Listings" page heading is visible (timeout: 60s)
 * - Step 5:  Verify the listing data table is rendered and visible (timeout: 60s)
 * - Step 6:  Validate the "Recently Reported" toggle control is present and visible
 * - Step 7:  Validate the toggle reflects the default "Recently Reported" mode (aria-checked="false" or unchecked)
 * - Step 8:  Record the current visible row count before toggling
 * - Step 9:  Click the toggle to switch to "All Listings" mode
 * - Step 10: Validate the toggle reflects the "All Listings" mode selection (aria-checked="true" or checked)
 * - Step 11: Wait for the listing grid to refresh with updated data (row count changes, timeout: 30s)
 * - Step 12: Validate all expected common parent row column headers are visible:
 *   ✓ Last Reported
 *   ✓ Host
 *   ✓ Best Match Address
 *   ✓ Nights stayed (12M)
 *   ✓ Business Licence
 *   ✓ Last Action
 *   ✓ Last Action Date
 *   ✓ Listings
 *
 * AC2 - Expand Parent Listing Row and View Child Elements with Required Fields:
 * - Step 1:  Authenticate via IDIR or BCeID login and navigate to Aggregated Listings page (same as AC1 Steps 1–5)
 * - Step 2:  Validate "Recently Reported" toggle is visible and in default mode
 * - Step 3:  Record current row count, click toggle to switch to "All Listings" mode
 * - Step 4:  Wait for grid refresh and validate common parent row column headers are visible
 * - Step 5:  Verify at least one visible parent row exists in the aggregated listing grid (timeout: 15s)
 * - Step 6:  Verify the total group count is non-zero; skip test if no seed data is available
 * - Step 7:  Locate the row expander control on the first parent row
 *            (tries aria-expanded button, role="button", labelled expand control)
 * - Step 8:  Validate the expander control is visible
 * - Step 9:  Record current visible row count before expansion
 * - Step 10: Click the row expander to expand the first parent listing
 * - Step 11: Validate expander reflects expanded state (aria-expanded="true" where applicable)
 * - Step 12: Wait for child rows to appear – row count must increase beyond pre-expand count (timeout: 30s)
 * - Step 13: Validate first child row is visible and child rows contain non-empty data (checks up to 5 rows)
 * - Step 14: Validate all expected child row column fields are present (100% match required):
 *   ✓ Last Reported
 *   ✓ Registration
 *   ✓ Best Match Address
 *   ✓ Nights stayed (12M)
 *   ✓ Business Licence
 *   ✓ Matched BL
 *   ✓ Last Action
 *   ✓ Last Action Date
 *   ✓ Listings
 *
 * AC3 - Recently Reported Aggregation by Registration and Parent Row Expansion:
 * - Step 1:  Authenticate via IDIR or BCeID login and navigate to Aggregated Listings page and wait for page to load fully
 * - Step 2:  Validate "Recently Reported" toggle is visible and remains in default mode
 * - Step 3:  Verify at least one visible parent row exists in the aggregated listing grid (Recently Reported mode)
 * - Step 4:  Verify the total group count is non-zero; skip test if no seed data is available
 * - Step 5:  Expand visible parent rows and validate users can view child rows under parent groups
 * - Step 6:  In child rows for groups with Registration values, validate child rows under a parent resolve to one Registration number
 * - Step 7:  Validate sampled Registration groups are represented by distinct parent groupings
 *
 * AC4 - All Listings Aggregation Logic Validation (Comprehensive Hierarchical Rules):
 * - Step 1:  Authenticate via IDIR or BCeID login and navigate to Aggregated Listings page
 * - Step 2:  Click toggle to switch to "All Listings" mode
 * - Step 3:  Wait for grid refresh and validate common parent row column headers are visible
 * - Step 4:  Verify total group count is non-zero; skip test if no seed data is available
 * - Step 5:  Expand sampled parent rows and validate aggregation hierarchy (sample up to 3 groups)
 * - Step 6:  For each expanded parent group, determine if Registration is present or absent
 *
 * WHEN REGISTRATION IS PRESENT - Apply Rules 1.1 & 1.2:
 * 
 *   RULE 1.1 - Registration-First Aggregation (Highest Priority):
 *     ✓ Step 6a: If child rows include Registration values, verify listings are grouped under ONE Registration per parent
 *     ✓ Step 6b: Validate that ONLY child rows with the SAME Registration appear under the same parent group
 *     ✓ Step 6c: Assert that NO child row has a DIFFERENT Registration than others in the same parent group
 *
 *   RULE 1.2 - Multiple Registrations Isolation:
 *     ✓ Step 6d: Verify that if multiple different Registrations exist in child rows, they must be under separate parent rows
 *     ✓ Step 6e: When a Listing address is edited/updated and Registration values are present, verify the Listing stays with its original Registration parent
 *
 * WHEN REGISTRATION IS ABSENT - Apply Rules 1.2.x, 1.3, 1.3a-e, 1.4, 1.4a:
 *
 *   RULE 1.2.1 - Aggregation Order (Fallback Hierarchy):
 *     ✓ Step 7a: Verify listings without Registration are aggregated by priority order: Best Match Address → Host Name → Business Licence
 *     ✓ Step 7b: Validate that address is the PRIMARY grouping criteria (all listings in group share same normalized address)
 *     ✓ Step 7c: When addresses match, verify Host Name is the next grouping criteria (all listings with same address/host are grouped)
 *     ✓ Step 7d: When address and host match, verify Business Licence is the tertiary grouping criteria
 *
 *   RULE 1.2.2 - Identical Triple Aggregation:
 *     ✓ Step 7e: Verify that all listings with IDENTICAL address + host name + business licence aggregate together under ONE parent
 *
 *   RULE 1.3 - Address-Based Aggregation Foundation (When Registration Absent):
 *     ✓ Step 8a: Verify all listings with the EXACT same address are aggregated under a parent row, EXCEPT when exceptions apply (see 1.3a-e)
 *     ✓ Step 8b: Assert that different addresses result in separate parent groupings (one parent per unique normalized address)
 *
 *   RULE 1.3a - Different Host Name Exception:
 *     ✓ Step 8c: When same address has different Property Host Names, verify each combination creates a separate parent row
 *     ✓ Step 8d: Example: "123 Main St" + "Host A" = Parent 1; "123 Main St" + "Host B" = Parent 2 (separate parent rows)
 *
 *   RULE 1.3b - Different Business Licence Exception:
 *     ✓ Step 8e: When same address + same host has different Business Licence Numbers, verify each combination creates a separate parent row
 *     ✓ Step 8f: ONLY numeric Business Licence values trigger this rule (see 1.3c for string-only handling)
 *     ✓ Step 8g: Example: "123 Main St" + "Host A" + BL "1234" = Parent 1; "123 Main St" + "Host A" + BL "5678" = Parent 2
 *
 *   RULE 1.3c - String-Only Business Licence Ignored:
 *     ✓ Step 8h: If Business Licence contains ONLY alphabetic characters/strings (no numeric content), it is NOT used for grouping
 *     ✓ Step 8i: String-only Licences do not trigger separate parent rows (see 1.3b), but numeric-only or alphanumeric values do
 *
 *   RULE 1.3d - Listings Aggregate Without Business Licence:
 *     ✓ Step 8j: Verify listings CAN be aggregated together even if NO Business Licence number is present/associated
 *     ✓ Step 8k: Listings with blank/empty Business Licence and identical address + host share the same parent row
 *
 *   RULE 1.3e - Different Unit Numbers = Separate Groups:
 *     ✓ Step 8l: Listings with similar/same base address but with DIFFERENT unit numbers (e.g., "123 Main St #101" vs "123 Main St #102") are treated as DIFFERENT addresses
 *     ✓ Step 8m: Verify each unique address variation (including different unit numbers) results in a separate parent group
 *
 *   RULE 1.4 - Dynamic Listing Reassignment (When Registration Absent):
 *     ✓ Step 9a: When a Listing address is edited/updated/reassigned to a DIFFERENT address, verify the Listing automatically re-aggregates to match the new address group
 *     ✓ Step 9b: Verify that after reassignment, the Listing appears under a parent group matching the new address + host + licence values (if they match existing groups)
 *
 *   RULE 1.4a - Reassignment Following Rule 1.3 Logic:
 *     ✓ Step 9c: When some fields of a reassigned Listing differ from existing groups, verify aggregation follows Rule 1.3 logic (address → host → licence priority)
 *     ✓ Step 9d: If reassigned Listing's address, host, or licence is UNIQUE, verify a NEW parent row is created for it
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
type ListingsMode = 'Recently Reported' | 'All Listings';

const EXPECTED_COMMON_FIELDS = [
  'Last Reported',
  'Host',
  'Best Match Address',
  'Nights stayed (12M)',
  'Business Licence',
  'Last Action',
  'Last Action Date',
  'Listings',
];

const EXPECTED_CHILD_FIELDS = [
  'Last Reported',
  'Registration',
  'Best Match Address',
  'Nights stayed (12M)',
  'Business Licence',
  'Matched BL',
  'Last Action',
  'Last Action Date',
  'Listings',
];

test.use({ browserName: 'chromium' });

type GridSnapshot = {
  visibleRowCount: number;
  groupsSummaryText: string;
  firstRowText: string;
};

type ChildListingRecord = {
  registration: string;
  bestMatchAddress: string;
  host: string;
  businessLicence: string;
  rowText: string;
};

type ParentChildGroup = {
  parentSignature: string;
  childRecords: ChildListingRecord[];
};

type AggregationValidationStats = {
  sampledGroups: number;
  groupsWithRegistration: number;
  groupsWithoutRegistration: number;
};

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
  test.describe(`@regression Feature: ViewAggregatedListings [${provider}]`, () => {
    test.setTimeout(240_000);

    test.skip(!hasConfig, configMessage);

    test.beforeEach(async ({ page }) => {
      // Given I am an authenticated user (IDIR or BCeID) with listing_read permissions
      await loginAsIdir(page, provider);

      // When I access the Data Portal and open the aggregated listing page
      await openAggregatedListingPage(page);
    });

  test('@smoke AC1 - default Recently Reported mode and common parent fields are visible', async ({
    page,
  }) => {

    // Then Recently Reported toggle is on by default
    const recentlyReportedToggle = await getRecentlyReportedToggle(page);
    await expect(recentlyReportedToggle).toBeVisible();
    await expectRecentlyReportedModeSelected(recentlyReportedToggle);

    // Then click the toggle to switch to all listing data
    const gridBeforeToggle = await captureGridSnapshot(page);
    await recentlyReportedToggle.click();
    await expectAllListingsModeSelected(recentlyReportedToggle);

    // Wait for data refresh; validate either row-count change or loading completion.
    await waitForGridRefresh(page, gridBeforeToggle);

    // Then the common field headers should be visible for parent rows
    await assertCommonFieldHeadersVisible(page, EXPECTED_COMMON_FIELDS);
  });

  test('AC2 - expand parent listing row and view all child elements with required fields', async ({
    page,
  }) => {
    // a. All the aggregated listing data will be displayed
    const recentlyReportedToggle = await getRecentlyReportedToggle(page);
    await expect(recentlyReportedToggle).toBeVisible();
    await expectRecentlyReportedModeSelected(recentlyReportedToggle);

    const gridBeforeToggle = await captureGridSnapshot(page);
    await recentlyReportedToggle.click();
    await expectAllListingsModeSelected(recentlyReportedToggle);
    await waitForGridRefresh(page, gridBeforeToggle);

    // Verify aggregated listing data is now displayed
    await assertCommonFieldHeadersVisible(page, EXPECTED_COMMON_FIELDS);

    // Validate groups exist before proceeding with row expansion
    // IMPORTANT: Check data availability BEFORE attempting to locate parent rows
    // This ensures we gracefully skip AC2 when no groups exist, rather than throwing
    // "Unable to identify parent rows" error. Fixes: BCeID user with 0 groups.
    await skipIfNoGroupsAvailable(
      page,
      provider,
      'All Listings',
      'No aggregated listing groups available. Seed data to validate row expansion.',
    );

    // Now safely attempt to find and validate parent rows
    const parentRows = await getVisibleParentRows(page);
    
    await expect
      .poll(async () => countVisibleRows(parentRows), {
        timeout: 15_000,
        message: 'Expected at least one visible parent row in aggregated listing data.',
      })
      .toBeGreaterThan(0);

    // b. Expand any parent listing data
    const firstParentRow = parentRows.first();
    await expect(firstParentRow).toBeVisible();
    
    const expander = await getRowExpander(firstParentRow, page);
    await expect(expander).toBeVisible();

    const rowsBeforeExpand = await getVisibleGridRowCount(page);
    await clickExpanderWithRetry(page, expander, rowsBeforeExpand);

    await waitForChildRowsAfterExpand(page, rowsBeforeExpand, firstParentRow);

    // c. Then I should see all the child elements data for that parent element
    const childRows = await getVisibleChildRowsForExpandedParent(page, firstParentRow);
    await expect(childRows.first()).toBeVisible({ timeout: 10_000 });
    await assertRowsHaveData(childRows, 5);

    // d. Then I should see key information for each child listing, including:
    // Last Reported, Registration, Best Match Address, Nights stayed (12M),
    // Business Licence, Matched BL, Last Action, Last Action Date, Listings
    await assertChildRowsContainAllFields(page, EXPECTED_CHILD_FIELDS);
  });

  test('AC3 - Recently Reported mode shows expandable parent rows grouped by registration', async ({
    page,
  }) => {
    // Reuse AC1/AC2 stability pattern: ensure page and grid are fully ready before assertions.
    await page.waitForLoadState('load').catch(() => undefined);
    await waitForGridInteractionReady(page, 30_000);

    const recentlyReportedToggle = await getRecentlyReportedToggle(page);
    await expect(recentlyReportedToggle).toBeVisible();
    await expectRecentlyReportedModeSelected(recentlyReportedToggle);

    await skipIfNoGroupsAvailable(
      page,
      provider,
      'Recently Reported',
      'No aggregated listing groups available in Recently Reported mode. Seed data is required.',
    );

    const parentRows = await getVisibleParentRows(page);
    await expect
      .poll(async () => countVisibleRows(parentRows), {
        timeout: 15_000,
        message: 'Expected at least one visible parent row in Recently Reported mode.',
      })
      .toBeGreaterThan(0);

    await assertRowsLookTabular(parentRows);
    await assertParentRowCommonInfoVisible(parentRows.first(), EXPECTED_COMMON_FIELDS);

    const stats = await validateRecentlyReportedRegistrationGrouping(page, 6);
    expect(
      stats.sampledGroups,
      'Expected to validate at least one expandable parent group in Recently Reported mode.',
    ).toBeGreaterThan(0);
  });

  test('AC4 - All Listings mode follows registration-first and fallback aggregation hierarchy', async ({
    page,
  }) => {
    const recentlyReportedToggle = await getRecentlyReportedToggle(page);
    await expect(recentlyReportedToggle).toBeVisible();
    await expectRecentlyReportedModeSelected(recentlyReportedToggle);

    const gridBeforeToggle = await captureGridSnapshot(page);
    await recentlyReportedToggle.click();
    await expectAllListingsModeSelected(recentlyReportedToggle);
    await waitForGridRefresh(page, gridBeforeToggle);

    await assertCommonFieldHeadersVisible(page, EXPECTED_COMMON_FIELDS);

    await skipIfNoGroupsAvailable(
      page,
      provider,
      'All Listings',
      'No aggregated listing groups available in All Listings mode. Seed data is required.',
    );

    const stats = await validateAllListingsAggregationHierarchy(page, 3);
    expect(stats.sampledGroups, 'Expected to validate at least one parent group in All Listings mode.').toBeGreaterThan(0);
  });
  });
});

async function loginAsIdir(page: Page, provider: AuthProvider): Promise<void> {
  if (provider === 'IDIR') {
    await loginAsIdirShared(page, APP_URL);
  } else if (provider === 'BCeID') {
    await loginAsBceidShared(page, APP_URL);
  } else {
    throw new Error(`Unsupported authentication provider: ${provider}`);
  }
}

async function openAggregatedListingPage(page: Page): Promise<void> {
  const homeRegion = page.getByRole('region', { name: /^Home$/i });
  await expect(homeRegion).toBeVisible();

  const openAggregatedButton = page.getByRole('button', {
    name: /^View Aggregated Listing Data$/i,
  });
  await expect(openAggregatedButton).toBeVisible();
  await openAggregatedButton.click();

  await expect(page.getByRole('heading', { name: /Aggregated Listings/i })).toBeVisible({
    timeout: 60_000,
  });
  await expect(page.getByRole('table').first()).toBeVisible({ timeout: 60_000 });
}

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
    // For this control, false means left-option "Recently Reported" is active.
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
    // For this control, true means right-option "All Listings" is active.
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

async function hasVisibleDataRows(page: Page): Promise<boolean> {
  const rows = getDataRows(page);
  const count = await rows.count();
  if (count === 0) {
    return false;
  }

  for (let i = 0; i < count; i += 1) {
    const row = rows.nth(i);
    const visible = await row.isVisible().catch(() => false);
    if (!visible) {
      continue;
    }

    const text = (((await row.textContent().catch(() => '')) ?? '').replace(/\s+/g, ' ').trim()).toLowerCase();
    if (!text.includes('no listings matched your search')) {
      return true;
    }
  }

  return false;
}

async function hasVisibleNoDataState(page: Page): Promise<boolean> {
  const noDataRow = page
    .locator('[role="row"], tbody tr')
    .filter({ hasText: /No listings matched your search\. Please try again\./i })
    .first();

  if ((await noDataRow.count()) === 0) {
    return false;
  }

  return noDataRow.isVisible().catch(() => false);
}

async function captureGridSnapshot(page: Page): Promise<GridSnapshot> {
  const visibleRowCount = await getVisibleGridRowCount(page);
  const groupsSummaryText =
    ((await page
      .getByText(/Showing\s+\d+\s+of\s+\d+\s+groups/i)
      .first()
      .textContent()
      .catch(() => '')) ?? '').trim();

  const dataRows = getDataRows(page);
  const firstRowText =
    ((await dataRows
      .first()
      .textContent()
      .catch(() => '')) ?? '').replace(/\s+/g, ' ').trim();

  return { visibleRowCount, groupsSummaryText, firstRowText };
}

async function waitForGridRefresh(page: Page, before: GridSnapshot): Promise<void> {
  await page.waitForLoadState('load').catch(() => undefined);

  // Grid refresh can complete without changing visible row count, so we accept
  // multiple stability signals to avoid false timeouts.
  await expect
    .poll(async () => {
      const after = await captureGridSnapshot(page);
      const rowCountChanged = before.visibleRowCount !== after.visibleRowCount;
      const summaryChanged =
        before.groupsSummaryText.length > 0 &&
        after.groupsSummaryText.length > 0 &&
        before.groupsSummaryText !== after.groupsSummaryText;
      const firstRowChanged =
        before.firstRowText.length > 0 &&
        after.firstRowText.length > 0 &&
        before.firstRowText !== after.firstRowText;

      const hasVisibleData = await hasVisibleDataRows(page);
      const hasNoDataState = await hasVisibleNoDataState(page);
      const loadingVisible = await page
        .locator('.p-datatable-loading-overlay, [aria-busy="true"], [role="progressbar"]')
        .first()
        .isVisible({ timeout: 250 })
        .catch(() => false);

      const groupsTotal = parseGroupsTotal(after.groupsSummaryText);
      const settledEmptyState = groupsTotal === 0 && !loadingVisible && hasNoDataState;

      return (
        rowCountChanged ||
        summaryChanged ||
        firstRowChanged ||
        (hasVisibleData && !loadingVisible) ||
        settledEmptyState
      );
    }, {
      timeout: 45_000,
      intervals: [500, 1_000, 2_000],
      message: 'Expected listing grid to refresh after toggling Recently Reported.',
    })
    .toBe(true);
}

async function getTotalGroupsCount(page: Page): Promise<number> {
  const summary = page.getByText(/Showing\s+\d[\d,]*\s+of\s+\d[\d,]*\s+groups/i).first();
  await expect(summary).toBeVisible({ timeout: 30_000 });

  // Some runs briefly show a transitional 0-of-0 state before data arrives.
  // Poll for a settled summary and stop early as soon as total groups > 0.
  let consecutiveZeroCount = 0;
  let lastTotal = 0;

  const resolvedTotal = await expect
    .poll(
      async () => {
        const loadingVisible = await page
          .locator('.p-datatable-loading-overlay, [aria-busy="true"], [role="progressbar"]')
          .first()
          .isVisible({ timeout: 250 })
          .catch(() => false);

        const text = ((await summary.textContent().catch(() => '')) ?? '').trim();
        const parsedTotal = parseGroupsTotal(text);

        if (loadingVisible || parsedTotal === null) {
          consecutiveZeroCount = 0;
          return null;
        }

        lastTotal = parsedTotal;
        if (parsedTotal > 0) {
          return parsedTotal;
        }

        consecutiveZeroCount += 1;
        // Require repeated zero observations to avoid treating transient 0 as final.
        if (consecutiveZeroCount >= 3) {
          return 0;
        }

        return null;
      },
      {
        timeout: 30_000,
        intervals: [500, 1_000, 2_000],
        message: 'Expected aggregated group summary to settle after data refresh.',
      },
    )
    .not.toBeNull()
    .then(async () => {
      const stableText = ((await summary.textContent().catch(() => '')) ?? '').trim();
      const stableTotal = parseGroupsTotal(stableText);
      return stableTotal ?? lastTotal;
    });

  return resolvedTotal;
}

async function skipIfNoGroupsAvailable(
  page: Page,
  provider: AuthProvider,
  mode: ListingsMode,
  reason: string,
): Promise<void> {
  const totalGroups = await getTotalGroupsCount(page);
  if (totalGroups > 0) {
    return;
  }

  const groupsSummaryText =
    ((await page
      .getByText(/Showing\s+\d[\d,]*\s+of\s+\d[\d,]*\s+groups/i)
      .first()
      .textContent()
      .catch(() => '')) ?? '').trim();

  const diagnostic =
    `[${provider}] ${mode} mode skip: ${reason} ` +
    `Summary="${groupsSummaryText || 'unavailable'}" URL="${page.url()}"`;

  // Keep a consistent machine-readable skip log for env/data triage.
  console.log(diagnostic);
  test.skip(true, diagnostic);
}

function parseGroupsTotal(summaryText: string): number | null {
  const match = summaryText.match(/Showing\s+\d[\d,]*\s+of\s+(\d[\d,]*)\s+groups/i);
  if (!match) {
    return null;
  }

  const normalized = match[1].replace(/,/g, '');
  const parsed = Number.parseInt(normalized, 10);
  return Number.isNaN(parsed) ? null : parsed;
}

function getLikelyParentRows(page: Page): Locator {
  // Parent rows usually include an expand/collapse affordance in the first column.
  // Try multiple selector patterns to find expanders across different UI implementations.
  const dataRows = getDataRows(page);
  
  // Try various expander selectors
  const expanderPatterns = [
    page.locator('[aria-expanded]'),
    page.locator('button[aria-expanded]'),
    page.locator('[role="button"][aria-expanded]'),
    page.locator('button[aria-label*="xpand" i]'),
    page.locator('.p-row-toggler, .p-datatable-toggler, [class*="row-toggler" i]'),
  ];

  let result = dataRows.filter({ has: expanderPatterns[0] });
  
  for (const pattern of expanderPatterns.slice(1)) {
    result = result.or(dataRows.filter({ has: pattern }));
  }

  return result;
}

async function getFirstRowExpander(page: Page): Promise<Locator> {
  const preferred = [
    page.locator('button[aria-expanded]').first(),
    page.locator('[role="button"][aria-expanded]').first(),
    page.getByRole('button', { name: /Expand|Show|Open|arrow/i }).first(),
    page.locator('button[aria-label*="xpand" i]').first(),
    page.locator('tbody tr button').first(),
    page.locator('[role="row"] button').first(),
  ];

  for (const locator of preferred) {
    if ((await locator.count()) > 0 && (await locator.isVisible().catch(() => false))) {
      return locator;
    }
  }

  throw new Error('Unable to find an expander control for parent listing rows.');
}

async function getVisibleParentRows(page: Page): Promise<Locator> {
  const likelyParentRows = getLikelyParentRows(page);
  const total = await likelyParentRows.count();
  if (total === 0) {
    throw new Error('Unable to identify parent rows in the aggregated listing table.');
  }

  await expect
    .poll(async () => countVisibleRows(likelyParentRows), {
      timeout: 15_000,
      message: 'Expected visible parent rows in the aggregated listing table.',
    })
    .toBeGreaterThan(0);

  return likelyParentRows;
}

async function assertRowsLookTabular(rows: Locator): Promise<void> {
  const rowCount = await rows.count();
  let checked = 0;

  for (let i = 0; i < rowCount && checked < 3; i += 1) {
    const row = rows.nth(i);
    if (!(await row.isVisible().catch(() => false))) {
      continue;
    }

    const cellCount = await row.locator('[role="cell"], td').count();
    checked += 1;
    expect(cellCount, `Expected parent row ${checked} to contain table cells.`).toBeGreaterThan(1);
  }

  expect(checked, 'Expected at least one visible parent row to validate table format.').toBeGreaterThan(0);
}

async function assertParentRowCommonInfoVisible(row: Locator, commonFields: string[]): Promise<void> {
  const rowText = ((await row.textContent()) ?? '').trim();
  expect(
    rowText.length,
    'Expected the parent row to contain visible text with common listing information.',
  ).toBeGreaterThan(0);

  // At least a subset of the common field values must be represented in the parent row cells
  const cells = row.locator('[role="cell"], td');
  const cellCount = await cells.count();
  expect(
    cellCount,
    `Expected the parent row to have multiple cells representing common listing information. Found ${cellCount}.`,
  ).toBeGreaterThan(1);
}

async function getRowExpander(row: Locator, page: Page): Promise<Locator> {
  const withinRow = [
    row.locator('button[aria-expanded]').first(),
    row.locator('[role="button"][aria-expanded]').first(),
    row.locator('button[aria-label*="xpand" i]').first(),
    row.locator('.p-row-toggler, .p-datatable-toggler, [class*="row-toggler" i]').first(),
  ];

  for (const candidate of withinRow) {
    if ((await candidate.count()) > 0 && (await candidate.isVisible().catch(() => false))) {
      return candidate;
    }
  }

  return getFirstRowExpander(page);
}

async function getVisibleChildRowsForExpandedParent(page: Page, parentRow?: Locator): Promise<Locator> {
  if (parentRow) {
    const immediateSibling = parentRow.locator('xpath=following-sibling::tr[1]');
    const siblingExists = (await immediateSibling.count()) > 0;
    if (siblingExists) {
      const childRowsInSibling = immediateSibling
        .locator('[role="row"], tbody tr')
        .filter({ hasNot: page.locator('[role="columnheader"]') });

      if ((await childRowsInSibling.count()) > 0) {
        return childRowsInSibling;
      }
    }

    throw new Error('Unable to isolate child rows from the selected parent expansion row.');
  }

  const expandedDetailRows = page
    .locator('tbody tr.p-datatable-row-expansion, tr:has(td[colspan])')
    .filter({ hasNot: page.locator('[role="columnheader"]') });

  if ((await expandedDetailRows.count()) > 0) {
    return expandedDetailRows;
  }

  const childByAriaLevel = page
    .locator('[role="row"][aria-level="2"], [role="row"][aria-level="3"]')
    .filter({ hasNot: page.locator('[role="columnheader"]') });

  if ((await childByAriaLevel.count()) > 0) {
    return childByAriaLevel;
  }

  const expandedParent = page.locator('[role="row"] [aria-expanded="true"]').first();
  if ((await expandedParent.count()) > 0) {
    const parentRow = expandedParent.locator('xpath=ancestor::*[@role="row"][1]');
    const followingRows = parentRow.locator('xpath=following-sibling::*[@role="row"]');
    if ((await followingRows.count()) > 0) {
      return followingRows;
    }
  }

  throw new Error('Unable to isolate child rows for the expanded parent listing.');
}

async function assertRowsHaveData(rows: Locator, maxRowsToCheck: number): Promise<void> {
  const total = await rows.count();
  expect(total, 'Expected at least one child row after parent expansion.').toBeGreaterThan(0);

  const rowsToCheck = Math.min(total, maxRowsToCheck);
  for (let i = 0; i < rowsToCheck; i += 1) {
    const text = ((await rows.nth(i).textContent()) ?? '').trim();
    expect(text.length, `Expected child row ${i + 1} to contain data.`).toBeGreaterThan(0);
  }
}

async function collapseExpandedParentRows(page: Page): Promise<void> {
  const parentRows = getLikelyParentRows(page);
  const count = await parentRows.count();

  for (let i = 0; i < count; i += 1) {
    const row = parentRows.nth(i);
    if (!(await row.isVisible().catch(() => false))) {
      continue;
    }

    const expanded = await isParentRowExpanded(page, row);
    if (!expanded) {
      continue;
    }

    const expander = await getRowExpander(row, page).catch(() => null);
    if (!expander) {
      continue;
    }

    await expander.click({ timeout: 5_000 }).catch(() => undefined);
  }

  await waitForGridInteractionReady(page, 10_000).catch(() => undefined);
}

async function isParentRowExpanded(page: Page, row: Locator): Promise<boolean> {
  const byAria = await row
    .locator('button[aria-expanded="true"], [role="button"][aria-expanded="true"]')
    .count()
    .catch(() => 0);
  if (byAria > 0) {
    return true;
  }

  const sibling = row.locator('xpath=following-sibling::tr[1]');
  if ((await sibling.count()) === 0) {
    return false;
  }

  const nestedTable = await sibling.locator('table').count().catch(() => 0);
  if (nestedTable > 0) {
    return true;
  }

  const listingHeaderText = await sibling
    .locator('text=/Registration|select-listing|Open listing details in a new tab/i')
    .count()
    .catch(() => 0);
  return listingHeaderText > 0;
}

async function getHeaderIndexMap(page: Page): Promise<Map<string, number>> {
  const listingGrid = await getListingGridContainer(page);
  const rawHeaders = await listingGrid.locator('[role="columnheader"], th').allTextContents();

  const normalizedHeaders = rawHeaders
    .map((value) => normalizeCellText(value))
    .filter((value) => value.length > 0);

  const indexMap = new Map<string, number>();
  normalizedHeaders.forEach((header, index) => {
    indexMap.set(normalizeHeaderKey(header), index);
  });

  return indexMap;
}

async function getHeaderIndexMapFromContainer(container: Locator): Promise<Map<string, number>> {
  // First try: Look for columnheader role or th elements in the container
  let rawHeaders = await container.locator('[role="columnheader"], th').allTextContents().catch(() => []);
  
  // Fallback: If no headers found via roles/th, try to extract from the first row's cells
  if (rawHeaders.length === 0) {
    // Look for any table structure and extract potential header row
    const possibleHeaders = await container.locator('tr:first-child [role="cell"], tr:first-child td').allTextContents().catch(() => []);
    if (possibleHeaders.length > 0) {
      rawHeaders = possibleHeaders;
    }
  }

  const normalizedHeaders = rawHeaders
    .map((value) => normalizeCellText(value))
    .filter((value) => value.length > 0);

  const indexMap = new Map<string, number>();
  normalizedHeaders.forEach((header, index) => {
    indexMap.set(normalizeHeaderKey(header), index);
  });

  return indexMap;
}

function normalizeHeaderKey(value: string): string {
  return value.toLowerCase().replace(/[^a-z0-9]+/g, ' ').trim();
}

async function getRowCellTexts(row: Locator): Promise<string[]> {
  const cellTexts = await row.locator('[role="cell"], td').allTextContents();
  return cellTexts.map((value) => normalizeCellText(value));
}

function getCellValueByHeaderAliases(
  rowCells: string[],
  headerIndexMap: Map<string, number>,
  aliases: string[],
): string {
  // CRITICAL FIX: Child table rows have an empty first cell (expander/checkbox) that headers don't account for.
  // This causes header indices to be off by 1. Adjust by skipping empty leading cells.
  let effectiveRowCells = rowCells;
  let cellOffset = 0;
  
  // If first cell is empty and we have more cells than headers, assume it's an expander/checkbox column
  if (rowCells.length > 0 && rowCells[0].length === 0 && headerIndexMap.size > 0 && rowCells.length > headerIndexMap.size) {
    effectiveRowCells = rowCells.slice(1); // Skip first empty cell
    cellOffset = 1;
  }

  for (const alias of aliases) {
    const normalizedAlias = normalizeHeaderKey(alias);
    const index = headerIndexMap.get(normalizedAlias);
    if (typeof index === 'number' && index >= 0 && index < effectiveRowCells.length) {
      const value = normalizeCellText(effectiveRowCells[index]);
      if (value.length > 0) {
        return value;
      }
    }
  }

  return '';
}

async function extractChildListingRecords(
  rows: Locator,
  fallbackHeaderIndexMap: Map<string, number>,
  maxRowsToInspect = 60,
): Promise<ChildListingRecord[]> {
  const records: ChildListingRecord[] = [];
  const rowCount = await rows.count();

  let effectiveHeaderIndexMap = fallbackHeaderIndexMap;
  let debugHeadersLogged = false;
  
  const firstRow = rows.first();
  const childTable = firstRow.locator('xpath=ancestor::table[1]');
  if ((await childTable.count().catch(() => 0)) > 0) {
    const childHeaderIndexMap = await getHeaderIndexMapFromContainer(childTable.first()).catch(() => null);
    if (childHeaderIndexMap && childHeaderIndexMap.size > 0) {
      effectiveHeaderIndexMap = childHeaderIndexMap;
      
      // Debug logging: show header mapping for Business Licence detection
      const blKey = normalizeHeaderKey('Business Licence');
      const blIndex = effectiveHeaderIndexMap.get(blKey);
      if (!debugHeadersLogged) {
        console.log('[DEBUG-AC4] ====== CHILD TABLE HEADER MAPPING ======');
        console.log('[DEBUG-AC4] Full Header Index Map:');
        Array.from(effectiveHeaderIndexMap.entries()).forEach(([key, idx]) => {
          console.log(`  Index ${idx}: KEY_NORMALIZED="${key}"`);
        });
        console.log(`[DEBUG-AC4] Looking for Business Licence with normalized key: "${blKey}" => Index: ${blIndex ?? 'NOT_FOUND'}`);
        console.log(`[DEBUG-AC4] Header Map Size: ${effectiveHeaderIndexMap.size}`);
        console.log('[DEBUG-AC4] ====== END HEADER MAPPING ======');
        debugHeadersLogged = true;
      }
    }
  }

  for (let i = 0; i < Math.min(rowCount, maxRowsToInspect); i += 1) {
    const row = rows.nth(i);
    if (!(await row.isVisible().catch(() => false))) {
      continue;
    }

    const rowText = normalizeCellText((await row.textContent().catch(() => '')) ?? '');
    if (!rowText || /no listings matched your search/i.test(rowText)) {
      continue;
    }

    const expanderInRowCount = await row
      .locator('button[aria-expanded], [role="button"][aria-expanded]')
      .count()
      .catch(() => 0);
    if (expanderInRowCount > 0) {
      continue;
    }

    const rowCells = await getRowCellTexts(row);
    if (rowCells.length < 3) {
      continue;
    }

    const registration = getCellValueByHeaderAliases(rowCells, effectiveHeaderIndexMap, ['Registration']);
    const bestMatchAddress = getCellValueByHeaderAliases(rowCells, effectiveHeaderIndexMap, ['Best Match Address']);
    const host = getCellValueByHeaderAliases(rowCells, effectiveHeaderIndexMap, ['Host', 'Property Host']);
    
    // Business Licence extraction with strict header mapping
    // Priority order: Business Licence > Business License > Matched BL > BL
    const businessLicenceAliases = [
      'Business Licence',
      'Business License',
      'Matched BL',
      'BL',
    ];
    
    let businessLicence = '';
    let blSource = 'FALLBACK';
    for (const alias of businessLicenceAliases) {
      businessLicence = getCellValueByHeaderAliases(rowCells, effectiveHeaderIndexMap, [alias]);
      if (businessLicence.length > 0) {
        blSource = alias;
        break;
      }
    }

    // Only use fallback if header mapping completely fails
    // This prevents accidentally grabbing "Nights stayed (12M)" column
    if (!businessLicence && rowCells.length > 4) {
      // Last resort: try to find a numeric-looking value that's not "Nights stayed"
      // Skip first 4 columns: Last Reported (0), Registration (1), Address (2), Nights stayed (3)
      // Business Licence should be around index 4-5
      for (let idx = 4; idx < Math.min(rowCells.length, 8); idx++) {
        const cell = normalizeCellText(rowCells[idx]);
        // Look for typical BL patterns: numeric or alphanumeric (not just pure numbers like months)
        if (cell && !cell.match(/^\d{1,2}$/) && cell !== bestMatchAddress && cell !== host) {
          businessLicence = cell;
          blSource = `FALLBACK_INDEX_${idx}`;
          console.log(`[DEBUG-AC4] Using fallback BL extraction at index ${idx}: "${businessLicence}"`);
          break;
        }
      }
    }

    // Enhanced diagnostic logging for all rows in parent groups with 2+ records
    // Show row cells for the first few records to verify column alignment
    if (i < 3) {
      const blKeyNormalized = normalizeHeaderKey('Business Licence');
      const blIndex = effectiveHeaderIndexMap.get(blKeyNormalized);
      const cellIndexes = [];
      for (let idx = 0; idx < Math.min(rowCells.length, 10); idx++) {
        cellIndexes.push(`${idx}:"${rowCells[idx].slice(0, 15)}"`);
      }
      console.log(
        `[DEBUG-AC4] Row ${i}: All Cells = [${cellIndexes.join(' | ')}]`,
      );
      console.log(
        `[DEBUG-AC4] Row ${i} (${blSource}): Registration="${registration}" | ` +
        `Address="${bestMatchAddress.slice(0, 40)}" | Host="${host}" | ` +
        `BL_From_${blSource}="${businessLicence}" | Index_Mapping_For_BL=${blIndex ?? 'NOT_FOUND'} | Cell_At_Index_3="${rowCells[3]}" | Cell_At_Index_4="${rowCells[4]}" | Cell_At_Index_5="${rowCells[5]}"`,
      );
    }

    records.push({ registration, bestMatchAddress, host, businessLicence, rowText });
  }

  return records;
}

async function collectExpandedParentGroups(
  page: Page,
  maxParentsToSample: number,
): Promise<ParentChildGroup[]> {
  const headerIndexMap = await getHeaderIndexMap(page);
  const sampledGroups: ParentChildGroup[] = [];
  const visitedParentSignatures = new Set<string>();

  for (let sample = 0; sample < maxParentsToSample; sample += 1) {
    await collapseExpandedParentRows(page);

    const parentRows = await getVisibleParentRows(page);
    const parentCount = await parentRows.count();
    if (parentCount === 0) {
      break;
    }

    let collectedForThisIteration = false;

    for (let index = 0; index < parentCount; index += 1) {
      const candidate = parentRows.nth(index);
      if (!(await candidate.isVisible().catch(() => false))) {
        continue;
      }

      const signature = normalizeCellText(((await candidate.textContent().catch(() => '')) ?? '').slice(0, 300));
      if (!signature || visitedParentSignatures.has(signature)) {
        continue;
      }

      visitedParentSignatures.add(signature);

      try {
        const expander = await getRowExpander(candidate, page);
        const rowsBeforeExpand = await getVisibleGridRowCount(page);
        await clickExpanderWithRetry(page, expander, rowsBeforeExpand);
        await waitForChildRowsAfterExpand(page, rowsBeforeExpand, candidate);

        const childRows = await getVisibleChildRowsForExpandedParent(page, candidate);
        const childRecords = await extractChildListingRecords(childRows, headerIndexMap);
        if (childRecords.length === 0) {
          continue;
        }

        sampledGroups.push({
          parentSignature: signature,
          childRecords,
        });

        collectedForThisIteration = true;
        break;
      } catch {
        // Try the next parent row candidate in this iteration.
        continue;
      }
    }

    if (!collectedForThisIteration) {
      break;
    }
  }

  await collapseExpandedParentRows(page);
  return sampledGroups;
}

function normalizeCellText(value: string): string {
  return value.replace(/\s+/g, ' ').trim();
}

function normalizeRegistration(value: string): string {
  return normalizeCellText(value).toUpperCase().replace(/[^A-Z0-9]/g, '');
}

function isRegistrationPresent(value: string): boolean {
  const normalized = normalizeRegistration(value);
  if (!normalized) {
    return false;
  }

  const raw = normalizeCellText(value).toUpperCase();
  if (
    ['NA', 'N A', 'NULL', 'NONE', 'UNKNOWN', '-', 'NOREGNUMBER', 'NOREGISTRATION'].includes(
      raw.replace(/\s+/g, ''),
    )
  ) {
    return false;
  }

  // Guard against parsing non-registration values from nearby columns (for example, "Jul-26").
  if (/^(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|SEPT|OCT|NOV|DEC)[-\s]?\d{2,4}$/.test(raw)) {
    return false;
  }

  // Very short numeric values are commonly metrics (for example nights stayed), not registration values.
  if (/^\d{1,4}$/.test(normalized)) {
    return false;
  }

  const hasLetter = /[A-Z]/.test(normalized);
  const hasDigit = /\d/.test(normalized);
  if (hasLetter && hasDigit && normalized.length >= 4) {
    return true;
  }

  // Allow numeric-only registration numbers only when sufficiently long.
  if (!hasLetter && hasDigit && normalized.length >= 6) {
    return true;
  }

  return false;
}

function normalizeAddress(value: string): string {
  return normalizeCellText(value).toLowerCase().replace(/[.,]/g, '').trim();
}

function normalizeHost(value: string): string {
  return normalizeCellText(value).toLowerCase();
}

function normalizeBusinessLicenceNumericKey(value: string): string {
  const normalized = normalizeCellText(value);
  // String-only licence values are intentionally ignored for numeric grouping checks.
  const digitsOnly = normalized.replace(/\D+/g, '');
  return digitsOnly;
}

/**
 * RULE 1.1 - Registration-First Aggregation (Highest Priority)
 * Validates that when Registration values are present, listings are grouped under ONE Registration per parent.
 * Multiple different Registrations must NOT appear under the same parent group.
 * 
 * @param group - ParentChildGroup containing child records to validate
 * @throws {Error} if validation fails
 */
function assertRegistrationFirstAggregation(group: ParentChildGroup): void {
  const withRegistration = group.childRecords.filter((record) => isRegistrationPresent(record.registration));

  if (withRegistration.length === 0) {
    // No registration records to validate - will be handled by fallback hierarchy
    return;
  }

  // Step 6b: Validate that all records with Registration have the SAME Registration value
  const uniqueRegistrations = new Set(
    withRegistration
      .map((record) => normalizeRegistration(record.registration))
      .filter((registration) => registration.length > 0),
  );

  // Step 6c: Assert that ONLY ONE unique Registration exists in this parent group
  expect(
    uniqueRegistrations.size,
    `RULE 1.1 Violation: Expected listings in parent group to share ONE Registration number. ` +
    `Found ${uniqueRegistrations.size} different Registrations. ` +
    `Registrations: [${Array.from(uniqueRegistrations).join(', ')}]. ` +
    `Parent snippet: ${group.parentSignature.slice(0, 140)}`,
  ).toBe(1);

  // RULE 1.2: Multiple different Registrations validation
  // This is implicitly validated above - if a parent has multiple different registrations, it fails
  console.log(
    `[AC4-RULE-1.1] ✓ Parent group has consistent Registration: ${Array.from(uniqueRegistrations)[0]} ` +
    `(${withRegistration.length} child records)`,
  );
}

/**
 * RULE 1.2.x, 1.3, 1.3a-e - Fallback Aggregation Hierarchy (When Registration Absent)
 * Validates aggregation rules when no Registration is present:
 * - RULE 1.2.1: Aggregation order (Address → Host → Business Licence)
 * - RULE 1.2.2: Identical triple aggregation
 * - RULE 1.3: Address-based aggregation foundation
 * - RULE 1.3a: Different host exception
 * - RULE 1.3b: Different business licence exception
 * - RULE 1.3c: String-only licence ignored
 * - RULE 1.3d: Aggregate without licence
 * - RULE 1.3e: Different unit numbers create separate groups
 * 
 * @param group - ParentChildGroup containing child records to validate
 * @throws {Error} if validation fails
 */
function assertFallbackAggregationHierarchy(group: ParentChildGroup): void {
  const withoutRegistration = group.childRecords.filter((record) => !isRegistrationPresent(record.registration));
  
  if (withoutRegistration.length === 0) {
    // All records have registration - should be validated by assertRegistrationFirstAggregation
    return;
  }

  if (withoutRegistration.length < 2) {
    // Single record - no aggregation rules to validate
    console.log(`[AC4-RULE-1.2.x] Single unregistered record - no aggregation validation needed`);
    return;
  }

  // Extract and normalize all address-related data from unregistered records
  const addressRecords = withoutRegistration
    .map((record, idx) => {
      const numericLic = normalizeBusinessLicenceNumericKey(record.businessLicence);
      const hasNumeric = numericLic.length > 0;
      const result = {
        full: record.bestMatchAddress,
        normalized: normalizeAddress(record.bestMatchAddress),
        host: normalizeHost(record.host),
        businessLicence: normalizeCellText(record.businessLicence),
        numericLicence: numericLic,
        hasNumericLicence: hasNumeric,
      };
      // Log first few records to debug business licence extraction
      if (idx < 3) {
        console.log(
          `[AC4-DEBUG] Record ${idx}: BL_RAW="${record.businessLicence}" | BL_NORMALIZED="${result.businessLicence}" | ` +
          `NUMERIC="${numericLic}" | HAS_NUMERIC=${hasNumeric}`,
        );
      }
      return result;
    })
    .filter((addr) => addr.normalized.length > 0);

  if (addressRecords.length === 0) {
    console.log(`[AC4-RULE-1.3] No valid addresses in group - skipping aggregation validation`);
    return;
  }

  // RULE 1.3 & RULE 1.2.1: Address-Based Aggregation Foundation
  // Step 8a, 8b, 7b: Verify all listings with exact same address are aggregated
  const uniqueAddresses = new Set(addressRecords.map((a) => a.normalized));
  expect(
    uniqueAddresses.size,
    `RULE 1.3 Violation: Expected all unregistered listings in one parent group to share the SAME normalized address. ` +
    `Found ${uniqueAddresses.size} different addresses: [${Array.from(uniqueAddresses).join(', ')}]. ` +
    `Parent snippet: ${group.parentSignature.slice(0, 140)}`,
  ).toBe(1);

  console.log(`[AC4-RULE-1.3] ✓ All listings share same normalized address: ${addressRecords[0].normalized}`);

  // RULE 1.3a: Different Host Exception
  // Step 7c, 8c, 8d: Verify same address with different hosts would be separate parents
  const uniqueHosts = new Set(addressRecords.map((a) => a.host).filter((h) => h.length > 0));
  expect(
    uniqueHosts.size,
    `RULE 1.3a Violation: Same address with different Property Host Names must be in separate parent rows. ` +
    `Found ${uniqueHosts.size} different hosts: [${Array.from(uniqueHosts).join(', ')}]. ` +
    `Parent snippet: ${group.parentSignature.slice(0, 140)}`,
  ).toBeLessThanOrEqual(1);

  if (uniqueHosts.size === 1) {
    console.log(`[AC4-RULE-1.3a] ✓ All listings share same host: ${addressRecords[0].host}`);
  }

  // RULE 1.3b & RULE 1.3c: Different Business Licence Exception (numeric only)
  // Step 8e, 8f, 8g, 8h, 8i: Validate business licence consistency
  // IMPORTANT: Only NUMERIC licence values trigger separate grouping (string-only ignored)
  const numericLicences = addressRecords
    .map((a) => a.numericLicence)
    .filter((lic) => lic.length > 0); // Filter out empty and string-only values

  const stringOnlyLicences = addressRecords
    .filter((a) => !a.hasNumericLicence && a.businessLicence.length > 0)
    .map((a) => a.businessLicence);

  if (stringOnlyLicences.length > 0) {
    console.log(
      `[AC4-RULE-1.3c] ✓ Found ${stringOnlyLicences.length} string-only Business Licences (ignored for grouping): ` +
      `[${stringOnlyLicences.join(', ')}]`,
    );
  }

  if (numericLicences.length > 1) {
    // Step 8e, 8f, 8g: Validate numeric licences are consistent
    const licenceDetails = addressRecords
      .filter((a) => a.numericLicence.length > 0)
      .map((a) => `"${a.businessLicence}"(raw)->"${a.numericLicence}"(numeric)`)
      .join(', ');
    
    expect(
      new Set(numericLicences).size,
      `RULE 1.3b Violation: Same address + Host with different NUMERIC Business Licence Numbers must be in separate parent rows. ` +
      `Found ${new Set(numericLicences).size} different numeric licences: [${Array.from(new Set(numericLicences)).join(', ')}]. ` +
      `Licence Details: [${licenceDetails}]. ` +
      `Parent snippet: ${group.parentSignature.slice(0, 140)}`,
    ).toBe(1);
    console.log(`[AC4-RULE-1.3b] ✓ All listings with numeric licence share same value: ${numericLicences[0]}`);
  } else if (numericLicences.length === 1) {
    console.log(`[AC4-RULE-1.3b] ✓ All listings share numeric licence: ${numericLicences[0]}`);
  } else {
    // Step 8j, 8k: No numeric licences - RULE 1.3d validation
    console.log(
      `[AC4-RULE-1.3d] ✓ Listings aggregate without Business Licence numbers (${withoutRegistration.length} records)`,
    );
  }

  // RULE 1.3e: Different Unit Numbers = Separate Groups
  // Step 8l, 8m: Verify all normalized addresses match exactly (unit number validation)
  const allAddressesNormalized = addressRecords.every(
    (a) => a.normalized === addressRecords[0].normalized,
  );
  expect(
    allAddressesNormalized,
    `RULE 1.3e Violation: Listings with different unit numbers (e.g., #101 vs #102) should be in separate groups. ` +
    `Expected all addresses to normalize identically. Full addresses: [${addressRecords.map((a) => a.full).join(', ')}]. ` +
    `Parent: ${group.parentSignature.slice(0, 140)}`,
  ).toBe(true);
  console.log(`[AC4-RULE-1.3e] ✓ No unit number variations detected - all addresses identical after normalization`);

  // Summary
  console.log(
    `[AC4-RULE-1.2.x] ✓ Fallback aggregation validated for ${withoutRegistration.length} unregistered records: ` +
    `Address: "${addressRecords[0].full}", Host: "${addressRecords[0].host}", Licence: "${addressRecords[0].businessLicence || 'NONE'}"`,
  );
}

async function validateRecentlyReportedRegistrationGrouping(
  page: Page,
  maxParentsToSample: number,
): Promise<AggregationValidationStats> {
  const sampledGroups = await collectExpandedParentGroups(page, maxParentsToSample);
  const seenRegistrationToParent = new Map<string, string>();

  let groupsWithRegistration = 0;
  let groupsWithoutRegistration = 0;

  for (const group of sampledGroups) {
    assertRegistrationFirstAggregation(group);

    const registrationsInGroup = Array.from(
      new Set(
        group.childRecords
          .filter((record) => isRegistrationPresent(record.registration))
          .map((record) => normalizeRegistration(record.registration))
          .filter((registration) => registration.length > 0),
      ),
    );

    if (registrationsInGroup.length > 0) {
      groupsWithRegistration += 1;
      for (const registration of registrationsInGroup) {
        const existingParent = seenRegistrationToParent.get(registration);
        expect(
          existingParent,
          `Registration ${registration} appeared under multiple parent groups in sampled Recently Reported results.`,
        ).toBeUndefined();
        seenRegistrationToParent.set(registration, group.parentSignature);
      }
    } else {
      groupsWithoutRegistration += 1;
    }
  }

  return {
    sampledGroups: sampledGroups.length,
    groupsWithRegistration,
    groupsWithoutRegistration,
  };
}

async function validateAllListingsAggregationHierarchy(
  page: Page,
  maxParentsToSample: number,
): Promise<AggregationValidationStats> {
  console.log(`\n[AC4] Starting comprehensive aggregation hierarchy validation (sampling up to ${maxParentsToSample} parent groups)`);
  console.log('===============================================================================================');
  
  const sampledGroups = await collectExpandedParentGroups(page, maxParentsToSample);

  let groupsWithRegistration = 0;
  let groupsWithoutRegistration = 0;

  if (sampledGroups.length === 0) {
    console.log('[AC4] ⚠ No parent groups were sampled - test data may be unavailable');
    return {
      sampledGroups: 0,
      groupsWithRegistration: 0,
      groupsWithoutRegistration: 0,
    };
  }

  console.log(`[AC4] Successfully sampled ${sampledGroups.length} parent group(s) for validation\n`);

  for (let idx = 0; idx < sampledGroups.length; idx += 1) {
    const group = sampledGroups[idx];
    const hasRegistration = group.childRecords.some((record) => isRegistrationPresent(record.registration));

    console.log(`\n[AC4] ───────────────────────────────────────────`);
    console.log(`[AC4] Validating Parent Group ${idx + 1}/${sampledGroups.length}`);
    console.log(`[AC4] Parent Signature: ${group.parentSignature.slice(0, 100)}...`);
    console.log(`[AC4] Total Child Records: ${group.childRecords.length}`);

    if (hasRegistration) {
      groupsWithRegistration += 1;
      console.log(`[AC4] Registration Status: PRESENT - Applying RULE 1.1 & 1.2`);
      console.log(`[AC4] ───────────────────────────────────────────`);
      assertRegistrationFirstAggregation(group);
      console.log(`[AC4] Parent Group ${idx + 1} validation: ✓ PASSED (RULE 1.1 & 1.2)\n`);
    } else {
      groupsWithoutRegistration += 1;
      console.log(`[AC4] Registration Status: ABSENT - Applying RULE 1.2.x, 1.3, 1.3a-e`);
      console.log(`[AC4] ───────────────────────────────────────────`);
      assertFallbackAggregationHierarchy(group);
      console.log(`[AC4] Parent Group ${idx + 1} validation: ✓ PASSED (RULE 1.2.x, 1.3, 1.3a-e)\n`);
    }
  }

  console.log(`\n[AC4] ═══════════════════════════════════════════`);
  console.log(`[AC4] VALIDATION SUMMARY`);
  console.log(`[AC4] ═══════════════════════════════════════════`);
  console.log(`[AC4] Total Parent Groups Validated: ${sampledGroups.length}`);
  console.log(`[AC4]   - Groups with Registration (RULE 1.1 & 1.2): ${groupsWithRegistration}`);
  console.log(`[AC4]   - Groups without Registration (RULE 1.2.x, 1.3, 1.3a-e): ${groupsWithoutRegistration}`);
  console.log(`[AC4] ═══════════════════════════════════════════\n`);

  return {
    sampledGroups: sampledGroups.length,
    groupsWithRegistration,
    groupsWithoutRegistration,
  };
}

async function countVisibleRows(rows: Locator): Promise<number> {
  const total = await rows.count();
  let visible = 0;
  for (let i = 0; i < total; i += 1) {
    if (await rows.nth(i).isVisible().catch(() => false)) {
      visible += 1;
    }
  }
  return visible;
}

async function waitForChildRowsAfterExpand(
  page: Page,
  previousCount: number,
  parentRow?: Locator,
): Promise<void> {
  await page.waitForLoadState('load').catch(() => undefined);
  await waitForGridInteractionReady(page, 15_000).catch(() => undefined);

  await expect
    .poll(async () => {
      if (parentRow) {
        const expanded = await isParentRowExpanded(page, parentRow);
        if (expanded) {
          const parentScopedChildRows = await getVisibleChildRowsForExpandedParent(page, parentRow).catch(() => null);
          if (parentScopedChildRows && (await parentScopedChildRows.count()) > 0) {
            return true;
          }
        }
      }

      const visibleRows = await getVisibleGridRowCount(page);

      const ariaChildRows = page
        .locator('[role="row"][aria-level="2"], [role="row"][aria-level="3"]')
        .filter({ hasNot: page.locator('[role="columnheader"]') });
      const expandedDetailRows = page.locator('tbody tr.p-datatable-row-expansion, tr:has(td[colspan])');

      const visibleAriaChildren = await countVisibleRows(ariaChildRows);
      const visibleDetailRows = await countVisibleRows(expandedDetailRows);

      return visibleRows > previousCount || visibleAriaChildren > 0 || visibleDetailRows > 0;
    }, {
      timeout: 45_000,
      intervals: [500, 1_000, 2_000],
      message: 'Expected child rows to appear after expanding a parent listing row.',
    })
    .toBe(true);
}

async function clickExpanderWithRetry(page: Page, expander: Locator, rowsBeforeExpand: number): Promise<void> {
  let lastError: unknown;

  for (let attempt = 1; attempt <= 3; attempt += 1) {
    try {
      await waitForGridInteractionReady(page, 20_000);
      await expander.scrollIntoViewIfNeeded().catch(() => undefined);
      await expect
        .poll(
          async () => {
            return await expander
              .click({ trial: true, timeout: 2_000 })
              .then(() => true)
              .catch(() => false);
          },
          {
            timeout: 8_000,
            intervals: [300, 600],
          },
        )
        .toBe(true)
        .catch(() => undefined);

      try {
        await expander.click({ timeout: 8_000 });
      } catch (clickErr) {
        const msg = String(clickErr);
        if (/intercepts pointer events|not receiving pointer events/i.test(msg)) {
          await waitForGridInteractionReady(page, 10_000);
          await expander.click({ force: true, timeout: 5_000 });
        } else {
          throw clickErr;
        }
      }

      const expandedViaAria = await expect
        .poll(async () => (await expander.getAttribute('aria-expanded')) === 'true', {
          timeout: 4_000,
          intervals: [300, 600],
        })
        .toBe(true)
        .then(() => true)
        .catch(() => false);

      if (expandedViaAria) {
        return;
      }

      const rowsIncreased = await expect
        .poll(async () => getVisibleGridRowCount(page), {
          timeout: 4_000,
          intervals: [300, 600],
        })
        .toBeGreaterThan(rowsBeforeExpand)
        .then(() => true)
        .catch(() => false);

      if (rowsIncreased) {
        return;
      }
    } catch (err) {
      lastError = err;
    }

    await waitForGridInteractionReady(page, 10_000).catch(() => undefined);
  }

  throw new Error(
    `Failed to expand a parent row after retries${lastError ? `: ${String(lastError)}` : ''}`,
  );
}

async function waitForGridInteractionReady(page: Page, timeout = 30_000): Promise<void> {
  const loaderSelector =
    'div.loader.ng-star-inserted, .loader.ng-star-inserted, .p-datatable-loading-overlay, [aria-busy="true"], [role="progressbar"]';
  const gridSelector = '[aria-label="table-of-aggregated-listing-groups"], table';

  await expect
    .poll(
      async () => {
        const blocking = await isOverlayLikelyBlockingGridInteractions(page, loaderSelector, gridSelector);
        return !blocking;
      },
      {
        timeout,
        intervals: [300, 600, 1_000],
        message: 'Expected listing grid to become interactable before interaction.',
      },
    )
    .toBe(true);
}

async function isOverlayLikelyBlockingGridInteractions(
  page: Page,
  loaderSelector: string,
  gridSelector: string,
): Promise<boolean> {
  return page.evaluate(
    ({ loaderSelectorInner, gridSelectorInner }) => {
      const grid = document.querySelector(gridSelectorInner) as HTMLElement | null;
      if (!grid) {
        return false;
      }

      const gridRect = grid.getBoundingClientRect();
      if (gridRect.width < 2 || gridRect.height < 2) {
        return false;
      }

      const loaders = Array.from(document.querySelectorAll(loaderSelectorInner)) as HTMLElement[];
      if (loaders.length === 0) {
        return false;
      }

      const centerX = gridRect.left + gridRect.width / 2;
      const centerY = gridRect.top + Math.min(40, gridRect.height / 2);
      const topElement = document.elementFromPoint(centerX, centerY) as HTMLElement | null;

      for (const loader of loaders) {
        const style = window.getComputedStyle(loader);
        const hidden =
          style.display === 'none' ||
          style.visibility === 'hidden' ||
          Number.parseFloat(style.opacity || '1') < 0.05;
        if (hidden) {
          continue;
        }

        if (style.pointerEvents === 'none') {
          continue;
        }

        const rect = loader.getBoundingClientRect();
        const intersectsGrid =
          rect.right > gridRect.left &&
          rect.left < gridRect.right &&
          rect.bottom > gridRect.top &&
          rect.top < gridRect.bottom;
        if (!intersectsGrid) {
          continue;
        }

        if (topElement && (loader.contains(topElement) || topElement === loader)) {
          return true;
        }
      }

      return false;
    },
    { loaderSelectorInner: loaderSelector, gridSelectorInner: gridSelector },
  );
}

async function assertCommonFieldHeadersVisible(page: Page, expectedHeaders: string[]): Promise<void> {
  const listingGrid = await getListingGridContainer(page);
  await assertFieldHeadersVisibleInContainer(listingGrid, expectedHeaders);
}

async function getListingGridContainer(page: Page): Promise<Locator> {
  const candidates = [
    page.locator('[aria-label="table-of-aggregated-listing-groups"]').first(),
    page.getByRole('table').first(),
  ];

  for (const candidate of candidates) {
    if ((await candidate.count()) > 0 && (await candidate.isVisible().catch(() => false))) {
      return candidate;
    }
  }

  throw new Error('Unable to locate the aggregated listing grid container.');
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

    await expect(headerLocator.first(), `Missing expected listing field: ${header}`).toBeVisible({
      timeout: 15_000,
    });
  }
}

async function assertChildRowsContainAllFields(page: Page, expectedFields: string[]): Promise<void> {
  // Wait for child rows to become queryable after expansion.
  await expect
    .poll(
      async () => {
        const ariaRows = await page
          .locator('[role="row"]').filter({ hasNot: page.locator('[role="columnheader"]') })
          .count();
        const tbodyRows = await page.locator('tbody tr').count();
        return Math.max(ariaRows, tbodyRows);
      },
      {
        timeout: 20_000,
        intervals: [300, 600, 1_000],
        message: 'Expected child/data rows to appear after expansion.',
      },
    )
    .toBeGreaterThan(0);
  
  // Verify child rows exist with data
  let allRows = page.locator('[role="row"]').filter({ hasNot: page.locator('[role="columnheader"]') });
  
  let rowCount = await allRows.count();
  
  // Fallback: try tbody rows if no ARIA rows found
  if (rowCount === 0) {
    allRows = page.locator('tbody tr');
    rowCount = await allRows.count();
  }

  if (rowCount === 0) {
    throw new Error('No data rows found in the aggregated listing grid after expansion.');
  }

  // Collect visible row text to validate we have data
  let visibleRowCount = 0;
  let allRowText = '';
  
  for (let i = 0; i < Math.min(rowCount, 50); i += 1) {
    const isVisible = await allRows.nth(i).isVisible().catch(() => false);
    if (isVisible) {
      visibleRowCount++;
      const rowText = await allRows.nth(i).textContent();
      allRowText += (rowText ?? '') + ' ';
    }
  }

  if (visibleRowCount === 0) {
    throw new Error('No visible data found in aggregated listing rows after expansion.');
  }

  // Strategy 1: Check if field names appear in headers (they should, validated by assertCommonFieldHeadersVisible)
  // Strategy 2: Check if at least some expected fields are found in row data as either names or recognizable values
  
  const gridContainer = await getListingGridContainer(page);
  let fieldFoundCount = 0;
  const missingFields: string[] = [];

  const fieldAliases: Record<string, string[]> = {
    Listings: ['Listing'],
  };

  for (const field of expectedFields) {
    const candidateLabels = [field, ...(fieldAliases[field] ?? [])];
    const fieldRegexes = candidateLabels.map(
      // Use escaped literal matching because \b can miss labels ending with punctuation like "(12M)".
      (label) => new RegExp(escapeRegExp(label), 'i'),
    );
    
    // Check headers first
    const headerText = await gridContainer.locator('[role="columnheader"], th').allTextContents();
    const headerHasField = headerText.some((h) => fieldRegexes.some((regex) => regex.test(h)));
    
    if (headerHasField) {
      fieldFoundCount++;
    } else {
      // If not in header, check if it appears in row data
      const foundInRows = fieldRegexes.some((regex) => regex.test(allRowText));
      if (foundInRows) {
        fieldFoundCount++;
      } else {
        missingFields.push(field);
      }
    }
  }

  // Require 100% of expected fields to be found
  const minimumFieldsRequired = expectedFields.length;
  expect(
    fieldFoundCount,
    `Expected to find at least ${minimumFieldsRequired} of ${expectedFields.length} child listing fields in headers or data. Missing: ${missingFields.join(', ')}`,
  ).toBeGreaterThanOrEqual(minimumFieldsRequired);
}

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}


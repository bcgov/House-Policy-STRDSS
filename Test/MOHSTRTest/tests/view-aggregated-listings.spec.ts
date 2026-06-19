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
    const totalGroups = await getTotalGroupsCount(page);
    test.skip(
      totalGroups === 0,
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

    await waitForChildRowsAfterExpand(page, rowsBeforeExpand);

    // c. Then I should see all the child elements data for that parent element
    const childRows = await getVisibleChildRowsForExpandedParent(page);
    await expect(childRows.first()).toBeVisible({ timeout: 10_000 });
    await assertRowsHaveData(childRows, 5);

    // d. Then I should see key information for each child listing, including:
    // Last Reported, Registration, Best Match Address, Nights stayed (12M),
    // Business Licence, Matched BL, Last Action, Last Action Date, Listings
    await assertChildRowsContainAllFields(page, EXPECTED_CHILD_FIELDS);
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
    page.locator('button:has(svg)'),
    page.locator('button:has-text("+")'),
    page.locator('button[aria-label*="xpand" i]'),
    page.locator('[role="button"][aria-expanded]'),
    page.locator('button'),  // Last resort: any button in a row
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
    row.locator('button').first(),
    row.locator('[role="button"]').first(),
  ];

  for (const candidate of withinRow) {
    if ((await candidate.count()) > 0 && (await candidate.isVisible().catch(() => false))) {
      return candidate;
    }
  }

  return getFirstRowExpander(page);
}

async function getVisibleChildRowsForExpandedParent(page: Page): Promise<Locator> {
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

  return getDataRows(page);
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

async function waitForChildRowsAfterExpand(page: Page, previousCount: number): Promise<void> {
  await page.waitForLoadState('load').catch(() => undefined);
  await waitForGridInteractionReady(page, 15_000).catch(() => undefined);

  await expect
    .poll(async () => {
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


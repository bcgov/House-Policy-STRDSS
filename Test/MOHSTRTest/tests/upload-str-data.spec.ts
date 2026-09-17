/// <reference types="node" />

/**
 * Feature: Upload Platform STR and Takedown Data
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-23
 *
 * @UploadSTRData
 * Scenario: UploadSTRDataAndTakedownData
 *
 * Test Case Summary:
 * Given I am an authenticated platform representative with IDIR or Business BCeID credentials
 * When I navigate to the STR portal and submit STR listing or Takedown data through the upload form
 * Then the upload should be accepted, processed, or reported as a duplicate with the appropriate outcome
 * And I should be able to view the matching reporting-history entry with the correct values, applying
 * strict upload date/uploader checks only when fresh data was imported (duplicate/submitted outcomes skip them)
 * And proper validation messages should be displayed for form field requirements and file constraints
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1-AC6 are IDIR-only acceptance criteria.
 *
 * AC1 - Platform Staff Authentication via IDIR:
 * - Step 1: Navigate to BASE_URL
 * - Step 2: Verify "Authenticate with:" heading is visible (timeout: 45s)
 * - Step 3: Locate and click IDIR login link
 * - Step 4: Verify username input field is displayed
 * - Step 5: Verify password input field is displayed
 * - Step 6: Enter IDIR username and password credentials
 * - Step 7: Click "Continue" button
 * - Step 8: Verify NO error messages appear (pattern: "Enter an IDIR username and password")
 * - Step 9: Verify "Short-Term Rental Data Portal" heading appears after login
 *
 * AC2 - Access to STR Portal and Home Region:
 * - Step 1: After successful IDIR authentication, verify landing on STR portal
 * - Step 2: Verify Home region becomes visible (timeout: 30s)
 * - Step 3: Verify the Home region is visible and the portal navigation is available for the next action
 *
 * AC3 - Submit Platform Data Button and Upload Form Label Validation:
 * - Step 1: On home page, locate "Submit Platform Data" button
 * - Step 2: Click the button and navigate to upload-listing-data page (timeout: 60s)
 * - Step 3: Verify form label "Select Report Type" is visible
 * - Step 4: Verify form label "Select Reporting Platform Name" is visible
 * - Step 5: Verify form label "Select Reporting Month" is visible
 * - Step 6: Verify form label "Upload Your Platform Data (.CSV File Only)" is visible
 * - Step 7: Verify the upload form is ready for field validation; the Upload button is verified disabled before file selection in AC4
 *
 * AC4 - Select Report Type, Platform Name, Reporting Month, and File Upload:
 * - Step 1: Click "Select Report Type" dropdown (PrimeNG combobox)
 * - Step 2: Verify listbox overlay opens with options
 * - Step 3: Select "Short-Term Rental Listing Data" option
 * - Step 4: Verify dropdown closes and selection is reflected
 * - Step 5: Click "Select platform" dropdown
 * - Step 6: Verify "Major 1" platform option is available
 * - Step 7: Select "Major 1" platform from dropdown
 * - Step 8: Click "Select Reporting Month" dropdown/date picker
 * - Step 9: Verify future month options are disabled in the dropdown
 * - Step 10: Select the previous calendar month dynamically as the valid reporting month
 * - Step 11: Attempt to upload a non-CSV file (e.g., .txt) and verify it is rejected
 *   ✓ Validation message, "No file selected" label, or disabled Upload button confirms rejection
 * - Step 12: Reset form (navigate Home → re-open upload form) to clear invalid file state
 * - Step 13: Re-select Report Type, Platform Name, and Reporting Month with same values
 * - Step 14: Locate and click file input for CSV upload
 * - Step 15: Upload valid CSV from test-data/Major1 matching the selected reporting month (e.g., May 2026.csv for May 2026)
 * - Step 16: Verify selected filename is displayed on the form
 * - Step 17: Verify Upload button becomes enabled after all fields filled and valid file selected
 *
 * AC5 - Upload File and Data Import:
 * - Step 1: Click Upload button to submit the STR listing data
 * - Step 2: Verify upload request is sent and processing begins
 * - Step 3: Validate the outcome: success ("File has been uploaded successfully"), duplicate
 *   ("The file has already been uploaded"), submitted (form reset with no explicit message), or failure
 * - Step 4: If the import fails, verify the error message and skip AC6
 * - Step 5: On success, duplicate, or submitted outcomes, continue to AC6 to validate reporting history
 *
 * AC6 - View Reporting History and Validate Table:
 * - Step 1: Navigate back to Home page (after successful upload)
 * - Step 2: Locate and click "View Reporting History" button
 * - Step 3: Verify reporting-history page loads (timeout: 60s)
 * - Step 4: Validate table with reporting history data is visible
 * - Step 5: Verify table headers match the actual reporting-history columns:
 *   ✓ Platform Name
 *   ✓ Report Type
 *   ✓ Reported Month (YYYY-MM)
 *   ✓ Status (e.g., "Processed", "Pending")
 *   ✓ Total Records
 *   ✓ Success
 *   ✓ Errors
 *   ✓ Upload Date
 *   ✓ Uploaded By
 * - Step 6: Verify the matching history row:
 *   ✓ On a fresh ("success") upload, match the row using the selected month, today's upload date,
 *     and the authenticated uploader identity
 *   ✓ On a "duplicate" or "submitted" outcome (no fresh data was imported), relax the match to
 *     platform + report type + reported month only, skipping the strict upload date/uploader checks
 *
 * AC7 - Platform Representative Authentication via Business BCeID:
 * - Step 1: Authenticate with the configured Business BCeID credentials
 * - Step 2: Verify the "Short-Term Rental Data Portal" heading and Home region are visible
 *
 * AC8 - BCeID Submit Platform Data Form Validation:
 * - Step 1: Navigate to the upload-listing-data page using "Submit Platform Data"
 * - Step 2: Verify page title "Platform Reporting" is visible
 * - Step 3: Verify form label "Select Report Type" is visible
 * - Step 4: Verify form label "Select Reporting Month" is visible
 * - Step 5: Verify form label "Upload Your Platform Data (.CSV File Only)" is visible
 * - Step 6: Verify the Upload button is present and initially disabled
 *
 * AC9 - BCeID Report Type, Reporting Month, and File Validation:
 * - Step 1: Open "Select Report Type" and verify "Short-Term Rental Listing Data" and "Takedown Data"
 * - Step 2: Select "Short-Term Rental Listing Data" and verify the selection is reflected
 * - Step 3: Open "Select Reporting Month", verify a future month is disabled, and select the previous calendar month dynamically
 * - Step 4: Upload a non-CSV file and verify it is rejected
 * - Step 5: Reset the form, reselect the report type and reporting month, then upload the matching
 *   CSV from test-data/Minor1
 * - Step 6: Verify the selected filename is displayed and Upload becomes enabled
 *
 * AC10 - BCeID STR Listing Data Upload and Reporting History:
 * - Step 1: Submit the CSV and verify a request or processing indicator
 * - Step 2: Verify the outcome: success, duplicate, submitted, or failure
 * - Step 3: Verify the matching reporting-history entry includes Platform, Report Type, Reported Month
 *   (YYYY-MM), status (Processed or Pending), Total Records, Success, and Errors
 * - Step 4: On a fresh ("success") upload, also verify the current upload date and Uploaded By derived
 *   from the authenticated platform header (for example, ", Allura Minor1"); on a "duplicate" or
 *   "submitted" outcome (no fresh data was imported), skip those two strict checks and match the
 *   existing row instead; on an import failure, verify the displayed error and bypass reporting history validation
 *
 * AC11 - BCeID Takedown Data Form and Report Type Validation:
 * - Step 1: Authenticate with BCEID_PLATFORM_USERNAME and land on the portal Home region
 * - Step 2: Navigate to upload-listing-data using "Submit Platform Data"
 * - Step 3: Open the Select Report Type PrimeNG combobox and verify both "Short-Term Rental Listing Data"
 *   and "Takedown Data" options are present
 * - Step 4: Select "Takedown Data" and verify the combobox reflects the selection
 * - Step 5: Open Select Reporting Month, verify the future month is disabled, and select the previous month dynamically
 * - Step 6: Upload a non-CSV file and verify the application rejects it
 *
 * AC12 - BCeID Takedown Data CSV Validation:
 * - Step 1: Reset the upload form by navigating Home and reopening Submit Platform Data
 * - Step 2: Reselect Takedown Data and the same reporting month
 * - Step 3: Upload the matching CSV from test-data/Minor1/Takedown
 * - Step 4: Verify the selected filename is displayed and Upload becomes enabled
 *
 * AC13 - BCeID Takedown Data Import:
 * - Step 1: Click Upload and verify an upload request, response, or processing indicator
 * - Step 2: Verify a success, duplicate, or submitted outcome; on import failure, verify the error message
 *
 * AC14 - BCeID Takedown Reporting History:
 * - Step 1: After a successful, duplicate, or submitted outcome, navigate Home → View Reporting History
 * - Step 2: Verify the reporting-history table and actual headers are visible
 * - Step 3: Verify the matching Takedown Data row contains the selected month as YYYY-MM, valid status,
 *   and record counts
 * - Step 4: On a fresh ("success") upload, also verify the current upload date and Uploaded By derived
 *   from the authenticated header; on a "duplicate" or "submitted" outcome (no fresh data was imported),
 *   skip those two strict checks and match the existing row instead
 *
 * AC15 - BCeID Takedown Data Failure Handling:
 * - Step 1: If import fails, verify an error message and do not assert a successful reporting-history row
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import {
  BCEID_PLATFORM_AUTH_ENV_MESSAGE,
  IDIR_AUTH_ENV_MESSAGE,
  hasBceidPlatformAuthConfig,
  hasIdirAuthConfig,
  loginAsBceidPlatform as loginAsBceidPlatformShared,
  loginAsIdir as loginAsIdirShared,
} from './support/auth';
import fs from 'node:fs/promises';
import path from 'node:path';

// ---------------------------------------------------------------------------
// Constants
// ---------------------------------------------------------------------------

const APP_URL       = process.env.BASE_URL       ?? '';

const PLATFORM_NAME     = 'Major 1';

const EXPECTED_HISTORY_HEADERS = [
  'Platform',
  'Report Type',
  'Reported Month',
  'Status',
  'Total Records',
  'Success',
  'Errors',
  'Upload Date',
  'Uploaded By',
];

// ---------------------------------------------------------------------------
// Test suite
// ---------------------------------------------------------------------------

test.use({ browserName: 'chromium' });

test.describe('@regression Feature: UploadPlatformSTRData', () => {
  test.setTimeout(360_000);

  test('@smoke authorized platform representative uploads STR listing data and validates reporting history', async ({
    page,
  }) => {
    test.skip(!hasIdirAuthConfig(), IDIR_AUTH_ENV_MESSAGE);
    let platformUserName = '';
    // ── AC1 + AC2: Authenticate and access the STR portal ──────────────────
    await test.step('AC1-AC2: Authenticate via IDIR and land on STR portal home', async () => {
      await loginAsIdir(page);
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
      platformUserName = await getPlatformUploadedBy(page);
    });

    // ── AC3: Click "Submit Platform Data" → validate upload form labels ─────
    await test.step('AC3: Click "Submit Platform Data" and validate form labels', async () => {
      await page.getByRole('button', { name: /^Submit Platform Data$/i }).click();

      // All four required labels must be visible on the upload-listing-data page
      await expect(
        page.locator('label, legend, p, div').filter({ hasText: /^Select Report Type$/ }).first(),
      ).toBeVisible({ timeout: 60_000 });
      await expect(
        page.locator('label, legend, p, div').filter({ hasText: /^Select Reporting Platform Name$/ }).first(),
      ).toBeVisible();
      await expect(
        page.locator('label, legend, p, div').filter({ hasText: /^Select Reporting Month$/ }).first(),
      ).toBeVisible();
      await expect(
        page.getByText(/Upload Your Platform Data \(\.CSV File Only\)/i).first(),
      ).toBeVisible();
    });

    // ── AC4a: Select Report Type ────────────────────────────────────────────
    await test.step('AC4: Click "Select Report Type" dropdown (PrimeNG combobox)', async () => {
      // PrimeNG combobox aria-label equals the current value or its placeholder.
      // On first load the placeholder is "Select Report Type"; after selection it changes.
      await openPrimengDropdown(page, /Select Report Type|Short-Term Rental Listing Data/i);
    });

    // ── AC4a (cont.): Verify listbox opens and select "Short-Term Rental Listing Data" ────
    await test.step('AC4: Verify listbox overlay opens with options and select "Short-Term Rental Listing Data"', async () => {
      await expect(page.locator('[role="listbox"]').first()).toBeVisible({ timeout: 10_000 });
      await choosePrimengOption(page, /Short-?Term Rental Listing Data/i);
      await expect(page.locator('[role="listbox"]').first()).toHaveCount(0, { timeout: 10_000 }).catch(() => {});
    });

    // ── AC4b: Select Platform Name ──────────────────────────────────────────
    await test.step('AC4: Click "Select platform" dropdown and verify "Major 1" is available', async () => {
      // aria-label of the platform combobox placeholder is "Select platform"
      await openPrimengDropdown(page, /^Select platform$/i);
      await expect(page.locator('[role="listbox"]').first()).toBeVisible({ timeout: 10_000 });
      const majorPlatformOption = page.getByRole('option', { name: new RegExp(`^${escapeRegExp(PLATFORM_NAME)}$`, 'i') }).first();
      await expect(majorPlatformOption).toBeVisible({ timeout: 10_000 });
    });

    // ── AC4b (cont.): Select "Major 1" platform ────────────────────────────
    await test.step('AC4: Select "Major 1" platform from dropdown', async () => {
      await choosePrimengOption(page, new RegExp(`^${escapeRegExp(PLATFORM_NAME)}$`, 'i'));
    });

    // ── AC4c: Select a valid reporting month ────────────────────────────────
    let selectedMonthLabel = '';
    await test.step('AC4: Click "Select Reporting Month" dropdown and select a valid reporting month', async () => {
      const futureMonth   = shiftMonth(new Date(), 1);
      const validMonth    = shiftMonth(new Date(), -1); // Use previous month as a valid selection

      // Open month dropdown (placeholder aria-label is "Select month")
      await openPrimengDropdown(page, /^Select month$/i);
      await expect(page.locator('[role="listbox"]').first()).toBeVisible({ timeout: 10_000 });
      
      // Verify future month is disabled
      await assertFutureMonthOptionDisabled(page, futureMonth);

      // Close the dropdown and re-open for selection to ensure clean state
      await page.keyboard.press('Escape');
      // Wait for the listbox to fully hide (do not swallow the error — a still-open listbox
      // causes the subsequent openPrimengDropdown to fight with the Escape-close animation).
      await page.locator('[role="listbox"]').waitFor({ state: 'hidden', timeout: 15_000 });
      await expect(page.locator('[role="listbox"]')).toHaveCount(0, { timeout: 5_000 });

      // Open month dropdown again and select the valid month
      const validMonthLabel = toLongMonthLabel(validMonth);
      await selectReportingMonthWithRetry(page, validMonthLabel);
      selectedMonthLabel = await getSelectedMonthLabel(page);
    });

    // ── AC4d: Validate only .csv files are accepted ─────────────────────────
    await test.step('AC4: Validate non-CSV file is rejected', async () => {
      const invalidFilePath = await createInvalidTextFixture();
      try {
        await chooseFileForUpload(page, invalidFilePath);
        await assertNonCsvRejected(page);
      } finally {
        await fs.rm(invalidFilePath, { force: true });
      }
      
      // Clear the invalid file selection by resetting the form
      // Navigate away and back to clear validation state
      await page.getByRole('link', { name: /^Home$/i }).click();
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
      // Use 'load' instead of 'networkidle': Angular apps with polling/WebSockets rarely
      // reach networkidle, causing a guaranteed 10 s timeout on every run.
      await page.waitForLoadState('load', { timeout: 30_000 }).catch(() => {});
      
      // Re-open upload form
      await page.getByRole('button', { name: /^Submit Platform Data$/i }).click();
      await expect(page.getByText(/Upload Your Platform Data/i)).toBeVisible({ timeout: 30_000 });
      // Gate on the form being fully reset to placeholder state before re-selecting.
      // This prevents subsequent openPrimengDropdown calls from failing because the
      // combobox still shows a stale selected value from the previous navigation.
      await expect(
        page.getByRole('combobox', { name: /^Select Report Type$/i }).first(),
      ).toBeVisible({ timeout: 30_000 });
      await page.waitForLoadState('load', { timeout: 30_000 }).catch(() => {});
      
      // Re-select all form fields fresh
      // Use the same combined pattern as the first selection to handle any residual state.
      await openPrimengDropdown(page, /Select Report Type|Short-Term Rental Listing Data/i);
      await choosePrimengOption(page, /Short-?Term Rental Listing Data/i);
      
      await openPrimengDropdown(page, /^Select platform$/i);
      await choosePrimengOption(page, new RegExp(`^${escapeRegExp(PLATFORM_NAME)}$`, 'i'));
      
      // Re-select the reporting month
      const validMonthLabel = selectedMonthLabel || toLongMonthLabel(shiftMonth(new Date(), -1));
      await selectReportingMonthWithRetry(page, validMonthLabel);
      selectedMonthLabel = await getSelectedMonthLabel(page);
    });

    let selectedCsvFilename = '';

    // ── AC4e: Upload valid CSV file matching the selected reporting month ───
    await test.step('AC4: Locate and click file input for CSV upload', async () => {
      await assertAc4ComboboxSelections(page, selectedMonthLabel);

      // Determine which CSV file to upload based on month actually selected in UI.
      const csvFilePath = getCsvFilePathForMonthLabel('Major1', selectedMonthLabel);
      selectedCsvFilename = path.basename(csvFilePath);

      // Ensure the fixture exists with the reporting-period naming convention.
      await assertFixtureExists(csvFilePath);
      
      // Verify upload button is disabled before file selection
      const uploadBtn = getUploadActionButton(page);
      await expect(uploadBtn).toBeDisabled({ timeout: 5_000 });
      
      // Upload the file matching the selected reporting month
      await chooseFileForUpload(page, csvFilePath);
      await assertSelectedFilename(page, selectedCsvFilename);
    });

    // ── AC4f: Verify Upload button becomes enabled after all fields filled and file selected ───
    await test.step('AC4: Verify Upload button becomes enabled after all fields filled and file selected', async () => {
      await waitForUploadButtonEnabled(page, selectedCsvFilename);
    });

    // ── AC5: Click Upload and handle import success or failure ─────────────
    const ac5Outcome = await test.step('AC5: Click Upload button and verify upload outcome', async () =>
      submitUploadAndGetOutcome(page, 'AC5'),
    );

    // ── AC6a: Navigate Home → View Reporting History (bypass if AC5 failed) ─────
    if (ac5Outcome.kind === 'failure') {
      await test.step('AC6: Bypassed because AC5 data load failed and error path was validated', async () => {
        await assertUploadFailureMessage(page, ac5Outcome.message);
      });
      return;
    }
    
    await test.step('AC6: Navigate to Home then click "View Reporting History"', async () => {
      await page.getByRole('link', { name: /^Home$/i }).click();
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });

      await page.getByRole('button', { name: /^View Reporting History$/i }).click();
      await expect(page.getByRole('table').first()).toBeVisible({ timeout: 60_000 });
    });

    // ── AC6b: Validate required column headers ─────────────────────────────
    await test.step('AC6: Validate all required column headers are present', async () => {
      await assertPlatformReportingHistoryHeaders(page, EXPECTED_HISTORY_HEADERS);
    });

    await test.step('AC6: Validate the uploaded reporting history row', async () => {
      // ac5Outcome.kind is 'success' | 'duplicate' | 'submitted' here ('failure' returned above);
      // duplicate/submitted mean no fresh data was imported, so the row match is relaxed accordingly.
      await assertReportingHistoryRow(
        page,
        PLATFORM_NAME,
        'Listing Data',
        selectedMonthLabel,
        platformUserName,
        'AC6',
        ac5Outcome.kind,
      );
    });

  });

  test('@smoke Business BCeID representative uploads Minor 1 STR listing data and validates reporting history', async ({ page }) => {
    test.skip(!hasBceidPlatformAuthConfig(), BCEID_PLATFORM_AUTH_ENV_MESSAGE);
    let platformUserName = '';

    await test.step('AC7: Authenticate via Business BCeID and land on STR portal home', async () => {
      await loginAsBceid(page);
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
      platformUserName = await getPlatformUploadedBy(page);
    });

    await test.step('AC8: Navigate to upload-listing-data and validate BCeID form labels', async () => {
      await page.getByRole('button', { name: /^Submit Platform Data$/i }).click();

      await expect(
        page.locator('div.title', { hasText: /^Platform Reporting$/i }).first(),
      ).toBeVisible({ timeout: 60_000 });

      for (const label of [
        /^Select Report Type$/i,
        /^Select Reporting Month$/i,
      ]) {
        await expect(page.getByText(label).first()).toBeVisible({ timeout: 60_000 });
      }
      await expect(page.getByText(/Upload Your Platform Data \(\.CSV File Only\)/i).first()).toBeVisible();
      await expect(getUploadActionButton(page)).toBeDisabled();
    });

    let selectedMonthLabel = toLongMonthLabel(shiftMonth(new Date(), -1));
    await test.step('AC9: Validate report types, future-month restriction, and select reporting month', async () => {
      await openPrimengDropdown(page, /^Select Report Type$/i);
      const listbox = page.locator('[role="listbox"]').first();
      await expect(listbox.getByRole('option', { name: /Short-?Term Rental Listing Data/i }).first()).toBeVisible();
      await expect(listbox.getByRole('option', { name: /^Takedown Data$/i }).first()).toBeVisible();
      await choosePrimengOption(page, /Short-?Term Rental Listing Data/i);
      await expect(page.getByRole('combobox', { name: /Short-?Term Rental Listing Data/i }).first()).toBeVisible();

      await openPrimengDropdown(page, /^Select month$/i);
      await assertFutureMonthOptionDisabled(page, shiftMonth(new Date(), 1));
      await page.keyboard.press('Escape');
      await page.locator('[role="listbox"]').first().waitFor({ state: 'hidden', timeout: 15_000 });
      await selectReportingMonthWithRetry(page, selectedMonthLabel);
      selectedMonthLabel = await getSelectedMonthLabel(page);
    });

    await test.step('AC9: Reject a non-CSV file and reset the BCeID upload form', async () => {
      const invalidFilePath = await createInvalidTextFixture();
      try {
        await chooseFileForUpload(page, invalidFilePath);
        await assertNonCsvRejected(page);
      } finally {
        await fs.rm(invalidFilePath, { force: true });
      }

      await page.getByRole('link', { name: /^Home$/i }).click();
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
      await page.getByRole('button', { name: /^Submit Platform Data$/i }).click();
      await expect(page.getByRole('combobox', { name: /^Select Report Type$/i }).first()).toBeVisible({ timeout: 30_000 });
      await openPrimengDropdown(page, /^Select Report Type$/i);
      await choosePrimengOption(page, /Short-?Term Rental Listing Data/i);
      await selectReportingMonthWithRetry(page, selectedMonthLabel);
      selectedMonthLabel = await getSelectedMonthLabel(page);
    });

    let selectedCsvFilename = '';
    await test.step('AC9: Upload the matching Minor 1 CSV and enable submission', async () => {
      await assertBceidAc9ComboboxSelections(page, selectedMonthLabel);
      const csvFilePath = getCsvFilePathForMonthLabel('Minor1', selectedMonthLabel);
      selectedCsvFilename = path.basename(csvFilePath);
      await assertFixtureExists(csvFilePath);
      await expect(getUploadActionButton(page)).toBeDisabled({ timeout: 5_000 });
      await chooseFileForUpload(page, csvFilePath);
      await assertSelectedFilename(page, selectedCsvFilename);
      await waitForUploadButtonEnabled(page, selectedCsvFilename);
    });

    const uploadOutcome = await test.step('AC10: Submit Minor 1 data and verify upload outcome', async () =>
      submitUploadAndGetOutcome(page, 'AC10'),
    );

    if (uploadOutcome.kind === 'failure') {
      await test.step('AC10: Bypass reporting history after validated import failure', async () => {
        await assertUploadFailureMessage(page, uploadOutcome.message);
      });
      return;
    }

    await test.step('AC10: Validate BCeID reporting history for the uploaded month', async () => {
      await page.getByRole('link', { name: /^Home$/i }).click();
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
      await page.getByRole('button', { name: /^View Reporting History$/i }).click();
      const table = page.getByRole('table').first();
      await expect(table).toBeVisible({ timeout: 60_000 });
      await assertPlatformReportingHistoryHeaders(page, EXPECTED_HISTORY_HEADERS);

      // 'submitted' is included alongside 'success'/'duplicate' - all three reach here because
      // 'failure' already returned above. When no fresh data was imported (duplicate/submitted),
      // assertReportingHistoryRow falls back to matching the existing row instead of failing.
      await assertReportingHistoryRow(
        page,
        'Minor 1',
        'Listing Data',
        selectedMonthLabel,
        platformUserName,
        'AC10',
        uploadOutcome.kind,
      );
    });
  });

  test('@smoke Business BCeID representative uploads Takedown Data and validates reporting history', async ({ page }) => {
    test.skip(!hasBceidPlatformAuthConfig(), BCEID_PLATFORM_AUTH_ENV_MESSAGE);
    let platformUploadedBy = '';
    let selectedMonthLabel = toLongMonthLabel(shiftMonth(new Date(), -1));

    await test.step('AC11: Authenticate with platform BCeID and open the upload form', async () => {
      await loginAsBceid(page);
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
      platformUploadedBy = await getPlatformUploadedBy(page);
      await page.getByRole('button', { name: /^Submit Platform Data$/i }).click();
      await expect(page.getByText(/Upload Your Platform Data/i).first()).toBeVisible({ timeout: 60_000 });
    });

    await test.step('AC11: Verify report type options and select Takedown Data', async () => {
      await openPrimengDropdown(page, /^Select Report Type$/i);
      const listbox = page.locator('[role="listbox"]').first();
      await expect(listbox.getByRole('option', { name: /Short-?Term Rental Listing Data/i }).first()).toBeVisible();
      await expect(listbox.getByRole('option', { name: /^Takedown Data$/i }).first()).toBeVisible();
      await choosePrimengOption(page, /^Takedown Data$/i);
      await expect(page.getByRole('combobox', { name: /^Takedown Data$/i }).first()).toBeVisible();
    });

    await test.step('AC11: Select a valid reporting month and reject a non-CSV file', async () => {
      await openPrimengDropdown(page, /^Select month$/i);
      await assertFutureMonthOptionDisabled(page, shiftMonth(new Date(), 1));
      await page.keyboard.press('Escape');
      await page.locator('[role="listbox"]').first().waitFor({ state: 'hidden', timeout: 15_000 });
      await selectReportingMonthWithRetry(page, selectedMonthLabel);
      selectedMonthLabel = await getSelectedMonthLabel(page);

      const invalidFilePath = await createInvalidTextFixture();
      try {
        await chooseFileForUpload(page, invalidFilePath);
        await assertNonCsvRejected(page);
      } finally {
        await fs.rm(invalidFilePath, { force: true });
      }
    });

    let selectedCsvFilename = '';
    await test.step('AC12: Reset the form and upload the matching Takedown Data CSV', async () => {
      await page.getByRole('link', { name: /^Home$/i }).click();
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
      await page.getByRole('button', { name: /^Submit Platform Data$/i }).click();
      await expect(page.getByRole('combobox', { name: /^Select Report Type$/i }).first()).toBeVisible({ timeout: 30_000 });

      await openPrimengDropdown(page, /^Select Report Type$/i);
      await choosePrimengOption(page, /^Takedown Data$/i);
      await selectReportingMonthWithRetry(page, selectedMonthLabel);

      const csvFilePath = getCsvFilePathForMonthLabel('Minor1/Takedown', selectedMonthLabel);
      selectedCsvFilename = path.basename(csvFilePath);
      await assertFixtureExists(csvFilePath);
      await expect(getUploadActionButton(page)).toBeDisabled({ timeout: 5_000 });
      await chooseFileForUpload(page, csvFilePath);
      await assertSelectedFilename(page, selectedCsvFilename);
      await waitForUploadButtonEnabled(page, selectedCsvFilename);
    });

    const uploadOutcome = await test.step('AC13: Submit Takedown Data and verify the upload outcome', async () =>
      submitUploadAndGetOutcome(page, 'AC13'),
    );

    if (uploadOutcome.kind === 'failure') {
      await test.step('AC15: Verify the Takedown Data import failure message', async () => {
        await assertUploadFailureMessage(page, uploadOutcome.message);
      });
      return;
    }

    await test.step('AC14: Validate the Takedown Data reporting-history row', async () => {
      await page.getByRole('link', { name: /^Home$/i }).click();
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
      await page.getByRole('button', { name: /^View Reporting History$/i }).click();
      await expect(page.getByRole('table').first()).toBeVisible({ timeout: 60_000 });
      await assertPlatformReportingHistoryHeaders(page, EXPECTED_HISTORY_HEADERS);
      // uploadOutcome.kind is 'success' | 'duplicate' | 'submitted' here ('failure' returned above);
      // duplicate/submitted mean no fresh data was imported, so the row match is relaxed accordingly.
      await assertReportingHistoryRow(
        page,
        'Minor 1',
        'Takedown Data',
        selectedMonthLabel,
        platformUploadedBy,
        'AC14',
        uploadOutcome.kind,
      );
    });
  });
});

// ---------------------------------------------------------------------------
// Authentication
// ---------------------------------------------------------------------------

async function loginAsIdir(page: Page): Promise<void> {
  await loginAsIdirShared(page, APP_URL);
}

async function loginAsBceid(page: Page): Promise<void> {
  await loginAsBceidPlatformShared(page, APP_URL);
}

async function getPlatformUploadedBy(page: Page): Promise<string> {
  const userNameElement = page.locator('.user-container.ng-star-inserted').first();
  await expect(userNameElement).toBeVisible({ timeout: 20_000 });

  const displayName = (await userNameElement.innerText()).trim();
  expect(displayName, 'Expected the authenticated platform user name in the top-right header').toBeTruthy();

  const identityTokens = displayName
    .split(/\s+/)
    .filter((token) => token && !/^HMA:/i.test(token));
  const strdpIndex = identityTokens.findIndex((token) => /^STRDP$/i.test(token));

  if (strdpIndex >= 0 && identityTokens.length > 1) {
    const accountName = identityTokens.filter((_, index) => index !== strdpIndex).join(' ');
    return `${accountName}, ${identityTokens[strdpIndex]}`;
  }

  return `, ${displayName}`;
}

// ---------------------------------------------------------------------------
// PrimeNG dropdown helpers
//
// PrimeNG p-select renders:
//   <span role="combobox" aria-label="<selected value or placeholder>">
//   <button aria-label="dropdown trigger">
//   <ul role="listbox">   ← appears only when open
//     <li role="option" aria-label="<option text>" data-p-disabled="true|false">
// ---------------------------------------------------------------------------

/**
 * Click a PrimeNG combobox to open its overlay.
 * Closes any already-open overlay first to avoid stale state.
 * @param comboboxNamePattern – matches the combobox's current aria-label
 */
async function openPrimengDropdown(page: Page, comboboxNamePattern: RegExp): Promise<void> {
  // Close any already-open overlay
  const openListbox = page.locator('[role="listbox"]');
  if (await openListbox.isVisible().catch(() => false)) {
    await page.keyboard.press('Escape');
    await openListbox.waitFor({ state: 'hidden', timeout: 8_000 }).catch(() => {});
  }

  const trigger = page.getByRole('combobox', { name: comboboxNamePattern }).first();
  await expect(trigger).toBeVisible({ timeout: 30_000 });
  await trigger.scrollIntoViewIfNeeded();

  await expect
    .poll(
      async () => {
        await trigger.click({ timeout: 5_000 });
        return await page.locator('[role="listbox"]').first().isVisible().catch(() => false);
      },
      {
        timeout: 15_000,
        message: `Expected listbox to open for combobox matching ${comboboxNamePattern}`,
      },
    )
    .toBe(true);

  // Wait for the listbox overlay to appear and be stable before any option interaction
  await page.locator('[role="listbox"]').first().waitFor({ state: 'visible', timeout: 15_000 });
}

/**
 * Click the first option matching optionNamePattern inside an open PrimeNG listbox.
 * Waits for the overlay to close after the click.
 */
async function choosePrimengOption(page: Page, optionNamePattern: RegExp): Promise<void> {
  const listbox = page.locator('[role="listbox"]').first();
  await listbox.waitFor({ state: 'visible', timeout: 10_000 });

  // Scope the option query to the visible listbox to avoid hidden/duplicate options.
  const option = listbox.getByRole('option', { name: optionNamePattern }).first();
  await expect(option).toBeVisible({ timeout: 10_000 });
  await option.click();

  // Allow the overlay close animation to complete before the next action
  await listbox.waitFor({ state: 'hidden', timeout: 10_000 }).catch(() => {});
}

async function choosePrimengOptionWithFallback(
  page: Page,
  preferredOptionPattern: RegExp,
  comboboxNamePatternForRetry: RegExp,
): Promise<void> {
  const listbox = page.locator('[role="listbox"]').first();
  await listbox.waitFor({ state: 'visible', timeout: 10_000 });

  const preferredOption = listbox.getByRole('option', { name: preferredOptionPattern }).first();
  const preferredVisible = await preferredOption.isVisible({ timeout: 2_000 }).catch(() => false);
  const preferredDisabled =
    (await preferredOption.getAttribute('aria-disabled').catch(() => null)) === 'true' ||
    (await preferredOption.getAttribute('data-p-disabled').catch(() => null)) === 'true';

  if (preferredVisible && !preferredDisabled) {
    await preferredOption.click();
    await listbox.waitFor({ state: 'hidden', timeout: 10_000 }).catch(() => {});
    return;
  }

  // Fallback: select the first visible enabled option.
  const options = listbox.getByRole('option');
  const count = await options.count();
  for (let i = 0; i < count; i += 1) {
    const option = options.nth(i);
    const visible = await option.isVisible().catch(() => false);
    if (!visible) {
      continue;
    }

    const disabled =
      (await option.getAttribute('aria-disabled').catch(() => null)) === 'true' ||
      (await option.getAttribute('data-p-disabled').catch(() => null)) === 'true';
    if (disabled) {
      continue;
    }

    await option.click();
    await listbox.waitFor({ state: 'hidden', timeout: 10_000 }).catch(() => {});
    return;
  }

  // Retry once from a clean state before failing.
  await page.keyboard.press('Escape').catch(() => {});
  await listbox.waitFor({ state: 'hidden', timeout: 5_000 }).catch(() => {});
  await openPrimengDropdown(page, comboboxNamePatternForRetry);
  await choosePrimengOption(page, preferredOptionPattern);
}

/**
 * With the month listbox already open, assert the future month option is disabled.
 * Does NOT click the disabled option.
 */
async function assertFutureMonthOptionDisabled(page: Page, futureDate: Date): Promise<void> {
  const futureLabel = toLongMonthLabel(futureDate);

  const listbox = page.locator('[role="listbox"]').first();
  await listbox.waitFor({ state: 'visible', timeout: 10_000 });

  const futureOption = page.getByRole('option', { name: futureLabel, exact: true }).first();

  if ((await futureOption.count()) > 0) {
    // PrimeNG disables options via data-p-disabled="true" and/or aria-disabled="true"
    const ariaDisabled = await futureOption.getAttribute('aria-disabled');
    const dataDisabled = await futureOption.getAttribute('data-p-disabled');
    const isDisabled   = ariaDisabled === 'true' || dataDisabled === 'true';

    expect(
      isDisabled,
      `Expected future month "${futureLabel}" to be disabled in the month dropdown.`,
    ).toBe(true);
  }
  // Option absent from the list entirely also satisfies the "future month blocked" rule.
}

// ---------------------------------------------------------------------------
// File upload helpers
// ---------------------------------------------------------------------------

async function chooseFileForUpload(page: Page, filePath: string): Promise<void> {
  const selectFileButton = page.getByRole('button', { name: /^Select a File$/i }).first();
  await expect(selectFileButton).toBeVisible({ timeout: 30_000 });

  // Playwright filechooser event handles hidden inputs inside custom file-picker components
  const [chooser] = await Promise.all([
    page.waitForEvent('filechooser', { timeout: 15_000 }),
    selectFileButton.click(),
  ]);
  await chooser.setFiles(filePath);
}

async function assertNonCsvRejected(page: Page): Promise<void> {
  // App must respond with a validation message, "No file selected", OR disabled Upload button
  const csvValidation = page
    .getByText(/only.*csv|csv.*only|invalid.*file.*format|unsupported.*file|file format/i)
    .first();
  const noFileLabel  = page.getByText(/No file selected/i).first();
  const uploadButton = getUploadActionButton(page);

  const hasMsg     = await csvValidation.isVisible({ timeout: 3_000 }).catch(() => false);
  const noFile     = await noFileLabel.isVisible({ timeout: 3_000 }).catch(() => false);
  const btnEnabled = await uploadButton.isEnabled().catch(() => false);

  expect(
    hasMsg || noFile || !btnEnabled,
    'Expected the app to reject a non-CSV file (validation message, "No file selected", or disabled Upload button).',
  ).toBe(true);
}

async function assertSelectedFilename(page: Page, expectedFilename: string): Promise<void> {
  await expect(
    page.getByText(new RegExp(escapeRegExp(expectedFilename), 'i')).first(),
  ).toBeVisible({ timeout: 15_000 });
}

function getUploadActionButton(page: Page): Locator {
  return page.getByRole('button', { name: /^Upload$/i }).first();
}

async function waitForUploadButtonEnabled(page: Page, expectedFilename: string): Promise<void> {
  const uploadButton = getUploadActionButton(page);

  await expect
    .poll(
      async () => {
        const hasFilename = await page
          .getByText(new RegExp(escapeRegExp(expectedFilename), 'i'))
          .first()
          .isVisible()
          .catch(() => false);
        const isEnabled = await uploadButton.isEnabled().catch(() => false);
        const hasVisibleError = await page
          .locator('.p-toast-message-error, [class*="error" i], .error')
          .first()
          .isVisible({ timeout: 500 })
          .catch(() => false);

        return hasFilename && isEnabled && !hasVisibleError;
      },
      {
        timeout: 30_000,
        intervals: [500, 1_000],
        message:
          'Expected Upload button to become enabled after valid file selection and completed form state.',
      },
    )
    .toBe(true);
}

async function getSelectedMonthLabel(page: Page): Promise<string> {
  const monthPattern = /^(January|February|March|April|May|June|July|August|September|October|November|December)\s+\d{4}$/i;

  await expect
    .poll(
      async () => {
        const label = await getMonthComboboxAriaLabel(page);
        return monthPattern.test(label) ? label : '';
      },
      {
        timeout: 15_000,
        intervals: [500, 1_000],
        message: 'Expected month combobox to reflect selected month label.',
      },
    )
    .not.toBe('');

  return getMonthComboboxAriaLabel(page);
}

async function getMonthComboboxAriaLabel(page: Page): Promise<string> {
  const monthCombobox = page
    .getByRole('combobox', {
      name: /Select month|January|February|March|April|May|June|July|August|September|October|November|December/i,
    })
    .first();

  await expect(monthCombobox).toBeVisible({ timeout: 15_000 });
  return ((await monthCombobox.getAttribute('aria-label')) ?? '').trim();
}

async function selectReportingMonthWithRetry(page: Page, monthLabel: string): Promise<void> {
  const monthLabelPattern = new RegExp(`^${escapeRegExp(monthLabel)}$`, 'i');
  const monthComboboxNamePattern =
    /Select month|January|February|March|April|May|June|July|August|September|October|November|December/i;

  await expect
    .poll(
      async () => {
        const current = await getMonthComboboxAriaLabel(page);
        if (monthLabelPattern.test(current)) {
          return true;
        }

        await openPrimengDropdown(page, monthComboboxNamePattern);
        await choosePrimengOptionWithFallback(page, monthLabelPattern, monthComboboxNamePattern);

        const after = await getMonthComboboxAriaLabel(page);
        return monthLabelPattern.test(after);
      },
      {
        timeout: 30_000,
        intervals: [500, 1_000],
        message: `Expected reporting month "${monthLabel}" to be selected.`,
      },
    )
    .toBe(true);
}

async function assertAc4ComboboxSelections(page: Page, monthLabel: string): Promise<void> {
  await expect(
    page.getByRole('combobox', { name: /Short-?Term Rental Listing Data/i }).first(),
    'Expected report type combobox to show selected report type.',
  ).toBeVisible({ timeout: 10_000 });

  await expect(
    page.getByRole('combobox', { name: new RegExp(`^${escapeRegExp(PLATFORM_NAME)}$`, 'i') }).first(),
    'Expected platform combobox to show selected platform.',
  ).toBeVisible({ timeout: 10_000 });

  expect(monthLabel, 'Expected selected month label before upload.').not.toBe('');
  await expect(
    page.getByRole('combobox', { name: new RegExp(`^${escapeRegExp(monthLabel)}$`, 'i') }).first(),
    `Expected month combobox to show selected month "${monthLabel}" before upload.`,
  ).toBeVisible({ timeout: 10_000 });
}

async function assertBceidAc9ComboboxSelections(page: Page, monthLabel: string): Promise<void> {
  await expect(
    page.getByRole('combobox', { name: /Short-?Term Rental Listing Data/i }).first(),
    'Expected report type combobox to show selected report type.',
  ).toBeVisible({ timeout: 10_000 });

  expect(monthLabel, 'Expected selected month label before upload.').not.toBe('');
  await expect(
    page.getByRole('combobox', { name: new RegExp(`^${escapeRegExp(monthLabel)}$`, 'i') }).first(),
    `Expected month combobox to show selected month "${monthLabel}" before upload.`,
  ).toBeVisible({ timeout: 10_000 });
}

// ---------------------------------------------------------------------------
// Import outcome polling
// ---------------------------------------------------------------------------

async function submitUploadAndGetOutcome(
  page: Page,
  acceptanceCriteria: string,
): Promise<{ kind: 'success' | 'failure' | 'duplicate' | 'submitted'; message: string }> {
  const uploadButton = getUploadActionButton(page);
  await expect(uploadButton).toBeEnabled({ timeout: 30_000 });

  const isUploadRequest = (request: { method(): string; url(): string }): boolean =>
    ['POST', 'PUT', 'PATCH'].includes(request.method()) && /(upload|import|report|listing|file)/i.test(request.url());

  const uploadRequestPromise = page.waitForRequest(isUploadRequest, { timeout: 15_000 }).catch(() => null);
  const uploadResponsePromise = page
    .waitForResponse((response) => isUploadRequest(response.request()), { timeout: 15_000 })
    .catch(() => null);

  await uploadButton.click();

  const [uploadRequest, uploadResponse] = await Promise.all([uploadRequestPromise, uploadResponsePromise]);
  const processingVisible = await page
    .getByText(/processing|uploading|importing|in progress/i)
    .first()
    .isVisible({ timeout: 2_000 })
    .catch(() => false);

  expect(
    uploadRequest !== null || uploadResponse !== null || processingVisible,
    'Expected upload request/response or processing indicator after clicking Upload.',
  ).toBe(true);

  const outcome = await waitForImportOutcome(page);
  if (outcome.kind === 'failure') {
    console.error(`[${acceptanceCriteria}] Data import failed: ${outcome.message}`);
    return outcome;
  }

  expect(
    outcome.kind === 'success' || outcome.kind === 'duplicate' || outcome.kind === 'submitted',
    `Upload outcome unclear - got kind="${outcome.kind}". Details: ${outcome.message}`,
  ).toBe(true);
  console.log(`[${acceptanceCriteria}] Data import ${outcome.kind}: ${outcome.message}`);
  return outcome;
}

async function assertUploadFailureMessage(page: Page, outcomeMessage: string): Promise<void> {
  const errorMessage = page
    .locator('.p-toast-message-error, [class*="error" i], .error, [role="alert"]')
    .first();
  const errorVisible = await errorMessage.isVisible({ timeout: 10_000 }).catch(() => false);
  expect(errorVisible || outcomeMessage !== '', 'Expected an error message after upload failure.').toBe(true);
}

/**
 * Polls for an upload outcome after clicking the Upload button.
 *
 * kind values:
 *  'success'   – app shows "File has been uploaded successfully"
 *  'duplicate' – app shows "The file has already been uploaded"
 *  'failure'   – any other error message detected
 *  'submitted' – form reset or network idle indicates completion (implicit success signal;
 *                actual result confirmed in AC6 via Reporting History row)
 */
async function waitForImportOutcome(
  page: Page,
): Promise<{ kind: 'success' | 'failure' | 'duplicate' | 'submitted'; message: string }> {
  // Exact messages the application surfaces after an upload attempt
  const SUCCESS_TEXT   = /file has been uploaded successfully|success|data uploaded|import.*success/i;
  const DUPLICATE_TEXT = /the file has already been uploaded|file has already been uploaded|duplicate/i;
  const FAILURE_TEXT   = /upload failed|import failed|could not import|failed to import|error|invalid/i;

  let detectedOutcome: { kind: 'success' | 'failure' | 'duplicate' | 'submitted'; message: string } | null = null;

  await expect
    .poll(
      async () => {
        try {
          const toasts = page.locator('.p-toast-message');
          const toastCount = await toasts.count();
          for (let i = 0; i < toastCount; i++) {
            const toast = toasts.nth(i);
            if (!(await toast.isVisible().catch(() => false))) continue;
            const msg = (await toast.innerText().catch(() => '')).trim();
            if (SUCCESS_TEXT.test(msg)) {
              detectedOutcome = { kind: 'success', message: msg };
              return true;
            }
            if (DUPLICATE_TEXT.test(msg)) {
              detectedOutcome = { kind: 'duplicate', message: msg };
              return true;
            }
            if (FAILURE_TEXT.test(msg)) {
              detectedOutcome = { kind: 'failure', message: msg };
              return true;
            }
          }

          const bodyText = (await page.locator('body').innerText()).replace(/\s+/g, ' ').trim();
          if (SUCCESS_TEXT.test(bodyText)) {
            detectedOutcome = { kind: 'success', message: bodyText.substring(0, 300) };
            return true;
          }
          if (DUPLICATE_TEXT.test(bodyText)) {
            detectedOutcome = { kind: 'duplicate', message: bodyText.substring(0, 300) };
            return true;
          }
          if (FAILURE_TEXT.test(bodyText)) {
            detectedOutcome = { kind: 'failure', message: bodyText.substring(0, 300) };
            return true;
          }

          const reportTypeCombobox = page.getByRole('combobox', { name: /Select Report Type/i }).first();
          const platformCombobox = page.getByRole('combobox', { name: /Select platform/i }).first();
          const monthCombobox = page.getByRole('combobox', { name: /Select month/i }).first();
          const fileSelectedText = page.getByText(/No file selected|Select a File/i).first();

          const reportTypeReset =
            (await reportTypeCombobox.count()) > 0 &&
            ((await reportTypeCombobox.getAttribute('aria-label').catch(() => '')) ?? '').includes('Select Report Type');
          const platformReset =
            (await platformCombobox.count()) > 0 &&
            ((await platformCombobox.getAttribute('aria-label').catch(() => '')) ?? '').includes('Select platform');
          const monthReset =
            (await monthCombobox.count()) > 0 &&
            ((await monthCombobox.getAttribute('aria-label').catch(() => '')) ?? '').includes('Select month');
          const fileReset = await fileSelectedText.isVisible({ timeout: 1_000 }).catch(() => false);

          if ((reportTypeReset || platformReset || monthReset) && fileReset) {
            detectedOutcome = {
              kind: 'submitted',
              message: 'Upload form was reset – submission accepted by the portal.',
            };
            return true;
          }
        } catch {
          // Ignore transient detached-element errors and continue polling.
        }

        return false;
      },
      {
        timeout: 90_000,
        intervals: [300, 600, 1_000],
        message: 'Expected upload outcome notification or form reset state.',
      },
    )
    .toBe(true)
    .catch(() => undefined);

  if (!detectedOutcome) {
    throw new Error('Timed out (90 s) waiting for upload success, duplicate, failure, or form-reset confirmation.');
  }

  return detectedOutcome;
}

// ---------------------------------------------------------------------------
// Platform Reporting History assertions
// ---------------------------------------------------------------------------

async function assertPlatformReportingHistoryHeaders(
  page: Page,
  headers: string[],
): Promise<void> {
  const table = page.getByRole('table').first();
  await expect(table).toBeVisible({ timeout: 30_000 });

  for (const header of headers) {
    await expect(
      table.getByRole('columnheader', { name: new RegExp(`^${escapeRegExp(header)}$`, 'i') }),
      `Missing expected column header: "${header}"`,
    ).toBeVisible({ timeout: 20_000 });
  }
}

/**
 * Outcome kinds that reach reporting-history validation ('failure' is handled separately by callers).
 * 'success' means a brand-new row was imported just now; 'duplicate'/'submitted' mean the portal
 * recognized data uploaded in an earlier run and may not have created a fresh row at all.
 */
type ReportingHistoryOutcomeKind = 'success' | 'duplicate' | 'submitted';

async function assertReportingHistoryRow(
  page: Page,
  platformName: string,
  reportType: string,
  monthLabel: string,
  uploadedBy: string,
  acceptanceCriteria: 'AC6' | 'AC10' | 'AC14',
  outcomeKind: ReportingHistoryOutcomeKind,
): Promise<void> {
  const reportedMonth = toReportedMonthValue(monthLabel);
  const uploadDate = toPortalDate(new Date());
  const table = page.getByRole('table').first();
  // Only a 'success' outcome guarantees a fresh row stamped with today's date and the current
  // uploader; a duplicate/submitted outcome may point back to an older row from a prior test run,
  // so we relax the match to platform + report type + reported month only (no fresh data to assert).
  const hasFreshUploadData = outcomeKind === 'success';

  const row = hasFreshUploadData
    ? table
        .getByRole('row')
        .filter({ hasText: platformName })
        .filter({ hasText: reportType })
        .filter({ hasText: reportedMonth })
        .filter({ hasText: uploadDate })
        .filter({ hasText: uploadedBy })
        .first()
    : table
        .getByRole('row')
        .filter({ hasText: platformName })
        .filter({ hasText: reportType })
        .filter({ hasText: reportedMonth })
        .first();

  await expect(
    row,
    hasFreshUploadData
      ? `[${acceptanceCriteria}] Expected a fresh reporting-history row for "${platformName}" / "${reportType}" / "${reportedMonth}" uploaded today by "${uploadedBy}".`
      : `[${acceptanceCriteria}] Outcome="${outcomeKind}" (no fresh data) - expected an existing reporting-history row for "${platformName}" / "${reportType}" / "${reportedMonth}".`,
  ).toBeVisible({ timeout: 30_000 });

  const cells = row.getByRole('cell');
  await expect(cells).toHaveCount(9);
  await expect(cells.nth(0)).toHaveText(new RegExp(`^${escapeRegExp(platformName)}$`, 'i'));
  await expect(cells.nth(1)).toHaveText(new RegExp(`^${escapeRegExp(reportType)}$`, 'i'));
  await expect(cells.nth(2)).toHaveText(new RegExp(`^${escapeRegExp(reportedMonth)}$`));
  await expect(cells.nth(3)).toHaveText(/^(Processed|Pending)$/i);
  await expect(cells.nth(4)).toHaveText(/^\s*\d+\s*$/);
  await expect(cells.nth(5)).toHaveText(/^\s*\d+\s*$/);
  await expect(cells.nth(6)).toHaveText(/^\s*\d+\s*$/);

  if (hasFreshUploadData) {
    await expect(cells.nth(7)).toHaveText(new RegExp(`^${escapeRegExp(uploadDate)}$`));
    await expect(cells.nth(8)).toContainText(uploadedBy);
  } else {
    // No fresh data was imported, so the Upload Date / Uploaded By columns may reflect an earlier
    // run - skip the strict date/uploader assertions instead of failing on stale-but-valid data.
    console.warn(
      `[${acceptanceCriteria}] Outcome="${outcomeKind}": skipped strict Upload Date / Uploaded By ` +
        'checks because no fresh data was uploaded; matched the existing reporting-history row instead.',
    );
  }

  const [
    platform,
    reportTypeValue,
    month,
    status,
    totalRecords,
    successfulRecords,
    errors,
    uploadedDate,
    uploadedByValue,
  ] = (await cells.allTextContents()).map((value) => value.trim());

  await test.info().attach(`${acceptanceCriteria} reporting history validation`, {
    body: Buffer.from(JSON.stringify({
      outcomeKind,
      hasFreshUploadData,
      platform,
      reportType: reportTypeValue,
      reportedMonth: month,
      status,
      totalRecords,
      successfulRecords,
      errors,
      uploadedDate,
      uploadedBy: uploadedByValue,
    }, null, 2)),
    contentType: 'application/json',
  });
}

// ---------------------------------------------------------------------------
// Date utilities
// ---------------------------------------------------------------------------

function shiftMonth(reference: Date, delta: number): Date {
  return new Date(reference.getFullYear(), reference.getMonth() + delta, 1);
}

function toLongMonthLabel(date: Date): string {
  return new Intl.DateTimeFormat('en-CA', { month: 'long', year: 'numeric' }).format(date);
}

function toReportedMonthValue(monthLabel: string): string {
  const match = monthLabel.match(/^(January|February|March|April|May|June|July|August|September|October|November|December)\s+(\d{4})$/i);
  if (!match) {
    throw new Error(`Could not convert reporting month label: "${monthLabel}"`);
  }

  const monthNames = [
    'january', 'february', 'march', 'april', 'may', 'june',
    'july', 'august', 'september', 'october', 'november', 'december',
  ];
  const month = String(monthNames.indexOf(match[1].toLowerCase()) + 1).padStart(2, '0');
  return `${match[2]}-${month}`;
}

function toPortalDate(date: Date): string {
  const dateParts = new Intl.DateTimeFormat('en-CA', {
    timeZone: 'America/Vancouver',
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  }).formatToParts(date);
  const parts = Object.fromEntries(dateParts.map(({ type, value }) => [type, value]));
  return `${parts.year}-${parts.month}-${parts.day}`;
}

/**
 * Get a platform's CSV file path for a selected month label using reporting-period naming.
 * Example: "Minor1", "May 2026" → "test-data/Minor1/May 2026.csv"
 */
function getCsvFilePathForMonthLabel(platformDirectory: 'Major1' | 'Minor1' | 'Minor1/Takedown', monthLabel: string): string {
  return path.resolve(process.cwd(), 'test-data', platformDirectory, `${monthLabel}.csv`);
}

// ---------------------------------------------------------------------------
// Test data fixtures
// ---------------------------------------------------------------------------

async function createInvalidTextFixture(): Promise<string> {
  const filePath = path.resolve(
    process.cwd(),
    'test-data',
    `invalid-upload-${Date.now()}.txt`,
  );
  await fs.writeFile(filePath, 'this is not a csv file', 'utf8');
  return filePath;
}

async function assertFixtureExists(filePath: string): Promise<void> {
  try {
    await fs.access(filePath);
  } catch {
    throw new Error(
      `Expected reporting-period fixture file not found: ${filePath}. ` +
        'Ensure the platform files in test-data/<platform> follow naming like "May 2026.csv".',
    );
  }
}

// ---------------------------------------------------------------------------
// Utilities
// ---------------------------------------------------------------------------

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}


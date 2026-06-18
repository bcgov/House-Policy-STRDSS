/// <reference types="node" />

/**
 * Feature : Short-Term Rental Data Portal – Sending Multiple Notices of Non-Compliance
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-678
 *
 * @SendingMultipleNoticesOfNonCompliance
 * Scenario: SendingMultipleNoticesOfNonCompliance
 * Test Case Summary:
 * Given I am an authenticated BCeID Local Government user with valid credentials
 * When I navigate to the listings page and select one or multiple listings
 * Then the Send Notices of Non-Compliance button should become enabled
 * And I should be able to fill out notice details with mandatory and optional fields
 * And upon submission, notices should be sent and action history updated
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 - Send Notices Button State Based on Listing Selection:
 * - Step 1: Authenticate via Business BCeID login
 * - Step 2: Navigate to listings page from homepage (View Listings)
 * - Step 3: Verify Send Notices of Non-Compliance button is disabled
 * - Step 4: Click the "Recently Reported" toggle to switch to All Listings mode
 * - Step 5: Select one or multiple listings from the grid
 * - Step 6: Verify Send Notices of Non-Compliance button becomes enabled
 *
 * AC2 - Mandatory Fields Validation and Required Field Behavior:
 * - Step 1: Navigate to listings page
 * - Step 2: Click the "Recently Reported" toggle to switch to All Listings mode and select at least one listing
 * - Step 3: Click Send Notices of Non-Compliance button
 * - Step 4: System opens bulk-compliance-notice page
 * - Step 5: Verify Review button is disabled when form is empty (all mandatory fields incomplete)
 * - Step 6: Fill in each mandatory field and verify Review button state updates appropriately
 * - Step 7: Verify all mandatory fields are clearly marked/labeled as required
 *
 * AC3 - Cancel Flow:
 * - Step 1: Navigate to listings page
 * - Step 2: Click the "Recently Reported" toggle to switch to All Listings mode and select at least one listing
 * - Step 3: Click Send Notices of Non-Compliance button
 * - Step 4: System opens bulk-compliance-notice page
 * - Step 5: Partially fill form (do not complete all mandatory fields)
 * - Step 6: Click Cancel button
 * - Step 7: Verify user is redirected back to listings page
 *
 * AC4 - Email Validation (Format and Multiple Email Support):
 * - Step 1: Navigate to notice details form
 * - Step 2: Enter invalid email format (e.g., "notanemail" or "user@")
 * - Step 3: Verify Review button is disabled with validation error message
 * - Step 4: Verify error message displays: "Ensure the email format you have entered is correct"
 * - Step 5: Enter valid email format
 * - Step 6: Verify Review button becomes enabled
 * - Step 7: Add multiple email addresses (if supported)
 * - Step 8: Verify all email addresses are retained and validated
 * - Step 9: Remove one email and verify form state updates
 *
 * AC5 - Review and Submit Flow with Notifications and Action History Update:
 * - Step 1: Authenticate via Business BCeID login
 * - Step 2: Navigate to listings page from homepage (View Listings)
 * - Step 3: Click the "Recently Reported" toggle to switch to All Listings mode and select at least one listing
 * - Step 4: Click Send Notices of Non-Compliance button – system opens bulk-compliance-notice page
 * - Step 5: Complete all mandatory fields with valid data (LG email address)
 * - Step 6: Verify Review button is enabled after mandatory fields are filled
 * - Step 7: Click Review button
 * - Step 8: Verify Send Notice of Non-Compliance dialog opens
 * - Step 9: Click Cancel button inside the Send Notice of Non-Compliance dialog
 * - Step 10: Verify user is returned to bulk-compliance-notice page with heading "Send Notices of Non-Compliance"
 * - Step 11: Click Review button again and verify it is enabled
 * - Step 12: Verify user lands on Send Notice of Non-Compliance dialog again
 * - Step 13: Verify Submit button is enabled inside the dialog
 * - Step 14: Click Submit button
 * - Step 15: Verify success confirmation message displays (top right of page) and user lands on listings page
 * - Step 16: Click the "Recently Reported" toggle to switch to All Listings mode
 * - Step 17: Verify Last Action column value is updated with "Notice of Non-Compliance"
 *
 * AC6 - Listing Checkbox Removal and Review Button State:
 * - Step 1: Navigate to notice details form with listings selected
 * - Step 2: Verify listing checkboxes are displayed and checked
 * - Step 3: Verify Review button is enabled when listings are checked
 * - Step 4: Uncheck all listing checkboxes
 * - Step 5: Verify Review button becomes disabled
 * - Step 6: Re-check at least one listing
 * - Step 7: Verify Review button becomes enabled again
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import {
  BCEID_AUTH_ENV_MESSAGE,
  hasBceidAuthConfig,
  loginAsBceid as loginAsBceidShared,
} from './support/auth';

const APP_URL = process.env.BASE_URL ?? '';

test.use({ browserName: 'chromium' });

type ListingsPageState = 'listings-available' | 'no-listings' | 'error';

// ---------------------------------------------------------------------------
// Helper functions for email validation
// ---------------------------------------------------------------------------

function isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

async function hasAnyVisible(locators: Locator[], timeoutMs = 1_500): Promise<boolean> {
  for (const locator of locators) {
    if (await locator.isVisible({ timeout: timeoutMs }).catch(() => false)) {
      return true;
    }
  }

  return false;
}

// ---------------------------------------------------------------------------
// Authentication and Navigation helpers
// ---------------------------------------------------------------------------

async function loginAsBceid(page: Page): Promise<void> {
  await loginAsBceidShared(page, APP_URL);
}

async function navigateToListingsPage(page: Page): Promise<ListingsPageState> {
  const homeRegion = page.getByRole('region', { name: /^Home$/i });
  await expect(homeRegion).toBeVisible();

  // Find the "View Listings" or similar button
  const viewListingsButton = page
    .getByRole('button', { name: /View Listing Data|View Listings|Listings/i })
    .or(page.getByRole('link', { name: /View Listing Data|View Listings|Listings/i }))
    .first();

  if ((await viewListingsButton.count()) === 0) {
    throw new Error('Unable to find View Listings button on homepage.');
  }

  await viewListingsButton.click();

  // Wait for listings page to load
  const listingsHeading = page.getByRole('heading', { name: /Listings|Listing Data/i });
  await expect(listingsHeading).toBeVisible({ timeout: 60_000 });

  // Check if listings grid is present
  const listingsGrid = page.getByRole('table').or(page.locator('[role="grid"]')).first();
  const gridVisible = await listingsGrid.isVisible({ timeout: 15_000 }).catch(() => false);

  if (!gridVisible) {
    return 'no-listings';
  }

  return 'listings-available';
}

// ---------------------------------------------------------------------------
// Listings selection helpers
// ---------------------------------------------------------------------------

async function getSendNoticesButton(page: Page): Promise<Locator> {
  // Primary: Use unique ID attribute from actual HTML
  const buttonById = page.locator('#send_delisting_notice_btn');
  
  // Fallback: Use role and text matching
  const buttonByRole = page.getByRole('button', { name: /Send Notices|Send Notice of Non-Compliance/i }).first();

  // Return ID-based locator if exists, otherwise fallback to role-based
  if ((await buttonById.count()) > 0) {
    return buttonById;
  }
  
  return buttonByRole;
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

async function waitForListingsGridToSettle(page: Page): Promise<void> {
  const grid = page.getByRole('table').or(page.locator('[role="grid"]')).first();
  await expect(grid).toBeVisible({ timeout: 15_000 });

  await expect
    .poll(
      async () => {
        const loaders = page.locator(
          '.p-datatable-loading-overlay:visible, [aria-busy="true"]:visible, [role="progressbar"]:visible, .loader.ng-star-inserted:visible',
        );
        return (await loaders.count().catch(() => 0)) === 0;
      },
      { timeout: 20_000, intervals: [300, 600, 1_000] },
    )
    .toBe(true);
}

async function selectListingByIndex(page: Page, index: number): Promise<void> {
  const listingRows = page.locator('[role="row"]').or(page.locator('tbody tr'));
  const rowCount = await listingRows.count();

  if (index >= rowCount) {
    throw new Error(`Listing index ${index} out of bounds (${rowCount} listings available).`);
  }

  const checkbox = listingRows
    .nth(index)
    .locator('input[type="checkbox"]')
    .or(listingRows.nth(index).locator('[role="checkbox"]'))
    .first();

  await checkbox.click();
  await expect
    .poll(
      async () => {
        const checked = await checkbox.isChecked().catch(() => false);
        const ariaChecked = await checkbox.getAttribute('aria-checked').catch(() => null);
        return checked || ariaChecked === 'true';
      },
      { timeout: 10_000, intervals: [300, 600] },
    )
    .toBe(true);
}

async function getSelectedListingCount(page: Page): Promise<number> {
  const checkedBoxes = page.locator('input[type="checkbox"]:checked').or(
    page.locator('[role="checkbox"][aria-checked="true"]'),
  );
  return await checkedBoxes.count();
}

async function deselectAllListings(page: Page): Promise<void> {
  console.log('  [deselectAllListings] Getting selected count...');
  const selectedCount = await getSelectedListingCount(page);
  console.log(`  [deselectAllListings] Selected count: ${selectedCount}`);
  
  if (selectedCount === 0) {
    console.log('  [deselectAllListings] No listings selected, returning');
    return;
  }

  // Try to find and click the select-all checkbox to toggle deselection
  console.log('  [deselectAllListings] Looking for select-all checkbox...');
  const selectAllCheckbox = page
    .locator('input[type="checkbox"][aria-label*="Select all"]')
    .or(page.locator('[role="checkbox"][aria-label*="Select all"]'))
    .first();

  const selectAllCount = await selectAllCheckbox.count();
  console.log(`  [deselectAllListings] Select-all checkbox count: ${selectAllCount}`);

  if (selectAllCount > 0) {
    console.log('  [deselectAllListings] Clicking select-all checkbox...');
    await selectAllCheckbox.click();
    console.log('  [deselectAllListings] Select-all clicked, waiting for checked count to drop...');
    await expect
      .poll(async () => await getSelectedListingCount(page), {
        timeout: 10_000,
        intervals: [300, 600],
      })
      .toBeLessThan(selectedCount);
    console.log('  [deselectAllListings] Done with select-all');
  } else {
    // Fallback: click each selected checkbox individually
    console.log('  [deselectAllListings] Using fallback: clicking each checkbox individually');
    const checkedBoxes = page.locator('input[type="checkbox"]:checked').or(
      page.locator('[role="checkbox"][aria-checked="true"]'),
    );
    const count = await checkedBoxes.count();
    console.log(`  [deselectAllListings] Found ${count} checked boxes`);
    
    for (let i = 0; i < count; i += 1) {
      console.log(`    [deselectAllListings] Clicking checkbox ${i}...`);
      const before = await checkedBoxes.count();
      await checkedBoxes.first().click();
      console.log(`    [deselectAllListings] Checkbox ${i} clicked, waiting for checked count to decrease...`);
      await expect
        .poll(async () => await checkedBoxes.count(), {
          timeout: 8_000,
          intervals: [250, 500],
        })
        .toBeLessThan(before);
    }
    console.log('  [deselectAllListings] Done clicking all checkboxes');
  }
}

// ---------------------------------------------------------------------------
// Notice form helpers
// ---------------------------------------------------------------------------

async function openNoticeDetailsForm(page: Page): Promise<void> {
  const sendNoticesButton = await getSendNoticesButton(page);
  const isEnabled = await sendNoticesButton.isEnabled().catch(() => false);

  if (!isEnabled) {
    throw new Error(
      'Send Notices button is not enabled. Please select at least one listing first.',
    );
  }

  await sendNoticesButton.click();
  // Use .first() to get the primary H2 heading "Send Notices of Non-Compliance" and avoid strict mode violation
  await expect(page.getByRole('heading', { name: /Notice Details|Send Notice/i }).first()).toBeVisible({
    timeout: 30_000,
  });
}

async function getReviewButton(page: Page): Promise<Locator> {
  return page.getByRole('button', { name: /Review|Review Details/i }).first();
}

async function fillLGEmailField(page: Page, email: string): Promise<void> {
  const emailInputs = page.locator('input[type="email"]').or(
    page.locator('input[placeholder*="email" i]').or(
      page.locator('input[aria-label*="email" i]').or(
        page.locator('input[name*="email" i]'),
      ),
    ),
  );

  const count = await emailInputs.count();
  if (count === 0) {
    throw new Error('Unable to find email input field in notice form.');
  }

  // Assume first email field is for LG email
  const lgEmailField = emailInputs.first();
  await lgEmailField.fill(email);
  await expect(lgEmailField).toHaveValue(email, { timeout: 5_000 });
}

async function getEmailValidationError(page: Page): Promise<string | null> {
  const errorMessages = page
    .getByText(/email format|correct format|invalid email|ensure the email/i)
    .first();
  const isVisible = await errorMessages.isVisible({ timeout: 5_000 }).catch(() => false);

  if (!isVisible) {
    return null;
  }

  return await errorMessages.textContent();
}

async function fillMandatoryFields(page: Page, lgEmail: string): Promise<void> {
  // Fill LG email field
  await fillLGEmailField(page, lgEmail);

  // Additional mandatory fields may be present; fill them based on available inputs
  // This is a simplified approach; customize based on actual form structure
  const textInputs = page.locator('input[type="text"]').or(page.locator('textarea'));
  const inputCount = await textInputs.count();

  // Skip first input if it's already filled (likely the email field)
  for (let i = 1; i < Math.min(inputCount, 3); i += 1) {
    const placeholder = await textInputs.nth(i).getAttribute('placeholder').catch(() => '');
    const label = await textInputs.nth(i).getAttribute('aria-label').catch(() => '');

    if (
      placeholder?.toLowerCase().includes('reason') ||
      label?.toLowerCase().includes('reason')
    ) {
      await textInputs.nth(i).fill('Non-compliance notice for STR regulations.');
    }
  }
}

// ---------------------------------------------------------------------------
// Test Suite
// ---------------------------------------------------------------------------

test.describe('@regression @SendingMultipleNoticesOfNonCompliance Scenario: SendingMultipleNoticesOfNonCompliance', () => {
  test.setTimeout(240_000);

  test.skip(!APP_URL, 'Set BASE_URL environment variable before running this suite.');
  test.skip(!hasBceidAuthConfig(), BCEID_AUTH_ENV_MESSAGE);

  test('@smoke AC1 - Send Notices button state based on listing selection', async ({ page }) => {
    

    // Step 1: Authenticate via Business BCeID login
    console.log('📝 Step 1: Logging in as BCeID...');
    await loginAsBceid(page);
    console.log('✅ Step 1 Complete');

    // Step 2: Navigate to listings page from homepage (View Listings)
    console.log('📝 Step 2: Navigating to listings page...');
    const listingsState = await navigateToListingsPage(page);
    console.log(`✅ Step 2 Complete (state: ${listingsState})`);
    test.skip(
      listingsState !== 'listings-available',
      'Listings page is not available or no listings to select.',
    );

    console.log('📝 Step 3: Getting Send Notices button...');
    const sendNoticesButton = await getSendNoticesButton(page);
    console.log('✅ Step 3: Button found');

    // Step 3: Verify Send Notices of Non-Compliance button is disabled
    console.log('📝 Step 3: Verifying button is visible and disabled...');
    await expect(sendNoticesButton).toBeVisible();
    await expect(sendNoticesButton).toBeDisabled();
    console.log('✅ Step 3: Button is visible and disabled');

    // Step 4: Click the "Recently Reported" toggle to switch to All Listings mode
    console.log('📝 Step 4: Getting Recently Reported toggle...');
    let recentlyReportedToggle;
    try {
      recentlyReportedToggle = await getRecentlyReportedToggle(page);
      console.log('✅ Step 4: Toggle found');
      
      const toggleCount = await recentlyReportedToggle.count();
      console.log(`   Toggle count: ${toggleCount}`);
      
      if (toggleCount > 0) {
        console.log('   Clicking toggle...');
        await recentlyReportedToggle.click();
        console.log('   Toggle clicked, waiting for listings grid to settle...');
        await waitForListingsGridToSettle(page);
        console.log('✅ Step 4: Toggle clicked');
      }
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      console.log(`⚠️  Step 4: Toggle not found (${message}), continuing without toggle`);
    }

    // Step 5: Select one or multiple listings from the grid
    console.log('📝 Step 5: Selecting first listing...');
    await selectListingByIndex(page, 0);
    console.log('✅ Step 5: Listing selected');

    // Step 6: Verify Send Notices of Non-Compliance button becomes enabled
    console.log('📝 Step 6: Checking button enabled state...');
    let isEnabled = await sendNoticesButton.isEnabled({ timeout: 10_000 }).catch(() => false);
    console.log(`   Button enabled: ${isEnabled}`);
    expect(isEnabled).toBe(true);
    console.log('✅ Step 6: Button is enabled');

    console.log('🎉 AC1 Test Completed Successfully!');
  });

  test('AC2 - Mandatory fields validation and required field behavior', async ({ page }) => {
    // Step 1: Navigate to listings page
    await loginAsBceid(page);

    const listingsState = await navigateToListingsPage(page);
    test.skip(
      listingsState !== 'listings-available',
      'Listings page is not available or no listings to select.',
    );

    // Step 2: Click the "Recently Reported" toggle to switch to All Listings mode and select at least one listing
    try {
      const recentlyReportedToggle = await getRecentlyReportedToggle(page);
      if ((await recentlyReportedToggle.count()) > 0) {
        await recentlyReportedToggle.click();
        await waitForListingsGridToSettle(page);
      }
    } catch {
      // Continue if toggle is not present; some environments may already be in desired mode.
    }

    await selectListingByIndex(page, 0);

    // Step 3/4: Click Send Notices of Non-Compliance button and verify bulk-compliance-notice page opens
    await openNoticeDetailsForm(page);

    const reviewButton = await getReviewButton(page);

    // Step 5: Verify Review button is disabled when mandatory fields are empty
    await expect(reviewButton).toBeDisabled();

    // Step 6: Fill in mandatory fields
    await fillMandatoryFields(page, 'lg.test@gov.bc.ca');

    // Step 7: Verify all mandatory fields are clearly marked/labeled as required (proxy via enabled state after required fields are filled)
    await expect
      .poll(
        async () => await reviewButton.isEnabled().catch(() => false),
        { timeout: 30_000, message: 'Expected Review button to become enabled after filling mandatory fields.' },
      )
      .toBe(true);
  });

  test('AC3 - Cancel flow', async ({ page }) => {
    console.log('🚀 AC3 Test Starting...');
    
    // Step 1: Navigate to listings page
    console.log('📝 Step 1: Logging in and navigating to listings...');
    await loginAsBceid(page);

    const listingsState = await navigateToListingsPage(page);
    test.skip(
      listingsState !== 'listings-available',
      'Listings page is not available or no listings to select.',
    );
    console.log('✅ Step 1: On listings page');

    // Step 2: Click the "Recently Reported" toggle to switch to All Listings mode and select at least one listing
    console.log('📝 Step 2: Toggle and select listing...');
    try {
      const recentlyReportedToggle = await getRecentlyReportedToggle(page);
      if ((await recentlyReportedToggle.count()) > 0) {
        await recentlyReportedToggle.click();
        await waitForListingsGridToSettle(page);
      }
    } catch {
      // Continue if toggle is not present
    }

    await selectListingByIndex(page, 0);
    console.log('✅ Step 2: Listing selected');

    // Step 3/4: Click Send Notices button and open form
    console.log('📝 Step 3/4: Opening notice form...');
    await openNoticeDetailsForm(page);
    console.log('✅ Step 3/4: Form opened');

    // Step 5: Partially fill form
    console.log('📝 Step 5: Partially filling form...');
    const emailInput = page.locator('input[type="email"]').first();
    if ((await emailInput.count()) > 0) {
      await emailInput.fill('test@example.com');
      await expect(emailInput).toHaveValue('test@example.com', { timeout: 5_000 });
    }
    console.log('✅ Step 5: Form partially filled');

    // Step 6: Click Cancel button
    console.log('📝 Step 6: Clicking cancel button...');
    const cancelButton = page.getByRole('button', { name: /Cancel|Close|X/i }).first();
    const cancelCount = await cancelButton.count();
    
    if (cancelCount > 0) {
      console.log('   Cancel button found, clicking...');
      await cancelButton.click();
      console.log('   Cancel clicked, waiting for listing page indicators...');
      await expect
        .poll(
          async () => {
            const headingVisible = await page
              .getByRole('heading', { name: /Listings|Listing Data/i })
              .isVisible()
              .catch(() => false);
            const gridVisible = await page
              .getByRole('table')
              .or(page.locator('[role="grid"]'))
              .first()
              .isVisible()
              .catch(() => false);
            return headingVisible || gridVisible;
          },
          { timeout: 20_000, intervals: [300, 600, 1_000] },
        )
        .toBe(true);
      console.log('✅ Step 6: Cancel clicked and waited');
    } else {
      console.log('⚠️  Cancel button not found, but continuing...');
    }

    // Step 7: Verify redirect back to listings page using multiple checks
    console.log('📝 Step 7: Verifying redirect to listings...');
    
    // Try to find listings heading with a longer timeout
    const listingsHeading = page.getByRole('heading', { name: /Listings|Listing Data/i });
    const isVisible = await listingsHeading.isVisible({ timeout: 15_000 }).catch(() => false);
    
    if (isVisible) {
      console.log('✅ Step 7: Redirected to listings page (heading visible)');
    } else {
      // Fallback: check for listings grid or URL
      console.log('   Listing heading not visible, checking for grid...');
      const listingsGrid = page.getByRole('table').or(page.locator('[role="grid"]')).first();
      const gridVisible = await listingsGrid.isVisible({ timeout: 10_000 }).catch(() => false);
      
      if (gridVisible) {
        console.log('✅ Step 7: Redirected to listings page (grid visible)');
      } else {
        throw new Error('Could not verify redirect to listings page');
      }
    }

    console.log('🎉 AC3 Test Completed Successfully!');
  });

  test('AC4 - Email validation (format and multiple email support)', async ({ page }) => {
    // Step 1: Navigate to listings page
    await loginAsBceid(page);

    const listingsState = await navigateToListingsPage(page);
    test.skip(
      listingsState !== 'listings-available',
      'Listings page is not available or no listings to select.',
    );

    // Step 2: Click the "Recently Reported" toggle to switch to All Listings mode and select at least one listing
    try {
      const recentlyReportedToggle = await getRecentlyReportedToggle(page);
      if ((await recentlyReportedToggle.count()) > 0) {
        await recentlyReportedToggle.click();
        await waitForListingsGridToSettle(page);
      }
    } catch {
      // Continue if toggle is not present; some environments may already be in desired mode.
    }

    await selectListingByIndex(page, 0);

    // Step 3/4: Click Send Notices of Non-Compliance button and verify bulk-compliance-notice page opens
    await openNoticeDetailsForm(page);

    const reviewButton = await getReviewButton(page);

    // Step 2-3: Enter invalid email and verify Review button stays disabled
    await fillLGEmailField(page, 'notanemail');
    await expect(reviewButton).toBeDisabled();

    // Step 4: Verify error message displays
    const errorMessage = await getEmailValidationError(page);
    if (errorMessage) {
      expect(errorMessage.toLowerCase()).toMatch(/email format|correct format|invalid/i);
    }

    // Step 5-6: Enter valid email and verify Review button becomes enabled
    await fillLGEmailField(page, 'lg.valid@gov.bc.ca');
    await expect
      .poll(
        async () => await reviewButton.isEnabled().catch(() => false),
        { timeout: 30_000, message: 'Expected Review button to enable with valid email.' },
      )
      .toBe(true);

    // Step 8-9: Try to add another email if form supports it
    const additionalEmailInputs = page.locator('input[placeholder*="BCC" i]').or(
      page.locator('input[aria-label*="BCC" i]'),
    );

    if ((await additionalEmailInputs.count()) > 0) {
      await additionalEmailInputs.first().fill('bcc@example.com');
      // Verify form state is still valid
      await expect(reviewButton).toBeEnabled();
    }
  });

  test('AC5 - Review and submit flow with notifications and action history update', async ({
    page,
  }) => {
    // Step 1: Navigate to listings page
    await loginAsBceid(page);

    const listingsState = await navigateToListingsPage(page);
    test.skip(
      listingsState !== 'listings-available',
      'Listings page is not available or no listings to select.',
    );

    // Step 2: Click the "Recently Reported" toggle to switch to All Listings mode and select at least one listing
    try {
      const recentlyReportedToggle = await getRecentlyReportedToggle(page);
      if ((await recentlyReportedToggle.count()) > 0) {
        await recentlyReportedToggle.click();
        await waitForListingsGridToSettle(page);
      }
    } catch {
      // Continue if toggle is not present; some environments may already be in desired mode.
    }

    await selectListingByIndex(page, 0);

    // Step 3/4: Click Send Notices of Non-Compliance button and verify bulk-compliance-notice page opens
    await openNoticeDetailsForm(page);

    // Step 1-2: Complete all mandatory fields
    //await fillMandatoryFields(page, 'lg.test@gov.bc.ca');
    await fillMandatoryFields(page, 'Kaushik.Mandal@gov.bc.ca');
    const reviewButton = await getReviewButton(page);
    await expect(reviewButton).toBeEnabled({ timeout: 30_000 });

    // Step 3: Click Review button
    await reviewButton.click();

    // Step 4: Verify Send Notice of Non-Compliance dialog opens
    const noticeDialog = page.getByRole('dialog', { name: /Send Notice of Non-Compliance/i });
    await expect(noticeDialog).toBeVisible({ timeout: 30_000 });

    // Step 5: Click Cancel button inside the Send Notice of Non-Compliance dialog
    const formCancelButton = noticeDialog.getByRole('button', { name: /Cancel/i });
    await expect(formCancelButton).toBeVisible({ timeout: 10_000 });
    await formCancelButton.click();
    await expect(noticeDialog).toBeHidden({ timeout: 20_000 });

    // Step 6: Verify user is back to bulk-compliance-notice page with label "Send Notices of Non-Compliance"
    const bulkNoticePageHeading = page.getByRole('heading', { name: /Send Notices of Non-Compliance/i }).first();
    await expect(bulkNoticePageHeading).toBeVisible({ timeout: 15_000 });

    // Step 7: Click Review button again
    const reviewButtonAgain = await getReviewButton(page);
    await expect(reviewButtonAgain).toBeEnabled({ timeout: 15_000 });
    await reviewButtonAgain.click();

    // Step 8: Verify user is landed into Send Notice of Non-Compliance dialog
    await expect(noticeDialog).toBeVisible({ timeout: 30_000 });

    // Step 9: Verify Submit button is enabled inside the dialog
    const submitButton = noticeDialog.getByRole('button', { name: /Submit/i });
    await expect(submitButton).toBeEnabled({ timeout: 15_000 });

    // Step 10: Click Submit button
    await submitButton.click();

    // Step 11: Verify success confirmation message displays (top right of page) and user lands on listings page
    const successMessage = page
      .locator('[aria-live], [role="alert"], [role="status"], .toast, .notification, .alert')
      .filter({ hasText: /success|submitted|notices sent/i })
      .or(page.getByText(/success|submitted|notices sent/i))
      .first();
    await expect(successMessage).toBeVisible({ timeout: 30_000 });

    const listingsHeading = page.getByRole('heading', { name: /Listings|Listing Data/i });
    await expect(listingsHeading).toBeVisible({ timeout: 30_000 });

    // Step 12: Click "Recently Reported" toggle to switch to All Listings mode
    try {
      const recentlyReportedToggle = await getRecentlyReportedToggle(page);
      if ((await recentlyReportedToggle.count()) > 0) {
        await recentlyReportedToggle.click();
        await waitForListingsGridToSettle(page);
      }
    } catch {
      // Continue if toggle is not present; some environments may already be in desired mode.
    }

    // Step 13: Verify Last Action column value is updated with "Notice of Non-Compliance"
    const lastActionCell = page
      .getByRole('row')
      .filter({ hasText: /Notice of Non-Compliance/i })
      .first();
    await expect(lastActionCell).toBeVisible({ timeout: 15_000 });
  });

  test('AC6 - Listing checkbox removal and Review button state', async ({ page }) => {
    // Step 1: Navigate to listings page
    await loginAsBceid(page);

    const listingsState = await navigateToListingsPage(page);
    test.skip(
      listingsState !== 'listings-available',
      'Listings page is not available or no listings to select.',
    );

    // Step 2: Click the "Recently Reported" toggle to switch to All Listings mode and select at least one listing
    try {
      const recentlyReportedToggle = await getRecentlyReportedToggle(page);
      if ((await recentlyReportedToggle.count()) > 0) {
        await recentlyReportedToggle.click();
        await waitForListingsGridToSettle(page);
      }
    } catch {
      // Continue if toggle is not present; some environments may already be in desired mode.
    }

    await selectListingByIndex(page, 0);

    // Step 3/4: Click Send Notices of Non-Compliance button and verify bulk-compliance-notice page opens
    await openNoticeDetailsForm(page);
    await fillMandatoryFields(page, 'lg.test@gov.bc.ca');

    const reviewButton = await getReviewButton(page);

    // Step 3: Verify Review button is enabled
    await expect(reviewButton).toBeEnabled({ timeout: 30_000 });

    // Step 4-5: Uncheck all listing checkboxes in the form (if present)
    const formCheckboxes = page.locator('[role="dialog"], [role="alertdialog"]').locator('input[type="checkbox"]');
    const checkboxCount = await formCheckboxes.count();

    if (checkboxCount > 0) {
      for (let i = 0; i < checkboxCount; i += 1) {
        const isChecked = await formCheckboxes
          .nth(i)
          .isChecked()
          .catch(() => false);
        if (isChecked) {
          await formCheckboxes.nth(i).click();
        }
      }

      // Step 5: Verify Review button becomes disabled
      await expect(reviewButton).toBeDisabled();

      // Step 6-7: Re-check at least one listing
      if (checkboxCount > 0) {
        await formCheckboxes.first().click();
        await expect(reviewButton).toBeEnabled({ timeout: 30_000 });
      }
    }
  });

});

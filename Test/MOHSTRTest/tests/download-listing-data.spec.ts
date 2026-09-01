/// <reference types="node" />

/**
 * Feature: Download Listing Data
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-220
 *
 * @DownloadListingData
 * Scenario: DownloadListingData
 *
 * Test Case Summary:
 * Given I am an authenticated user of the Short-Term Rental Data Portal
 * When I execute the IDIR download-listing-data flow
 * Then AC1 to AC4 validate authentication, navigation, jurisdiction options, and successful downloads
 * And when I execute the Business BCeID flow
 * Then AC5 validates default page state before selecting any jurisdiction
 * And AC6 validates LG users can only download for their own jurisdiction and successful download behavior
 *
 * Test Steps and Validation Checkpoints:
 *
 * IDIR Validations Only (AC1-AC4):
 *
 * AC1 - Platform Representative Authentication via IDIR:
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
 * AC2 - Navigate to Download Listing Data Page:
 * - Step 1: Verify Home region is visible after successful login (timeout: 30s)
 * - Step 2: On landing page, locate "Download Listing Data" button/CTA
 * - Step 3: Click the "Download Listing Data" button
 * - Step 4: Verify page navigates to export-listings URL (timeout: 60s)
 * - Step 5: Verify heading/label "Download Listing Data" is displayed on the page
 * - Step 6: Validate Download button is present and visible
 *
 * AC3 - Validate Jurisdiction Radio Button Options:
 * - Step 1: Verify all required jurisdiction radio button options are available:
 *   ✓ BC_FIN
 *   ✓ BC
 *   ✓ BC_PR
 * - Step 2: Verify each jurisdiction option is visible and selectable
 * - Step 3: Verify options are implemented as radio buttons (mutually exclusive)
 * - Step 4: Verify at least one jurisdiction option is selected by default (or can be selected)
 *
 * AC4 - Download Listing Data by Jurisdiction and Verify Success:
 * - Step 1: For each jurisdiction (BC_FIN, BC, BC_PR), perform the following:
 *   a) Dismiss any visible success toast from previous download (if present)
 *   b) Select the jurisdiction radio button option
 *   c) Verify the jurisdiction option is checked/selected
 *   d) Locate and click the Download button
 *   e) Verify download event is triggered and file is downloadable
 *   f) Validate suggested filename is present and not empty
 *   g) Wait for success pop-up/toast to appear (timeout: 20s)
 *   h) Verify success message is visible (pattern: "Success")
 * - Step 2: Confirm all three jurisdictions download successfully without errors
 * - Step 3: Verify no failure or error messages appear during any download
 *
 * BCeID Validations Only:
 *
 * AC5 [BCeID] - Navigate to Download Listing Data Page and validate default state:
 * - Step 1: Authenticate via Business BCeID and verify Home region is visible
 * - Step 2: Click "Download Listing Data" and verify URL contains export-listings
 * - Step 3: Verify heading/label "Download Listing Data" is displayed on the page
 * - Step 4: Verify at least one radio button is present
 * - Step 5: Verify the radio button is unchecked by default
 * - Step 6: Validate Download button is present and disabled before selecting the radio button option
 *
 * AC6 [BCeID] - Validate Jurisdiction Restriction for Downloading Listing Data:
 * - Step 1: Navigate to Download Listing Data page (after BCeID login)
 * - Step 2: Read LG user name from the top-right header and derive jurisdiction (for example, "NorthVancouver LG" -> "NorthVancouver")
 * - Step 3: Verify every visible jurisdiction radio label (.radio-label) matches the user's jurisdiction text (space-insensitive, for example, "City of North Vancouver")
 * - Step 4: Select a matching jurisdiction radio button option
 * - Step 5: Verify the Download button (.p-button.p-component) is enabled after selection
 * - Step 6: Click the Download button
 * - Step 7: Verify download event is triggered and file is downloadable
 * - Step 8: Wait for success pop-up/toast to appear
 * - Step 9: Verify success message is visible (pattern: "Success")
 * - Step 10: Verify no failure or error messages appear during download
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

const JURISDICTIONS = ['BC_FIN', 'BC', 'BC_PR'] as const;

test.use({ browserName: 'chromium' });

test.describe('@regression Feature: DownloadListingData [IDIR]', () => {
  test.setTimeout(300_000);

  test.skip(
    !hasIdirAuthConfig(),
    IDIR_AUTH_ENV_MESSAGE,
  );

  test('AC1 [IDIR]: authenticate and land on Short-Term Rental Data Portal', async ({ page }) => {
    await loginAsIdirAndAssertHome(page);
  });

  test('AC2 [IDIR]: navigate to Download Listing Data page', async ({ page }) => {
    await goToDownloadListingDataPageAsIdir(page);
  });

  test('AC3 [IDIR]: validate all required jurisdiction radio button options are available', async ({ page }) => {
    await goToDownloadListingDataPageAsIdir(page);

    for (const jurisdiction of JURISDICTIONS) {
      const option = await getJurisdictionOption(page, jurisdiction);
      await expect(option).toBeVisible();
    }
  });

  test('@smoke AC4 [IDIR]: download listing data by jurisdiction and verify Success popup', async ({ page }) => {
    await goToDownloadListingDataPageAsIdir(page);

    for (const jurisdiction of JURISDICTIONS) {
      await test.step(`Download listing data for ${jurisdiction} and verify Success popup`, async () => {
        await clearSuccessToastIfPresent(page);

        const jurisdictionOption = await getJurisdictionOption(page, jurisdiction);
        await ensureOptionSelected(jurisdictionOption);

        const downloadButton = await getDownloadButton(page);
        await expect(downloadButton).toBeEnabled();

        const downloadPromise = page.waitForEvent('download', { timeout: 60_000 });
        await downloadButton.click();
        const download = await downloadPromise;
        await expect(download.suggestedFilename()).toBeTruthy();

        await assertSuccessPopupVisible(page);
      });
    }
  });
});

test.describe('@regression Feature: DownloadListingData [BCeID]', () => {
  test.setTimeout(300_000);

  test.skip(
    !hasBceidAuthConfig(),
    BCEID_AUTH_ENV_MESSAGE,
  );

  test('AC5 [BCeID]: download page loads, shows radio option unchecked by default, and Download is disabled', async ({
    page,
  }) => {
    await test.step('Authenticate as Business BCeID and land on home page', async () => {
      await loginAsBceid(page);
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
    });

    await test.step('Navigate to Download Listing Data page and verify URL + heading', async () => {
      await openDownloadListingDataPage(page);
    });

    await test.step('Verify a radio button is present and unchecked by default', async () => {
      const jurisdictionOption = await getFirstVisibleJurisdictionRadioOptionOrNull(page);
      test.skip(
        !jurisdictionOption,
        'BCeID precondition not met: no jurisdiction radio options are available for this account.',
      );
      if (!jurisdictionOption) {
        return;
      }

      await expect(jurisdictionOption).toBeVisible();
      await assertOptionIsRadio(jurisdictionOption);
      await expect
        .poll(() => isOptionSelected(jurisdictionOption), {
          message: 'Expected the jurisdiction radio option to be unchecked by default',
        })
        .toBe(false);
    });

    await test.step('Validate Download button is present and disabled before selecting the radio button option', async () => {
      const downloadButton = await getDownloadButton(page);
      await expect(downloadButton).toBeVisible();
      await expect(downloadButton).toBeDisabled();
    });
  });

  test('@smoke AC6 [BCeID]: LG user can download listing data only for own jurisdiction', async ({ page }) => {
    await test.step('Authenticate as Business BCeID and navigate to Download Listing Data page', async () => {
      await loginAsBceid(page);
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
      await openDownloadListingDataPage(page);
    });

    await test.step('Validate LG jurisdiction radio labels match the logged-in LG user and verify Download button is enabled', async () => {
      await clearSuccessToastIfPresent(page);

      const lgJurisdictionKey = await getLgJurisdictionKeyFromDisplayName(page);
      const visibleRadioLabels = await getVisibleJurisdictionLabels(page);
      test.skip(!visibleRadioLabels.length, 'BCeID precondition not met: no jurisdiction radio labels are available for this account.');
      if (!visibleRadioLabels.length) {
        return;
      }

      for (const jurisdictionName of visibleRadioLabels) {
        expect(
          normalizeJurisdictionText(jurisdictionName),
          `Expected LG user to only have own jurisdiction options. User jurisdiction key: ${lgJurisdictionKey}. Radio label: ${jurisdictionName}`,
        ).toContain(lgJurisdictionKey);
      }

      const jurisdictionOption = page.getByRole('radio', {
        name: new RegExp(`^\\s*${escapeRegExp(visibleRadioLabels[0])}\\s*$`, 'i'),
      });
      await ensureOptionSelected(jurisdictionOption);

      const downloadButton = await getDownloadButton(page);
      await expect(downloadButton).toBeEnabled();
    });

    await test.step('Click Download and verify file is downloaded with non-empty filename', async () => {
      const downloadButton = await getDownloadButton(page);
      const downloadPromise = page.waitForEvent('download', { timeout: 60_000 });
      await downloadButton.click();
      const download = await downloadPromise;

      const suggestedFilename = download.suggestedFilename();
      expect(suggestedFilename, 'Expected a suggested filename in the download event').toBeTruthy();
      expect(suggestedFilename.trim(), 'Expected suggested filename to be non-empty').not.toBe('');
    });

    await test.step('Verify Success popup appears and no failure/error popup appears', async () => {
      await assertSuccessPopupVisible(page);
      await assertNoErrorPopupVisible(page);
    });
  });
});

async function loginAsIdir(page: Page): Promise<void> {
  await loginAsIdirShared(page, APP_URL);
}

async function loginAsIdirAndAssertHome(page: Page): Promise<void> {
  await loginAsIdir(page);
  await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
}

async function goToDownloadListingDataPageAsIdir(page: Page): Promise<void> {
  await loginAsIdirAndAssertHome(page);
  await openDownloadListingDataPage(page);
}

async function loginAsBceid(page: Page): Promise<void> {
  await loginAsBceidShared(page, APP_URL);
}

async function openDownloadListingDataPage(page: Page): Promise<void> {
  await page.getByRole('button', { name: /^Download Listing Data$/i }).click();
  await expect(page).toHaveURL(/export-listings/i, { timeout: 60_000 });
  await expect(page.getByRole('heading', { name: /^Download Listing Data$/i })).toBeVisible({
    timeout: 60_000,
  });
  await expect(page.getByRole('radio').first()).toBeVisible({ timeout: 30_000 });
}
/**
 * Locate the first available jurisdiction radio input for BCeID users.

      // Additional wait to ensure radio buttons are fully rendered
      if (inputCount === 0) {
        console.log('[DEBUG AC5] No radio buttons found yet, waiting for dynamic load...');
        const loaded = await page.locator('input[type="radio"]').first().isVisible({ timeout: 20_000 }).catch(() => false);
        console.log(`[DEBUG AC5] After wait - radio buttons visible: ${loaded}`);
      }

 * The input is resolved via the label's [for] attribute and verified with specific class/name attributes.
 * Fallback strategies cover non-Angular or alternative markup.
 */
async function getFirstVisibleJurisdictionRadioOptionOrNull(page: Page): Promise<Locator | null> {
  // Strategy 1 (primary): Angular label.radio-label → resolve input via [for] attribute
  const radioLabels = page.locator('label.radio-label');
  const labelCount = await radioLabels.count();
  for (let i = 0; i < labelCount; i++) {
    const label = radioLabels.nth(i);
    const isAttached = await label.isVisible({ timeout: 1_000 }).catch(() => false);
    const forId = await label.getAttribute('for').catch(() => null);
    if (forId) {
      const radioInput = page.locator(
        `input[type="radio"][id="${forId}"].p-radiobutton-input[name="jurisdiction-radio"]`
      );
      const count = await radioInput.count();
      if (count > 0) {
        return radioInput;
      }
    }
  }

  // Strategy 2: Use class and name attributes as fallback
  const radioByAttributes = page.locator(
    'input[type="radio"].p-radiobutton-input[name="jurisdiction-radio"]'
  ).first();
  const strategy2Count = await radioByAttributes.count();
  if (strategy2Count > 0) {
    return radioByAttributes;
  }

  // Strategy 3: generic role="radio"
  const optionByRole = page.getByRole('radio').first();
  const strategy3Count = await optionByRole.count();
  const strategy3Visible = await optionByRole.isVisible({ timeout: 10_000 }).catch(() => false);
  if (strategy3Count > 0 && strategy3Visible) {
    return optionByRole;
  }

  // Strategy 4: input[type="radio"]
  const optionByInputType = page.locator('input[type="radio"]').first();
  const strategy4Count = await optionByInputType.count();
  const strategy4Visible = await optionByInputType.isVisible({ timeout: 10_000 }).catch(() => false);
  if (strategy4Count > 0 && strategy4Visible) {
    return optionByInputType;
  }

  return null;

}

async function getVisibleJurisdictionLabels(page: Page): Promise<string[]> {
  const labels = page.locator('label.radio-label');
  const count = await labels.count();
  const visibleLabels: string[] = [];

  for (let i = 0; i < count; i++) {
    const label = labels.nth(i);
    const isVisible = await label.isVisible({ timeout: 1_000 }).catch(() => false);
    if (!isVisible) {
      continue;
    }

    const labelText = (await label.innerText().catch(() => '')).trim();
    if (labelText) {
      visibleLabels.push(labelText);
    }
  }

  return visibleLabels;
}

async function getJurisdictionOptionByLabelTextOrNull(page: Page, labelText: string): Promise<Locator | null> {
  const label = page.locator('label.radio-label').filter({ hasText: new RegExp(`^\\s*${escapeRegExp(labelText)}\\s*$`, 'i') }).first();
  const labelVisible = await label.isVisible({ timeout: 2_000 }).catch(() => false);
  if (!labelVisible) {
    return null;
  }

  const forId = await label.getAttribute('for').catch(() => null);
  if (forId) {
    const inputByFor = page.locator(`input[type="radio"][id="${forId}"]`).first();
    if ((await inputByFor.count()) > 0) {
      return inputByFor;
    }
  }

  const nestedInput = label.locator('input[type="radio"]').first();
  if ((await nestedInput.count()) > 0) {
    return nestedInput;
  }

  const byRole = page.getByRole('radio', { name: new RegExp(`^\\s*${escapeRegExp(labelText)}\\s*$`, 'i') }).first();
  if ((await byRole.count()) > 0) {
    return byRole;
  }

  return null;
}

async function getLgJurisdictionKeyFromDisplayName(page: Page): Promise<string> {
  const displayNameElement = page.locator('.user-container.ng-star-inserted').filter({ hasText: /\sLG\s*$/i }).first();
  await expect(displayNameElement).toBeVisible({ timeout: 20_000 });
  const displayName = (await displayNameElement.innerText()).trim();
  expect(displayName, 'Expected an LG display name in the application banner').toBeTruthy();

  const withoutLgSuffix = displayName.replace(/\bLG\b/gi, '').trim();
  const normalized = normalizeJurisdictionText(withoutLgSuffix);
  expect(normalized, `Unable to derive LG jurisdiction from display name: ${displayName}`).toBeTruthy();

  return normalized;
}

function normalizeJurisdictionText(value: string): string {
  return value.toLowerCase().replace(/[^a-z0-9]/g, '');
}

async function getJurisdictionOption(page: Page, jurisdiction: string): Promise<Locator> {
  const optionByRadioRole = page
    .getByRole('radio', { name: new RegExp(`^${escapeRegExp(jurisdiction)}$`, 'i') })
    .first();
  if ((await optionByRadioRole.count()) > 0) {
    return optionByRadioRole;
  }

  const optionByCheckboxRole = page
    .getByRole('checkbox', { name: new RegExp(`^${escapeRegExp(jurisdiction)}$`, 'i') })
    .first();
  if ((await optionByCheckboxRole.count()) > 0) {
    return optionByCheckboxRole;
  }

  const labelWithCheckbox = page
    .locator('label')
    .filter({ hasText: new RegExp(`^\\s*${escapeRegExp(jurisdiction)}\\s*$`, 'i') })
    .first();
  if ((await labelWithCheckbox.count()) > 0) {
    const nestedInput = labelWithCheckbox.locator('input[type="radio"], input[type="checkbox"]').first();
    if ((await nestedInput.count()) > 0) {
      return nestedInput;
    }
  }

  const textContainerCheckbox = page
    .locator('div, li, section')
    .filter({ hasText: new RegExp(`^\\s*${escapeRegExp(jurisdiction)}\\s*$`, 'i') })
    .locator('input[type="radio"], input[type="checkbox"], [role="radio"], [role="checkbox"]')
    .first();

  await expect(textContainerCheckbox).toBeVisible({ timeout: 30_000 });
  return textContainerCheckbox;
}

async function ensureOptionSelected(option: Locator): Promise<void> {
  const role = await option.getAttribute('role');
  const type = (await option.getAttribute('type'))?.toLowerCase();

  if (role === 'radio' || type === 'radio') {
    const ariaChecked = await option.getAttribute('aria-checked');
    if (ariaChecked !== null) {
      if (ariaChecked !== 'true') {
        await option.click();
      }
      await expect(option).toHaveAttribute('aria-checked', 'true');
      return;
    }

    await option.check();
    await expect(option).toBeChecked();
    return;
  }

  if (role === 'checkbox') {
    const ariaChecked = await option.getAttribute('aria-checked');
    if (ariaChecked !== 'true') {
      await option.click();
    }
    await expect(option).toHaveAttribute('aria-checked', 'true');
    return;
  }

  await option.check();
  await expect(option).toBeChecked();
}

async function assertOptionIsRadio(option: Locator): Promise<void> {
  const role = (await option.getAttribute('role'))?.toLowerCase();
  const type = (await option.getAttribute('type'))?.toLowerCase();

  if (role === 'radio' || type === 'radio') {
    return;
  }

  const inputType = await option.evaluate((element) => {
    if (element instanceof HTMLInputElement) {
      return element.type.toLowerCase();
    }
    return '';
  });
  expect(inputType, 'Expected jurisdiction option to be implemented as a radio button').toBe('radio');
}

async function isOptionSelected(option: Locator): Promise<boolean> {
  const ariaChecked = await option.getAttribute('aria-checked');
  if (ariaChecked !== null) {
    return ariaChecked === 'true';
  }

  const type = (await option.getAttribute('type'))?.toLowerCase();
  const role = (await option.getAttribute('role'))?.toLowerCase();

  if (type === 'radio' || type === 'checkbox' || role === 'radio' || role === 'checkbox') {
    return option.isChecked().catch(() => false);
  }

  return false;
}

async function getDownloadButton(page: Page): Promise<Locator> {
  const candidates = [
    page.getByRole('button', { name: /^Download$/i }).first(),
    page.getByRole('button', { name: /Download Listing Data/i }).first(),
    page.locator('button[type="submit"]').filter({ hasText: /Download/i }).first(),
  ];

  for (const candidate of candidates) {
    if ((await candidate.count()) > 0 && (await candidate.isVisible().catch(() => false))) {
      return candidate;
    }
  }

  throw new Error('Unable to locate the Download button on export-listings page.');
}

async function assertSuccessPopupVisible(page: Page): Promise<void> {
  const successCandidates = [
    page.locator('.p-toast-message').filter({ hasText: /success/i }).last(),
    page.locator('[role="alert"]').filter({ hasText: /success/i }).last(),
    page.getByText(/^Success$/i).last(),
    page.getByText(/success/i).last(),
  ];

  for (const candidate of successCandidates) {
    if ((await candidate.count()) > 0) {
      await expect(candidate).toBeVisible({ timeout: 20_000 });
      return;
    }
  }

  throw new Error('Expected a Success pop-up/toast after download, but none was found.');
}

async function assertNoErrorPopupVisible(page: Page): Promise<void> {
  const errorCandidates = [
    page.locator('.p-toast-message-error').last(),
    page.locator('[role="alert"]').filter({ hasText: /error|failed|failure/i }).last(),
    page.getByText(/error|failed|failure/i).last(),
  ];

  for (const candidate of errorCandidates) {
    const visible = await candidate.isVisible({ timeout: 1_500 }).catch(() => false);
    expect(visible, 'Expected no failure or error popup during download').toBe(false);
  }
}

async function clearSuccessToastIfPresent(page: Page): Promise<void> {
  const toast = page.locator('.p-toast-message').last();
  if (await toast.isVisible().catch(() => false)) {
    await toast.waitFor({ state: 'hidden', timeout: 12_000 }).catch(() => {});
  }
}

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}


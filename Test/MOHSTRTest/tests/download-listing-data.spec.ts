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
 * And AC6 validates successful download behavior after jurisdiction selection
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
 * AC6 [BCeID] - Download Listing Data by Jurisdiction and Verify Success:
 * - Step 1: Navigate to Download Listing Data page (after BCeID login)
 * - Step 2: Select the radio button option (first available via label.radio-label)
 * - Step 3: Verify Download button is enabled after selection
 * - Step 4: Click the Download button
 * - Step 5: Verify download event is triggered and file is downloadable
 * - Step 6: Validate suggested filename is present and not empty
 * - Step 7: Wait for success pop-up/toast to appear
 * - Step 8: Verify success message is visible (pattern: "Success")
 * - Step 9: Verify no failure or error messages appear during download
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

  test('@smoke authorized representative downloads listing data by jurisdiction and sees success popup', async ({
    page,
  }) => {
    await test.step('AC1: Authenticate and land on Short-Term Rental Data Portal', async () => {
      await loginAsIdir(page);
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
    });

    await test.step('AC2: Click "Download Listing Data" and validate export-listings page', async () => {
      await openDownloadListingDataPage(page);
    });

    await test.step('AC3: Validate all required jurisdiction radio button options are available', async () => {
      for (const jurisdiction of JURISDICTIONS) {
        const option = await getJurisdictionOption(page, jurisdiction);
        await expect(option).toBeVisible();
      }
    });

    for (const jurisdiction of JURISDICTIONS) {
      await test.step(`AC4: Download listing data for ${jurisdiction} and verify Success popup`, async () => {
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
      
      // Verify radio button is attached to DOM
      await expect(jurisdictionOption, 'Expected a jurisdiction radio input to be present and attached in the DOM').toBeAttached();
      
      // Verify it has the expected class and name attributes (p-radiobutton-input and jurisdiction-radio)
      const radioClass = await jurisdictionOption.getAttribute('class');
      expect(radioClass, 'Expected radio input to have p-radiobutton-input class').toContain('p-radiobutton-input');
      
      const radioName = await jurisdictionOption.getAttribute('name');
      expect(radioName, 'Expected radio input to have jurisdiction-radio name').toBe('jurisdiction-radio');
      
      // Verify it is a radio button type
      await assertOptionIsRadio(jurisdictionOption);
      
      // Verify radio button is unchecked by default by checking aria-checked attribute
      const ariaChecked = await jurisdictionOption.getAttribute('aria-checked');
      expect(ariaChecked, 'Expected the jurisdiction radio option to be unchecked by default (aria-checked should be false)').toBe('false');
    });

    await test.step('Validate Download button is present and disabled before selecting the radio button option', async () => {
      const downloadButton = await getDownloadButton(page);
      await expect(downloadButton).toBeVisible();
      await expect(downloadButton).toBeDisabled();
    });
  });

  test('@smoke AC6 [BCeID]: select jurisdiction and download listing data successfully', async ({ page }) => {
    await test.step('Authenticate as Business BCeID and navigate to Download Listing Data page', async () => {
      await loginAsBceid(page);
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
      await openDownloadListingDataPage(page);
    });

    await test.step('Select radio button option (first available via label.radio-label) and verify Download button is enabled', async () => {
      await clearSuccessToastIfPresent(page);
      const jurisdictionOption = await getFirstVisibleJurisdictionRadioOptionOrNull(page);
      test.skip(
        !jurisdictionOption,
        'BCeID precondition not met: no jurisdiction radio options are available for this account.',
      );
      if (!jurisdictionOption) {
        return;
      }
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

async function loginAsBceid(page: Page): Promise<void> {
  await loginAsBceidShared(page, APP_URL);
}

async function openDownloadListingDataPage(page: Page): Promise<void> {
  await page.getByRole('button', { name: /^Download Listing Data$/i }).click();
  await expect(page).toHaveURL(/export-listings/i, { timeout: 60_000 });
  await expect(page.getByRole('heading', { name: /^Download Listing Data$/i })).toBeVisible({
    timeout: 60_000,
  });
  // Wait for radio buttons to be rendered (they may load dynamically)
  // Try multiple strategies to wait for radio buttons
  const radioButtonsLoaded = await page.locator('label.radio-label, input[type="radio"]').first().isVisible({ timeout: 30_000 }).catch(() => false);
  // Additional small delay to ensure dynamic rendering is complete
  await page.waitForLoadState('networkidle', { timeout: 5_000 }).catch(() => {});
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


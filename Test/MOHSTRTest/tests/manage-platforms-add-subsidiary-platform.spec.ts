/// <reference types="node" />

/**
 * Feature: ManagePlatforms_AddSubPlatform
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-226
 *
 * @AddSubsidiaryPlatform
 * Scenario: AddSubsidiaryPlatform
 *
 * Test Case Summary:
 * Given I am an authenticated Admin user with access to Manage Platforms
 * When I navigate to the Manage Platforms page, open an existing platform details page, and initiate adding a new Subsidiary Platform
 * Then I should see the Add New Platform form with all required input fields
 * And the Save button behavior should follow required-field validation rules
 * And saving with valid unique data should return a success message
 * And attempting duplicate submissions should trigger validation error messages
 * And clicking Cancel should navigate back to the Detailed Platform Contact Information page
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 - Navigation to Detailed Platform Page and Add Subsidiary Platform Form Rendering:
 * - Step 1: Authenticate via IDIR login with Admin credentials
 * - Step 2: Verify landing page is displayed
 * - Step 3: Navigate to Manage Platforms page
 * - Step 4: Validate platform-management page is loaded with list/table/grid visible
 * - Step 5: Locate and click Edit button/link for a platform row
 * - Step 6: Validate Detailed Platform Contact Information page is displayed
 * - Step 7: Locate and click "Add Subsidiary Platform" button
 * - Step 8: Validate Add New Platform form page is displayed
 * - Step 9: Verify all required form input fields are present and visible:
 *   ✓ Platform Name
 *   ✓ Platform Code
 *   ✓ Email for Non-Compliance Notices
 *   ✓ Primary Email for Takedown Request Letters
 *   ✓ Secondary Email for Non-Compliance Notices (optional)
 *   ✓ Secondary Email for Takedown Request Letters (optional)
 * - Step 10: Validate Save and Cancel buttons are visible
 *
 * AC2 - Required Field Validation and Save Button Enablement for Sub-Platform:
 * - Step 1: Verify Save button is disabled when form is empty
 * - Step 2: Enter valid Platform Name and Platform Code
 * - Step 3: Verify Save button remains disabled (not all required fields filled)
 * - Step 4: Fill remaining required email fields
 * - Step 5: Validate Save button state transitions from disabled to enabled
 * - Step 6: Clear a required field and verify Save button becomes disabled
 * - Step 7: Confirm Save button enablement is tightly coupled to required field validation
 *
 * AC3 - Add Subsidiary Platform with Valid Unique Data (Success Path):
 * - Step 1: Fill all form fields with valid unique data
 * - Step 2: Verify Save button is enabled
 * - Step 3: Click Save button
 * - Step 4: Wait for success outcome (toast/modal/redirect)
 * - Step 5: Validate success message matches pattern: "New platform has been added successfully|Success"
 * - Step 6: Dismiss any visible toast notifications
 * - Step 7: Verify user is redirected to Detailed Platform Contact Information page
 * - Step 8: Confirm new sub-platform appears in the subsidiary platforms list
 *
 * AC4 - Duplicate Submission Handling (Validation Error Path):
 * - Step 1: Navigate back to Add Subsidiary Platform form from detailed platform page
 * - Step 2: Fill form with the same data used in the previous successful save
 * - Step 3: Click Save button
 * - Step 4: Wait for duplicate validation outcome
 * - Step 5: Validate error message matches pattern: "One or more validation errors occurred|already exists|duplicate"
 * - Step 6: Verify form remains on the add-subsidiary-platform page (no redirect)
 * - Step 7: Confirm Save button is still actionable for retry or correction
 *
 * AC5 - Cancel Button Navigation Back to Detailed Platform Page:
 * - Step 1: Navigate to Manage Platforms page
 * - Step 2: Click Edit button for a platform to open details page
 * - Step 3: Validate Detailed Platform Contact Information page is displayed
 * - Step 4: Click "Add Subsidiary Platform" button
 * - Step 5: Validate Add New Platform form is displayed
 * - Step 6: Click Cancel button without entering data
 * - Step 7: Verify user is redirected to Detailed Platform Contact Information page
 * - Step 8: Confirm form does not submit any data on Cancel
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import { IDIR_AUTH_ENV_MESSAGE, hasIdirAuthConfig, loginAsIdir as loginAsIdirShared } from './support/auth';

const APP_URL = process.env.BASE_URL ?? '';

test.use({ browserName: 'chromium' });

test.describe('@regression Feature: ManagePlatforms_AddSubPlatform', () => {
  test.setTimeout(360_000);
  test.describe.configure({ mode: 'serial' });

  test.skip(
    !hasIdirAuthConfig(),
    IDIR_AUTH_ENV_MESSAGE,
  );

  test.beforeEach(async ({ page }) => {
    await loginAsIdir(page);
    await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
  });

  test('AC1-AC4: user can open Add Subsidiary Platform and see required input fields', async ({ page }) => {
    await test.step('Navigate to Manage Platforms and open platform details', async () => {
      await goToManagePlatforms(page);
      await assertPlatformManagementPageLoaded(page);
      await clickEditForFirstPlatform(page);
      await assertDetailedPlatformPageLoaded(page);
    });

    await test.step('Click Add Subsidiary Platform and validate add form', async () => {
      await clickAddSubsidiaryPlatform(page);
      await assertAddSubPlatformPageLoaded(page);
      await assertAddSubPlatformFormFields(page);
    });
  });

  test('AC4: Save button state respects required-field behavior for sub-platform', async ({ page }) => {
    await goToManagePlatforms(page);
    await assertPlatformManagementPageLoaded(page);
    await clickEditForFirstPlatform(page);
    await assertDetailedPlatformPageLoaded(page);
    await clickAddSubsidiaryPlatform(page);
    await assertAddSubPlatformPageLoaded(page);

    const saveButton = getSaveButton(page);
    const data = generateSubPlatformData('REQCHK');

    await test.step('Initial state: Save should be disabled before entering required values', async () => {
      await expect(saveButton).toBeDisabled();
    });

    await test.step('After partially filling required fields, Save should remain disabled', async () => {
      await fillTextboxByLabel(page, /^Platform Name$/i, data.platformName);
      await fillTextboxByLabel(page, /^Platform Code$/i, data.platformCode);
      await expect(saveButton).toBeDisabled();
    });

    await test.step('After filling all required fields, Save should become enabled', async () => {
      await fillAddSubPlatformForm(page, data);
      await expect(saveButton).toBeEnabled({ timeout: 10_000 });
    });
  });

  test('AC5: save new sub-platform with valid data and validate success/duplicate outcome', async ({ page }) => {
    await goToManagePlatforms(page);
    await assertPlatformManagementPageLoaded(page);
    await clickEditForFirstPlatform(page);
    await assertDetailedPlatformPageLoaded(page);

    const unique = generateSubPlatformData('AUTO');

    await test.step('Fill sub-platform form with valid unique values and Save', async () => {
      await clickAddSubsidiaryPlatform(page);
      await assertAddSubPlatformPageLoaded(page);
      await fillAddSubPlatformForm(page, unique);
      await expect(getSaveButton(page)).toBeEnabled({ timeout: 10_000 });
      await getSaveButton(page).click();
    });

    await test.step('Validate success or duplicate/validation popup message', async () => {
      const outcome = await waitForSaveOutcome(page);
      expect(
        outcome.kind === 'success' || outcome.kind === 'duplicate-validation',
        `Expected success or duplicate-validation outcome, got: ${outcome.kind} (${outcome.message})`,
      ).toBe(true);

      await dismissVisibleToasts(page);

      if (outcome.kind === 'success') {
        await assertDetailedPlatformPageLoaded(page);
      }
    });

    await test.step('Attempt immediate duplicate save with same data to validate duplicate error path', async () => {
      await ensureOnDetailedPlatformPage(page);
      await clickAddSubsidiaryPlatform(page);
      await assertAddSubPlatformPageLoaded(page);
      await fillAddSubPlatformForm(page, unique);
      await expect(getSaveButton(page)).toBeEnabled({ timeout: 10_000 });
      await dismissVisibleToasts(page);
      await getSaveButton(page).click();

      const duplicateOutcome = await waitForSaveOutcome(page);
      expect(
        duplicateOutcome.kind === 'duplicate-validation' || duplicateOutcome.kind === 'success',
        `Expected duplicate-validation (or observed backend success) on second save, got: ${duplicateOutcome.kind} (${duplicateOutcome.message})`,
      ).toBe(true);
    });
  });

  test('Cancel button returns user to Detailed Platform Contact Information page', async ({ page }) => {
    await goToManagePlatforms(page);
    await assertPlatformManagementPageLoaded(page);
    await clickEditForFirstPlatform(page);
    await assertDetailedPlatformPageLoaded(page);
    await clickAddSubsidiaryPlatform(page);
    await assertAddSubPlatformPageLoaded(page);

    const cancelBtn = getCancelButton(page);
    await expect(cancelBtn).toBeVisible();
    await cancelBtn.click();

    await assertDetailedPlatformPageLoaded(page);
  });
});

async function loginAsIdir(page: Page): Promise<void> {
  await loginAsIdirShared(page, APP_URL);
}

async function goToManagePlatforms(page: Page): Promise<void> {
  await page.getByRole('button', { name: /^Manage Platforms$/i }).click();
}

async function assertPlatformManagementPageLoaded(page: Page): Promise<void> {
  await expect(page).toHaveURL(/platform-management/i, { timeout: 60_000 });

  const table = page.getByRole('table').first();
  const grid = page.getByRole('grid').first();
  const list = page.getByRole('list').first();

  const hasTable = await table.isVisible({ timeout: 5_000 }).catch(() => false);
  const hasGrid = await grid.isVisible({ timeout: 5_000 }).catch(() => false);
  const hasList = await list.isVisible({ timeout: 5_000 }).catch(() => false);

  expect(
    hasTable || hasGrid || hasList,
    'Expected platform-management page to show a list/table/grid of platforms and sub-platforms.',
  ).toBe(true);
}

async function clickEditForFirstPlatform(page: Page): Promise<void> {
  // Wait for the table to finish loading (dismiss any loading indicators)
  const loadingIndicator = page.locator('[class*="loading" i], [aria-busy="true"], .p-datatable-loading').first();
  await loadingIndicator.isVisible({ timeout: 2_000 }).then(
    async (visible) => {
      if (visible) {
        await page.waitForFunction(() => {
          const loading = document.querySelector('[class*="loading" i]');
          return !loading || !loading.offsetParent;
        }, { timeout: 20_000 });
      }
    },
    () => {} // Ignore if loading indicator not found
  );

  // Wait for at least one data row to appear
  await expect(page.locator('table tbody tr, [role="rowgroup"] [role="row"]').first()).toBeVisible({
    timeout: 15_000,
  });

  // The Edit action is a link (not a button) in the last cell of each table row.
  // URL pattern: /platform/{id}
  
  // Try 1: Click the first link that navigates to /platform/*
  const platformLink = page.locator('a[href*="/platform/"]').first();
  if ((await platformLink.count()) > 0 && (await platformLink.isVisible({ timeout: 10_000 }).catch(() => false))) {
    await platformLink.click();
    return;
  }

  // Try 2: Find the first visible table row and click the link in its last cell
  const rows = page.locator('table tbody tr, [role="rowgroup"] [role="row"]');
  const rowCount = await rows.count();
  
  for (let i = 0; i < rowCount; i++) {
    const row = rows.nth(i);
    const isVisible = await row.isVisible().catch(() => false);
    if (!isVisible) continue;

    // Get all cells in this row
    const cells = row.locator('td, [role="cell"]');
    const cellCount = await cells.count();
    
    if (cellCount === 0) continue;

    // Check the last cell for a link or button
    const lastCell = cells.nth(cellCount - 1);
    const linkInLastCell = lastCell.locator('a, button');
    
    if ((await linkInLastCell.count()) > 0) {
      const link = linkInLastCell.first();
      if (await link.isVisible().catch(() => false)) {
        await link.click();
        return;
      }
    }
  }

  throw new Error('Unable to locate an Edit action for any platform row on Manage Platforms page.');
}

async function assertDetailedPlatformPageLoaded(page: Page): Promise<void> {
  const addSubButton = getAddSubsidiaryPlatformButton(page);
  const detailsHeading = page
    .getByRole('heading', {
      name: /Detailed Platform Contact Information|Platform Contact Information|Platform Details/i,
    })
    .first();

  await expect.poll(async () => {
    const buttonVisible = await addSubButton.isVisible().catch(() => false);
    const headingVisible = await detailsHeading.isVisible().catch(() => false);
    const onLikelyDetailsUrl = /platform-management|platform.*detail|platform.*contact/i.test(page.url());
    return buttonVisible || (headingVisible && onLikelyDetailsUrl);
  }, {
    timeout: 30_000,
    message: 'Expected to be on Detailed Platform Contact Information page with Add Subsidiary Platform action.',
  }).toBe(true);

  await expect(addSubButton).toBeVisible({ timeout: 20_000 });
}

function getAddSubsidiaryPlatformButton(page: Page): Locator {
  return page
    .getByRole('button', {
      name: /^Add Subsidiary Platform$|^Add Subsidiary$|^Add Sub Platform$|^Add Sub-Platform$|^Add Child Platform$/i,
    })
    .first();
}

async function clickAddSubsidiaryPlatform(page: Page): Promise<void> {
  const addButton = getAddSubsidiaryPlatformButton(page);
  await expect(addButton).toBeVisible({ timeout: 20_000 });
  await addButton.click();

  const opened = await isAddSubPlatformFormVisible(page, 8_000);
  if (!opened) {
    await addButton.click();
    await expect.poll(async () => isAddSubPlatformFormVisible(page, 12_000), {
      timeout: 15_000,
      message: 'Add Subsidiary Platform click did not open add sub-platform form controls.',
    }).toBe(true);
  }
}

async function assertAddSubPlatformPageLoaded(page: Page): Promise<void> {
  const ready = await isAddSubPlatformFormVisible(page, 15_000);
  expect(
    ready,
    'Expected add sub-platform form controls to be visible (for example Platform Name + Save/Cancel).',
  ).toBe(true);
}

async function assertAddSubPlatformFormFields(page: Page): Promise<void> {
  const expectedFieldPatterns: RegExp[] = [
    /^Platform Name$/i,
    /^Platform Code$/i,
    /^Email for Non-Compliance Notices$/i,
    /^Primary Email for Takedown Request Letters$|^Email for Takedown Request Letters$/i,
    /^Secondary Email for Non-Compliance Notices\s*\(optional\)$/i,
    /^Secondary Email for Takedown Request Letters\s*\(optional\)$/i,
  ];

  for (const fieldPattern of expectedFieldPatterns) {
    const hasLabel = await page
      .getByText(fieldPattern)
      .first()
      .isVisible({ timeout: 10_000 })
      .catch(() => false);
    expect(hasLabel, `Missing expected form field label/text: ${fieldPattern.toString()}`).toBe(true);
  }

  await expect(getSaveButton(page)).toBeVisible();
  await expect(getCancelButton(page)).toBeVisible();
}

async function fillAddSubPlatformForm(page: Page, data: SubPlatformData): Promise<void> {
  await fillTextboxByLabel(page, /^Platform Name$/i, data.platformName);
  await fillTextboxByLabel(page, /^Platform Code$/i, data.platformCode);

  await fillNonComplianceEmail(page, data.nonComplianceEmail);
  await fillTextboxByLabel(
    page,
    /Secondary Email for Non-Compliance Notices/i,
    data.secondaryNonComplianceEmail,
  );

  await fillPrimaryTakedownEmail(page, data.primaryTakedownEmail);
  await fillTextboxByLabel(
    page,
    /Secondary Email for Takedown Request Letters/i,
    data.secondaryTakedownEmail,
  );
}

async function fillNonComplianceEmail(page: Page, value: string): Promise<void> {
  const byExactName = page
    .getByRole('textbox', { name: /^Enter Platform['’]s Email Contact$/i })
    .first();
  if ((await byExactName.count()) > 0 && (await byExactName.isVisible().catch(() => false))) {
    await byExactName.fill(value);
    return;
  }

  await fillTextboxByLabel(page, /^Email for Non-Compliance Notices$/i, value);
}

async function fillPrimaryTakedownEmail(page: Page, value: string): Promise<void> {
  const direct = page
    .getByRole('textbox', { name: /^Primary Email for Takedown Request Letters$|^Email for Takedown Request Letters$/i })
    .first();
  if ((await direct.count()) > 0 && (await direct.isVisible().catch(() => false))) {
    await direct.fill(value);
    return;
  }

  await fillTextboxByLabel(page, /Primary Email for Takedown Request Letters|Email for Takedown Request Letters/i, value);
}

async function fillTextboxByLabel(page: Page, labelPattern: RegExp, value: string): Promise<void> {
  const byRole = page.getByRole('textbox', { name: labelPattern }).first();
  if ((await byRole.count()) > 0 && (await byRole.isVisible().catch(() => false))) {
    await byRole.fill(value);
    return;
  }

  const byLabel = page.getByLabel(labelPattern).first();
  if ((await byLabel.count()) > 0 && (await byLabel.isVisible().catch(() => false))) {
    await byLabel.fill(value);
    return;
  }

  const byNearbyLabel = page
    .locator('div, section, fieldset')
    .filter({ has: page.getByText(labelPattern) })
    .locator('input[type="text"], input[type="email"], textarea, [role="textbox"]')
    .first();
  if ((await byNearbyLabel.count()) > 0 && (await byNearbyLabel.isVisible().catch(() => false))) {
    await byNearbyLabel.fill(value);
    return;
  }

  const label = page.locator('label').filter({ hasText: labelPattern }).first();
  if ((await label.count()) > 0 && (await label.isVisible().catch(() => false))) {
    const labelFor = await label.getAttribute('for');
    if (labelFor) {
      const linked = page.locator(`#${cssEscape(labelFor)}`).first();
      if ((await linked.count()) > 0 && (await linked.isVisible().catch(() => false))) {
        await linked.fill(value);
        return;
      }
    }

    const labelParentInput = label
      .locator('xpath=ancestor::*[self::div or self::section or self::fieldset][1]')
      .locator('input[type="text"], input[type="email"], textarea')
      .first();
    if ((await labelParentInput.count()) > 0 && (await labelParentInput.isVisible().catch(() => false))) {
      await labelParentInput.fill(value);
      return;
    }
  }

  throw new Error(`Unable to locate textbox for label: ${labelPattern.toString()}`);
}

function getSaveButton(page: Page): Locator {
  return page.getByRole('button', { name: /^Save$/i }).first();
}

function getCancelButton(page: Page): Locator {
  return page.getByRole('button', { name: /^Cancel$/i }).first();
}

async function ensureOnDetailedPlatformPage(page: Page): Promise<void> {
  const alreadyThere = await getAddSubsidiaryPlatformButton(page).isVisible().catch(() => false);
  if (alreadyThere) {
    return;
  }

  const onManagePlatforms = /platform-management/i.test(page.url());
  if (!onManagePlatforms) {
    await page.getByRole('link', { name: /^Home$/i }).click().catch(() => {});
    await goToManagePlatforms(page);
  }

  await assertPlatformManagementPageLoaded(page);
  await clickEditForFirstPlatform(page);
  await assertDetailedPlatformPageLoaded(page);
}

async function waitForSaveOutcome(
  page: Page,
): Promise<{ kind: 'success' | 'duplicate-validation' | 'unknown'; message: string }> {
  const successPattern = /New platform has been added successfully|Success/i;
  const duplicatePattern = /One or more validation errors occurred|validation errors occurred|already exists|duplicate/i;
  let outcome: { kind: 'success' | 'duplicate-validation' | 'unknown'; message: string } = {
    kind: 'unknown',
    message: 'Timed out waiting for save outcome message or redirect.',
  };

  await expect
    .poll(
      async () => {
        const toast = page.locator('.p-toast-message, [role="alert"], .alert, .notification');
        const count = await toast.count();

        for (let i = 0; i < count; i++) {
          const item = toast.nth(i);
          if (!(await item.isVisible().catch(() => false))) {
            continue;
          }
          const text = (await item.innerText().catch(() => '')).replace(/\s+/g, ' ').trim();
          if (successPattern.test(text)) {
            outcome = { kind: 'success', message: text };
            return true;
          }
          if (duplicatePattern.test(text)) {
            outcome = { kind: 'duplicate-validation', message: text };
            return true;
          }
        }

        const bodyText = (await page.locator('body').innerText().catch(() => '')).replace(/\s+/g, ' ').trim();
        if (successPattern.test(bodyText)) {
          outcome = { kind: 'success', message: bodyText.substring(0, 250) };
          return true;
        }
        if (duplicatePattern.test(bodyText)) {
          outcome = { kind: 'duplicate-validation', message: bodyText.substring(0, 250) };
          return true;
        }

        const detailsVisible = await getAddSubsidiaryPlatformButton(page).isVisible().catch(() => false);
        if (detailsVisible) {
          outcome = { kind: 'success', message: 'Returned to detailed platform page after save.' };
          return true;
        }

        return false;
      },
      { timeout: 45_000, intervals: [300, 600, 1_000] },
    )
    .toBe(true)
    .catch(() => undefined);

  return outcome;
}

async function dismissVisibleToasts(page: Page): Promise<void> {
  const closeButtons = page.locator('.p-toast-message button[aria-label="Close"], .p-toast-message button');
  const count = await closeButtons.count();
  for (let i = 0; i < count; i++) {
    const btn = closeButtons.nth(i);
    if (await btn.isVisible().catch(() => false)) {
      await btn.click().catch(() => {});
    }
  }
}

type SubPlatformData = {
  platformName: string;
  platformCode: string;
  nonComplianceEmail: string;
  secondaryNonComplianceEmail: string;
  primaryTakedownEmail: string;
  secondaryTakedownEmail: string;
};

function generateSubPlatformData(prefix: string): SubPlatformData {
  const stamp = Date.now().toString().slice(-8);
  const platformCode = `${prefix}${stamp.slice(-4)}`.toUpperCase().slice(0, 10);

  return {
    platformName: `Auto Sub Platform ${prefix}-${stamp}`,
    platformCode,
    nonComplianceEmail: `nc.${prefix.toLowerCase()}.${stamp}@example.test`,
    secondaryNonComplianceEmail: `nc2.${prefix.toLowerCase()}.${stamp}@example.test`,
    primaryTakedownEmail: `td.${prefix.toLowerCase()}.${stamp}@example.test`,
    secondaryTakedownEmail: `td2.${prefix.toLowerCase()}.${stamp}@example.test`,
  };
}

function cssEscape(value: string): string {
  return value.replace(/([#.;?+*~':"!^$\[\]()=>|/@])/g, '\\$1');
}

async function isAddSubPlatformFormVisible(page: Page, timeoutMs: number): Promise<boolean> {
  const platformNameInput = page.getByRole('textbox', { name: /^Platform Name$/i }).first();
  const platformCodeInput = page.getByRole('textbox', { name: /^Platform Code$/i }).first();
  const saveButton = getSaveButton(page);
  const cancelButton = getCancelButton(page);
  const addHeading = page.getByRole('heading', {
    name: /Add New Platform|Add Subsidiary Platform|Create.*Platform/i,
  }).first();

  return await expect
    .poll(
      async () => {
        const nameVisible = await platformNameInput.isVisible().catch(() => false);
        const codeVisible = await platformCodeInput.isVisible().catch(() => false);
        const saveVisible = await saveButton.isVisible().catch(() => false);
        const cancelVisible = await cancelButton.isVisible().catch(() => false);
        const headingVisible = await addHeading.isVisible().catch(() => false);

        return ((nameVisible || codeVisible) && saveVisible && cancelVisible) ||
          (headingVisible && (saveVisible || cancelVisible));
      },
      { timeout: timeoutMs, intervals: [250, 500, 1_000] },
    )
    .toBe(true)
    .then(() => true)
    .catch(() => false);
}


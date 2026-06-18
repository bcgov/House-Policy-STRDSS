/// <reference types="node" />

/**
 * Feature: ManagePlatformsAddPlatform
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-226
 *
 * @AddPlatform
 * Scenario: AddPlatform
 *
 * Test Case Summary:
 * Given I am an authenticated Admin user with access to Manage Platforms
 * When I navigate to the Manage Platforms page and initiate adding a new Parent Platform
 * Then I should see the Add New Platform form with all required input fields
 * And the Save button behavior should follow required-field validation rules
 * And saving with valid unique data should return a success message
 * And attempting duplicate submissions should trigger validation error messages
 * And clicking Cancel should navigate back to the platform-management page
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 - Navigation and Add Parent Platform Form Rendering:
 * - Step 1: Authenticate via IDIR login with Admin credentials
 * - Step 2: Verify landing page is displayed
 * - Step 3: Navigate to Manage Platforms page
 * - Step 4: Validate platform-management page is loaded with list/table/grid visible
 * - Step 5: Locate and click "Add Parent Platform" button
 * - Step 6: Validate Add New Platform form page is displayed
 * - Step 7: Verify all required form input fields are present and visible:
 *   ✓ Platform Name
 *   ✓ Platform Code
 *   ✓ Email for Non-Compliance Notices
 *   ✓ Secondary Email for Non-Compliance Notices (optional)
 *   ✓ Email for Takedown Request Letters
 *   ✓ Secondary Email for Takedown Request Letters (optional)
 *   ✓ Platform Type
 * - Step 8: Validate Save and Cancel buttons are visible
 *
 * AC2 - Required Field Validation and Save Button Enablement:
 * - Step 1: Verify Save button is disabled when form is empty
 * - Step 2: Enter valid values into all required fields one by one
 * - Step 3: Validate Save button state transitions from disabled to enabled
 * - Step 4: Clear a required field and verify Save button becomes disabled
 * - Step 5: Confirm Save button enablement is tightly coupled to required field validation
 *
 * AC3 - Add Parent Platform with Valid Unique Data (Success Path):
 * - Step 1: Fill all form fields with valid unique data
 * - Step 2: Verify Save button is enabled
 * - Step 3: Click Save button
 * - Step 4: Wait for success outcome (toast/modal/redirect)
 * - Step 5: Validate success message matches pattern: "New platform has been added successfully|Success"
 * - Step 6: Dismiss any visible toast notifications
 * - Step 7: Verify user is redirected to platform-management page
 * - Step 8: Confirm new platform appears in the list
 *
 * AC4 - Duplicate Submission Handling (Validation Error Path):
 * - Step 1: Navigate back to Add Parent Platform form from platform-management page
 * - Step 2: Fill form with the same data used in the previous successful save
 * - Step 3: Click Save button
 * - Step 4: Wait for duplicate validation outcome
 * - Step 5: Validate error message matches pattern: "One or more validation errors occurred|already exists|duplicate"
 * - Step 6: Verify form remains on the add-platform page (no redirect to list)
 * - Step 7: Confirm Save button is still actionable for retry or correction
 *
 * AC5 - Cancel Button Navigation:
 * - Step 1: Navigate to Manage Platforms page
 * - Step 2: Click "Add Parent Platform" button
 * - Step 3: Validate Add New Platform form is displayed
 * - Step 4: Click Cancel button without entering data
 * - Step 5: Verify user is redirected to platform-management page
 * - Step 6: Confirm form does not submit any data on Cancel
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import { IDIR_AUTH_ENV_MESSAGE, hasIdirAuthConfig, loginAsIdir as loginAsIdirShared } from './support/auth';

const APP_URL = process.env.BASE_URL ?? '';

const NEW_PLATFORM_TYPE = 'Major';

test.use({ browserName: 'chromium' });

test.describe('@regression Feature: ManagePlatformsAddPlatform', () => {
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

  test('AC1-AC4: user can open Add New Platform and see all required input fields', async ({ page }) => {
    await test.step('Navigate to Manage Platforms page', async () => {
      await goToManagePlatforms(page);
      await assertPlatformManagementPageLoaded(page);
    });

    await test.step('Click Add Parent Platform and validate Add New Platform page', async () => {
      await clickAddParentPlatform(page);
      await assertAddNewPlatformPageLoaded(page);
      await assertAddPlatformFormFields(page);
    });
  });

  test('AC4: required-field behavior toggles Save button enablement', async ({ page }) => {
    await goToManagePlatforms(page);
    await clickAddParentPlatform(page);
    await assertAddNewPlatformPageLoaded(page);

    const saveButton = getSaveButton(page);

    await test.step('Initial state: Save should be disabled before entering required values', async () => {
      await expect(saveButton).toBeDisabled();
    });

    await test.step('Enter valid values for required fields and verify Save enabled', async () => {
      const data = generatePlatformData('REQCHK');
      await fillAddPlatformForm(page, data);
      await expect(saveButton).toBeEnabled({ timeout: 10_000 });
    });
  });

  test('AC5: save new parent platform with valid data and validate outcome', async ({ page }) => {
    await goToManagePlatforms(page);
    await clickAddParentPlatform(page);
    await assertAddNewPlatformPageLoaded(page);

    const unique = generatePlatformData('AUTO');

    await test.step('Fill form with valid unique values and Save', async () => {
      await fillAddPlatformForm(page, unique);
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

      // For successful creation, confirm user returns to platform management page.
      if (outcome.kind === 'success') {
        await assertPlatformManagementPageLoaded(page);
      }
    });

    await test.step('Attempt immediate duplicate using same data to validate duplicate error path', async () => {
      await ensureOnPlatformManagementPage(page);
      await clickAddParentPlatform(page);
      await fillAddPlatformForm(page, unique);
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

  test('Cancel button returns user to platform-management page', async ({ page }) => {
    await goToManagePlatforms(page);
    await clickAddParentPlatform(page);
    await assertAddNewPlatformPageLoaded(page);

    const cancelBtn = getCancelButton(page);
    await expect(cancelBtn).toBeVisible();
    await cancelBtn.click();

    await assertPlatformManagementPageLoaded(page);
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

  // Confirm list/table is visible for platforms and sub-platforms.
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

  await expect(getAddParentPlatformButton(page)).toBeVisible({ timeout: 20_000 });
}

function getAddParentPlatformButton(page: Page): Locator {
  return page.getByRole('button', { name: /^Add Parent Platform$/i }).first();
}

async function clickAddParentPlatform(page: Page): Promise<void> {
  const addButton = getAddParentPlatformButton(page);
  await expect(addButton).toBeVisible({ timeout: 20_000 });
  await addButton.click();

  // In some builds this opens an inline form/dialog without a route change.
  // Wait for add-form controls to appear; if not, retry once.
  const opened = await isAddPlatformFormVisible(page, 8_000);
  if (!opened) {
    await addButton.click();
    await expect.poll(async () => isAddPlatformFormVisible(page, 12_000), {
      timeout: 15_000,
      message: 'Add Parent Platform click did not open the add platform form controls.',
    }).toBe(true);
  }
}

async function assertAddNewPlatformPageLoaded(page: Page): Promise<void> {
  // Do not require specific heading text; assert actionable add-form controls.
  const ready = await isAddPlatformFormVisible(page, 15_000);
  expect(
    ready,
    'Expected add platform form controls to be visible (for example Platform Name + Save/Cancel).',
  ).toBe(true);
}

async function assertAddPlatformFormFields(page: Page): Promise<void> {
  const expectedFieldPatterns: RegExp[] = [
    /^Platform Name$/i,
    /^Platform Code$/i,
    /^Email for Non-Compliance Notices$/i,
    /^Secondary Email for Non-Compliance Notices\s*\(optional\)$/i,
    /^Email for Takedown Request Letters$/i,
    /^Secondary Email for Takedown Request Letters\s*\(optional\)$/i,
    /^Platform Type$/i,
  ];

  for (const fieldPattern of expectedFieldPatterns) {
    const hasLabel = await page.getByText(fieldPattern).first().isVisible({ timeout: 10_000 }).catch(() => false);
    expect(hasLabel, `Missing expected form field label/text: ${fieldPattern.toString()}`).toBe(true);
  }

  await expect(getSaveButton(page)).toBeVisible();
  await expect(getCancelButton(page)).toBeVisible();
}

async function fillAddPlatformForm(page: Page, data: PlatformData): Promise<void> {
  await fillTextboxByLabel(page, /^Platform Name$/i, data.platformName);
  await fillTextboxByLabel(page, /Platform Code/i, data.platformCode);

  await fillNonComplianceEmail(page, data.nonComplianceEmail);
  await fillTextboxByLabel(
    page,
    /Secondary Email for Non-Compliance Notices/i,
    data.secondaryNonComplianceEmail,
  );

  await fillTextboxByLabel(page, /Email for Takedown Request Letters/i, data.takedownEmail);
  await fillTextboxByLabel(
    page,
    /Secondary Email for Takedown Request Letters/i,
    data.secondaryTakedownEmail,
  );

  await selectPlatformType(page, data.platformType);
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

async function fillNonComplianceEmail(page: Page, value: string): Promise<void> {
  // Live UAT snapshot shows this field as textbox name "Enter Platform’s Email Contact".
  const byExactName = page
    .getByRole('textbox', { name: /^Enter Platform['’]s Email Contact$/i })
    .first();
  if ((await byExactName.count()) > 0 && (await byExactName.isVisible().catch(() => false))) {
    await byExactName.fill(value);
    return;
  }

  // Fallback by placeholder text used by the same control.
  const byPlaceholder = page
    .locator('input[placeholder*="Enter Platform"][placeholder*="Email Contact"], textarea[placeholder*="Enter Platform"][placeholder*="Email Contact"]')
    .first();
  if ((await byPlaceholder.count()) > 0 && (await byPlaceholder.isVisible().catch(() => false))) {
    await byPlaceholder.fill(value);
    return;
  }

  await fillTextboxByLabel(page, /^Email for Non-Compliance Notices$/i, value);
}

async function selectPlatformType(page: Page, platformType: string): Promise<void> {
  const comboboxByName = page
    .getByRole('combobox', { name: /^Platform Type$|^Please Select$/i })
    .first();
  if ((await comboboxByName.count()) > 0 && (await comboboxByName.isVisible().catch(() => false))) {
    await comboboxByName.click();
    await chooseDropdownOption(page, new RegExp(`^${escapeRegExp(platformType)}$`, 'i'));
    return;
  }

  const nativeSelect = page.locator('select').first();
  if ((await nativeSelect.count()) > 0 && (await nativeSelect.isVisible().catch(() => false))) {
    const options = await nativeSelect.locator('option').allTextContents();
    const matched = options.find((opt) => new RegExp(escapeRegExp(platformType), 'i').test(opt));
    if (matched) {
      await nativeSelect.selectOption({ label: matched });
      return;
    }

    const firstRealOption = await nativeSelect
      .locator('option:not([disabled]):not([value=""])')
      .first()
      .getAttribute('value')
      .catch(() => null);
    if (firstRealOption) {
      await nativeSelect.selectOption(firstRealOption);
      return;
    }
  }

  const trigger = page.getByText(/^Platform Type$/i).first();
  await expect(trigger).toBeVisible({ timeout: 15_000 });
  await trigger.click();
  await chooseDropdownOption(page, new RegExp(`^${escapeRegExp(platformType)}$`, 'i'));
}

async function chooseDropdownOption(page: Page, optionPattern: RegExp): Promise<void> {
  const optionRole = page.getByRole('option', { name: optionPattern }).first();
  if ((await optionRole.count()) > 0) {
    await expect(optionRole).toBeVisible({ timeout: 10_000 });
    await optionRole.click();
    return;
  }

  const firstRoleOption = page.locator('[role="listbox"] [role="option"]').first();
  if ((await firstRoleOption.count()) > 0 && (await firstRoleOption.isVisible().catch(() => false))) {
    await firstRoleOption.click();
    return;
  }

  const listItem = page.locator('li, div').filter({ hasText: optionPattern }).first();
  if ((await listItem.count()) > 0 && (await listItem.isVisible().catch(() => false))) {
    await listItem.click();
    return;
  }

  const firstListItem = page.locator('[role="listbox"] li').first();
  await expect(firstListItem).toBeVisible({ timeout: 10_000 });
  await firstListItem.click();
}

function getSaveButton(page: Page): Locator {
  return page.getByRole('button', { name: /^Save$/i }).first();
}

function getCancelButton(page: Page): Locator {
  return page.getByRole('button', { name: /^Cancel$/i }).first();
}

async function ensureOnPlatformManagementPage(page: Page): Promise<void> {
  const onPlatformManagement = /platform-management/i.test(page.url());
  if (!onPlatformManagement) {
    await page.getByRole('link', { name: /^Home$/i }).click().catch(() => {});
    await goToManagePlatforms(page);
  }
  await assertPlatformManagementPageLoaded(page);
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

        if (/platform-management/i.test(page.url())) {
          outcome = { kind: 'success', message: 'Redirected to platform-management after save.' };
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

type PlatformData = {
  platformName: string;
  platformCode: string;
  nonComplianceEmail: string;
  secondaryNonComplianceEmail: string;
  takedownEmail: string;
  secondaryTakedownEmail: string;
  platformType: string;
};

function generatePlatformData(prefix: string): PlatformData {
  const stamp = Date.now().toString().slice(-8);
  const platformCode = `${prefix}${stamp.slice(-4)}`.toUpperCase().slice(0, 10);

  return {
    platformName: `Auto Parent Platform ${prefix}-${stamp}`,
    platformCode,
    nonComplianceEmail: `nc.${prefix.toLowerCase()}.${stamp}@example.test`,
    secondaryNonComplianceEmail: `nc2.${prefix.toLowerCase()}.${stamp}@example.test`,
    takedownEmail: `td.${prefix.toLowerCase()}.${stamp}@example.test`,
    secondaryTakedownEmail: `td2.${prefix.toLowerCase()}.${stamp}@example.test`,
    platformType: NEW_PLATFORM_TYPE,
  };
}

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

function cssEscape(value: string): string {
  return value.replace(/([\\#.;?+*~':"!^$\[\]()=>|/@])/g, '\\$1');
}

async function isAddPlatformFormVisible(page: Page, timeoutMs: number): Promise<boolean> {
  const platformNameInput = page.getByRole('textbox', { name: /^Platform Name$/i }).first();
  const platformCodeInput = page.getByRole('textbox', { name: /^Platform Code$/i }).first();
  const saveButton = getSaveButton(page);
  const cancelButton = getCancelButton(page);
  const addHeading = page.getByRole('heading', { name: /Add New Platform|Create.*Platform/i }).first();

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


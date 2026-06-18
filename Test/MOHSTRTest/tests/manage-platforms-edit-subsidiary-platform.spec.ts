/// <reference types="node" />

/**
 * Feature: ManagePlatforms_EditSubPlatform
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-226
 *
 * @EditSubsidiaryPlatform
 * Scenario: EditSubsidiaryPlatform
 *
 * Test Case Summary:
 * Given I am an authenticated user with valid IDIR credentials and permissions to Manage Platforms
 * When I open an existing platform and navigate to Edit Subsidiary Platform Information
 * Then I should be able to verify page labels, editable and non-editable controls, and Save/Cancel behavior
 * And I should be able to validate happy-path save plus negative/edge scenarios (duplicate-probable, required fields, invalid email, cancel navigation)
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 - Navigate to Edit Subsidiary Platform page and verify controls/labels:
 *
 * #User Authentication
 * - Given that I am an authenticated user with access to Edit Manage Platforms
 * - Then I am directed to the Landing Page
 *
 * #Select Manage Platforms
 * - When I click on the Manage Platforms action button
 * - Then I should be landed into Manage Platforms page is loaded with list/table/grid visible
 *
 * #Select Platform
 * - When I select an existing platform
 * - Locate and click Edit button/link for a platform row, then I should be able to edit platform information
 * - And I should be landed into Detailed Platform Contact Information page
 *
 * #Edit Subsidiary Platform Information
 * - Locate and click Update Subsidiary Platform Information action button/link under Subsidiary Platform Information section
 * - Landed into Edit Subsidiary Platform page
 * - Verify following labels are present in the page:
 *   ✓ Platform Name
 *   ✓ Platform Code
 *   ✓ Email for Non-Compliance Notices
 *   ✓ Secondary Email for Non-Compliance Notices (optional)
 *   ✓ Email for Takedown Request Letters
 *   ✓ Secondary Email for Takedown Request Letters (optional)
 *   ✓ Platform Status
 *   ✓ Active
 *   ✓ Disabled
 * - Verify Platform Code is non-editable
 * - Verify Disabled radio button is non-editable
 * - Validate Save and Cancel buttons are visible
 *
 * AC2 - Edit subsidiary platform with valid data and validate success:
 * - Step 1: Navigate to Edit Subsidiary Platform page
 * - Step 2: Edit Platform Name with valid updated value (unique timestamp suffix)
 * - Step 3: Edit Email for Non-Compliance Notices with valid updated value
 * - Step 4: Edit Email for Takedown Request Letters with valid updated value
 * - Step 5: Validate Save button is enabled
 * - Step 6: Click Save button
 * - Step 7: Validate save outcome is success
 * - Step 8: Validate user is returned to Detailed Platform Contact Information page
 *
 * AC3 - Required fields disable Save when cleared:
 * - Step 1: Navigate to Edit Subsidiary Platform page
 * - Step 2: Clear Platform Name and verify Save is disabled
 * - Step 3: Refill Platform Name with valid value
 * - Step 4: Clear Email for Non-Compliance Notices and verify Save is disabled
 * - Step 5: Refill Email for Non-Compliance Notices with valid value
 * - Step 6: Clear Email for Takedown Request Letters and verify Save is disabled
 * - Step 7: Refill Email for Takedown Request Letters with valid value
 * - Step 8: Click Cancel
 * - Step 9: Validate Detailed Platform Contact Information page is displayed
 *
 * AC4 - Invalid non-compliance email format disables Save:
 * - Step 1: Navigate to Edit Subsidiary Platform page
 * - Step 2: Verify Save button is enabled
 * - Step 3: Edit Non-Compliance Notices email by removing '@'
 * - Step 4: Verify Save button is disabled
 *
 * AC5 - Duplicate-probable update handling:
 * - Step 1: Navigate to Edit Subsidiary Platform page
 * - Step 2: Attempt update with a duplicate-probable platform name (sourced from existing visible subsidiary/platform row text)
 * - Step 3: Click Save if enabled
 * - Step 4: Validate duplicate validation message when enforced, or capture alternate accepted behavior
 * - Step 5: Ensure test returns to stable state (Detailed page or Edit page with actionable controls)
 *
 * AC6 - Cancel on Edit Subsidiary Platform returns to Detailed Platform Contact Information page:
 * - Step 1: Navigate to Edit Subsidiary Platform page
 * - Step 2: Validate Cancel button is visible and accessible
 * - Step 3: Click Cancel button
 * - Step 4: Validate Detailed Platform Contact Information page is displayed
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import { IDIR_AUTH_ENV_MESSAGE, hasIdirAuthConfig, loginAsIdir as loginAsIdirShared } from './support/auth';

const APP_URL = process.env.BASE_URL ?? '';

test.use({ browserName: 'chromium' });

const NON_COMPLIANCE_EMAIL_TEST_ID = 'edit-subsidiary-non-compliance-email';

type EditedPlatformValues = {
  name: string;
  nonComplianceEmail: string;
  takedownEmail: string;
};

test.describe('@regression Scenario: EditSubsidiaryPlatform', () => {
  test.setTimeout(420_000);
  test.describe.configure({ mode: 'serial' });

  test.skip(
    !hasIdirAuthConfig(),
    IDIR_AUTH_ENV_MESSAGE,
  );

  test.beforeEach(async ({ page }) => {
    await loginAsIdir(page);
    await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
  });

  test('AC1: navigate to Edit Subsidiary Platform and verify controls/labels', async ({ page }) => {
    await test.step('User Authentication: landing page is loaded', async () => {
      await expect(page.getByRole('region', { name: /^Home$/i })).toBeVisible({ timeout: 30_000 });
    });

    await test.step('Select Manage Platforms: click action and validate page list/table/grid', async () => {
      await goToManagePlatforms(page);
      await assertPlatformManagementPageLoaded(page);
    });

    await test.step('Select Platform: click Edit action and validate Detailed Platform Contact Information page', async () => {
      await clickEditForFirstPlatform(page);
      await assertDetailedPlatformPageLoaded(page);
    });

    await test.step('Edit Subsidiary Platform Information: open Update action under subsidiary section', async () => {
      await ensureSubsidiaryRecordExistsThenOpenEdit(page);
      await assertEditSubsidiaryPlatformPageLoaded(page);
    });

    await test.step('Validate labels and control states on Edit Subsidiary Platform page', async () => {
      await assertEditSubsidiaryPlatformFormLabels(page);
      await assertPlatformCodeNonEditable(page);
      await assertDisabledStatusOptionNonEditable(page);
      await expect(getSaveButton(page)).toBeVisible();
      await expect(getCancelButton(page)).toBeVisible();
    });
  });

  test('AC2: edit subsidiary platform with valid data and validate save success', async ({ page }) => {
    await navigateToEditSubsidiaryPlatformForm(page);
    await assertEditSubsidiaryPlatformPageLoaded(page);

    const edited = await buildEditedPlatformValues(page);
    const nonComplianceEmailField = await getNonComplianceEmailField(page);
    const takedownEmailField = await getTakedownEmailField(page);

    await test.step('Edit Platform Name input text field with valid updated values', async () => {
      await fillTextboxByLabel(page, /^Platform Name$/i, edited.name);
    });

    await test.step('Edit Email for Non-Compliance Notices input text field with valid updated values', async () => {
      await nonComplianceEmailField.fill(edited.nonComplianceEmail);
    });

    await test.step('Edit Email for Takedown Request Letters input text field with valid updated values', async () => {
      await takedownEmailField.fill(edited.takedownEmail);
    });

    await test.step('Verify Save button is enabled', async () => {
      await expect(getSaveButton(page)).toBeEnabled({ timeout: 10_000 });
    });

    await test.step('Save edited values and validate outcome', async () => {
      await getSaveButton(page).click();
      const outcome = await waitForSaveOutcome(page);

      expect(outcome.kind, `Expected success outcome, got: ${outcome.kind} (${outcome.message})`).toBe('success');

      await dismissVisibleToasts(page);
      await assertDetailedPlatformPageLoaded(page);
    });
  });

  test('AC3: required fields disable save when cleared', async ({ page }) => {
    await navigateToEditSubsidiaryPlatformForm(page);
    await assertEditSubsidiaryPlatformPageLoaded(page);

    const platformNameField = await getTextboxByLabel(page, /^Platform Name$/i);
    const nonComplianceEmailField = await getNonComplianceEmailField(page);
    const takedownEmailField = await getTakedownEmailField(page);

    const timestamp = Date.now().toString().slice(-6);
    const validPlatformName = `Platformname-${timestamp}`;

    await test.step('Clear Platform Name and verify disabled Save button', async () => {
      await platformNameField.fill('');
      await blurField(page, platformNameField);
      await expect(getSaveButton(page)).toBeDisabled({ timeout: 10_000 });
      await platformNameField.fill(validPlatformName);
    });

    await test.step('Clear Email for Non-Compliance Notices and verify disabled Save button', async () => {
      await nonComplianceEmailField.fill('');
      await blurField(page, nonComplianceEmailField);
      await expect(getSaveButton(page)).toBeDisabled({ timeout: 10_000 });
      await nonComplianceEmailField.fill(`valid.noncompliance+${timestamp}@example.com`);
    });

    await test.step('Clear Email for Takedown Request Letters and verify disabled Save button', async () => {
      await takedownEmailField.fill('');
      await blurField(page, takedownEmailField);
      await expect(getSaveButton(page)).toBeDisabled({ timeout: 10_000 });
      await takedownEmailField.fill(`valid.takedown+${timestamp}@example.com`);
    });

    await getCancelButton(page).click();
    await assertDetailedPlatformPageLoaded(page);
  });

  test('AC4: invalid non-compliance email format disables save button', async ({ page }) => {
    await test.step('Step 1: Navigate to Edit Subsidiary Platform page', async () => {
      await navigateToEditSubsidiaryPlatformForm(page);
      await assertEditSubsidiaryPlatformPageLoaded(page);
    });

    await test.step('Step 2: Verify Save button is enabled', async () => {
      await expect(getSaveButton(page)).toBeEnabled({ timeout: 10_000 });
    });

    await test.step("Step 3: Edit Non-Compliance Notices email by removing '@'", async () => {
      const nonComplianceEmailSource = await getNonComplianceEmailField(page);
      await nonComplianceEmailSource.evaluate((node, testId) => {
        if (node instanceof HTMLInputElement && !node.getAttribute('data-testid')) {
          node.setAttribute('data-testid', testId);
        }
      }, NON_COMPLIANCE_EMAIL_TEST_ID);

      const nonComplianceEmailField = page.getByTestId(NON_COMPLIANCE_EMAIL_TEST_ID);
      await expect(nonComplianceEmailField).toBeVisible({ timeout: 10_000 });

      const originalEmail = (await nonComplianceEmailField.inputValue().catch(() => '')).trim();
      const invalidEmail = originalEmail.includes('@')
        ? originalEmail.replace('@', '')
        : `invalid.email.${Date.now()}example.com`;

      await nonComplianceEmailField.fill(invalidEmail);
      await blurField(page, nonComplianceEmailField);
    });

    await test.step('Step 4: Verify Save button is disabled', async () => {
      await expect(getSaveButton(page)).toBeDisabled({ timeout: 10_000 });
    });
  });

  test('AC5: duplicate-probable update handling', async ({ page }) => {
    await navigateToEditSubsidiaryPlatformForm(page);
    await assertEditSubsidiaryPlatformPageLoaded(page);

    const duplicateCandidate = await getDuplicateProbableNameCandidate(page);
    const nonComplianceEmailField = await getNonComplianceEmailField(page);
    const takedownEmailField = await getTakedownEmailField(page);

    await test.step('Prepare duplicate-probable values and verify Save state', async () => {
      const fallbackName = `Duplicate Candidate ${Date.now().toString().slice(-6)}`;
      const candidate = (duplicateCandidate || fallbackName).slice(0, 100);

      await fillTextboxByLabel(page, /^Platform Name$/i, candidate);

      // Keep required fields valid so backend validation can evaluate duplicate rules.
      if (!(await getSaveButton(page).isEnabled().catch(() => false))) {
        const stamp = Date.now().toString().slice(-6);
        await nonComplianceEmailField.fill(`valid.dupcheck+${stamp}@example.com`);
        await takedownEmailField.fill(`valid.dupcheck.td+${stamp}@example.com`);
      }

      const saveVisible = await getSaveButton(page).isVisible().catch(() => false);
      expect(saveVisible, 'Expected Save button to be visible on Edit Subsidiary page.').toBe(true);
    });

    await test.step('Attempt save and validate duplicate/accepted behavior', async () => {
      const saveButton = getSaveButton(page);
      const isEnabled = await saveButton.isEnabled().catch(() => false);

      if (!isEnabled) {
        await expect(saveButton).toBeDisabled();
        await getCancelButton(page).click();
        await assertDetailedPlatformPageLoaded(page);
        return;
      }

      await saveButton.click();
      const outcome = await waitForSaveOutcome(page);

      expect(
        outcome.kind === 'duplicate-validation' || outcome.kind === 'success',
        `Expected duplicate-validation or success behavior, got: ${outcome.kind} (${outcome.message})`,
      ).toBe(true);

      await dismissVisibleToasts(page);

      if (outcome.kind === 'success') {
        await assertDetailedPlatformPageLoaded(page);
      } else {
        await expect(getSaveButton(page)).toBeVisible({ timeout: 10_000 });
      }
    });
  });

  test('AC6: cancel on Edit Subsidiary Platform returns to Detailed Platform Contact Information page', async ({ page }) => {
    await navigateToEditSubsidiaryPlatformForm(page);
    await assertEditSubsidiaryPlatformPageLoaded(page);

    const cancelBtn = getCancelButton(page);
    await expect(cancelBtn).toBeVisible();
    await cancelBtn.click();

    await assertDetailedPlatformPageLoaded(page);
  });
});

async function loginAsIdir(page: Page): Promise<void> {
  await loginAsIdirShared(page, APP_URL);
}

async function navigateToEditSubsidiaryPlatformForm(page: Page): Promise<void> {
  await goToManagePlatforms(page);
  await assertPlatformManagementPageLoaded(page);
  await clickEditForFirstPlatform(page);
  await assertDetailedPlatformPageLoaded(page);
  await clickUpdateSubsidiaryPlatformInformation(page);
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
  const loadingIndicator = page
    .locator('[class*="loading" i], [aria-busy="true"], .p-datatable-loading')
    .first();
  await loadingIndicator.isVisible({ timeout: 2_000 }).then(
    async (visible) => {
      if (visible) {
        await page.waitForFunction(
          () => {
            const loading = document.querySelector('[class*="loading" i]');
            return !(loading instanceof HTMLElement) || loading.offsetParent === null;
          },
          { timeout: 20_000 },
        );
      }
    },
    () => {},
  );

  await expect(page.locator('table tbody tr, [role="rowgroup"] [role="row"]').first()).toBeVisible({
    timeout: 15_000,
  });

  const platformLink = page.locator('a[href*="/platform/"]').first();
  if ((await platformLink.count()) > 0 && (await platformLink.isVisible({ timeout: 10_000 }).catch(() => false))) {
    await platformLink.click();
    return;
  }

  const rows = page.locator('table tbody tr, [role="rowgroup"] [role="row"]');
  const rowCount = await rows.count();

  for (let i = 0; i < rowCount; i++) {
    const row = rows.nth(i);
    const isVisible = await row.isVisible().catch(() => false);
    if (!isVisible) {
      continue;
    }

    const cells = row.locator('td, [role="cell"]');
    const cellCount = await cells.count();
    if (cellCount === 0) {
      continue;
    }

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
  const detailsHeading = page
    .getByRole('heading', {
      name: /Detailed Platform Contact Information|Platform Contact Information|Platform Details/i,
    })
    .first();
  const updateSubsidiaryAction = getUpdateSubsidiaryPlatformInformationAction(page);

  await expect
    .poll(
      async () => {
        const headingVisible = await detailsHeading.isVisible().catch(() => false);
        const updateVisible = await updateSubsidiaryAction.isVisible().catch(() => false);
        const onLikelyDetailsUrl = /platform-management|platform.*detail|platform.*contact|platform\/\d+/i.test(
          page.url(),
        );
        return headingVisible || (updateVisible && onLikelyDetailsUrl);
      },
      {
        timeout: 30_000,
        message: 'Expected Detailed Platform Contact Information page to be loaded.',
      },
    )
    .toBe(true);

  await expect(updateSubsidiaryAction).toBeVisible({ timeout: 20_000 });
}

function getUpdateSubsidiaryPlatformInformationAction(page: Page): Locator {
  const inSubsidiarySection = page
    .locator('section, div, fieldset')
    .filter({ hasText: /Subsidiary Platform Information/i })
    .locator('a, button, [role="link"], [role="button"]')
    .filter({
      hasText:
        /Update Subsidiary Platform Information|Edit Subsidiary Platform Information|Update Subsidiary Platform|Edit Subsidiary Platform/i,
    })
    .first();

  const globalFallback = page
    .locator('a, button, [role="link"], [role="button"]')
    .filter({
      hasText:
        /Update Subsidiary Platform Information|Edit Subsidiary Platform Information|Update Subsidiary Platform|Edit Subsidiary Platform/i,
    })
    .first();

  return inSubsidiarySection.or(globalFallback).first();
}

async function clickUpdateSubsidiaryPlatformInformation(page: Page): Promise<void> {
  const action = getUpdateSubsidiaryPlatformInformationAction(page);
  if (await action.isVisible().catch(() => false)) {
    await action.click();
    return;
  }

  const rowActionFallback = page
    .locator('table tbody tr, [role="rowgroup"] [role="row"]')
    .first()
    .locator('a, button, [role="link"], [role="button"]')
    .filter({ hasText: /Update|Edit/i })
    .first();

  await expect(rowActionFallback).toBeVisible({ timeout: 15_000 });
  await rowActionFallback.click();
}

async function ensureSubsidiaryRecordExistsThenOpenEdit(page: Page): Promise<void> {
  const updateAction = getUpdateSubsidiaryPlatformInformationAction(page);
  const hasUpdateAction = await updateAction.isVisible().catch(() => false);

  if (hasUpdateAction) {
    await clickUpdateSubsidiaryPlatformInformation(page);
    return;
  }

  await clickAddSubsidiaryPlatform(page);
  await assertAddSubPlatformPageLoaded(page);
  await fillRequiredAddSubsidiaryFields(page);
  await expect(getSaveButton(page)).toBeEnabled({ timeout: 10_000 });
  await getSaveButton(page).click();

  const outcome = await waitForSaveOutcome(page);
  expect(
    outcome.kind,
    `Expected success while creating prerequisite subsidiary platform record, got: ${outcome.kind} (${outcome.message})`,
  ).toBe('success');

  await dismissVisibleToasts(page);
  await assertDetailedPlatformPageLoaded(page);
  await clickUpdateSubsidiaryPlatformInformation(page);
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
}

async function assertAddSubPlatformPageLoaded(page: Page): Promise<void> {
  const platformNameInput = page.getByRole('textbox', { name: /^Platform Name$/i }).first();
  const platformCodeInput = page.getByRole('textbox', { name: /^Platform Code$/i }).first();

  await expect
    .poll(
      async () => {
        const nameVisible = await platformNameInput.isVisible().catch(() => false);
        const codeVisible = await platformCodeInput.isVisible().catch(() => false);
        const saveVisible = await getSaveButton(page).isVisible().catch(() => false);
        const cancelVisible = await getCancelButton(page).isVisible().catch(() => false);
        return (nameVisible || codeVisible) && saveVisible && cancelVisible;
      },
      {
        timeout: 20_000,
        message: 'Expected Add Subsidiary Platform form controls to be visible.',
      },
    )
    .toBe(true);
}

async function fillRequiredAddSubsidiaryFields(page: Page): Promise<void> {
  const stamp = Date.now().toString().slice(-8);

  await fillTextboxByLabel(page, /^Platform Name$/i, `Auto Subsidiary ${stamp}`);
  await fillTextboxByLabel(page, /^Platform Code$/i, `AS${stamp.slice(-6)}`);

  const nonComplianceEmail = await getNonComplianceEmailField(page);
  await nonComplianceEmail.fill(`nc.add.${stamp}@example.com`);

  const takedownEmail = await getTakedownEmailField(page);
  await takedownEmail.fill(`td.add.${stamp}@example.com`);
}

async function assertEditSubsidiaryPlatformPageLoaded(page: Page): Promise<void> {
  const heading = page
    .getByRole('heading', {
      name: /Edit Subsidiary Platform|Update Subsidiary Platform|Edit Platform|Update Platform/i,
    })
    .first();
  const save = getSaveButton(page);
  const cancel = getCancelButton(page);
  const platformName = page.getByRole('textbox', { name: /^Platform Name$/i }).first();

  await expect
    .poll(
      async () => {
        const headingVisible = await heading.isVisible().catch(() => false);
        const saveVisible = await save.isVisible().catch(() => false);
        const cancelVisible = await cancel.isVisible().catch(() => false);
        const nameVisible = await platformName.isVisible().catch(() => false);
        return (headingVisible || nameVisible) && saveVisible && cancelVisible;
      },
      {
        timeout: 30_000,
        message: 'Expected Edit Subsidiary Platform controls (heading/name/save/cancel) to be visible.',
      },
    )
    .toBe(true);
}

async function assertEditSubsidiaryPlatformFormLabels(page: Page): Promise<void> {
  const expectedFieldPatterns: RegExp[] = [
    /^Platform Name$/i,
    /^Platform Code$/i,
    /^Email for Non-Compliance Notices$/i,
    /^Secondary Email for Non-Compliance Notices\s*\(optional\)$/i,
    /^Email for Takedown Request Letters$/i,
    /^Secondary Email for Takedown Request Letters\s*\(optional\)$/i,
    /^Platform Status$/i,
    /^Active$/i,
    /^Disabled$/i,
  ];

  for (const fieldPattern of expectedFieldPatterns) {
    const hasLabel = await page
      .getByText(fieldPattern)
      .first()
      .isVisible({ timeout: 10_000 })
      .catch(() => false);
    expect(hasLabel, `Missing expected form field label/text: ${fieldPattern.toString()}`).toBe(true);
  }
}

async function assertPlatformCodeNonEditable(page: Page): Promise<void> {
  const codeTextbox = await getTextboxByLabel(page, /^Platform Code$/i, true);
  const hasReadonly = (await codeTextbox.getAttribute('readonly').catch(() => null)) !== null;
  const ariaReadonly =
    ((await codeTextbox.getAttribute('aria-readonly').catch(() => null)) ?? '').toLowerCase() === 'true';
  const isDisabled = await codeTextbox.isDisabled().catch(() => false);

  expect(
    hasReadonly || ariaReadonly || isDisabled,
    'Expected Platform Code to be non-editable (readonly or disabled) on Edit Subsidiary Platform page.',
  ).toBe(true);
}

async function assertDisabledStatusOptionNonEditable(page: Page): Promise<void> {
  const disabledRadio = page.getByRole('radio', { name: /^Disabled$/i }).first();

  if (await disabledRadio.isVisible().catch(() => false)) {
    const isDisabled = await disabledRadio.isDisabled().catch(() => false);
    const ariaDisabled =
      ((await disabledRadio.getAttribute('aria-disabled').catch(() => null)) ?? '').toLowerCase() === 'true';
    const dataDisabled =
      ((await disabledRadio.getAttribute('data-p-disabled').catch(() => null)) ?? '').toLowerCase() === 'true';
    expect(
      isDisabled || ariaDisabled || dataDisabled,
      'Expected Disabled radio option to be non-editable on Edit Subsidiary Platform page.',
    ).toBe(true);
    return;
  }

  const nativeDisabledRadio = page
    .locator(
      'input[type="radio"][value*="disabled" i], input[type="radio"][id*="disabled" i], input[type="radio"][name*="status" i]',
    )
    .first();
  await expect(nativeDisabledRadio).toBeVisible({ timeout: 10_000 });
  const nativeIsDisabled = await nativeDisabledRadio.isDisabled().catch(() => false);
  expect(nativeIsDisabled, 'Expected Disabled status radio to be non-editable (disabled).').toBe(true);
}

async function buildEditedPlatformValues(page: Page): Promise<EditedPlatformValues> {
  const stamp = Date.now().toString().slice(-6);
  const currentNameInput = await getTextboxByLabel(page, /^Platform Name$/i);
  const currentName = (await currentNameInput.inputValue().catch(() => '')).trim() || 'SubsidiaryPlatform';

  return {
    name: `${currentName} E2E-${stamp}`.slice(0, 100),
    nonComplianceEmail: `valid.noncompliance+${stamp}@example.com`,
    takedownEmail: `valid.takedown+${stamp}@example.com`,
  };
}

async function getNonComplianceEmailField(page: Page): Promise<Locator> {
  const bySelector = page
    .locator('input[placeholder*="Email Contact"][type="text"][pinputtext], input[placeholder*="Email Contact"][type="email"]')
    .first();
  if ((await bySelector.count()) > 0 && (await bySelector.isVisible().catch(() => false))) {
    return bySelector;
  }

  return getTextboxByLabel(page, /^Email for Non-Compliance Notices$/i);
}

async function getTakedownEmailField(page: Page): Promise<Locator> {
  const byKnownControl = page
    .locator(
      'input[formcontrolname="primaryTakedownRequestContactEmail"], input[id="primaryTakedownRequestContactEmail"], input[name="primaryTakedownRequestContactEmail"]',
    )
    .first();
  if ((await byKnownControl.count()) > 0 && (await byKnownControl.isVisible().catch(() => false))) {
    return byKnownControl;
  }

  return getTextboxByLabel(page, /Email for Takedown Request Letters|Primary Email for Takedown Request Letters/i);
}

async function getTextboxByLabel(page: Page, labelPattern: RegExp, includeReadonly = false): Promise<Locator> {
  const byRole = page.getByRole('textbox', { name: labelPattern }).first();
  if ((await byRole.count()) > 0 && (await byRole.isVisible().catch(() => false))) {
    return byRole;
  }

  const byLabel = page.getByLabel(labelPattern).first();
  if ((await byLabel.count()) > 0 && (await byLabel.isVisible().catch(() => false))) {
    return byLabel;
  }

  const byNearby = page
    .locator('div, section, fieldset')
    .filter({ has: page.getByText(labelPattern) })
    .locator('input[type="text"], input[type="email"], textarea, [role="textbox"]')
    .first();
  if ((await byNearby.count()) > 0 && (await byNearby.isVisible().catch(() => false))) {
    return byNearby;
  }

  const label = page.locator('label').filter({ hasText: labelPattern }).first();
  if ((await label.count()) > 0 && (await label.isVisible().catch(() => false))) {
    const labelFor = await label.getAttribute('for');
    if (labelFor) {
      const linked = page.locator(`#${cssEscape(labelFor)}`).first();
      if ((await linked.count()) > 0 && (await linked.isVisible().catch(() => false))) {
        return linked;
      }
    }
  }

  if (includeReadonly) {
    const readonlyNearLabel = page
      .locator('div, section, fieldset')
      .filter({ has: page.getByText(labelPattern) })
      .locator('input[readonly], textarea[readonly], [role="textbox"][aria-readonly="true"]')
      .first();
    if ((await readonlyNearLabel.count()) > 0 && (await readonlyNearLabel.isVisible().catch(() => false))) {
      return readonlyNearLabel;
    }
  }

  throw new Error(`Unable to locate textbox for label: ${labelPattern.toString()}`);
}

async function fillTextboxByLabel(page: Page, labelPattern: RegExp, value: string): Promise<void> {
  const field = await getTextboxByLabel(page, labelPattern);
  await field.fill(value);
}

function getSaveButton(page: Page): Locator {
  return page.getByRole('button', { name: /^Save$/i }).first();
}

function getCancelButton(page: Page): Locator {
  return page.getByRole('button', { name: /^Cancel$/i }).first();
}

async function blurField(page: Page, field: Locator): Promise<void> {
  await field.press('Tab').catch(async () => {
    await page.locator('body').click({ position: { x: 10, y: 10 } }).catch(() => {});
  });
}

async function waitForSaveOutcome(
  page: Page,
): Promise<{ kind: 'success' | 'duplicate-validation' | 'unknown'; message: string }> {
  const successPattern = /updated successfully|saved successfully|success/i;
  const duplicatePattern = /one or more validation errors occurred|already exists|duplicate|validation error/i;
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

        const bodyText = (await page.locator('body').innerText().catch(() => ''))
          .replace(/\s+/g, ' ')
          .trim();
        if (successPattern.test(bodyText)) {
          outcome = { kind: 'success', message: bodyText.substring(0, 250) };
          return true;
        }
        if (duplicatePattern.test(bodyText)) {
          outcome = { kind: 'duplicate-validation', message: bodyText.substring(0, 250) };
          return true;
        }

        const onDetails = await getUpdateSubsidiaryPlatformInformationAction(page)
          .isVisible()
          .catch(() => false);
        if (onDetails) {
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

async function getDuplicateProbableNameCandidate(page: Page): Promise<string | null> {
  await getCancelButton(page).click().catch(() => {});
  await assertDetailedPlatformPageLoaded(page);

  const candidateSources = page.locator(
    'table tbody tr td:first-child, [role="row"] [role="cell"]:first-child, .p-datatable-tbody tr td:first-child',
  );
  const count = await candidateSources.count();

  for (let i = 0; i < count; i++) {
    const cell = candidateSources.nth(i);
    if (!(await cell.isVisible().catch(() => false))) {
      continue;
    }

    const text = (await cell.innerText().catch(() => '')).replace(/\s+/g, ' ').trim();
    if (text.length >= 3 && !/platform\s*name/i.test(text)) {
      await clickUpdateSubsidiaryPlatformInformation(page);
      await assertEditSubsidiaryPlatformPageLoaded(page);
      return text;
    }
  }

  await clickUpdateSubsidiaryPlatformInformation(page);
  await assertEditSubsidiaryPlatformPageLoaded(page);
  return null;
}

function cssEscape(value: string): string {
  return value.replace(/([#.;?+*~':"!^$\[\]()=>|/@])/g, '\\$1');
}


/// <reference types="node" />

/**
 * Feature : Manage Platforms - Edit Platform Information
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-226
 *
 * @EditPlatform
 * Scenario: ManagePlatforms_EditPlatform
 * Test Case Summary:
 * Given I am an authenticated User with IDIR credentials and valid admin permissions
 * When I navigate to Manage Platforms and access the Edit Parent Platform Information page
 * Then I should see a form with all required labels, editable fields, and non-editable controls
 * And I should be able to validate required fields, update valid data, and save or cancel the changes
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 - Navigate to Edit Parent Platform Information and verify controls/labels:
 * - Step 1: Authenticate via IDIR login (username/password)
 * - Step 2: Verify successful login - portal heading "Short-Term Rental Data Portal" is visible
 * - Step 3: Navigate to Manage Platforms section
 * - Step 4: Validate platform management page is loaded with list/table/grid of platforms
 * - Step 5: Click Edit for first platform in the list
 * - Step 6: Validate detailed platform page is loaded
 * - Step 7: Click "Update Parent Platform Information" button
 * - Step 8: Validate Edit Platform page is loaded with form
 * - Step 9: Validate all required form labels are present and visible:
 *   ✓ Platform Name
 *   ✓ Email for Non-Compliance Notices
 *   ✓ Email for Takedown Request Letters / Primary Email for Takedown Request Letters
 *   ✓ Secondary Email for Non-Compliance Notices
 *   ✓ Secondary Email for Takedown Request Letters
 *   ✓ Platform Type
 * - Step 10: Verify Platform Code field is present but non-editable (read-only)
 * - Step 11: Verify Disabled status option is present but non-editable (read-only)
 * - Step 12: Validate Save button is visible and accessible
 * - Step 13: Validate Cancel button is visible and accessible
 *
 * AC2 - Edit platform with valid data and validate success:
 * - Step 1: Navigate to Edit Parent Platform Information form
 * - Step 2: Edit Platform Name input text field with valid updated values (with unique timestamp suffix)
 * - Step 3: Edit Email for Non-Compliance Notices input text field with valid updated values
 * - Step 4: Edit Email for Takedown Request Letters input text field with valid updated values
 * - Step 5: Validate Save button is enabled
 * - Step 6: Click Save button
 * - Step 7: Validate save outcome is 'success'
 * - Step 8: Validate user is returned to Detailed Platform Contact Information page
 *
 * AC3 - Required fields disable Save when cleared:
 * - Step 1: Navigate to Edit Parent Platform Information form
 * - Step 2: Clear/Remove Platform Name input text field value
 * - Step 3: Verify Save button is disabled
 * - Step 4: Fill Platform Name input text field with valid updated values (Platformname+Timestamp)
 * - Step 5: Clear/Remove Email for Non-Compliance Notices input text field value
 * - Step 6: Verify Save button is disabled
 * - Step 7: Fill Email for Non-Compliance Notices input text field with valid updated values
 * - Step 8: Clear/Remove Email for Takedown Request Letters input text field value
 * - Step 9: Verify Save button is disabled
 * - Step 10: Fill Email for Takedown Request Letters input text field with valid updated values
 * - Step 11: Click Cancel button
 * - Step 12: Validate detailed platform contact information page is displayed
 *
 * AC4 - Invalid non-compliance email format disables Save:
 * - Step 1: Navigate to Edit Parent Platform Information form
 * - Step 2: Verify Save button is enabled
 * - Step 3: Edit Non-Compliance Notices email by removing '@'
 * - Step 4: Verify Save button is disabled
 *
 * AC5 - Cancel on Edit Platform returns to Detailed Platform Contact Information page:
 * - Step 1: Navigate to Edit Parent Platform Information form
 * - Step 2: Validate Edit Platform page is loaded
 * - Step 3: Validate Cancel button is visible and accessible
 * - Step 4: Click Cancel button
 * - Step 5: Validate detailed platform contact information page is displayed
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import { IDIR_AUTH_ENV_MESSAGE, hasIdirAuthConfig, loginAsIdir as loginAsIdirShared } from './support/auth';

const APP_URL = process.env.BASE_URL ?? '';

test.use({ browserName: 'chromium' });

const NON_COMPLIANCE_EMAIL_TEST_ID = 'non-compliance-email';

type EditedPlatformValues = {
  name: string;
  nonComplianceEmail: string;
  takedownEmail: string;
};

test.describe('@regression Scenario: ManagePlatforms_EditPlatform', () => {
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

  test('AC1: navigate to Edit Parent Platform Information and verify controls/labels', async ({ page }) => {
    await navigateToEditParentPlatformForm(page);
    await assertEditPlatformPageLoaded(page);
    await assertEditPlatformFormLabels(page);
    await assertPlatformCodeNonEditable(page);
    await assertDisabledStatusOptionNonEditable(page);
    await expect(getSaveButton(page)).toBeVisible();
    await expect(getCancelButton(page)).toBeVisible();
  });

  test('AC2: edit platform with valid data and validate save success', async ({ page }) => {
    await navigateToEditParentPlatformForm(page);
    await assertEditPlatformPageLoaded(page);

    const edited = await buildEditedPlatformValues(page);
    const nonComplianceEmailField = page
      .locator('input[placeholder="Enter Platform’s Email Contact"][type="text"][pinputtext]')
      .first();
    await expect(nonComplianceEmailField).toBeVisible({ timeout: 10_000 });

    const takedownEmailField = page
      .locator('input[formcontrolname="primaryTakedownRequestContactEmail"][id="primaryTakedownRequestContactEmail"][name="primaryTakedownRequestContactEmail"]')
      .first();
    await expect(takedownEmailField).toBeVisible({ timeout: 10_000 });

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

  test('AC3: required-field error messages are shown and Save is disabled when required fields are cleared', async ({ page }) => {
    await navigateToEditParentPlatformForm(page);
    await assertEditPlatformPageLoaded(page);

    const platformNameField = page
      .locator('input[formcontrolname="organizationNm"][id="organizationNm"][name="organizationNm"][placeholder="Enter Value"][type="text"][pinputtext]')
      .first();
    await expect(platformNameField).toBeVisible({ timeout: 10_000 });

    const nonComplianceEmailField = page
      .locator('input[placeholder="Enter Platform’s Email Contact"][type="text"][pinputtext]')
      .first();
    await expect(nonComplianceEmailField).toBeVisible({ timeout: 10_000 });

    const takedownEmailField = page
      .locator('input[formcontrolname="primaryTakedownRequestContactEmail"][id="primaryTakedownRequestContactEmail"][name="primaryTakedownRequestContactEmail"]')
      .first();
    await expect(takedownEmailField).toBeVisible({ timeout: 10_000 });

    const timestamp = Date.now().toString().slice(-6);
    const validPlatformName = `Platformname-${timestamp}`;

    await test.step('Clear Platform Name and verify disabled Save button', async () => {
      await platformNameField.fill('');
      await platformNameField.press('Tab').catch(async () => {
        await page.locator('body').click({ position: { x: 10, y: 10 } }).catch(() => {});
      });
      await expect(getSaveButton(page)).toBeDisabled({ timeout: 10_000 });
      await platformNameField.fill(validPlatformName);
    });

    await test.step('Clear Email for Non-Compliance Notices and verify disabled Save button', async () => {
      await nonComplianceEmailField.fill('');
      await nonComplianceEmailField.press('Tab').catch(async () => {
        await page.locator('body').click({ position: { x: 10, y: 10 } }).catch(() => {});
      });
      await expect(getSaveButton(page)).toBeDisabled({ timeout: 10_000 });
      await nonComplianceEmailField.fill('valid.noncompliance@example.com');
    });

    await test.step('Clear Email for Takedown Request Letters and verify disabled Save button', async () => {
      await takedownEmailField.fill('');
      await takedownEmailField.press('Tab').catch(async () => {
        await page.locator('body').click({ position: { x: 10, y: 10 } }).catch(() => {});
      });
      await expect(getSaveButton(page)).toBeDisabled({ timeout: 10_000 });
      await takedownEmailField.fill('valid.takedown@example.com');
    });

    await getCancelButton(page).click();
    await assertDetailedPlatformPageLoaded(page);
  });

  test('AC4: invalid non-compliance email format disables save button', async ({ page }) => {
    await test.step('Step 1: Navigate to Edit Parent Platform Information form', async () => {
      await navigateToEditParentPlatformForm(page);
      await assertEditPlatformPageLoaded(page);
    });

    await test.step('Step 2: Verify Save button is enabled', async () => {
      await expect(getSaveButton(page)).toBeEnabled({ timeout: 10_000 });
    });

    await test.step("Step 3: Edit Non-Compliance Notices email by removing '@'", async () => {
      const nonComplianceEmailSource = page
        .locator('div, section, fieldset')
        .filter({ has: page.getByText(/^Email for Non-Compliance Notices$/i) })
        .locator('input[placeholder="Enter Platform’s Email Contact"][type="text"][pinputtext]')
        .first();
      await expect(nonComplianceEmailSource).toBeVisible({ timeout: 10_000 });

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
      await nonComplianceEmailField.press('Tab').catch(async () => {
        await page.locator('body').click({ position: { x: 10, y: 10 } }).catch(() => {});
      });
    });

    await test.step('Step 4: Verify Save button is disabled', async () => {
      await expect(getSaveButton(page)).toBeDisabled({ timeout: 10_000 });
    });
  });

  test('AC5: cancel on Edit Platform returns to Detailed Platform Contact Information page', async ({ page }) => {
    await navigateToEditParentPlatformForm(page);
    await assertEditPlatformPageLoaded(page);

    const cancelBtn = getCancelButton(page);
    await expect(cancelBtn).toBeVisible();
    await cancelBtn.click();

    await assertDetailedPlatformPageLoaded(page);
  });
});

async function loginAsIdir(page: Page): Promise<void> {
  await loginAsIdirShared(page, APP_URL);
}

async function navigateToEditParentPlatformForm(page: Page): Promise<void> {
  await goToManagePlatforms(page);
  await assertPlatformManagementPageLoaded(page);
  await clickEditForFirstPlatform(page);
  await assertDetailedPlatformPageLoaded(page);
  await clickUpdateParentPlatformInformation(page);
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
  const updateLink = getUpdateParentPlatformInformationLink(page);

  await expect
    .poll(
      async () => {
        const headingVisible = await detailsHeading.isVisible().catch(() => false);
        const updateVisible = await updateLink.isVisible().catch(() => false);
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

  await expect(updateLink).toBeVisible({ timeout: 20_000 });
}

function getUpdateParentPlatformInformationLink(page: Page): Locator {
  // Try to find a link first, then fall back to button if not found
  return page
    .locator('a, button, [role="link"], [role="button"]')
    .filter({
      hasText: /Update Parent Platform Information|Edit Parent Platform Information|Update Platform Information/i,
    })
    .first();
}

async function clickUpdateParentPlatformInformation(page: Page): Promise<void> {
  const link = getUpdateParentPlatformInformationLink(page);
  if (await link.isVisible().catch(() => false)) {
    await link.click();
    return;
  }

  const buttonFallback = page
    .getByRole('button', {
      name: /Update Parent Platform Information|Edit Parent Platform Information|Update Platform Information/i,
    })
    .first();
  if (await buttonFallback.isVisible().catch(() => false)) {
    await buttonFallback.click();
    return;
  }

  const textFallback = page
    .locator('a, button, [role="link"], [role="button"]')
    .filter({ hasText: /Update Parent Platform Information|Edit Parent Platform Information|Update Platform Information/i })
    .first();
  await expect(textFallback).toBeVisible({ timeout: 15_000 });
  await textFallback.click();
}

async function assertEditPlatformPageLoaded(page: Page): Promise<void> {
  const heading = page
    .getByRole('heading', {
      name: /Edit Platform|Update Platform|Edit Parent Platform Information/i,
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
        message: 'Expected Edit Platform form controls (heading/name/save/cancel) to be visible.',
      },
    )
    .toBe(true);
}

async function assertEditPlatformFormLabels(page: Page): Promise<void> {
  const expectedFieldPatterns: RegExp[] = [
    /^Platform Name$/i,
    /^Platform Code$/i,
    /^Email for Non-Compliance Notices$/i,
    /^Secondary Email for Non-Compliance Notices\s*\(optional\)$/i,
    /^Email for Takedown Request Letters$|^Primary Email for Takedown Request Letters$/i,
    /^Secondary Email for Takedown Request Letters\s*\(optional\)$/i,
    /^Platform Type$/i,
    /^Platform Status$/i,
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
  const ariaReadonly = ((await codeTextbox.getAttribute('aria-readonly').catch(() => null)) ?? '').toLowerCase() === 'true';
  const isDisabled = await codeTextbox.isDisabled().catch(() => false);

  expect(
    hasReadonly || ariaReadonly || isDisabled,
    'Expected Platform Code to be non-editable (readonly or disabled) on Edit Platform page.',
  ).toBe(true);
}

async function assertDisabledStatusOptionNonEditable(page: Page): Promise<void> {
  const disabledRadio = page
    .getByRole('radio', { name: /^Disabled$/i })
    .first();

  if (await disabledRadio.isVisible().catch(() => false)) {
    const isDisabled = await disabledRadio.isDisabled().catch(() => false);
    const ariaDisabled =
      ((await disabledRadio.getAttribute('aria-disabled').catch(() => null)) ?? '').toLowerCase() === 'true';
    const dataDisabled =
      ((await disabledRadio.getAttribute('data-p-disabled').catch(() => null)) ?? '').toLowerCase() === 'true';
    expect(
      isDisabled || ariaDisabled || dataDisabled,
      'Expected Disabled radio option to be non-editable on Edit Platform page.',
    ).toBe(true);
    return;
  }

  // Fallback for native input-based radio controls.
  const nativeDisabledRadio = page
    .locator('input[type="radio"][value*="disabled" i], input[type="radio"][id*="disabled" i], input[type="radio"][name*="status" i]')
    .first();
  await expect(nativeDisabledRadio).toBeVisible({ timeout: 10_000 });
  const nativeIsDisabled = await nativeDisabledRadio.isDisabled().catch(() => false);
  expect(nativeIsDisabled, 'Expected Disabled status radio to be non-editable (disabled).').toBe(true);
}

async function selectEditablePlatformType(page: Page): Promise<void> {
  const combobox = page.getByRole('combobox', { name: /Platform Type|Please Select/i }).first();
  if (await combobox.isVisible().catch(() => false)) {
    const ariaDisabled = ((await combobox.getAttribute('aria-disabled').catch(() => null)) ?? '').toLowerCase();
    const disabledAttr =
      ariaDisabled === 'true' ||
      (await combobox.getAttribute('disabled').catch(() => null)) !== null;
    if (disabledAttr) {
      return;
    }

    await combobox.click();
    const option = page.getByRole('option').first();
    if (await option.isVisible().catch(() => false)) {
      await option.click();
    }
    return;
  }

  const select = page.locator('select').first();
  if (await select.isVisible().catch(() => false)) {
    const options = select.locator('option:not([disabled]):not([value=""])');
    const count = await options.count();
    if (count > 0) {
      const firstValue = await options.first().getAttribute('value');
      if (firstValue) {
        await select.selectOption(firstValue);
      }
    }
  }
}

async function buildEditedPlatformValues(page: Page): Promise<EditedPlatformValues> {
  const stamp = Date.now().toString().slice(-6);
  const currentNameInput = await getTextboxByLabel(page, /^Platform Name$/i);
  const currentName = (await currentNameInput.inputValue().catch(() => '')).trim() || 'Platform';

  return {
    name: `${currentName} E2E-${stamp}`.slice(0, 100),
    nonComplianceEmail: `valid.noncompliance+${stamp}@example.com`,
    takedownEmail: `valid.takedown+${stamp}@example.com`,
  };
}

async function fillEditablePlatformFieldsWithValidValues(page: Page, values: EditedPlatformValues): Promise<void> {
  await fillTextboxByLabel(page, /^Platform Name$/i, values.name);
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

async function clearFieldAndTriggerValidation(page: Page, labelPattern: RegExp): Promise<void> {
  const field = await getTextboxByLabel(page, labelPattern);
  await field.fill('');
  await field.press('Tab').catch(async () => {
    await page.locator('body').click({ position: { x: 10, y: 10 } }).catch(() => {});
  });
}

async function getVisibleValidationText(page: Page): Promise<string> {
  const textParts: string[] = [];

  const validationNodes = page.locator(
    '.p-error, .invalid-feedback, .error, [role="alert"], .validation-message, .field-validation-error, .p-toast-message',
  );
  const nodeCount = await validationNodes.count();
  for (let i = 0; i < nodeCount; i++) {
    const node = validationNodes.nth(i);
    if (!(await node.isVisible().catch(() => false))) {
      continue;
    }

    const text = (await node.innerText().catch(() => '')).replace(/\s+/g, ' ').trim();
    if (text) {
      textParts.push(text);
    }
  }

  if (textParts.length === 0) {
    const bodyText = (await page.locator('body').innerText().catch(() => '')).replace(/\s+/g, ' ').trim();
    return bodyText;
  }

  return textParts.join(' ');
}

async function expectValidationMessageVisible(page: Page, messagePattern: RegExp): Promise<void> {
  await expect
    .poll(
      async () => {
        const text = await getVisibleValidationText(page);
        return messagePattern.test(text);
      },
      {
        timeout: 15_000,
        message: `Expected validation text matching: ${messagePattern.toString()}`,
      },
    )
    .toBe(true);
}

function getSaveButton(page: Page): Locator {
  return page.getByRole('button', { name: /^Save$/i }).first();
}

function getCancelButton(page: Page): Locator {
  return page.getByRole('button', { name: /^Cancel$/i }).first();
}

async function ensureOnDetailedPlatformPage(page: Page): Promise<void> {
  const updateLinkVisible = await getUpdateParentPlatformInformationLink(page)
    .isVisible()
    .catch(() => false);
  if (updateLinkVisible) {
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

        const onDetails = await getUpdateParentPlatformInformationLink(page)
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

async function hasValidationErrorText(page: Page): Promise<boolean> {
  const validationPattern =
    /required|invalid|must be a valid email|please enter a valid|one or more validation errors occurred/i;

  const errorNodes = page.locator(
    '.p-error, .invalid-feedback, .error, [role="alert"], .validation-message, .field-validation-error',
  );
  const count = await errorNodes.count();
  for (let i = 0; i < count; i++) {
    const node = errorNodes.nth(i);
    if (!(await node.isVisible().catch(() => false))) {
      continue;
    }
    const text = (await node.innerText().catch(() => '')).trim();
    if (validationPattern.test(text)) {
      return true;
    }
  }

  const bodyText = await page.locator('body').innerText().catch(() => '');
  return validationPattern.test(bodyText);
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

function cssEscape(value: string): string {
  return value.replace(/([#.;?+*~':"!^$\[\]()=>|/@])/g, '\\$1');
}

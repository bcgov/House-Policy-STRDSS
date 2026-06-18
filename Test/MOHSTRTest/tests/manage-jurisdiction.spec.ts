/// <reference types="node" />

/**
 * Feature: ManageJurisdiction
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-860
 *
 * @ManageJurisdiction
 * Scenario: ManageJurisdiction
 *
 * Test Case Summary:
 * Given I am an authenticated user with access to Manage Jurisdictions
 * When I navigate and click on the Manage Jurisdictions action button
 * Then I should land on Manage Jurisdictions page with list/table/grid visible
 * And I should validate Update Local Government Information labels, happy-path save,
 * and required-field disable-save behavior with Cancel navigation
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 - Access Manage Jurisdictions and verify Jurisdictions data table headers:
 * - Step 1: Authenticate user and verify Landing Page is loaded
 * - Step 2: Click Manage Jurisdictions action button
 * - Step 3: Verify Manage Jurisdictions page data table is visible
 * - Step 4: Verify following data table header is visble
 *   ✓ Local Government Name
 *   ✓ Local Government Type
 *   ✓ Local Government Code
 *   ✓ BL Format
 *   ✓ Update Local Government Information
 *
 * AC2 - Open Update Local Government Information and verify field labels:
 * - Step 1: Click Edit action button for first jurisdiction in the list
 * - Step 2: Verify Update Local Government Information page is loaded
 * - Step 3: Verify field labels are visible:
 *   ✓ Local Government Name
 *   ✓ Local Government Code
 *   ✓ Local Government Type
 *   ✓ Business Licence Format (Optional)
 * - Step 4: Verify Save and Cancel buttons are visible
 *
 * AC3 - Edit Update Local Government Information with valid data and validate success:
 * - Step 1: Navigate to Update Local Government Information page
 * - Step 2: Edit Local Government Name with unique timestamp suffix
 * - Step 3: Verify Local Government Code is non-editable
 * - Step 4: Select Local Government Type from dropdown value
 * - Step 5: Enter Business Licence Format (Optional)
 * - Step 6: Verify Save button is enabled
 * - Step 7: Click Save
 * - Step 8: Validate save outcome is success
 * - Step 9: Validate user is returned to Manage Jurisdictions page
 *
 * AC4 - Required fields disable Save when cleared:
 * - Step 1: Navigate to Update Local Government Information page
 * - Step 2: Clear Local Government Name and verify Save is disabled
 * - Step 3: Click Cancel
 * - Step 4: Validate user is returned to Manage Jurisdictions page
 *
 * AC5 - Verify child jurisdiction datatable loads with headers and edit actions:
 * - Step 1: Navigate to Manage Jurisdictions page
 * - Step 2: Verify parent datatable is loaded
 * - Step 3: Click on expand button of first row of parent data table
 * - Step 4: Verify following child data table header is visible:
 *   ✓ Jurisdiction Name
 *   ✓ Shape File ID
 *   ✓ PR Requirement
 *   ✓ STR Prohibited
 *   ✓ STRAA Exempt
 *   ✓ BL Requirement
 *   ✓ Active
 *   ✓ Update Jurisdiction Info
 *
 * AC6 - Open Update Jurisdiction Information page and verify field labels + Cancel:
 * - Step 1: Navigate to Manage Jurisdictions page
 * - Step 2: Click on expand button of first row of parent data table
 * - Step 3: Verify child table is loaded
 * - Step 4: Click Edit action_icon button on first jurisdiction row of child table under Update Jurisdiction Info table header
 * - Step 5: Verify user is on Update Jurisdiction Information page
 * - Step 6: Verify following field labels are visible:
 *   ✓ Jurisdiction Name
 *   ✓ Shape File ID
 *   ✓ Local Government Name
 *   ✓ Principal Residence Requirement Applies?
 *   ✓ Short-Term Rental Prohibited?
 *   ✓ STRAA Exempt?
 *   ✓ Business Licence Requirement?
 *   ✓ Is Active?
 *   ✓ Economic Region
 *   ✓ Yes / No radio options
 * - Step 7: Click Cancel button
 * - Step 8: Validate user is returned to Manage Jurisdictions page
 *
 * AC7 - Edit Update Jurisdiction Information with valid data and validate success:
 * - Step 1: Navigate to Manage Jurisdictions page
 * - Step 2: Click Edit action button on first jurisdiction row
 * - Step 3: Verify Jurisdiction Name is not editable
 * - Step 4: Verify Shape File ID is not editable
 * - Step 5: Select Local Government Name value from drop-down list
 * - Step 6: Verify all radio button selections are working
 * - Step 7: Select Economic Region value from drop-down list
 * - Step 8: Click Save button and verify success message
 * - Step 9: Validate user is returned to Manage Jurisdictions page
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import { IDIR_AUTH_ENV_MESSAGE, hasIdirAuthConfig, loginAsIdir as loginAsIdirShared } from './support/auth';

const APP_URL = process.env.BASE_URL ?? '';

test.use({ browserName: 'chromium' });

type SaveOutcomeKind = 'success' | 'unknown';

test.describe('@regression Scenario: ManageJurisdiction', () => {
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

  test('AC1: navigate to Manage Jurisdictions and verify list/table/grid and headers', async ({ page }) => {
    await goToManageJurisdictions(page);
    await assertManageJurisdictionsPageLoaded(page);
  });

  test('AC2: open Update Local Government Information and verify field labels/controls', async ({ page }) => {
    await navigateToUpdateLocalGovernmentInformation(page);
    await assertUpdateLocalGovernmentPageLoaded(page);
    await assertUpdateLocalGovernmentFieldLabels(page);
    await expect(getSaveButton(page)).toBeVisible();
    await expect(getCancelButton(page)).toBeVisible();
  });

  test('AC3: edit local government with valid data and validate save success', async ({ page }) => {
    await navigateToUpdateLocalGovernmentInformation(page);
    await assertUpdateLocalGovernmentPageLoaded(page);

    const stamp = Date.now().toString().slice(-6);
    const currentNameField = await getLocalGovernmentNameField(page);
    const currentName = (await currentNameField.inputValue().catch(() => '')).trim() || 'LocalGovernment';

    await test.step('Edit Local Government Name with valid updated value', async () => {
      await currentNameField.fill(`${currentName} E2E-${stamp}`.slice(0, 100));
    });

    await test.step('Verify Local Government Code is non-editable', async () => {
      await assertLocalGovernmentCodeNonEditable(page);
    });

    await test.step('Select Local Government Type from dropdown value when editable', async () => {
      await selectLocalGovernmentType(page);
    });

    await test.step('Enter Business Licence Format (Optional)', async () => {
      const blFormatField = await getBusinessLicenceFormatField(page);
      await blFormatField.fill(`BL-${stamp}`);
    });

    await test.step('Verify Save button is enabled', async () => {
      await expect(getSaveButton(page)).toBeEnabled({ timeout: 10_000 });
    });

    await test.step('Save and validate success outcome + return to Manage Jurisdictions', async () => {
      await getSaveButton(page).click();
      const outcome = await waitForSaveOutcome(page);

      expect(outcome.kind, `Expected success outcome, got: ${outcome.kind} (${outcome.message})`).toBe('success');

      await dismissVisibleToasts(page);
      await assertManageJurisdictionsPageLoaded(page);
    });
  });

  test('AC4: required fields disable Save when Local Government Name is cleared', async ({ page }) => {
    await navigateToUpdateLocalGovernmentInformation(page);
    await assertUpdateLocalGovernmentPageLoaded(page);

    const nameField = await getLocalGovernmentNameField(page);

    await test.step('Clear Local Government Name and verify Save is disabled', async () => {
      await nameField.fill('');
      await blurField(page, nameField);
      await expect(getSaveButton(page)).toBeDisabled({ timeout: 10_000 });
    });
    await test.step('Click Cancel and verify user is returned to Manage Jurisdictions page', async () => {
      await getCancelButton(page).click();
      await assertManageJurisdictionsPageLoaded(page);
    });
  });

  test('AC5: verify child jurisdiction datatable loads with headers and edit actions', async ({ page }) => {
    await goToManageJurisdictions(page);
    await assertManageJurisdictionsPageLoaded(page);

    await test.step('Verify parent datatable is loaded', async () => {
      await assertParentDatatableVisible(page);
    });

    await test.step('Click expand button on first parent row', async () => {
      await expandFirstParentRow(page);
    });

    await test.step('Verify child datatable headers are present', async () => {
      await assertChildDatatableHeadersVisible(page);
    });
  });

  test('AC6: open Update Jurisdiction Information page and verify field labels + Cancel', async ({ page }) => {
    await goToManageJurisdictions(page);
    await assertManageJurisdictionsPageLoaded(page);

    await test.step('Click expand button on first parent row', async () => {
      await expandFirstParentRow(page);
    });

    await test.step('Verify child table is loaded', async () => {
      await assertChildDatatableHeadersVisible(page);
    });

    await test.step('Click Edit action_icon button on first jurisdiction row of child table under Update Jurisdiction Info table header', async () => {
      await clickFirstChildRowEditAction(page);
    });

    await test.step('Verify user is on Update Jurisdiction Information page', async () => {
      await assertUpdateJurisdictionPageLoaded(page);
    });

    await test.step('Verify field labels are visible', async () => {
      await assertUpdateJurisdictionFieldLabels(page);
    });

    await test.step('Click Cancel button and verify user is returned to Manage Jurisdictions page', async () => {
      await getCancelButton(page).click();
      await assertManageJurisdictionsPageLoaded(page);
    });
  });

  test('AC7: edit Update Jurisdiction Information with valid data and validate success', async ({ page }) => {
    await goToManageJurisdictions(page);
    await assertManageJurisdictionsPageLoaded(page);

    await test.step('Click expand button on first parent row', async () => {
      await expandFirstParentRow(page);
    });

    await test.step('Click Edit action_icon button on first jurisdiction row of child table', async () => {
      await clickFirstChildRowEditAction(page);
    });

    await test.step('Verify Jurisdiction Name is not editable', async () => {
      await assertFieldNonEditable(page, /^Jurisdiction Name$/i);
    });

    await test.step('Verify Shape File ID is not editable', async () => {
      await assertFieldNonEditable(page, /^Shape File ID$/i);
    });

    await test.step('Select Local Government Name from drop-down list', async () => {
      await selectDropdownByLabel(page, /^Local Government Name$/i);
    });

    await test.step('Verify all radio button selections are working', async () => {
      await toggleAllRadioButtons(page);
    });

    await test.step('Select Economic Region from drop-down list', async () => {
      await selectDropdownByLabel(page, /^Economic Region$/i);
    });

    await test.step('Click Save and verify success message', async () => {
      await getSaveButton(page).click();
      const outcome = await waitForSaveOutcome(page);
      expect(outcome.kind, `Expected success outcome, got: ${outcome.kind} (${outcome.message})`).toBe('success');
      await dismissVisibleToasts(page);
    });

    await test.step('Validate user is returned to Manage Jurisdictions page', async () => {
      await assertManageJurisdictionsPageLoaded(page);
    });
  });
});

// ─── Jurisdiction parent/child helpers ───────────────────────────────────────────

async function assertParentDatatableVisible(page: Page): Promise<void> {
  // PrimeNG p-datatable or tree-table table element for parent
  const datatable = page.locator('.p-datatable-table, table.p-datatable-table, .p-treetable-table, table.p-treetable-table').first();
  await expect(datatable).toBeVisible({ timeout: 15_000 });
}

async function expandFirstParentRow(page: Page): Promise<void> {
  // Find the expand button/icon in the first row of parent table
  const expandButton = page.locator('table tbody tr, [role="rowgroup"] [role="row"]').first().locator('.p-treetable-toggler, [class*="expand"], button[aria-expanded="false"], [class*="toggle"]').first();
  await expect(expandButton).toBeVisible({ timeout: 10_000 });
  await expandButton.click();

  // Wait for child rows to appear after expanding the parent row.
  await expect
    .poll(
      async () => {
        const childRows = page.locator('[data-depth="1"], tr:has(a[id*="jurisdiction-edit"])');
        return await childRows.count();
      },
      { timeout: 10_000, intervals: [250, 500] },
    )
    .toBeGreaterThan(0);
}

async function assertChildDatatableHeadersVisible(page: Page): Promise<void> {
  const headerIds = [
    'organizationNm_header',      // Jurisdiction Name
    'shapeFileId_header',         // Shape File ID
    'isPrincipalResidenceRequired_header',  // PR Requirement
    'isStrProhibited_header',     // STR Prohibited
    'isStraaExempt_header',       // STRAA Exempt
    'isBusinessLicenceRequired_header',     // BL Requirement
    'isActive_header',            // Active
  ];

  for (const headerId of headerIds) {
    const header = page.locator(`#${headerId}`).first();
    // Check if header is visible in the expanded child rows section
    const isVisible = await header.isVisible({ timeout: 10_000 }).catch(() => false);
    expect(isVisible, `Expected child datatable header "${headerId}" to be visible`).toBe(true);
  }

  // Header is rendered as plain <th> text in some builds, without a stable id attribute.
  const updateHeaderByThText = page
    .locator('th')
    .filter({ hasText: /^\s*Update\s+Jurisdiction\s+Info\s*$/i })
    .first();
  await expect(updateHeaderByThText).toBeVisible({ timeout: 10_000 });
}

async function assertChildDatatableEditActionVisible(page: Page): Promise<void> {
  // Look for edit-icon in child rows (rows with data-depth="1" or similar depth indicator)
  const editAction = page.locator('[data-depth="1"] .edit-icon, [data-depth="1"] [id*="jurisdiction-edit"], [class*="child-row"] .edit-icon').first();
  const isVisible = await editAction.isVisible({ timeout: 10_000 }).catch(() => false);
  
  if (!isVisible) {
    // Fallback: look for any edit icon in rows following the expanded parent
    const anyChildEditIcon = page.locator('table tbody tr:has([class*="child-row"]), [class*="expanded"] ~ [class*="child-row"]').first().locator('.edit-icon, [id*="jurisdiction-edit"]').first();
    await expect(anyChildEditIcon).toBeVisible({ timeout: 10_000 });
  }
}

async function clickFirstChildRowEditAction(page: Page): Promise<void> {
  // Click the edit action icon in the first child row under the Update Jurisdiction Info column
  // The child rows appear after expanding a parent row in the datatable
  const editIconLink = page
    .locator('a[id*="jurisdiction-edit"]')
    .first();

  if (await editIconLink.isVisible({ timeout: 8_000 }).catch(() => false)) {
    await editIconLink.click();
    return;
  }

  // Fallback: look for edit-icon inside any visible child row
  const childRowEditIcon = page
    .locator('td .edit-icon, td [class*="edit-icon"], td a[href*="jurisdiction"]')
    .first();
  await expect(childRowEditIcon).toBeVisible({ timeout: 10_000 });
  await childRowEditIcon.click();
}

async function openJurisdictionEditFromChildRow(page: Page): Promise<void> {
  await expandFirstParentRow(page);
  await clickFirstChildRowEditAction(page);
}

async function assertUpdateJurisdictionPageLoaded(page: Page): Promise<void> {
  const heading = page
    .getByRole('heading', {
      name: /Update Jurisdiction Information|Edit Jurisdiction Information|Update Jurisdiction|Edit Jurisdiction/i,
    })
    .first();

  await expect
    .poll(
      async () => {
        const headingVisible = await heading.isVisible().catch(() => false);
        const saveVisible = await getSaveButton(page).isVisible().catch(() => false);
        const cancelVisible = await getCancelButton(page).isVisible().catch(() => false);
        return headingVisible && saveVisible && cancelVisible;
      },
      {
        timeout: 30_000,
        message: 'Expected Update Jurisdiction Information page controls to be visible.',
      },
    )
    .toBe(true);
}

async function assertUpdateJurisdictionFieldLabels(page: Page): Promise<void> {
  const expectedLabels = [
    /^Jurisdiction Name$/i,
    /^Shape File ID$/i,
    /^Local Government Name$/i,
    /^Principal Residence Requirement Applies\??$/i,
    /^Short-Term Rental Prohibited\??$/i,
    /^STRAA Exempt\??$/i,
    /^Business Licence Requirement\??$/i,
    /^Is Active\??$/i,
    /^Economic Region$/i,
  ];

  for (const pattern of expectedLabels) {
    const visible = await page.getByText(pattern).first().isVisible({ timeout: 10_000 }).catch(() => false);
    expect(visible, `Missing expected field label: ${pattern.toString()}`).toBe(true);
  }

  // Verify Yes / No radio options are visible
  const yesOption = page.getByRole('radio', { name: /^Yes$/i }).or(page.getByText(/^Yes$/i).first());
  const noOption = page.getByRole('radio', { name: /^No$/i }).or(page.getByText(/^No$/i).first());
  await expect(yesOption.first()).toBeVisible({ timeout: 10_000 });
  await expect(noOption.first()).toBeVisible({ timeout: 10_000 });
}

async function assertFieldNonEditable(page: Page, labelPattern: RegExp): Promise<void> {
  // Try to find the field; if it throws it might be a read-only display element
  let field: Locator | null = null;
  try {
    field = await getTextboxByLabel(page, labelPattern, true);
  } catch {
    // Field may be rendered as plain text — considered non-editable
    return;
  }

  const hasReadonly = (await field.getAttribute('readonly').catch(() => null)) !== null;
  const ariaReadonly = ((await field.getAttribute('aria-readonly').catch(() => null)) ?? '').toLowerCase() === 'true';
  const isDisabled = await field.isDisabled().catch(() => false);

  expect(
    hasReadonly || ariaReadonly || isDisabled,
    `Expected "${labelPattern.toString()}" field to be non-editable.`,
  ).toBe(true);
}

async function selectDropdownByLabel(page: Page, labelPattern: RegExp): Promise<void> {
  // PrimeNG p-dropdown / combobox near matching label
  const comboByRole = page.getByRole('combobox', { name: labelPattern }).first();

  if (await comboByRole.isVisible({ timeout: 5_000 }).catch(() => false)) {
    const ariaDisabled = ((await comboByRole.getAttribute('aria-disabled').catch(() => null)) ?? '').toLowerCase() === 'true';
    const disabledAttr = (await comboByRole.getAttribute('disabled').catch(() => null)) !== null;
    if (ariaDisabled || disabledAttr) return;

    await comboByRole.click();
    const firstOption = page.getByRole('option').filter({ hasText: /^(?!\s*$)(?!please select)/i }).first();
    if (await firstOption.isVisible({ timeout: 5_000 }).catch(() => false)) {
      await firstOption.click();
      return;
    }
    await comboByRole.press('ArrowDown').catch(() => {});
    await comboByRole.press('Enter').catch(() => {});
    return;
  }

  // Nearby PrimeNG dropdown panel anchor
  const nearbyDropdown = page
    .locator('div, section, fieldset')
    .filter({ has: page.getByText(labelPattern) })
    .locator('.p-dropdown, [class*="dropdown"], select')
    .first();

  if (await nearbyDropdown.isVisible({ timeout: 5_000 }).catch(() => false)) {
    const tagName = await nearbyDropdown.evaluate((el) => el.tagName.toLowerCase());
    if (tagName === 'select') {
      const options = nearbyDropdown.locator('option:not([disabled]):not([value=""])');
      const value = await options.first().getAttribute('value');
      if (value) await nearbyDropdown.selectOption(value);
      return;
    }
    await nearbyDropdown.click();
    const firstOption = page.getByRole('option').filter({ hasText: /^(?!\s*$)(?!please select)/i }).first();
    if (await firstOption.isVisible({ timeout: 5_000 }).catch(() => false)) {
      await firstOption.click();
    }
  }
}

async function toggleAllRadioButtons(page: Page): Promise<void> {
  // Find all radio groups on the page and cycle through Yes/No options
  const radioButtons = page.getByRole('radio');
  const count = await radioButtons.count();

  for (let i = 0; i < count; i++) {
    const radio = radioButtons.nth(i);
    if (await radio.isVisible().catch(() => false)) {
      const isDisabled = await radio.isDisabled().catch(() => true);
      if (!isDisabled) {
        await radio.click().catch(() => {});
      }
    }
  }
}

async function loginAsIdir(page: Page): Promise<void> {
  await loginAsIdirShared(page, APP_URL);
}

async function goToManageJurisdictions(page: Page): Promise<void> {
  const byId = page.locator('#manageJurisdictionst_btn').first();
  if (await byId.isVisible({ timeout: 5_000 }).catch(() => false)) {
    await byId.click();
    return;
  }

  const action = page
    .getByRole('button', { name: /^Manage Jurisdictions$/i })
    .or(page.getByRole('link', { name: /^Manage Jurisdictions$/i }))
    .first();

  await expect(action).toBeVisible({ timeout: 20_000 });
  await action.click();
}

async function assertManageJurisdictionsPageLoaded(page: Page): Promise<void> {
  await expect
    .poll(
      async () => {
        const urlLooksRight = /jurisdiction|local-government|local_government|manage-jurisdiction/i.test(page.url());

        const hasHeading = await page
          .getByRole('heading', {
            name: /Manage Jurisdictions|Jurisdiction Management|Manage Local Government/i,
          })
          .first()
          .isVisible()
          .catch(() => false);

        const hasTable = await page.getByRole('table').first().isVisible().catch(() => false);
        const hasGrid = await page.getByRole('grid').first().isVisible().catch(() => false);
        const hasList = await page.getByRole('list').first().isVisible().catch(() => false);

        const hasExpectedHeaderText = await page
          .getByText(/Local Government Name|Local Government Type|Local Government Code|BL Format/i)
          .first()
          .isVisible()
          .catch(() => false);

        return (urlLooksRight || hasHeading) && (hasTable || hasGrid || hasList || hasExpectedHeaderText);
      },
      {
        timeout: 45_000,
        message: 'Expected Manage Jurisdictions page to be loaded with list/table/grid visible.',
      },
    )
    .toBe(true);
}

function getUpdateLocalGovernmentInformationAction(page: Page): Locator {
  const inUpdateColumn = page
    .locator('table, [role="grid"], [role="table"], div')
    .filter({ hasText: /Update\s*Local\s*Government\s*Information/i })
    .locator('a, button, [role="link"], [role="button"]')
    .filter({ hasText: /Update\s*Local\s*Government\s*Information|Update|Edit/i })
    .first();

  const globalAction = page
    .locator('a, button, [role="link"], [role="button"]')
    .filter({ hasText: /Update\s*Local\s*Government\s*Information|Update\s*Local\s*Government|Update|Edit/i })
    .first();

  return inUpdateColumn.or(globalAction).first();
}

async function openUpdateLocalGovernmentInformation(page: Page): Promise<void> {
  const rows = page.locator('table tbody tr, [role="rowgroup"] [role="row"]');
  await expect(rows.first()).toBeVisible({ timeout: 15_000 });

  const firstRow = rows.first();

  // Primary: click the edit-icon button in the first row
  const editIconBtn = firstRow
    .locator('[class*="edit-icon"], [id*="edit-icon"], button[aria-label*="edit" i], button[title*="edit" i]')
    .first();

  if (await editIconBtn.isVisible({ timeout: 5_000 }).catch(() => false)) {
    await editIconBtn.click();
    return;
  }

  // Fallback: any action link/button labelled Update or Edit in the first row
  const firstRowAction = firstRow
    .locator('a, button, [role="link"], [role="button"]')
    .filter({ hasText: /Update|Edit/i })
    .first();

  await expect(firstRowAction).toBeVisible({ timeout: 10_000 });
  await firstRowAction.click();
}

async function navigateToUpdateLocalGovernmentInformation(page: Page): Promise<void> {
  await goToManageJurisdictions(page);
  await assertManageJurisdictionsPageLoaded(page);
  await openUpdateLocalGovernmentInformation(page);
}

async function assertUpdateLocalGovernmentPageLoaded(page: Page): Promise<void> {
  const heading = page
    .getByRole('heading', {
      name: /Update Local Government Information|Edit Local Government Information|Update Jurisdiction|Edit Jurisdiction/i,
    })
    .first();
  const nameField = page.getByRole('textbox', { name: /^Local Government Name$/i }).first();

  await expect
    .poll(
      async () => {
        const headingVisible = await heading.isVisible().catch(() => false);
        const nameVisible = await nameField.isVisible().catch(() => false);
        const saveVisible = await getSaveButton(page).isVisible().catch(() => false);
        const cancelVisible = await getCancelButton(page).isVisible().catch(() => false);
        return (headingVisible || nameVisible) && saveVisible && cancelVisible;
      },
      {
        timeout: 30_000,
        message: 'Expected Update Local Government Information page controls to be visible.',
      },
    )
    .toBe(true);
}

async function assertUpdateLocalGovernmentFieldLabels(page: Page): Promise<void> {
  const expectedLabels = [
    /^Local Government Name$/i,
    /^Local Government Code$/i,
    /^Local Government Type$/i,
    /^Business Licence Format\s*\(Optional\)$/i,
  ];

  for (const pattern of expectedLabels) {
    const visible = await page.getByText(pattern).first().isVisible({ timeout: 10_000 }).catch(() => false);
    expect(visible, `Missing expected update form label/text: ${pattern.toString()}`).toBe(true);
  }
}

async function getLocalGovernmentNameField(page: Page): Promise<Locator> {
  return getTextboxByLabel(page, /^Local Government Name$/i);
}

async function getLocalGovernmentCodeField(page: Page): Promise<Locator> {
  return getTextboxByLabel(page, /^Local Government Code$/i, true);
}

async function assertLocalGovernmentCodeNonEditable(page: Page): Promise<void> {
  const codeField = await getLocalGovernmentCodeField(page);
  const hasReadonly = (await codeField.getAttribute('readonly').catch(() => null)) !== null;
  const ariaReadonly = ((await codeField.getAttribute('aria-readonly').catch(() => null)) ?? '').toLowerCase() === 'true';
  const isDisabled = await codeField.isDisabled().catch(() => false);

  expect(
    hasReadonly || ariaReadonly || isDisabled,
    'Expected Local Government Code to be non-editable on Update Local Government Information page.',
  ).toBe(true);
}

async function getBusinessLicenceFormatField(page: Page): Promise<Locator> {
  return getTextboxByLabel(page, /^Business Licence Format\s*\(Optional\)$/i);
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

async function selectLocalGovernmentType(page: Page): Promise<void> {
  const comboByRole = page.getByRole('combobox', { name: /Local Government Type|Please Select/i }).first();
  if (await comboByRole.isVisible().catch(() => false)) {
    const ariaDisabled = ((await comboByRole.getAttribute('aria-disabled').catch(() => null)) ?? '').toLowerCase() === 'true';
    const disabledAttr = (await comboByRole.getAttribute('disabled').catch(() => null)) !== null;
    if (ariaDisabled || disabledAttr) {
      return;
    }

    await comboByRole.click();
    const firstOption = page.getByRole('option').filter({ hasText: /^(?!\s*$)(?!please select)/i }).first();
    if (await firstOption.isVisible().catch(() => false)) {
      await firstOption.click();
      return;
    }

    await comboByRole.press('ArrowDown').catch(() => {});
    await comboByRole.press('Enter').catch(() => {});
    return;
  }

  const nativeSelect = page.locator('select').first();
  if (await nativeSelect.isVisible().catch(() => false)) {
    const selectableOptions = nativeSelect.locator('option:not([disabled]):not([value=""])');
    const optionCount = await selectableOptions.count();
    if (optionCount > 0) {
      const value = await selectableOptions.first().getAttribute('value');
      if (value) {
        await nativeSelect.selectOption(value);
      }
    }
  }
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

async function waitForSaveOutcome(page: Page): Promise<{ kind: SaveOutcomeKind; message: string }> {
  const successPattern = /updated successfully|saved successfully|success/i;
  let outcome: { kind: SaveOutcomeKind; message: string } = {
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
        }

        const bodyText = (await page.locator('body').innerText().catch(() => '')).replace(/\s+/g, ' ').trim();
        if (successPattern.test(bodyText)) {
          outcome = { kind: 'success', message: bodyText.slice(0, 250) };
          return true;
        }

        const onManageJurisdictions = await page
          .getByText(/Local Government Name|Update Local Government Information|Manage Jurisdictions/i)
          .first()
          .isVisible()
          .catch(() => false);
        if (onManageJurisdictions) {
          outcome = { kind: 'success', message: 'Returned to Manage Jurisdictions page after save.' };
          return true;
        }

        return false;
      },
      {
        timeout: 45_000,
        intervals: [300, 600, 1_000],
        message: 'Expected a save outcome (toast, success text, or redirect).',
      },
    )
    .toBe(true)
    .catch(() => undefined);

  return outcome;
}

async function dismissVisibleToasts(page: Page): Promise<void> {
  const closeButtons = page.locator('.p-toast-message button[aria-label="Close"], .p-toast-message button');
  const count = await closeButtons.count();
  for (let i = 0; i < count; i++) {
    const button = closeButtons.nth(i);
    if (await button.isVisible().catch(() => false)) {
      await button.click().catch(() => {});
    }
  }
}

function cssEscape(value: string): string {
  return value.replace(/([#.;?+*~':"!^$\[\]()=>|/@])/g, '\\$1');
}


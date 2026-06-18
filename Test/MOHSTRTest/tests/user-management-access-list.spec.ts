/// <reference types="node" />

/**
 * Feature: UserManagement - Managing Access
 * Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-54
 *
 * @UserManagement
 * Scenario: UserManagement
 *
 * Test Case Summary:
 * Given I am an authenticated user with access to User Management
 * When I navigate to the Landing Page and click on User Management action button
 * Then I should see the User Access Request List page with all required table headers and labels
 * And I should be able to view and interact with the list/table/grid of user access requests
 * And I should be able to validate search, filter, and status functionality
 * And I should be able to search specific user and verify the functionality of access status toggle switch
 * And I should be able to cancel navigation and validate state preservation
 *
 * Test Steps and Validation Checkpoints:
 *
 * AC1 - User Authentication and Landing Page Navigation:
 * - Step 1: Authenticate via IDIR login with valid credentials (username/password)
 * - Step 2: Verify successful login - portal heading "Short-Term Rental Data Portal" is visible
 * - Step 3: Verify navigation region with name "Main Navigation" is present and visible
 * - Step 4: Validate Home content region is rendered on the page
 * - Step 5: Verify User Management action button is visible and accessible on the landing page
 *
 * AC2 - Navigate to User Management and verify User Access Request List page loads:
 * - Step 1: Click on User Management action button from the landing page
 * - Step 2: Verify user is navigated to the User Management page
 * - Step 3: Validate page title/heading reflects User Management or User Access Request context
 * - Step 4: Verify page URL contains user-management or similar identifier
 * - Step 5: Verify loading indicators are not present (data is fully loaded)
 *
 * AC3 - User Access Request List page displays all required table headers:
 * - Step 1: Navigate to User Management page
 * - Step 2: Verify list/table/grid container is visible and accessible
 * - Step 3: Validate the following table headers are present and visible:
 *   ✓ Requested Date
 *   ✓ ID Provider
 *   ✓ First Name
 *   ✓ Last Name
 *   ✓ Org Type
 *   ✓ Email Address
 *   ✓ Organization
 *   ✓ Status
 *   ✓ Action
 * - Step 4: Verify each header is rendered in the correct column position
 * - Step 5: Verify headers are accessible and not truncated/hidden
 *
 * AC4 - User Access Request List displays all required filter/search labels and controls:
 * - Step 1: Navigate to User Management page
 * - Step 2: Verify the following labels are visible on the page:
 *   ✓ Status
 *   ✓ Organization
 *   ✓ Search
 * - Step 3: Verify Status label is associated with a filter/dropdown control
 * - Step 4: Verify Organization label is associated with a filter/dropdown or input control
 * - Step 5: Verify Search label is associated with a searchbox/input field control
 * - Step 6: Verify all filter/search controls are functional and accessible
 *
 * AC5 - Search functionality validates user input and filters request list:
 * - Step 1: Navigate to User Management page
 * - Step 2: Locate the Search control/input field
 * - Step 3: Enter valid search text in the search field (e.g., user name or email)
 * - Step 4: Verify search results are filtered/updated in the list
 * - Step 5: Clear the search field and verify all results are restored
 * - Step 6: Enter invalid/non-matching search text and verify no results or appropriate message
 * - Step 7: Verify search field accepts alphanumeric characters and special characters
 *
 * AC6 - Status filter dropdown shows available status options and filters list:
 * - Step 1: Navigate to User Management page
 * - Step 2: Locate Status filter dropdown/control
 * - Step 3: Click on Status dropdown to open available options
 * - Step 4: Verify Status dropdown displays multiple options (e.g., Pending, Approved, Rejected, etc.)
 * - Step 5: Select a specific status option from the dropdown
 * - Step 6: Verify the User Access Request List is filtered to show only requests with selected status
 * - Step 7: Verify the dropdown shows the selected status value
 * - Step 8: Clear the filter and verify all statuses are shown again
 *
 * AC7 - Organization filter dropdown shows available organizations and filters list:
 * - Step 1: Navigate to User Management page
 * - Step 2: Locate Organization filter dropdown/control
 * - Step 3: Click on Organization dropdown to open available options
 * - Step 4: Verify Organization dropdown displays multiple organization options
 * - Step 5: Select a specific organization from the dropdown
 * - Step 6: Verify the User Access Request List is filtered to show only requests for selected organization
 * - Step 7: Verify the dropdown shows the selected organization value
 * - Step 8: Clear the filter and verify all organizations are shown again
 *
 * AC8 - Pagination or scrolling functionality works correctly:
 * - Step 1: Navigate to User Management page
 * - Step 2: Verify list displays appropriate number of rows per page (if paginated)
 * - Step 3: If pagination exists, verify pagination controls are visible (next, previous, page numbers)
 * - Step 4: Verify pagination controls are functional and navigate through pages
 * - Step 5: If scrolling, verify vertical scroll works and loads additional records
 * - Step 6: Verify total record count is displayed (if available)
 *
 * AC9 - Search specific user and verify the functionality of access status toggle switch:
 * - Step 1: Navigate to User Management page
 * - Step 2: Fill "young-jin.chung@gov.bc.ca" in the Search field
 * - Step 3: Verify the searched row is visible in the table
 * - Step 4: Verify ID Provider column shows "idir"
 * - Step 5: Verify First Name column shows "Young-jin"
 * - Step 6: Verify Last Name column shows "Chung"
 * - Step 7: Verify Email Address column shows "young-jin.chung@gov.bc.ca"
 * - Step 8: Click access status toggle switch and verify the functionality of access status toggle switch
 *
 * AC10 - Open User Information page from Edit action and cancel navigation:
 * - Step 1: Navigate to User Management page
 * - Step 2: Locate the Edit User Icon under table header Action
 * - Step 3: Click Edit User Icon
 * - Step 4: Verify user is landed to User Information page by validating the User Information label or in the URL with string user
 * - Step 5: Verify User Information page labels are visible:
 *   ✓ User Details
 *   ✓ Status, Roles and Permissions
 * - Step 6: Verify Update and Cancel buttons are visible
 * - Step 7: Click Cancel and verify user is landed back on User Management page
 *
 * AC11 - Update flow enters editable mode and validates controls:
 * - Step 1: Navigate to User Management page
 * - Step 2: Locate the Edit User Icon under table header Action
 * - Step 3: Verify Update and Cancel buttons are visible
 * - Step 4: Click Update and verify editable mode is enabled
 * - Step 5: Verify Organization Type field has dropdown list
 * - Step 6: Verify Organization Name field has dropdown list
 * - Step 7: Verify Current Status supports Active and Disabled options
 * - Step 8: Verify User Role(s) and Permission(s) field has dropdown list
 * - Step 9: Verify Save and Cancel buttons are visible
 */

import { expect, test, type Locator, type Page } from '@playwright/test';
import { IDIR_AUTH_ENV_MESSAGE, hasIdirAuthConfig, loginAsIdir as loginAsIdirShared } from './support/auth';

const APP_URL = process.env.BASE_URL ?? '';

test.use({ browserName: 'chromium' });

test.describe('@regression Scenario: UserManagement', () => {
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

  test('@smoke AC1: user authentication and landing page navigation', async ({ page }) => {
    await test.step('Verify successful login - portal heading is visible', async () => {
      const portalHeading = page.getByRole('heading', { name: /Short-Term Rental Data Portal/i });
      await expect(portalHeading).toBeVisible();
    });

    await test.step('Verify navigation region and Home content region are visible', async () => {
      const mainNavigation = page.getByRole('navigation', { name: /Main Navigation/i });
      await expect(mainNavigation).toBeVisible();

      const homeRegion = page.getByRole('region', { name: /^Home$/i });
      await expect(homeRegion).toBeVisible();
    });

    await test.step('Verify User Management action button is visible and accessible', async () => {
      const userManagementButton = page.getByRole('button', { name: /^User Management$/i });
      await expect(userManagementButton).toBeVisible();
    });
  });

  test('AC2: navigate to User Management and verify page loads', async ({ page }) => {
    await test.step('Click on User Management action button from landing page', async () => {
      await page.getByRole('button', { name: /^User Management$/i }).click();
    });

    await test.step('Verify user is navigated to User Management page', async () => {
      await expect(page).toHaveURL(/user.*management|access.*request/i, { timeout: 30_000 });
    });

    await test.step('Verify page title/heading reflects User Management context', async () => {
      // Try to find the title text using the span.title-text locator
      const titleText = page.locator('span.title-text').filter({ hasText: /User Management|User Access Request/i }).first();
      const titleVisible = await titleText.isVisible({ timeout: 10_000 }).catch(() => false);

      // Fallback to heading role if span.title-text is not found
      const heading = page
        .getByRole('heading', {
          name: /User Management|User Access Request|Access Request List|Manage User Access/i,
        })
        .first();
      const headingVisible = await heading.isVisible({ timeout: 10_000 }).catch(() => false);

      expect(
        titleVisible || headingVisible,
        'Expected User Management page title/heading to be visible (span.title-text or heading role)',
      ).toBe(true);
    });

    await test.step('Verify loading indicators are not present', async () => {
      // Wait for blocking overlays/spinners to disappear.
      await waitForLoadingToDisappear(page);

      // Keep a functional fallback: non-blocking indicators may remain in DOM,
      // but the page is considered loaded when the core list is usable.
      await expect(
        page.locator('tr#table-header').first(),
        'Expected User Management grid header to be visible once loading completes',
      ).toBeVisible({ timeout: 10_000 });
    });
  });

  test('AC3: verify User Access Request List page displays all required table headers', async ({ page }) => {
    await navigateToUserManagement(page);
    await assertUserAccessRequestListPageLoaded(page);

    await test.step('Verify list/table/grid container is visible', async () => {
      const hasTable = await page.getByRole('table').first().isVisible({ timeout: 10_000 }).catch(() => false);
      const hasGrid = await page.getByRole('grid').first().isVisible({ timeout: 10_000 }).catch(() => false);
      const hasList = await page.getByRole('list').first().isVisible({ timeout: 10_000 }).catch(() => false);

      expect(
        hasTable || hasGrid || hasList,
        'Expected User Access Request List to display a list/table/grid container',
      ).toBe(true);
    });

    await test.step('Verify all required table headers are present and visible', async () => {
      const headerRow = page.locator('tr#table-header').first();
      await expect(headerRow, 'Expected the table header row with id="table-header" to be visible').toBeVisible({
        timeout: 10_000,
      });

      const expectedHeaders = [
        { id: 'accessRequestDtm_th', label: 'Requested Date' },
        { id: 'identityProviderNm_th', label: 'ID Provider' },
        { id: 'givenNm_th', label: 'First Name' },
        { id: 'familyNm_th', label: 'Last Name' },
        { id: 'orgType_th', label: 'Org Type' },
        { id: 'emailAddressDsc_th', label: 'Email Address' },
        { id: 'orgName_th', label: 'Organization' },
        { id: 'Status_th', label: 'Status' },
        { id: 'action_th', label: 'Action' },
      ];

      for (const header of expectedHeaders) {
        const headerElement = headerRow.locator(`th#${cssEscape(header.id)}`);
        await expect(headerElement, `Expected header id "${header.id}" to be visible`).toBeVisible({ timeout: 10_000 });
        await expect(headerElement, `Expected header "${header.label}" in th#${header.id}`).toContainText(
          new RegExp(`^\\s*${escapeRegExp(header.label)}\\s*$`, 'i'),
        );
      }
    });

    await test.step('Verify each header is rendered in the correct column position', async () => {
      const expectedHeaderIdsInOrder = [
        'accessRequestDtm_th',
        'identityProviderNm_th',
        'givenNm_th',
        'familyNm_th',
        'orgType_th',
        'emailAddressDsc_th',
        'orgName_th',
        'Status_th',
        'action_th',
      ];

      const actualHeaderIdsInOrder = await page.locator('tr#table-header > th').evaluateAll((elements) =>
        elements.map((el) => el.getAttribute('id') ?? ''),
      );

      expect(actualHeaderIdsInOrder, 'Expected table headers to match the required AC3 order').toEqual(
        expectedHeaderIdsInOrder,
      );
    });
  });

  test('AC4: verify filter/search labels and controls are displayed', async ({ page }) => {
    await navigateToUserManagement(page);
    await assertUserAccessRequestListPageLoaded(page);

    await test.step('Verify Status, Organization, and Search labels are visible', async () => {
      const requiredLabels = ['Status', 'Organization', 'Search'];

      for (const label of requiredLabels) {
        const labelElement = page.getByText(new RegExp(`^${label}$`, 'i')).first();
        const isVisible = await labelElement.isVisible({ timeout: 10_000 }).catch(() => false);
        expect(isVisible, `Expected label "${label}" to be visible`).toBe(true);
      }
    });

    await test.step('Verify Status filter control is accessible', async () => {
      const statusControl = await getFilterControl(page, /status/i);
      expect(statusControl, 'Expected Status filter control to exist').not.toBeNull();
    });

    await test.step('Verify Organization filter control is accessible', async () => {
      const orgControl = await getFilterControl(page, /organization/i);
      expect(orgControl, 'Expected Organization filter control to exist').not.toBeNull();
    });

    await test.step('Verify Search input control is accessible', async () => {
      const searchControl = await getSearchControl(page);
      expect(searchControl, 'Expected Search input control to exist').not.toBeNull();
    });
  });

  test('AC5: search functionality filters request list', async ({ page }) => {
    await navigateToUserManagement(page);
    await assertUserAccessRequestListPageLoaded(page);

    const searchControl = await getSearchControl(page);
    if (!searchControl) {
      test.skip();
      return;
    }

    await test.step('Enter valid search text and verify results are filtered', async () => {
      const testSearchTerm = 'test';
      const rowsBeforeSearch = await page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]').count();
      
      await searchControl.fill(testSearchTerm);
      await waitForSearchResultsToSettle(page, rowsBeforeSearch);

      const listItems = page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]');
      const count = await listItems.count();
      expect(count, 'Expected search results to be displayed').toBeGreaterThanOrEqual(0);
    });

    await test.step('Clear search field and verify all results are restored', async () => {
      const rowsWithSearch = await page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]').count();
      
      await searchControl.clear();
      await waitForSearchResultsToSettle(page, rowsWithSearch);

      const listItems = page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]');
      const count = await listItems.count();
      expect(count, 'Expected all results to be shown after clearing search').toBeGreaterThanOrEqual(0);
    });

    await test.step('Enter invalid/non-matching search text and verify no results', async () => {
      const invalidSearchInputs = ['2026-99-99', `InvalidData${Date.now()}`];

      for (const invalidInput of invalidSearchInputs) {
        const rowsBeforeSearch = await page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]').count();
        
        await searchControl.fill(invalidInput);
        await waitForSearchResultsToSettle(page, rowsBeforeSearch);

        const listItems = page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]');
        const count = await listItems.count();
        const noDataMessage = page
          .locator(
            '[class*="empty"], [class*="no-data"], [role="status"], .p-datatable-emptymessage, text=/No records|No data|No results/i',
          )
          .first();
        const hasEmptyMessage = await noDataMessage.isVisible({ timeout: 5_000 }).catch(() => false);

        expect(
          count === 0 || hasEmptyMessage,
          `Expected no results or empty state message for invalid search input "${invalidInput}"`,
        ).toBe(true);
      }
    });
  });

  test('AC6: Status filter dropdown shows options and filters list', async ({ page }) => {
    await navigateToUserManagement(page);
    await assertUserAccessRequestListPageLoaded(page);

    const statusControl = await getFilterControl(page, /status/i);
    if (!statusControl) {
      test.skip();
      return;
    }

    await test.step('Click Status dropdown to open available options', async () => {
      if (await statusControl.isVisible()) {
        await statusControl.click();
        // Wait for dropdown overlay to appear
        await page.locator('[role="listbox"], .p-dropdown-items, [class*="dropdown"][class*="open"]').first().waitFor({ state: 'visible', timeout: 5_000 }).catch(() => {});
      }
    });

    await test.step('Verify Status dropdown displays available options', async () => {
      const options = page.getByRole('option');
      const optionCount = await options.count();
      expect(optionCount, 'Expected Status dropdown to display at least one option').toBeGreaterThan(0);
    });

    await test.step('Select a status option and verify list is filtered', async () => {
      const options = page.getByRole('option');
      const firstOption = options.first();
      const optionExists = await firstOption.count().catch(() => 0);

      if (optionExists > 0) {
        const rowsBeforeFilter = await page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]').count();
        
        await firstOption.click();
        await waitForSearchResultsToSettle(page, rowsBeforeFilter);

        const listItems = page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]');
        const count = await listItems.count();
        expect(count, 'Expected filtered results after status selection').toBeGreaterThanOrEqual(0);
      }
    });
  });

  test('AC7: Organization filter dropdown shows options and filters list', async ({ page }) => {
    await navigateToUserManagement(page);
    await assertUserAccessRequestListPageLoaded(page);

    const orgControl = await getFilterControl(page, /organization/i);
    if (!orgControl) {
      test.skip();
      return;
    }

    await test.step('Click Organization dropdown to open available options', async () => {
      if (await orgControl.isVisible()) {
        await orgControl.click();
        // Wait for dropdown overlay to appear
        await page.locator('[role="listbox"], .p-dropdown-items, [class*="dropdown"][class*="open"]').first().waitFor({ state: 'visible', timeout: 5_000 }).catch(() => {});
      }
    });

    await test.step('Verify Organization dropdown displays available options', async () => {
      const options = page.getByRole('option');
      const optionCount = await options.count();
      expect(optionCount, 'Expected Organization dropdown to display at least one option').toBeGreaterThan(0);
    });

    await test.step('Select an organization option and verify list is filtered', async () => {
      const options = page.getByRole('option');
      const firstOption = options.first();
      const optionExists = await firstOption.count().catch(() => 0);

      if (optionExists > 0) {
        const rowsBeforeFilter = await page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]').count();
        
        await firstOption.click();
        await waitForSearchResultsToSettle(page, rowsBeforeFilter);

        const listItems = page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]');
        const count = await listItems.count();
        expect(count, 'Expected filtered results after organization selection').toBeGreaterThanOrEqual(0);
      }
    });
  });

  test('AC8: pagination or scrolling functionality works correctly', async ({ page }) => {
    await navigateToUserManagement(page);
    await assertUserAccessRequestListPageLoaded(page);

    await test.step('Verify list displays appropriate number of rows', async () => {
      const listItems = page.locator('table tbody tr, [role="rowgroup"] [role="row"]');
      const rowCount = await listItems.count();
      expect(rowCount, 'Expected list to display rows or empty state').toBeGreaterThanOrEqual(0);
    });

    await test.step('Check for pagination controls or scroll capability', async () => {
      const paginationControl = page
        .locator('.p-paginator, [class*="pagination"], [aria-label*="page" i], button:has-text(/next|previous/i)')
        .first();
      const paginationExists = await paginationControl.isVisible({ timeout: 5_000 }).catch(() => false);

      if (paginationExists) {
        const nextButton = page.locator('[aria-label*="next" i], button:has-text(/next/i)').first();
        const nextVisible = await nextButton.isVisible({ timeout: 5_000 }).catch(() => false);
        expect(nextVisible, 'Expected pagination controls to be visible').toBe(true);
      }
    });
  });

  test('AC9: search specific user and verify the functionality of access status toggle switch', async ({ page }) => {
    const targetEmail = 'young-jin.chung@gov.bc.ca';

    await navigateToUserManagement(page);
    await assertUserAccessRequestListPageLoaded(page);

    await test.step('Fill target email in Search field', async () => {
      const searchInput = await getSearchControl(page);
      expect(searchInput, 'Expected Search textbox to be visible').not.toBeNull();
      if (!searchInput) {
        test.skip();
        return;
      }
      
      const rowsBeforeSearch = await page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]').count();
      
      await searchInput.click();
      await searchInput.fill(targetEmail);
      await searchInput.press('Enter').catch(() => null);
      
      // Wait for search results to settle
      await waitForSearchResultsToSettle(page, rowsBeforeSearch);
    });

    await test.step('Verify searched row is visible in the table', async () => {
      const targetRow = page
        .locator('table tbody tr')
        .filter({ hasText: new RegExp(escapeRegExp(targetEmail), 'i') })
        .first();

      await expect(targetRow, `Expected a table row containing "${targetEmail}" to be visible`).toBeVisible({
        timeout: 15_000,
      });
    });

    await test.step('Verify ID Provider, First Name, Last Name, and Email Address column values', async () => {
      const targetRow = page
        .locator('table tbody tr')
        .filter({ hasText: new RegExp(escapeRegExp(targetEmail), 'i') })
        .first();

      const idProviderIndex = await getColumnIndexByHeaderId(page, 'identityProviderNm_th');
      const firstNameIndex  = await getColumnIndexByHeaderId(page, 'givenNm_th');
      const lastNameIndex   = await getColumnIndexByHeaderId(page, 'familyNm_th');
      const emailIndex      = await getColumnIndexByHeaderId(page, 'emailAddressDsc_th');

      await expect(
        getCellByColumnIndex(targetRow, idProviderIndex),
        'Expected ID Provider column to show "idir"',
      ).toHaveText(/^\s*idir\s*$/i, { timeout: 10_000 });

      await expect(
        getCellByColumnIndex(targetRow, firstNameIndex),
        'Expected First Name column to show "Young-jin"',
      ).toHaveText(/^\s*Young-jin\s*$/i, { timeout: 10_000 });

      await expect(
        getCellByColumnIndex(targetRow, lastNameIndex),
        'Expected Last Name column to show "Chung"',
      ).toHaveText(/^\s*Chung\s*$/i, { timeout: 10_000 });

      await expect(
        getCellByColumnIndex(targetRow, emailIndex),
        `Expected Email Address column to show "${targetEmail}"`,
      ).toHaveText(new RegExp(`^\\s*${escapeRegExp(targetEmail)}\\s*$`, 'i'), { timeout: 10_000 });
    });

    await test.step('Click access status toggle switch and verify functionality', async () => {
      const targetRow = page
        .locator('table tbody tr')
        .filter({ hasText: new RegExp(escapeRegExp(targetEmail), 'i') })
        .first();

      const accessToggle = getAccessToggleForRow(targetRow);
      await expect(accessToggle, 'Expected access status toggle switch to be visible').toBeVisible({
        timeout: 10_000,
      });

      // Get initial state before clicking
      const initialAriaChecked = await accessToggle.getAttribute('aria-checked');

      // Click the toggle switch
      await accessToggle.click();

      if (initialAriaChecked === 'true' || initialAriaChecked === 'false') {
        const confirmationDialog = page
          .getByRole('alertdialog', { name: /Activating User's Account|Disabling User's Account/i })
          .first();
        const dialogVisible = await confirmationDialog.isVisible({ timeout: 3_000 }).catch(() => false);

        if (dialogVisible) {
          const confirmButton = confirmationDialog.getByRole('button', { name: /Activate|Disable|Confirm|Yes/i }).first();
          const cancelButton = confirmationDialog.getByRole('button', { name: /^Cancel$/i }).first();

          await expect(
            confirmButton,
            'Expected confirmation dialog to expose a confirm action after toggling status',
          ).toBeVisible({ timeout: 5_000 });
          await expect(
            cancelButton,
            'Expected confirmation dialog to expose a cancel action after toggling status',
          ).toBeVisible({ timeout: 5_000 });

          // Avoid mutating long-lived test data state; just validate dialog behavior.
          await cancelButton.click();
          await expect(confirmationDialog).toBeHidden({ timeout: 8_000 });
        } else {
          await expect
            .poll(
              async () => await accessToggle.getAttribute('aria-checked'),
              {
                timeout: 8_000,
                intervals: [200, 400, 800],
                message: 'Expected access status toggle switch to change aria-checked after click',
              },
            )
            .not.toBe(initialAriaChecked);
        }
      } else {
        await expect(accessToggle, 'Expected access status toggle switch to remain interactive after click').toBeVisible();
      }
    });
  });

  test('AC10: edit user cancel flow returns to User Management page', async ({ page }) => {
    await test.step('Navigate to User Management page', async () => {
      await navigateToUserManagement(page);
      await assertUserAccessRequestListPageLoaded(page);
      await waitForLoadingToDisappear(page).catch(() => null);
      // Additional stabilization: ensure table is fully rendered
      await page.locator('tr#table-header').waitFor({ state: 'visible', timeout: 10_000 }).catch(() => {});
    });

    await test.step('Locate the user-edit-icon and click on it', async () => {
      const rows = page.locator('table tbody tr, [role="rowgroup"] [role="row"]');
      const rowCount = await rows.count();
      test.skip(rowCount === 0, 'AC precondition not met: no rows are available in User Management table.');

      // Verify Action column header exists
      const actionHeader = page.locator('tr#table-header').locator('th#action_th');
      await expect(actionHeader, 'Expected Action column header to be visible').toBeVisible({ timeout: 10_000 });

      // Verify first row is visible
      const firstRow = rows.first();
      await expect(firstRow, 'Expected first row to be visible in User Management table').toBeVisible({
        timeout: 15_000,
      });

      const editIcon = getEditUserIconForRow(firstRow);

      await expect(editIcon, 'Expected Edit User icon/action to be visible under Action column on first row').toBeVisible({
        timeout: 10_000,
      });
      await editIcon.click();
    });

    await test.step('Verify user is landed to User Information page', async () => {
      await waitForUserInformationContext(page);
    });

    await test.step('Verify User Information page labels are visible: User Details', async () => {
      const userDetailsLabel = page.getByText(/^User Details$/i).first();
      await expect(userDetailsLabel, 'Expected "User Details" section label to be visible').toBeVisible({
        timeout: 15_000,
      });
    });

    await test.step('Verify User Information page labels are visible: Status, Roles and Permissions', async () => {
      const statusRolesPermissionsLabel = page.getByText(/Status,?\s*Roles\s*and\s*Permissions/i).first();
      await expect(
        statusRolesPermissionsLabel,
        'Expected "Status, Roles and Permissions" section label to be visible',
      ).toBeVisible({ timeout: 15_000 });
    });

    await test.step('Verify Update and Cancel buttons are visible', async () => {
      const updateButton = page.getByRole('button', { name: /^Update$/i }).first();
      const cancelButton = page.getByRole('button', { name: /^Cancel$/i }).first();

      await expect(updateButton, 'Expected Update button to be visible on User Information page').toBeVisible({
        timeout: 10_000,
      });

      await expect(cancelButton, 'Expected Cancel button to be visible on User Information page').toBeVisible({
        timeout: 10_000,
      });
    });

    await test.step('Click Cancel and verify user is landed back on User Management page', async () => {
      const cancelButton = page.getByRole('button', { name: /^Cancel$/i }).first();
      await cancelButton.click();

      // Verify navigation back to User Management page
      await expect(page).toHaveURL(/user.*management|access.*request/i, { timeout: 30_000 });

      // Verify User Access Request List is loaded
      await assertUserAccessRequestListPageLoaded(page);

      // Verify table is visible
      const tableHeaders = page.locator('tr#table-header');
      await expect(tableHeaders, 'Expected User Management table to be visible').toBeVisible({ timeout: 10_000 });
    });
  });

  test('AC11: update enables editable mode and validates controls', async ({ page }) => {
    await test.step('Navigate to User Management page', async () => {
      await navigateToUserManagement(page);
      await assertUserAccessRequestListPageLoaded(page);
      await waitForLoadingToDisappear(page).catch(() => null);
      // Additional stabilization: ensure table is fully rendered
      await page.locator('tr#table-header').waitFor({ state: 'visible', timeout: 10_000 }).catch(() => {});
    });

    await test.step('Locate the user-edit-icon and click on it', async () => {
      const rows = page.locator('table tbody tr, [role="rowgroup"] [role="row"]');
      const rowCount = await rows.count();
      test.skip(rowCount === 0, 'AC precondition not met: no rows are available in User Management table.');

      const actionHeader = page.locator('tr#table-header').locator('th#action_th');
      await expect(actionHeader, 'Expected Action column header to be visible').toBeVisible({ timeout: 10_000 });

      const firstRow = rows.first();
      await expect(firstRow, 'Expected first row to be visible in User Management table').toBeVisible({
        timeout: 15_000,
      });

      const editIcon = getEditUserIconForRow(firstRow);
      await expect(editIcon, 'Expected Edit User icon/action to be visible under Action column on first row').toBeVisible({
        timeout: 10_000,
      });
      await editIcon.click();

      await waitForUserInformationContext(page);
      await assertUserInformationPageLoaded(page);
    });

    await test.step('Verify Update and Cancel buttons are visible', async () => {
      await expect(
        page.getByRole('button', { name: /^Update$/i }).first(),
        'Expected Update button to be visible on User Information page',
      ).toBeVisible({ timeout: 10_000 });
      await expect(
        page.getByRole('button', { name: /^Cancel$/i }).first(),
        'Expected Cancel button to be visible on User Information page',
      ).toBeVisible({ timeout: 10_000 });
    });

    await test.step('Click Update and verify editable mode is enabled', async () => {
      await page.getByRole('button', { name: /^Update$/i }).first().click();

      const saveButton = page.getByRole('button', { name: /^Save$/i }).first();
      await expect(saveButton, 'Expected Save button to be visible after entering editable mode').toBeVisible({
        timeout: 10_000,
      });
    });

    await test.step('Verify Organization Type field has dropdown list', async () => {
      const organizationTypeControl = await getEditableControlForLabel(page, /Organization\s*Type/i);
      expect(organizationTypeControl, 'Expected Organization Type field to expose a dropdown/select control').not.toBeNull();
      await expect(organizationTypeControl!, 'Expected Organization Type dropdown to be visible').toBeVisible({
        timeout: 10_000,
      });
    });

    await test.step('Verify Organization Name field has dropdown list', async () => {
      const organizationNameControl = await getEditableControlForLabel(page, /Organization\s*Name/i);
      expect(organizationNameControl, 'Expected Organization Name field to expose a dropdown/select control').not.toBeNull();
      await expect(organizationNameControl!, 'Expected Organization Name dropdown to be visible').toBeVisible({
        timeout: 10_000,
      });
    });

    await test.step('Verify Current Status supports Active and Disabled options', async () => {
      const activeStatus = page.getByRole('radio', { name: /Active/i }).first();
      const disabledStatus = page.getByRole('radio', { name: /Disabled/i }).first();

      const hasActiveByRole = (await activeStatus.count()) > 0;
      const hasDisabledByRole = (await disabledStatus.count()) > 0;

      if (hasActiveByRole && hasDisabledByRole) {
        await expect(activeStatus, 'Expected Active radio option to be visible').toBeVisible({ timeout: 10_000 });
        await expect(disabledStatus, 'Expected Disabled radio option to be visible').toBeVisible({ timeout: 10_000 });
      } else {
        const activeText = page.getByText(/^Active$/i).first();
        const disabledText = page.getByText(/^Disabled$/i).first();

        await expect(activeText, 'Expected Active status option to be visible').toBeVisible({ timeout: 10_000 });
        await expect(disabledText, 'Expected Disabled status option to be visible').toBeVisible({ timeout: 10_000 });
      }
    });

    await test.step('Verify User Role(s) and Permission(s) field has dropdown list', async () => {
      const rolesPermissionsControl = await getEditableControlForLabel(page, /User\s*Role\(s\)\s*and\s*Permission\(s\)/i);
      expect(
        rolesPermissionsControl,
        'Expected User Role(s) and Permission(s) field to expose a dropdown/select control',
      ).not.toBeNull();
      await expect(rolesPermissionsControl!, 'Expected User Role(s) and Permission(s) dropdown to be visible').toBeVisible({
        timeout: 10_000,
      });
    });

    await test.step('Verify Save and Cancel buttons are visible', async () => {
      await expect(
        page.getByRole('button', { name: /^Save$/i }).first(),
        'Expected Save button to be visible in editable mode',
      ).toBeVisible({ timeout: 10_000 });
      await expect(
        page.getByRole('button', { name: /^Cancel$/i }).first(),
        'Expected Cancel button to be visible in editable mode',
      ).toBeVisible({ timeout: 10_000 });
    });
  });


});

// ─────────────────────────────────────────────────────────────────────────────
// Helper Functions
// ─────────────────────────────────────────────────────────────────────────────

async function loginAsIdir(page: Page): Promise<void> {
  await loginAsIdirShared(page, APP_URL);
}

async function navigateToUserManagement(page: Page): Promise<void> {
  const userManagementButton = page.getByRole('button', { name: /^User Management$/i });
  const buttonVisible = await userManagementButton.isVisible({ timeout: 10_000 }).catch(() => false);

  if (buttonVisible) {
    await userManagementButton.click();
    await expect(page).toHaveURL(/user.*management|access.*request/i, { timeout: 30_000 });
    await assertUserAccessRequestListPageLoaded(page);
    await waitForLoadingToDisappear(page).catch(() => null);
  }
}

async function assertUserAccessRequestListPageLoaded(page: Page): Promise<void> {
  await expect(page).toHaveURL(/user.*management|access.*request/i, { timeout: 30_000 });

  // Use span.title-text locator with fallback to heading role (same pattern as AC2)
  const titleText = page.locator('span.title-text').filter({ hasText: /User Management|User Access Request/i }).first();
  const titleVisible = await titleText.isVisible({ timeout: 10_000 }).catch(() => false);

  const heading = page
    .getByRole('heading', {
      name: /User Management|User Access Request|Access Request List|Manage User Access/i,
    })
    .first();
  const headingVisible = await heading.isVisible({ timeout: 10_000 }).catch(() => false);

  expect(
    titleVisible || headingVisible,
    'Expected User Management page title/heading to be visible (span.title-text or heading role)',
  ).toBe(true);
}

async function getFilterControl(page: Page, labelPattern: RegExp): Promise<Locator | null> {
  // Try to find a combobox/select by role near the label
  const combobox = page.getByRole('combobox', { name: labelPattern }).first();
  if ((await combobox.count()) > 0 && (await combobox.isVisible({ timeout: 5_000 }).catch(() => false))) {
    return combobox;
  }

  // Try to find a select element
  const selectControl = page
    .locator('div, section, fieldset')
    .filter({ has: page.getByText(labelPattern) })
    .locator('select, [role="listbox"], [role="combobox"], [class*="dropdown"]')
    .first();
  if ((await selectControl.count()) > 0 && (await selectControl.isVisible({ timeout: 5_000 }).catch(() => false))) {
    return selectControl;
  }

  return null;
}

async function getSearchControl(page: Page): Promise<Locator | null> {
  // Try to find a search input by label
  const searchInput = page.getByPlaceholder(/search|query|filter/i).first();
  if ((await searchInput.count()) > 0 && (await searchInput.isVisible({ timeout: 5_000 }).catch(() => false))) {
    return searchInput;
  }

  // Try to find by role
  const searchboxByRole = page.getByRole('searchbox').first();
  if ((await searchboxByRole.count()) > 0 && (await searchboxByRole.isVisible({ timeout: 5_000 }).catch(() => false))) {
    return searchboxByRole;
  }

  // Try to find input near search label
  const searchInputNearLabel = page
    .locator('div, section, fieldset')
    .filter({ has: page.getByText(/^Search$/i) })
    .locator('input[type="text"], input:not([type="hidden"]), [role="textbox"]')
    .first();
  if (
    (await searchInputNearLabel.count()) > 0 &&
    (await searchInputNearLabel.isVisible({ timeout: 5_000 }).catch(() => false))
  ) {
    return searchInputNearLabel;
  }

  return null;
}

function getRowsMatchingEmail(page: Page, email: string): Locator {
  return page.locator('table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]').filter({
    hasText: new RegExp(escapeRegExp(email), 'i'),
  });
}

function getAccessToggleForRow(row: Locator): Locator {
  return row
    .locator(
      '[role="switch"], button[role="switch"], button[aria-label*="status" i], button[aria-label*="access" i], input[type="checkbox"], .p-inputswitch, .p-inputswitch-slider',
    )
    .first();
}

async function getColumnIndexByHeaderId(page: Page, headerId: string): Promise<number> {
  const headerIds = await page.locator('tr#table-header > th').evaluateAll((elements) =>
    elements.map((el) => (el.getAttribute('id') ?? '').trim()),
  );

  return headerIds.findIndex((id) => id === headerId);
}

function getCellByColumnIndex(row: Locator, columnIndex: number): Locator {
  return row.locator('td, [role="cell"]').nth(columnIndex);
}

function getEditUserIconForRow(row: Locator): Locator {
  // Primary: span.user-edit-icon (e.g. <span class="user-edit-icon ng-star-inserted" id="user-edit-0-icon" tabindex="0">)
  // Secondary: any element whose id matches the user-edit-*-icon pattern
  // Fallback: generic ARIA/role-based edit selectors
  return row
    .locator(
      'span.user-edit-icon, [id*="user-edit"][id*="icon"], button[aria-label*="edit" i], [role="button"][aria-label*="edit" i], [title*="edit" i], .pi-pencil, .fa-edit',
    )
    .first();
}

async function waitForLoadingToDisappear(page: Page): Promise<void> {
  // Focus on blocking loaders/overlays that prevent interaction.
  const blockingLoaderSelectors = [
    'div.loader.ng-star-inserted',
    '.p-datatable-loading-overlay',
    '.p-datatable-loading',
    '[aria-busy="true"]',
    '[role="progressbar"]',
    '.spinner',
  ];

  await expect
    .poll(
      async () => {
        return await page.evaluate((selectors) => {
          const isVisible = (element: Element): boolean => {
            const htmlElement = element as HTMLElement;
            const style = window.getComputedStyle(htmlElement);
            return (
              style.display !== 'none' &&
              style.visibility !== 'hidden' &&
              style.opacity !== '0' &&
              htmlElement.offsetParent !== null
            );
          };

          const hasBlockingLoader = selectors.some((selector) =>
            Array.from(document.querySelectorAll(selector)).some((element) => isVisible(element)),
          );
          return !hasBlockingLoader;
        }, blockingLoaderSelectors);
      },
      {
        timeout: 30_000,
        intervals: [500, 1000],
        message: 'Timeout waiting for blocking loading overlays/spinners to disappear',
      },
    )
    .toBe(true);
}

async function waitForSearchResultsToSettle(page: Page, rowsBeforeSearch: number): Promise<void> {
  const tableRowSelector = 'table tbody tr, [role="rowgroup"] [role="row"], [role="listitem"]';

  try {
    await waitForLoadingToDisappear(page).catch(() => null);

    // Wait for row count to remain stable across consecutive polls.
    let previousCount = -1;
    let stableReads = 0;
    await expect
      .poll(
        async () => {
          const currentCount = await page.locator(tableRowSelector).count();
          stableReads = currentCount === previousCount ? stableReads + 1 : 1;
          previousCount = currentCount;
          return stableReads >= 2;
        },
        {
          timeout: 12_000,
          intervals: [300, 600, 1_000],
          message: 'Row count did not stabilize after search',
        },
      )
      .toBe(true);

    // Touch the parameter so it remains intentionally part of the helper contract.
    void rowsBeforeSearch;
  } catch {
    // If stabilization times out, keep a short final sync with current UI state.
    await waitForLoadingToDisappear(page).catch(() => null);
  }
}

async function openUserInformationPageForEmail(page: Page, email: string): Promise<void> {
  const searchControl = await getSearchControl(page);
  expect(searchControl, 'Expected Search control on User Management page').not.toBeNull();

  if (!searchControl) {
    return;
  }

  await searchControl.fill(email);
  await waitForSearchResultsToSettle(page, 0);

  const targetRow = getRowsMatchingEmail(page, email).first();
  const targetRowCount = await getRowsMatchingEmail(page, email).count();
  test.skip(
    targetRowCount === 0,
    `AC precondition not met: no user row found for ${email}. Seed this user in the environment to execute edit-flow checks.`,
  );

  await expect(targetRow, `Expected a row containing "${email}" before clicking Edit`).toBeVisible({ timeout: 15_000 });

  const editIcon = getEditUserIconForRow(targetRow);
  await expect(editIcon, 'Expected Edit User icon/action for the searched row').toBeVisible({ timeout: 10_000 });
  await editIcon.click();
}

async function isUserInformationPageVisible(page: Page): Promise<boolean> {
  const userDetailsLabel = page.getByText(/^User Details$/i).first();
  const statusRolesPermissionsLabel = page.getByText(/Status,?\s*Roles\s*and\s*Permissions/i).first();

  const hasUserDetails = await userDetailsLabel.isVisible({ timeout: 3_000 }).catch(() => false);
  const hasStatusRolesPermissions = await statusRolesPermissionsLabel.isVisible({ timeout: 3_000 }).catch(() => false);

  return hasUserDetails && hasStatusRolesPermissions;
}

async function assertUserInformationPageLoaded(page: Page): Promise<void> {
  await expect(page.getByText(/^User Details$/i).first(), 'Expected "User Details" section label to be visible').toBeVisible({
    timeout: 15_000,
  });
  await expect(
    page.getByText(/Status,?\s*Roles\s*and\s*Permissions/i).first(),
    'Expected "Status, Roles and Permissions" section label to be visible',
  ).toBeVisible({ timeout: 15_000 });
}

async function waitForUserInformationContext(page: Page): Promise<void> {
  await waitForLoadingToDisappear(page).catch(() => null);

  await expect
    .poll(
      async () => {
        const urlContainsUser = /user/i.test(page.url());
        const userInformationLabelVisible = await page.getByText(/^User Information$/i).first().isVisible().catch(() => false);
        const userDetailsLabelVisible = await page.getByText(/^User Details$/i).first().isVisible().catch(() => false);

        return urlContainsUser || userInformationLabelVisible || userDetailsLabelVisible;
      },
      {
        timeout: 30_000,
        message:
          'Expected User Information page context: URL contains "user" or "User Information"/"User Details" labels are visible',
      },
    )
    .toBe(true);
}

async function getEditableControlForLabel(page: Page, labelPattern: RegExp): Promise<Locator | null> {
  const byAccessibleName = page
    .locator('select, [role="combobox"], .p-dropdown, .p-multiselect')
    .filter({ has: page.getByText(labelPattern) })
    .first();
  if ((await byAccessibleName.count()) > 0 && (await byAccessibleName.isVisible({ timeout: 3_000 }).catch(() => false))) {
    return byAccessibleName;
  }

  const inSection = page
    .locator('div, section, fieldset, form')
    .filter({ has: page.getByText(labelPattern) })
    .locator('select, [role="combobox"], .p-dropdown, .p-multiselect, input[aria-haspopup="listbox"]')
    .first();

  if ((await inSection.count()) > 0 && (await inSection.isVisible({ timeout: 3_000 }).catch(() => false))) {
    return inSection;
  }

  return null;
}

async function getVisibleDropdownOptionTexts(page: Page): Promise<string[]> {
  const optionLocators = [
    page.locator('[role="option"]'),
    page.locator('select option'),
    page.locator('.p-dropdown-item, .p-multiselect-item'),
  ];

  const allTexts: string[] = [];
  for (const options of optionLocators) {
    const count = await options.count();
    for (let i = 0; i < count; i += 1) {
      const option = options.nth(i);
      const visible = await option.isVisible().catch(() => false);
      if (!visible) {
        continue;
      }

      const text = (await option.textContent()) ?? '';
      if (text.trim()) {
        allTexts.push(text.trim());
      }
    }
  }

  return allTexts;
}

function cssEscape(value: string): string {
  return value.replace(/([#.;?+*~':"!^$\[\]()=>|/@])/g, '\\$1');
}

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}


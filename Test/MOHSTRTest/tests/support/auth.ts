import { expect, type Page } from '@playwright/test';

const DEFAULT_BASE_URL = process.env.BASE_URL ?? '';
const IDIR_USERNAME = process.env.IDIR_USERNAME ?? '';
const IDIR_PASSWORD = process.env.IDIR_PASSWORD ?? '';
const BCEID_USERNAME = process.env.BCEID_USERNAME ?? process.env.BUSINESS_BCEID_USERNAME ?? '';
const BCEID_PASSWORD = process.env.BCEID_PASSWORD ?? process.env.BUSINESS_BCEID_PASSWORD ?? '';
const BCEID_PLATFORM_USERNAME = process.env.BCEID_PLATFORM_USERNAME ?? '';
const BCEID_PLATFORM_PASSWORD = process.env.BCEID_PLATFORM_PASSWORD ?? '';

export const IDIR_AUTH_ENV_MESSAGE =
  'Set BASE_URL, IDIR_USERNAME and IDIR_PASSWORD environment variables before running this suite.';

export const BCEID_AUTH_ENV_MESSAGE =
  'Set BASE_URL, BCEID_USERNAME and BCEID_PASSWORD environment variables before running this suite.';

export const BCEID_PLATFORM_AUTH_ENV_MESSAGE =
  'Set BASE_URL, BCEID_PLATFORM_USERNAME and BCEID_PLATFORM_PASSWORD environment variables before running this suite.';

export function hasIdirAuthConfig(): boolean {
  return DEFAULT_BASE_URL !== '' && IDIR_USERNAME !== '' && IDIR_PASSWORD !== '';
}

export function hasBceidAuthConfig(): boolean {
  return DEFAULT_BASE_URL !== '' && BCEID_USERNAME !== '' && BCEID_PASSWORD !== '';
}

export function hasBceidPlatformAuthConfig(): boolean {
  return DEFAULT_BASE_URL !== '' && BCEID_PLATFORM_USERNAME !== '' && BCEID_PLATFORM_PASSWORD !== '';
}

async function clickIdentityProvider(page: Page, providerName: RegExp): Promise<void> {
  const providerLink = page.getByRole('link', { name: providerName }).first();
  const linkCount = await providerLink.count();
  if (linkCount > 0) {
    await providerLink.click();
    return;
  }

  const providerButton = page.getByRole('button', { name: providerName }).first();
  await expect(providerButton).toBeVisible({ timeout: 30_000 });
  await providerButton.click();
}

export async function loginAsIdir(page: Page, baseUrl: string = DEFAULT_BASE_URL): Promise<void> {
  if (!baseUrl) {
    throw new Error('BASE_URL is not configured.');
  }

  await page.goto(baseUrl, { waitUntil: 'domcontentloaded' });

  const portalHeading = page.getByRole('heading', { name: /Short-Term Rental Data Portal/i });
  const alreadyLoggedIn = await portalHeading.isVisible({ timeout: 5_000 }).catch(() => false);
  if (alreadyLoggedIn) {
    return;
  }

  await expect(page.getByRole('heading', { name: /Authenticate with:/i })).toBeVisible({
    timeout: 45_000,
  });
  await clickIdentityProvider(page, /^IDIR$/i);

  const landedBackOnPortal = await portalHeading.isVisible({ timeout: 20_000 }).catch(() => false);
  if (landedBackOnPortal) {
    return;
  }

  const usernameInput = page
    .getByRole('textbox')
    .filter({ hasNot: page.locator('[type="password"]') })
    .first();
  const passwordInput = page.getByRole('textbox', { name: /Password/i });

  await expect(usernameInput).toBeVisible({ timeout: 60_000 });
  await expect(passwordInput).toBeVisible({ timeout: 60_000 });

  await usernameInput.fill(IDIR_USERNAME);
  await passwordInput.fill(IDIR_PASSWORD);
  await page.getByRole('button', { name: /^Continue$/i }).click();

  await expect(page.getByText(/Enter an IDIR username and password/i)).toHaveCount(0);
  await expect(portalHeading).toBeVisible({ timeout: 60_000 });
}

export async function loginAsBceid(page: Page, baseUrl: string = DEFAULT_BASE_URL): Promise<void> {
  await loginWithBceidCredentials(page, BCEID_USERNAME, BCEID_PASSWORD, baseUrl);
}

export async function loginAsBceidPlatform(page: Page, baseUrl: string = DEFAULT_BASE_URL): Promise<void> {
  await loginWithBceidCredentials(page, BCEID_PLATFORM_USERNAME, BCEID_PLATFORM_PASSWORD, baseUrl);
}

async function loginWithBceidCredentials(
  page: Page,
  username: string,
  password: string,
  baseUrl: string,
): Promise<void> {
  if (!baseUrl) {
    throw new Error('BASE_URL is not configured.');
  }

  await page.goto(baseUrl, { waitUntil: 'domcontentloaded' });

  const portalHeading = page.getByRole('heading', { name: /Short-Term Rental Data Portal/i });
  const alreadyLoggedIn = await portalHeading.isVisible({ timeout: 5_000 }).catch(() => false);
  if (alreadyLoggedIn) {
    return;
  }

  await expect(page.getByRole('heading', { name: /Authenticate with:/i })).toBeVisible({
    timeout: 45_000,
  });
  await clickIdentityProvider(page, /Business\s*BCeID/i);

  const landedBackOnPortal = await portalHeading.isVisible({ timeout: 20_000 }).catch(() => false);
  if (landedBackOnPortal) {
    return;
  }

  const usernameInput = page
    .getByRole('textbox')
    .filter({ hasNot: page.locator('[type="password"]') })
    .first();
  const passwordInput = page.getByRole('textbox', { name: /Password/i });

  await expect(usernameInput).toBeVisible({ timeout: 60_000 });
  await expect(passwordInput).toBeVisible({ timeout: 60_000 });

  await usernameInput.fill(username);
  await passwordInput.fill(password);
  await page.getByRole('button', { name: /^Continue$/i }).click();

  await expect(page.getByText(/Enter a Business BCeID username and password/i)).toHaveCount(0);
  await expect(portalHeading).toBeVisible({ timeout: 60_000 });
}

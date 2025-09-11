import { test, expect } from '@playwright/test';

test('Add item to the cart', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'Ready for a new adventure?' })).toBeVisible();
  await page.getByRole('link', { name: 'Adventurer GPS Watch' }).click();
  await page.getByRole('button', { name: 'Add to shopping bag' }).click();
  await page.getByRole('link', { name: 'shopping bag' }).click();
  await page.getByRole('heading', { name: 'Shopping bag' }).click();

  await page.getByText('Total').nth(1).click();
  await page.getByLabel('product quantity').getByText('1');

  await expect.poll(() => page.getByLabel('product quantity').count()).toBeGreaterThan(0);
});

test('Add more than 20 different items to the cart', async ({ page }) => {
  await page.goto('/');

  for (let i = 0; i < 21; i++) {
    await page.getByRole('link', { name: `Item ${i}` }).click();
    await page.getByRole('button', { name: 'Add to shopping bag' }).click();
    await page.goto('/');
  }

  await page.getByRole('link', { name: 'shopping bag' }).click();
  await page.getByRole('heading', { name: 'Shopping bag' }).click();

  await expect(page.getByText('Basket cannot have more than 20 different items')).toBeVisible();
});

test('Add more than 100 units of an item to the cart', async ({ page }) => {
  await page.goto('/');

  await page.getByRole('link', { name: 'Adventurer GPS Watch' }).click();
  await page.getByRole('button', { name: 'Add to shopping bag' }).click();
  await page.getByRole('link', { name: 'shopping bag' }).click();
  await page.getByRole('heading', { name: 'Shopping bag' }).click();

  await page.getByLabel('product quantity').fill('101');
  await page.getByRole('button', { name: 'Update' }).click();

  await expect(page.getByText('Basket cannot have more than 100 units of a single item')).toBeVisible();
});

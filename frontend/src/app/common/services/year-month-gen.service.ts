import { Injectable } from '@angular/core';
import { DropdownOption } from '../models/dropdown-option';

@Injectable({
  providedIn: 'root'
})
export class YearMonthGenService {
  getPreviousMonths(numberOfMonths: number): Array<DropdownOption> {
    const today = new Date();
    const monthsDropdown = new Array<DropdownOption>();

    for (let i = 0; i < numberOfMonths; i++) {
      const prevMonth = today.getMonth() - i - 1;
      const year = prevMonth < 0 ? today.getFullYear() - 1 : today.getFullYear();
      const month = (prevMonth % 12 + 12) % 12 + 1;
      const monthString = month.toString().padStart(2, '0');

      monthsDropdown.push({
        label: `${this.getMonthNameLocale(month - 1)} ${year}`,
        value: `${year}-${monthString}`,
      })
    }

    return monthsDropdown;
  }

  /**
   * Stringifies a given number to a locale representation of a month
   * @param monthNumber number between 0 and 11 inclusive
   * @returns string representation of monthNumber param based on locale
   */
  private getMonthNameLocale(monthNumber: number): string {
    if (monthNumber < 0 || monthNumber > 11) {
      throw Error(`The numeric monthNumber parameter is expected to be between 0 and 11. Currently it is ${monthNumber}.`);
    }

    const date = new Date(2024, monthNumber, 1);

    return date.toLocaleString('default', { month: 'long' });
  }
}

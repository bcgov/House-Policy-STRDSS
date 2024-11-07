import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'textCleanup',
  standalone: true,
})
export class TextCleanupPipe implements PipeTransform {

  transform(value: string, ...args: unknown[]): string {
    let cleanedValue = value;

    cleanedValue = cleanedValue.trim();

    if (cleanedValue.startsWith(',')) {
      cleanedValue = cleanedValue.slice(1);
      cleanedValue = cleanedValue.trim();
    }

    return cleanedValue;
  }

}

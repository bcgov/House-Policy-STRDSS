import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'notReported',
  standalone: true
})
export class NotReportedPipe implements PipeTransform {
  transform(value: number | string | null | undefined): string {
    if (value === -1) {
      return 'Not Reported';
    }
    return value?.toString() || '';
  }
}

import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'urlProtocol',
  standalone: true
})
export class UrlProtocolPipe implements PipeTransform {

  transform(value: string): string {
    return value.startsWith('http') ? value : `https://${value}`;
  }

}

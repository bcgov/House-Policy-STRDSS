import { UrlProtocolPipe } from './url-protocol.pipe';

describe('UrlProtocolPipe', () => {
  it('create an instance', () => {
    const pipe = new UrlProtocolPipe();
    expect(pipe).toBeTruthy();
  });
});

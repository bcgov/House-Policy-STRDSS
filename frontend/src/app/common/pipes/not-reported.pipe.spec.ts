import { NotReportedPipe } from './not-reported.pipe';

describe('NotReportedPipe', () => {
  let pipe: NotReportedPipe;

  beforeEach(() => {
    pipe = new NotReportedPipe();
  });

  it('create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('should return "Not Reported" when value is -1', () => {
    expect(pipe.transform(-1)).toBe('Not Reported');
  });

  it('should return the original value when value is not -1', () => {
    expect(pipe.transform(0)).toBe('0');
    expect(pipe.transform(5)).toBe('5');
    expect(pipe.transform(100)).toBe('100');
  });

  it('should handle null and undefined values', () => {
    expect(pipe.transform(null)).toBe('');
    expect(pipe.transform(undefined)).toBe('');
  });

  it('should handle string values', () => {
    expect(pipe.transform('5')).toBe('5');
    expect(pipe.transform('test')).toBe('test');
  });
});

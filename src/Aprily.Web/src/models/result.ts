export interface Error {
  code: string;
  message: string;
}

export interface Result<T = void> {
  isSuccess: boolean;
  isFailure: boolean;
  error?: Error | null;
  data?: T;
}

export interface OperationResult<T> {
  isSuccess: boolean;
  errorMessage: string | null;
  result: T;
}

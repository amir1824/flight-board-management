import type { AxiosError } from "axios";

type ApiErrorBody = {
  message?: string;
  error?: string;
  detail?: string;
  errors?: Record<string, string[] | string>; 
};

export type ApiErrorShape = { status?: number; statusCode?: number; message?: string };

export function getApiErrorMessage(err: unknown): string {

  if (isAxiosError<ApiErrorBody>(err)) {
    const data = err.response?.data;
    const msgFromBody =
      data?.message ??
      data?.detail ??
      (data?.errors && firstErrorFromErrorsMap(data.errors));
    return msgFromBody || err.response?.statusText || err.message || "Request failed";
  }

  if (isApiErrorShape(err)) {
    return err.message || "Request failed";
  }

  if (err instanceof Error) {
    return err.message || "Something went wrong";
  }

  try {
    return JSON.stringify(err);
  } catch {
    return "Unknown error";
  }
}

function isAxiosError<T = unknown>(e: any): e is AxiosError<T> {
  return !!e && typeof e === "object" && e.isAxiosError === true;
}

function isApiErrorShape(e: any): e is ApiErrorShape {
  return !!e && typeof e === "object" && ("message" in e || "status" in e || "statusCode" in e);
}

function firstErrorFromErrorsMap(errors: Record<string, string[] | string>): string | undefined {
  for (const key of Object.keys(errors)) {
    const val = errors[key];
    if (Array.isArray(val) && val.length) return val[0];
    if (typeof val === "string" && val) return val;
  }
  return undefined;
}

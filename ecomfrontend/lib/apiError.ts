export type ApiError = {
  message?: string;
  errorCode?: string;
  statusCode?: number;
  validationErrors?: Record<string, string[]>;
};

export type AuthApiError = ApiError;

export function parseApiError(body: unknown): ApiError | null {
  if (!body || typeof body !== "object") return null;
  const b = body as Record<string, unknown>;
  const pick = (camel: string, pascal: string) => b[camel] ?? b[pascal];

  const message = pick("message", "Message");
  const errorCode = pick("errorCode", "ErrorCode");
  const statusCode = pick("statusCode", "StatusCode");
  const validationErrors = pick("validationErrors", "ValidationErrors");

  return {
    message: typeof message === "string" ? message : undefined,
    errorCode: typeof errorCode === "string" ? errorCode : undefined,
    statusCode: typeof statusCode === "number" ? statusCode : undefined,
    validationErrors:
      validationErrors && typeof validationErrors === "object"
        ? (validationErrors as Record<string, string[]>)
        : undefined,
  };
}

export function extractAuthApiError(rawErrorObject: unknown): ApiError | null {

  const authErrorObject = rawErrorObject as { cause?: { err?: { message?: string } }; message?: string };
  const errorObject = authErrorObject?.cause?.err?.message ?? authErrorObject?.message;

  if (!errorObject) return null;
  try {
    return parseApiError(JSON.parse(errorObject));
  } catch {
    return null;
  }
}

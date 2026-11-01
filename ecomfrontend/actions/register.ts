"use server";

import { AuthError } from "next-auth";
import { signIn } from "@/auth";
import { extractAuthApiError, parseApiError } from "@/lib/apiError";

const API_URL = process.env.ECOM_API_URL ?? "http://localhost:5041";

export type RegisterResult = {
  error?: string;
  validationErrors?: Record<string, string[]>;
};

export async function registerAction(
  values: { email: string; password: string; displayName: string },
  redirectTo: string
): Promise<RegisterResult | undefined> {
  const res = await fetch(`${API_URL}/api/auth/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(values),
  });

  if (!res.ok) {
    const api = parseApiError(await res.json().catch(() => null));
    if (api?.validationErrors) {
      return { error: api.message, validationErrors: api.validationErrors };
    }
    return { error: api?.message ?? "Could not create your account." };
  }

  // Account created — establish the session via the credentials provider.
  try {
    await signIn("credentials", {
      email: values.email,
      password: values.password,
      redirectTo,
    });
  } catch (error) {
    if (error instanceof AuthError) {
      const api = extractAuthApiError(error);
      return { error: api?.message ?? "Account created, but sign in failed." };
    }
    throw error;
  }
}
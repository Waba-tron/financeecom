"use server";

import { AuthError } from "next-auth";
import { signIn } from "@/auth";
import { extractAuthApiError } from "@/lib/apiError";

export type LoginResult = {
  error?: string;
  validationErrors?: Record<string, string[]>;
};

export async function loginAction(
  values: { email: string; password: string },
  redirectTo: string
): Promise<LoginResult | undefined> {
  try {
    await signIn("credentials", { ...values, redirectTo });
  } catch (error) {
    if (error instanceof AuthError) {
      const api = extractAuthApiError(error);
      if (api?.validationErrors) {
        return { error: api.message, validationErrors: api.validationErrors };
      }

      console.log(api?.message);

      return { error: "Something went wrong. Please try again." };
    }
    throw error;
  }
}

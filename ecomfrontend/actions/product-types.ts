"use server";

import { authHeaders } from "@/lib/authHeaders";

const API_URL = process.env.ECOM_API_URL ?? "http://localhost:5041";

export type ProductType = {
  id: number;
  name: string;
  description: string | null;
};

export async function getProductTypes(): Promise<ProductType[]> {
  const res = await fetch(`${API_URL}/api/product-types`, {
    cache: "no-store",
    headers: await authHeaders(),
  });
  if (!res.ok) throw new Error(`Failed to load product types (${res.status})`);
  return res.json();
}

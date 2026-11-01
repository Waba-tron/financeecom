"use server";

import { auth } from "@/auth";
import { authHeaders } from "@/lib/authHeaders";

export type Product = {
  id: number;
  name: string;
  description: string;
  price: number;
  productTypeId: number;
  productTypeName: string;
};

const API_URL = process.env.ECOM_API_URL ?? "http://localhost:5041";

export type PagedProducts = {
  items: Product[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
};

export async function getProducts(params: {
  productTypeId?: number;
  search?: string;
  page?: number;
  pageSize?: number;
}): Promise<PagedProducts> {
  const session = await auth();

  const query = new URLSearchParams();

  if (params.productTypeId != null) query.set("productTypeId", String(params.productTypeId));
  if (params.search?.trim()) query.set("search", params.search.trim());
  
  query.set("page", String(params.page ?? 1));
  query.set("pageSize", String(params.pageSize ?? 20));

  const res = await fetch(`${API_URL}/api/products?${query.toString()}`, {
    cache: "no-store",
    headers: { Authorization: `Bearer ${session?.accessToken}` },
  });

  if (!res.ok) throw new Error(`Failed to load products (${res.status})`);

  return res.json();
}

export type ProductParams = {
  name: string;
  description: string;
  price: number;
  productTypeId: number;
};

export async function createProduct(product: ProductParams): Promise<Product> {
  const res = await fetch(`${API_URL}/api/products`, {
    method: "POST",
    cache: "no-store",
    headers: await authHeaders(),
    body: JSON.stringify(product),
  });
  if (!res.ok) throw new Error(`Failed to create product (${res.status})`);
  return res.json();
}

export async function updateProduct(id: number, product: ProductParams): Promise<Product> {
  const res = await fetch(`${API_URL}/api/products/${id}`, {
    method: "PUT",
    cache: "no-store",
    headers: await authHeaders(),
    body: JSON.stringify(product),
  });
  if (!res.ok) throw new Error(`Failed to update product (${res.status})`);
  return res.json();
}

export async function deleteProduct(id: number): Promise<void> {
  const res = await fetch(`${API_URL}/api/products/${id}`, {
    method: "DELETE",
    cache: "no-store",
    headers: await authHeaders(),
  });
  if (!res.ok) throw new Error(`Failed to delete product (${res.status})`);
}

"use server";

import { auth } from "@/auth";

const API_URL = process.env.ECOM_API_URL ?? "http://localhost:5041";

export type CartItem = {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
};

export type Cart = {
  id: number;
  userId: number;
  items: CartItem[];
  total: number;
};

async function authHeaders() {
  const session = await auth();
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${session?.accessToken}`,
  };
}

export async function getMyCart(): Promise<Cart | null> {
  const session = await auth();
  if (!session?.accessToken) return null;

  const res = await fetch(`${API_URL}/api/cart`, {
    cache: "no-store",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${session.accessToken}`,
    },
  });
  if (!res.ok) throw new Error(`Failed to load cart (${res.status})`);
  return res.json();
}

export async function addToCart(productId: number, quantity = 1): Promise<Cart> {
  const res = await fetch(`${API_URL}/api/cart/items`, {
    method: "POST",
    cache: "no-store",
    headers: await authHeaders(),
    body: JSON.stringify({ productId, quantity }),
  });
  if (!res.ok) throw new Error(`Failed to add to cart (${res.status})`);
  return res.json();
}

export async function updateCartItem(productId: number, quantity: number): Promise<Cart> {
  const res = await fetch(`${API_URL}/api/cart/items/${productId}`, {
    method: "PUT",
    cache: "no-store",
    headers: await authHeaders(),
    body: JSON.stringify({ quantity }),
  });
  if (!res.ok) throw new Error(`Failed to update cart item (${res.status})`);
  return res.json();
}

export async function removeCartItem(productId: number): Promise<Cart> {
  const res = await fetch(`${API_URL}/api/cart/items/${productId}`, {
    method: "DELETE",
    cache: "no-store",
    headers: await authHeaders(),
  });
  if (!res.ok) throw new Error(`Failed to remove cart item (${res.status})`);
  return res.json();
}

export async function clearCart(): Promise<void> {
  const res = await fetch(`${API_URL}/api/cart`, {
    method: "DELETE",
    cache: "no-store",
    headers: await authHeaders(),
  });
  if (!res.ok) throw new Error(`Failed to clear cart (${res.status})`);
}


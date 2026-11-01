"use server";

import { auth } from "@/auth";

const API_URL = process.env.ECOM_API_URL ?? "http://localhost:5041";

export type OrderItem = {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
};

export type Order = {
  id: number;
  userId: number;
  status: number;
  total: number;
  createdDate: string;
  checkoutUrl: string | null;
  items: OrderItem[];
};

export async function checkout(): Promise<Order> {
  const session = await auth();
  if (!session?.accessToken) throw new Error("Not authenticated.");

  const res = await fetch(`${API_URL}/api/orders/checkout`, {
    method: "POST",
    cache: "no-store",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${session.accessToken}`,
    },
  });
  if (!res.ok) throw new Error(`Failed to start checkout (${res.status})`);
  return res.json();
}

export async function getMyOrders(): Promise<Order[]> {
  const session = await auth();
  if (!session?.accessToken) throw new Error("Not authenticated.");

  const res = await fetch(`${API_URL}/api/orders/my-orders`, {
    cache: "no-store",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${session.accessToken}`,
    },
  });
  if (!res.ok) throw new Error(`Failed to load orders (${res.status})`);
  return res.json();
}

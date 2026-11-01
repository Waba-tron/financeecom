"use client";

import { createContext, useContext, useEffect, useState } from "react";
import { addToCart, clearCart, getMyCart, removeCartItem, updateCartItem, type Cart } from "@/actions/cart";

type CartContextValue = {
  cart: Cart | null;
  itemCount: number;
  loading: boolean;
  addItem: (productId: number, quantity?: number) => Promise<void>;
  updateItem: (productId: number, quantity: number) => Promise<void>;
  removeItem: (productId: number) => Promise<void>;
  clearCartItems: () => Promise<void>;
  loadCart: () => Promise<void>;
};

const CartContext = createContext<CartContextValue | null>(null);

export function CartProvider({ children }: { children: React.ReactNode }) {
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(true);

  const loadCart = async () => {
    setLoading(true);
    try {
      setCart(await getMyCart());
    } finally {
      setLoading(false);
    }
    setLoading(false);
  };

  const addItem = async (productId: number, quantity = 1) => {
    setCart(await addToCart(productId, quantity));
  };

  const updateItem = async (productId: number, quantity: number) => {
    setCart(await updateCartItem(productId, quantity));
  };

  const removeItem = async (productId: number) => {
    setCart(await removeCartItem(productId));
  };

  const clearCartItems = async () => {
    await clearCart();
    setCart(null);
  };

  const itemCount = cart?.items.reduce((sum, item) => sum + item.quantity, 0) ?? 0;

  const value = { cart, itemCount, loading, addItem, updateItem, removeItem, clearCartItems, loadCart };

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart() {
  const context = useContext(CartContext);
  if (!context) throw new Error("useCart must be used within a CartProvider");
  return context;
}

export function useLoadCart() {
  const { loadCart } = useCart();
  useEffect(() => {
    loadCart();
  }, []);
}

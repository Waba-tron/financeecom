"use client";

import { useState } from "react";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Divider,
  IconButton,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { useCart, useLoadCart } from "@/context/CartProvider";
import { checkout } from "@/actions/orders";

export function CheckoutCart() {
  const { cart, loading, updateItem, removeItem, clearCartItems } = useCart();
  const [pendingId, setPendingId] = useState<number | null>(null);
  const [clearing, setClearing] = useState(false);
  const [checkingOut, setCheckingOut] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useLoadCart();

  const itemAction = async (
    productId: number,
    cartAction: () => Promise<void>,
  ) => {
    setPendingId(productId);
    setError(null);
    try {
      await cartAction();
    } catch {
      setError("Something went wrong updating your cart. Please try again.");
    } finally {
      setPendingId(null);
    }
  };

  const handleClearItems = async () => {
    setClearing(true);
    setError(null);
    try {
      await clearCartItems();
    } catch {
      setError("Could not clear your cart. Please try again.");
    } finally {
      setClearing(false);
    }
  };

  const handleCheckout = async () => {
    setCheckingOut(true);
    setError(null);
    try {
      const order = await checkout();
      if (!order.checkoutUrl) {
        throw new Error("No checkout URL returned.");
      }
      window.location.href = order.checkoutUrl;
    } catch {
      setError("Could not start checkout. Please try again.");
      setCheckingOut(false);
    }
  };

  if (loading && !cart) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", mt: 6 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!cart || cart.items.length === 0) {
    return (
      <Typography color="text.secondary" sx={{ mt: 4 }}>
        Your cart is empty.
      </Typography>
    );
  }

  return (
    <Stack spacing={3} sx={{ mt: 2 }}>
      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined">
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Product</TableCell>
              <TableCell align="right">Price</TableCell>
              <TableCell align="center">Quantity</TableCell>
              <TableCell align="right">Line total</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {cart.items.map((item) => {
              const isUpdating = pendingId === item.productId;
              return (
                <TableRow key={item.id}>
                  <TableCell>{item.productName}</TableCell>
                  <TableCell align="right">
                    ${item.unitPrice.toFixed(2)}
                  </TableCell>
                  <TableCell align="center">
                    <Stack
                      direction="row"
                      spacing={1}
                      sx={{ alignItems: "center", justifyContent: "center" }}
                    >
                      <IconButton
                        size="small"
                        disabled={isUpdating || item.quantity <= 1}
                        onClick={() =>
                          itemAction(item.productId, () =>
                            updateItem(item.productId, item.quantity - 1),
                          )
                        }
                        aria-label="Decrease quantity"
                      >
                        −
                      </IconButton>
                      <Typography sx={{ minWidth: 24, textAlign: "center" }}>
                        {isUpdating ? (
                          <CircularProgress size={14} />
                        ) : (
                          item.quantity
                        )}
                      </Typography>
                      <IconButton
                        size="small"
                        disabled={isUpdating || item.quantity >= 100}
                        onClick={() =>
                          itemAction(item.productId, () =>
                            updateItem(item.productId, item.quantity + 1),
                          )
                        }
                        aria-label="Increase quantity"
                      >
                        +
                      </IconButton>
                    </Stack>
                  </TableCell>
                  <TableCell align="right">
                    ${item.lineTotal.toFixed(2)}
                  </TableCell>
                  <TableCell align="right">
                    <Button
                      size="small"
                      color="error"
                      disabled={isUpdating}
                      onClick={() =>
                        itemAction(item.productId, () =>
                          removeItem(item.productId),
                        )
                      }
                    >
                      Remove
                    </Button>
                  </TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </Paper>

      <Divider />

      <Stack
        direction="row"
        sx={{ alignItems: "center", justifyContent: "space-between" }}
      >
        <Button
          color="error"
          variant="outlined"
          loading={clearing}
          onClick={handleClearItems}
        >
          Clear cart
        </Button>
        <Typography variant="h6" sx={{ fontWeight: 600 }}>
          Total: ${cart.total.toFixed(2)}
        </Typography>
      </Stack>

      <Button
        variant="contained"
        size="large"
        loading={checkingOut}
        onClick={handleCheckout}
        sx={{ alignSelf: "flex-end" }}
      >
        Proceed to payment
      </Button>
    </Stack>
  );
}

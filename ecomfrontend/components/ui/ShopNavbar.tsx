"use client";

import { useState } from "react";
import {
  AppBar,
  Badge,
  Box,
  Button,
  Divider,
  List,
  ListItem,
  ListItemText,
  Popover,
  Toolbar,
  Typography,
} from "@mui/material";
import Link from "next/link";
import { logoutAction } from "@/actions/logout";
import { useCart, useLoadCart } from "../../context/CartProvider";
import { useSession } from "next-auth/react";

export function ShopNavbar({
  userName,
  role,
}: {
  userName: string | undefined | null;
  role: string | undefined | null;
}) {
  const { cart, itemCount, loading } = useCart();
  const [anchorEl, setAnchorEl] = useState<HTMLButtonElement | null>(null);

  useLoadCart();

  return (
    <AppBar position="static" color="default" elevation={1}>
      <Toolbar sx={{ gap: 2 }}>
        <Typography
          variant="h6"
          component="div"
          sx={{ fontWeight: 600, flexGrow: 1 }}
        >
          Shop
        </Typography>

        {userName && (
          <>
            <Button color="inherit" component={Link} href="/shop">
              Shop
            </Button>

            <Button color="inherit" component={Link} href="/orders">
              My Orders
            </Button>

            {role === "Admin" && (
              <Button color="inherit" component={Link} href="/admin">
                Admin
              </Button>
            )}

            <Button
              color="inherit"
              onClick={(e) => setAnchorEl(e.currentTarget)}
            >
              <Badge badgeContent={itemCount} color="primary" showZero>
                Cart
              </Badge>
            </Button>

            <Popover
              open={Boolean(anchorEl)}
              anchorEl={anchorEl}
              onClose={() => setAnchorEl(null)}
              anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
              transformOrigin={{ vertical: "top", horizontal: "right" }}
            >
              <Box sx={{ width: 320, p: 2 }}>
                <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>
                  Your cart
                </Typography>
                <Divider sx={{ my: 1 }} />
                {loading && !cart ? (
                  <Typography color="text.secondary">Loading…</Typography>
                ) : !cart || cart.items.length === 0 ? (
                  <Typography color="text.secondary">
                    Your cart is empty.
                  </Typography>
                ) : (
                  <>
                    <List dense disablePadding>
                      {cart.items.map((item) => (
                        <ListItem
                          key={item.id}
                          disableGutters
                          secondaryAction={
                            <Typography>
                              ${item.lineTotal.toFixed(2)}
                            </Typography>
                          }
                        >
                          <ListItemText
                            primary={item.productName}
                            secondary={`Qty: ${item.quantity}`}
                          />
                        </ListItem>
                      ))}
                    </List>
                    <Divider sx={{ my: 1 }} />
                    <Box
                      sx={{ display: "flex", justifyContent: "space-between" }}
                    >
                      <Typography sx={{ fontWeight: 600 }}>Total</Typography>
                      <Typography sx={{ fontWeight: 600 }}>
                        ${cart.total.toFixed(2)}
                      </Typography>
                    </Box>
                    <Button
                      component={Link}
                      href="/checkout"
                      variant="contained"
                      fullWidth
                      sx={{ mt: 2 }}
                      onClick={() => setAnchorEl(null)}
                    >
                      Go to checkout
                    </Button>
                  </>
                )}
              </Box>
            </Popover>

            <Typography variant="body2" color="text.secondary">
              {userName}
            </Typography>

            <form action={logoutAction}>
              <Button type="submit" variant="outlined" size="small">
                Sign out
              </Button>
            </form>
          </>
        )}
      </Toolbar>
    </AppBar>
  );
}

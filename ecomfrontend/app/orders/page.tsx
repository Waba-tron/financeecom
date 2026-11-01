
import {
  Chip,
  Container,
  Divider,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { getMyOrders } from "@/actions/orders";

const STATUS_LABELS: Record<number, { label: string; color: "warning" | "success" | "error" }> = {
  0: { label: "Pending", color: "warning" },
  1: { label: "Paid", color: "success" },
  2: { label: "Cancelled", color: "error" },
};

export default async function MyOrdersPage() {
    
  const orders = await getMyOrders();

  return (
    <Container component="main" maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" sx={{ fontWeight: 600, mb: 3 }}>
        My Orders
      </Typography>

      {orders.length === 0 ? (
        <Typography color="text.secondary">
          You have not placed any orders yet.
        </Typography>
      ) : (
        <Stack spacing={3}>
          {orders.map((order) => {
            const status = STATUS_LABELS[order.status] ?? {
              label: "Unknown",
              color: "warning" as const,
            };
            return (
              <Paper key={order.id} variant="outlined" sx={{ p: 2 }}>
                <Stack
                  direction="row"
                  sx={{
                    alignItems: "center",
                    justifyContent: "space-between",
                    mb: 1,
                  }}
                >
                  <Stack>
                    <Typography sx={{ fontWeight: 600 }}>
                      Order #{order.id}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {new Date(order.createdDate).toLocaleString()}
                    </Typography>
                  </Stack>
                  <Chip label={status.label} color={status.color} size="small" />
                </Stack>

                <Divider sx={{ my: 1 }} />

                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>Product</TableCell>
                      <TableCell align="right">Price</TableCell>
                      <TableCell align="center">Qty</TableCell>
                      <TableCell align="right">Line total</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {order.items.map((item) => (
                      <TableRow key={item.id}>
                        <TableCell>{item.productName}</TableCell>
                        <TableCell align="right">
                          ${item.unitPrice.toFixed(2)}
                        </TableCell>
                        <TableCell align="center">{item.quantity}</TableCell>
                        <TableCell align="right">
                          ${item.lineTotal.toFixed(2)}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>

                <Stack
                  direction="row"
                  sx={{ justifyContent: "flex-end", mt: 1 }}
                >
                  <Typography sx={{ fontWeight: 600 }}>
                    Total: ${order.total.toFixed(2)}
                  </Typography>
                </Stack>
              </Paper>
            );
          })}
        </Stack>
      )}
    </Container>
  );
}

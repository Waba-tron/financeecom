import { Button, Container, Paper, Stack, Typography } from "@mui/material";

export default async function CheckoutSuccessPage({
  searchParams,
}: {
  searchParams: Promise<{ orderId?: string }>;
}) {
  const { orderId } = await searchParams;

  return (
    <Container component="main" maxWidth="sm" sx={{ py: 6 }}>
      <Paper variant="outlined" sx={{ p: 4 }}>
        <Stack spacing={2} sx={{ alignItems: "center", textAlign: "center" }}>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 600 }}>
            Payment successful
          </Typography>
          <Typography color="text.secondary">
            {orderId
              ? `Thank you! Your order #${orderId} has been placed.`
              : "Thank you! Your order has been placed."}
          </Typography>
          <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
            <Button href="/orders" variant="contained">
              View my orders
            </Button>
            <Button href="/shop" variant="outlined">
              Continue shopping
            </Button>
          </Stack>
        </Stack>
      </Paper>
    </Container>
  );
}

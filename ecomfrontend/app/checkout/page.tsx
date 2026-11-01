import { Container, Typography } from "@mui/material";
import { CheckoutCart } from "@/components/shop/CheckoutCart";

export default function CheckoutPage() {
  return (
    <Container component="main" maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" sx={{ fontWeight: 600 }}>
        Checkout
      </Typography>

      <CheckoutCart />
    </Container>
  );
}

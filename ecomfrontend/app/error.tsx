"use client";

import { Alert, AlertTitle, Button, Container } from "@mui/material";

export default function ShopError({ error, reset }: { error: Error & { digest?: string }; reset: () => void }) {
  return (
    <Container component="main" maxWidth="md" sx={{ py: 4 }}>
      <Alert
        severity="error"
        action={
          <Button color="inherit" size="small" onClick={reset}>
            Try again
          </Button>
        }
      >
        <AlertTitle>Something went wrong</AlertTitle>
        {error.message}
      </Alert>
    </Container>
  );
}

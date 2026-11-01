import { Container, Typography } from "@mui/material";
import { getProducts } from "@/actions/products";
import { getProductTypes } from "@/actions/product-types";
import { AdminProducts } from "@/components/admin/AdminProducts";

export default async function AdminPage() {
  const [productsResult, productTypes] = await Promise.all([
    getProducts({ page: 1, pageSize: 100 }),
    getProductTypes(),
  ]);

  return (
    <Container component="main" maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" sx={{ fontWeight: 600, mb: 3 }}>
        Admin · Products
      </Typography>

      <AdminProducts
        initialProducts={productsResult.items}
        productTypes={productTypes}
      />
    </Container>
  );
}

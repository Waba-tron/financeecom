import { Container, Typography } from "@mui/material";
import { getProductTypes } from "@/actions/product-types";
import { ProductTypeSelect } from "@/components/shop/ProductTypeSelect";


export default async function ShopPage() {

  const productTypes = await getProductTypes();

  return (
    <Container component="main" maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" sx={{ fontWeight: 600 }}>
        Shop
      </Typography>

      <ProductTypeSelect productTypes={productTypes} />
    </Container>
  );
}

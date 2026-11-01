"use client";

import { useEffect, useState } from "react";
import {
  Box,
  Button,
  CircularProgress,
  FormControl,
  InputLabel,
  List,
  ListItem,
  ListItemText,
  MenuItem,
  Pagination,
  Select,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { getProducts, type PagedProducts } from "@/actions/products";
import { useCart, useLoadCart } from "../../context/CartProvider";
import { useDebounce } from "@/hooks/useDebounce";

export type ProductType = {
  id: number;
  name: string;
  description: string | null;
};

const PAGE_SIZE = 10;

export function ProductTypeSelect({
  productTypes,
}: {
  productTypes: ProductType[];
}) {
  const [typeId, setTypeId] = useState("");
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebounce(search);
  const [page, setPage] = useState(1);
  const [result, setResult] = useState<PagedProducts | null>(null);
  const [loading, setLoading] = useState(false);
  const { addItem } = useCart();
  const [addingId, setAddingId] = useState<number | null>(null);

  useLoadCart();

  useEffect(() => {

    let active = true;

    const loadProducts = async () => {
      setLoading(true);
      try {
        const data = await getProducts({
          productTypeId: typeId ? Number(typeId) : undefined,
          search: debouncedSearch,
          page,
          pageSize: PAGE_SIZE,
        });
        if (active) setResult(data);

      } finally {

        if (active) setLoading(false);
        
      }
    };

    loadProducts();

    return () => {
      active = false;
    };

  }, [typeId, debouncedSearch, page]);

  const handleTypeChange = (id: string) => {
    setTypeId(id);
    setPage(1);
  };

  const handleSearchChange = (value: string) => {
    setSearch(value);
    setPage(1);
  };

  const handleAdd = async (productId: number) => {
    setAddingId(productId);
    try {
      await addItem(productId, 1);
    } finally {
      setAddingId(null);
    }
  };

  const products = result?.items ?? [];

  return (
    <>
      <Stack direction={{ xs: "column", sm: "row" }} spacing={2} sx={{ mt: 2 }}>
        <FormControl fullWidth size="small">
          <InputLabel id="product-type-label">Product type</InputLabel>
          <Select
            labelId="product-type-label"
            label="Product type"
            value={typeId}
            onChange={(e) => handleTypeChange(e.target.value)}
          >
            <MenuItem value="">All types</MenuItem>
            {productTypes.map((type) => (
              <MenuItem key={type.id} value={String(type.id)}>
                {type.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <TextField
          fullWidth
          size="small"
          label="Search products"
          value={search}
          onChange={(e) => handleSearchChange(e.target.value)}
        />
      </Stack>

      {loading && <CircularProgress size={24} sx={{ mt: 2 }} />}

      {!loading && products.length === 0 && (
        <Typography color="text.secondary" sx={{ mt: 2 }}>
          No products found.
        </Typography>
      )}

      <List sx={{ mt: 1 }}>
        {products.map((product) => (
          <ListItem
            key={product.id}
            disableGutters
            secondaryAction={
              <Stack direction="row" spacing={2} sx={{ alignItems: "center" }}>
                <Typography>${product.price.toFixed(2)}</Typography>
                <Button
                  variant="contained"
                  size="small"
                  loading={addingId === product.id}
                  onClick={() => handleAdd(product.id)}
                >
                  Add to cart
                </Button>
              </Stack>
            }
          >
            <ListItemText
              primary={product.name}
              secondary={product.description}
            />
          </ListItem>
        ))}
      </List>

      {result && result.totalPages > 1 && (
        <Box sx={{ display: "flex", justifyContent: "center", mt: 2 }}>
          <Pagination
            count={result.totalPages}
            page={result.page}
            onChange={(_, value) => setPage(value)}
            color="primary"
          />
        </Box>
      )}
    </>
  );
}

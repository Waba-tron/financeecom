"use client";

import { useState } from "react";
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import {
  createProduct,
  deleteProduct,
  updateProduct,
  type Product,
  type ProductParams,
} from "@/actions/products";
import type { ProductType } from "@/components/shop/ProductTypeSelect";

type FormState = ProductParams & { id: number | null };

const emptyForm = (productTypeId: number): FormState => ({
  id: null,
  name: "",
  description: "",
  price: 0,
  productTypeId,
});

export function AdminProducts({
  initialProducts,
  productTypes,
}: {
  initialProducts: Product[];
  productTypes: ProductType[];
}) {
  const [products, setProducts] = useState<Product[]>(initialProducts);
  const [form, setForm] = useState<FormState | null>(null);
  const [saving, setSaving] = useState(false);
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const defaultTypeId = productTypes[0]?.id ?? 0;

  const openCreate = () => {
    setError(null);
    setForm(emptyForm(defaultTypeId));
  };

  const openEdit = (product: Product) => {
    setError(null);
    setForm({
      id: product.id,
      name: product.name,
      description: product.description,
      price: product.price,
      productTypeId: product.productTypeId,
    });
  };

  const closeForm = () => setForm(null);

  const handleSave = async () => {

    if (!form) return;
    setSaving(true);
    setError(null);

    try {
      const product: ProductParams = {
        name: form.name,
        description: form.description,
        price: form.price,
        productTypeId: form.productTypeId,
      };

      if (form.id == null) {
        const created = await createProduct(product);
        setProducts((prev) => [...prev, created]);

      } else {
        const updated = await updateProduct(form.id, product);
        setProducts((prev) =>
          prev.map((p) => (p.id === updated.id ? updated : p)),
        );
      }
      
      setForm(null);

    } catch {
      setError("Could not save the product. Please try again.");
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id: number) => {
    setDeletingId(id);
    setError(null);
    try {
      await deleteProduct(id);
      setProducts((prev) => prev.filter((p) => p.id !== id));
    } catch {
      setError("Could not delete the product. Please try again.");
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <Stack spacing={2}>
      {error && <Alert severity="error">{error}</Alert>}

      <Stack direction="row" sx={{ justifyContent: "flex-end" }}>
        <Button variant="contained" onClick={openCreate}>
          Add product
        </Button>
      </Stack>

      <Paper variant="outlined">
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Type</TableCell>
              <TableCell align="right">Price</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {products.map((product) => (
              <TableRow key={product.id}>
                <TableCell>{product.name}</TableCell>
                <TableCell>{product.productTypeName}</TableCell>
                <TableCell align="right">${product.price.toFixed(2)}</TableCell>
                <TableCell align="right">
                  <Stack
                    direction="row"
                    spacing={1}
                    sx={{ justifyContent: "flex-end" }}
                  >
                    <Button size="small" onClick={() => openEdit(product)}>
                      Edit
                    </Button>
                    <Button
                      size="small"
                      color="error"
                      loading={deletingId === product.id}
                      onClick={() => handleDelete(product.id)}
                    >
                      Delete
                    </Button>
                  </Stack>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>

      <Dialog open={form != null} onClose={closeForm} fullWidth maxWidth="sm">
        <DialogTitle>
          {form?.id == null ? "Add product" : "Edit product"}
        </DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="Name"
              value={form?.name ?? ""}
              onChange={(e) =>
                setForm((f) => (f ? { ...f, name: e.target.value } : f))
              }
              fullWidth
            />
            <TextField
              label="Description"
              value={form?.description ?? ""}
              onChange={(e) =>
                setForm((f) =>
                  f ? { ...f, description: e.target.value } : f,
                )
              }
              fullWidth
              multiline
              minRows={2}
            />
            <TextField
              label="Price"
              type="number"
              value={form?.price ?? 0}
              onChange={(e) =>
                setForm((f) =>
                  f ? { ...f, price: Number(e.target.value) } : f,
                )
              }
              fullWidth
            />
            <TextField
              label="Product type"
              select
              value={form?.productTypeId ?? ""}
              onChange={(e) =>
                setForm((f) =>
                  f ? { ...f, productTypeId: Number(e.target.value) } : f,
                )
              }
              fullWidth
            >
              {productTypes.map((type) => (
                <MenuItem key={type.id} value={type.id}>
                  {type.name}
                </MenuItem>
              ))}
            </TextField>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={closeForm}>Cancel</Button>
          <Button variant="contained" loading={saving} onClick={handleSave}>
            Save
          </Button>
        </DialogActions>
      </Dialog>
    </Stack>
  );
}

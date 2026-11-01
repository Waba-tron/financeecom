"use client";

import { Alert, Button, Stack, TextField } from "@mui/material";
import { Controller, useForm } from "react-hook-form";
import { loginAction } from "@/actions/login";

type LoginValues = { email: string; password: string };

export function LoginForm({ redirectTo }: { redirectTo: string }) {
  const {
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<LoginValues>({ defaultValues: { email: "", password: "" } });

  const onSubmit = async (values: LoginValues) => {
    const result = await loginAction(values, redirectTo);
    if (!result) return;

    if (result.validationErrors) {
      for (const [field, messages] of Object.entries(result.validationErrors)) {
        const key = field.charAt(0).toLowerCase() + field.slice(1);
        const message = messages.join(" ");
        if (key === "email" || key === "password") {
          setError(key, { message });
        } else {
          setError("root", { message });
        }
      }
      return;
    }

    if (result.error) setError("root", { message: result.error });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} noValidate>
      <Stack spacing={2}>
        {errors.root && <Alert severity="error">{errors.root.message}</Alert>}
        <Controller
          name="email"
          control={control}
          rules={{ required: "Email is required" }}
          render={({ field, fieldState }) => (
            <TextField
              {...field}
              label="Email"
              autoComplete="email"
              fullWidth
              error={!!fieldState.error}
              helperText={fieldState.error?.message}
            />
          )}
        />
        <Controller
          name="password"
          control={control}
          rules={{ required: "Password is required" }}
          render={({ field, fieldState }) => (
            <TextField
              {...field}
              label="Password"
              type="password"
              autoComplete="current-password"
              fullWidth
              error={!!fieldState.error}
              helperText={fieldState.error?.message}
            />
          )}
        />
        <Button type="submit" variant="contained" size="large" loading={isSubmitting}>
          Sign in
        </Button>
      </Stack>
    </form>
  );
}
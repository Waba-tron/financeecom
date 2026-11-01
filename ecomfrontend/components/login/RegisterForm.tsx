"use client";

import { Alert, Button, Stack, TextField } from "@mui/material";
import { Controller, useForm } from "react-hook-form";
import { registerAction } from "@/actions/register";

type RegisterValues = {
  displayName: string;
  email: string;
  password: string;
  confirmPassword: string;
};

export function RegisterForm({ redirectTo }: { redirectTo: string }) {
  const {
    control,
    handleSubmit,
    getValues,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<RegisterValues>({
    defaultValues: { displayName: "", email: "", password: "", confirmPassword: "" },
  });

  const onSubmit = async (values: RegisterValues) => {
    const result = await registerAction(
      { email: values.email, password: values.password, displayName: values.displayName },
      redirectTo
    );
    if (!result) return;

    if (result.validationErrors) {
      for (const [field, messages] of Object.entries(result.validationErrors)) {
        const key = field.charAt(0).toLowerCase() + field.slice(1);
        const message = messages.join(" ");
        if (key === "email" || key === "password" || key === "displayName") {
          setError(key as keyof RegisterValues, { message });
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
          name="displayName"
          control={control}
          rules={{ required: "Display name is required" }}
          render={({ field, fieldState }) => (
            <TextField
              {...field}
              label="Display name"
              autoComplete="name"
              fullWidth
              error={!!fieldState.error}
              helperText={fieldState.error?.message}
            />
          )}
        />

        <Controller
          name="email"
          control={control}
          rules={{ required: "Email is required" }}
          render={({ field, fieldState }) => (
            <TextField
              {...field}
              label="Email"
              type="email"
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
              autoComplete="new-password"
              fullWidth
              error={!!fieldState.error}
              helperText={fieldState.error?.message}
            />
          )}
        />

        <Controller
          name="confirmPassword"
          control={control}
          rules={{
            required: "Please confirm your password",
            validate: (v) => v === getValues("password") || "Passwords do not match",
          }}
          render={({ field, fieldState }) => (
            <TextField
              {...field}
              label="Confirm password"
              type="password"
              autoComplete="new-password"
              fullWidth
              error={!!fieldState.error}
              helperText={fieldState.error?.message}
            />
          )}
        />

        <Button type="submit" variant="contained" size="large" loading={isSubmitting}>
          Create account
        </Button>
        
      </Stack>
    </form>
  );
}
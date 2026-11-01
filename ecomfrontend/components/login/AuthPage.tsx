"use client";

import { useState } from "react";
import { Box, Container, Link, Paper, Typography } from "@mui/material";
import { LoginForm } from "@/components/login/LoginForm";
import { RegisterForm } from "@/components/login/RegisterForm";

export type AuthMode = "login" | "register";

const copy = {
  login: {
    title: "Sign in",
    subtitle: "Welcome back. Enter your details to continue.",
    switchText: "No account yet?",
    switchLabel: "Create one",
  },
  register: {
    title: "Create account",
    subtitle: "Sign up to start shopping.",
    switchText: "Already registered?",
    switchLabel: "Sign in",
  },
} as const;

export function AuthPage({ initialMode, redirectTo }: { initialMode: AuthMode; redirectTo: string }) {
  const [mode, setMode] = useState<AuthMode>(initialMode);
  const { title, subtitle, switchText, switchLabel } = copy[mode];

  return (
    <Container component="main" maxWidth="xs" sx={{ flex: 1, display: "flex", alignItems: "center", py: 6 }}>
      <Paper variant="outlined" sx={{ width: "100%", p: 4 }}>
        <Typography variant="h5" component="h1" sx={{ fontWeight: 600 }}>
          {title}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          {subtitle}
        </Typography>
        <Box sx={{ mt: 3 }}>{mode === "login" ? <LoginForm redirectTo={redirectTo} /> : <RegisterForm />}</Box>
        <Typography variant="body2" color="text.secondary" align="center" sx={{ mt: 3 }}>
          {switchText}{" "}
          <Link
            component="button"
            type="button"
            sx={{ fontWeight: 500, verticalAlign: "baseline" }}
            onClick={() => setMode(mode === "login" ? "register" : "login")}
          >
            {switchLabel}
          </Link>
        </Typography>
      </Paper>
    </Container>
  );
}

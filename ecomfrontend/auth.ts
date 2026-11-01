import NextAuth from "next-auth"
import Credentials from "next-auth/providers/credentials"
import type { Provider } from "next-auth/providers"
 
declare module "next-auth" {
  interface User {
    accessToken?: string
    claims?: JwtClaims
  }
  interface Session {
    accessToken?: string
    claims?: JwtClaims
  }
}

export type JwtClaims = {
  sub: string
  email: string
  role: string
  jti: string
  exp: number
  iss?: string
  aud?: string
}

function decodeJwt(token: string): JwtClaims {
  return JSON.parse(Buffer.from(token.split(".")[1], "base64url").toString("utf8"))
}

const API_URL = process.env.ECOM_API_URL ?? "http://localhost:5041"

const providers: Provider[] = [
  Credentials({
    credentials: {
      email: { label: "Email", type: "email" },
      password: { label: "Password", type: "password" },
    },
    async authorize(credentials) {
      if (typeof credentials.email !== "string" || typeof credentials.password !== "string") return null

      const res = await fetch(`${API_URL}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: credentials.email, password: credentials.password }),
      })

      // Carry the backend error payload out so the UI can show field-level messages.
      if (!res.ok) {
        const body = await res.text()
        throw new Error(body || JSON.stringify({ StatusCode: res.status }))
      }

      const data: { token: string; displayName: string } = await res.json()
      const claims = decodeJwt(data.token);

      return {
        id: claims.sub,
        name: data.displayName,
        email: claims.email,
        accessToken: data.token,
        claims,
      }
    },
  }),
]
 
 
export const { handlers, auth, signIn, signOut } = NextAuth({
  providers,
  pages: {
    signIn: "/login",
  },
  callbacks: {
    jwt({ token, user }) {
      if (user?.accessToken) {
        token.accessToken = user.accessToken
        token.claims = user.claims
      }
      return token
    },
    session({ session, token }) {
      session.accessToken = token.accessToken as string | undefined
      session.claims = token.claims as JwtClaims | undefined
      return session
    },
  },
})
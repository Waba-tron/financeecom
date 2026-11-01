import { NextRequest, NextResponse } from "next/server";
import { auth } from "@/auth";

export async function proxy(req: NextRequest) {
  const { pathname, origin } = req.nextUrl;
  const protectedPaths = ["/shop", "/checkout", "/orders", "/admin"];

  const isProtected = protectedPaths.some((path) => pathname.startsWith(path));
  if (!isProtected) return NextResponse.next();

  const session = await auth();
  if (!session?.user) return NextResponse.redirect(new URL("/", origin));

  if (pathname.startsWith("/admin") && session.claims?.role !== "Admin") {
    return NextResponse.redirect(new URL("/", origin));
  }

  return NextResponse.next();
}

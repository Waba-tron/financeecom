import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import { ShopNavbar } from "@/components/ui/ShopNavbar";
import "./globals.css";
import { AppThemeProvider } from "@/context/AppThemeProvider";
import { CartProvider } from "@/context/CartProvider";
import { auth } from "@/auth";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Ecom Shop",
  description: "Online shop",
};

export default async function RootLayout({ children }: LayoutProps<"/">) {
  const session = await auth();

  return (
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="min-h-full flex flex-col">
        <AppThemeProvider>
          <CartProvider>
            <ShopNavbar
              userName={session?.user?.name}
              role={session?.claims?.role}
            />

            {children}
          </CartProvider>
        </AppThemeProvider>
      </body>
    </html>
  );
}

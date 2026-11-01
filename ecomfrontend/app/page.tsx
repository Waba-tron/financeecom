import { AuthPage } from "@/components/login/AuthPage";

export default function Home() {
  return <AuthPage initialMode="login" redirectTo="/shop" />;
}

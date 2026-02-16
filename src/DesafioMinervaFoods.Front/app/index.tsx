import { Redirect } from "expo-router";
import { useAuth } from "@/contexts/AuthContext";

export default function Index() {
  const { user, isLoading } = useAuth();

  // Enquanto carrega o estado de login (ex: lendo do AsyncStorage)
  if (isLoading) return null;

  // Se estiver logado, manda para o caminho exato onde está seu orders
  if (user) {
    return <Redirect href="/" />;
  }

  // Caso contrário, manda para o login
  return <Redirect href="/auth" />;
}
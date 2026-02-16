
import React, { createContext, useContext, useState, useEffect, ReactNode } from "react";
import { Platform } from "react-native";
import { OrderService } from "@/services/OrderService";
import { setBearerToken, clearBearerToken, BACKEND_URL } from "@/utils/api";

interface User {
  id: string;
  email: string;
  name?: string;
  image?: string;
}

interface AuthContextType {
  user: User | null;
  loading: boolean;
  signInWithEmail: (username: string, password: string) => Promise<void>;
  signOut: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(false);

  const signInWithEmail = async (username: string, password: string) => {
    try {
      console.log('AuthContext: Signing in with email', { username });
      setLoading(true);

      const response = await fetch(`${BACKEND_URL}/api/auth/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ username, password }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        console.error('AuthContext: Login failed', errorData);
        
        if (errorData.isSuccess === false && errorData.errors) {
          throw new Error(errorData.errors.join(', '));
        }
        
        throw new Error(`Login falhou: ${response.statusText}`);
      }

      const result = await response.json();
      console.log('AuthContext: Login response', result);

      if (result.isSuccess && result.data) {
        const token = result.data.token || result.data;
        await setBearerToken(token);
        
        setUser({
          id: username,
          email: username,
          name: username,
        });

        // Connect to SignalR for real-time notifications
        OrderService.connectWebSocket();
        
        console.log('AuthContext: Login successful');
      } else {
        throw new Error(result.errors?.join(', ') || 'Login falhou');
      }
    } catch (error: any) {
      console.error('AuthContext: Login error', error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  const signOut = async () => {
    try {
      console.log('AuthContext: Signing out');
      setUser(null);
      await clearBearerToken();
      OrderService.disconnectWebSocket();
      console.log('AuthContext: Sign out successful');
    } catch (error) {
      console.error('AuthContext: Sign out error', error);
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        signInWithEmail,
        signOut,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
}

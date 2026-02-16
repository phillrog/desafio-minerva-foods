
import { useColorScheme } from "react-native";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { useFonts } from "expo-font";
import { WidgetProvider } from "@/contexts/WidgetContext";
import { AuthProvider } from "@/contexts/AuthContext";
import { SystemBars } from "react-native-edge-to-edge";
import { StatusBar } from "expo-status-bar";
import {
  DarkTheme,
  DefaultTheme,
  ThemeProvider,
} from "@react-navigation/native";
import "react-native-reanimated";
import React, { useEffect } from "react";
import { useNetworkState } from "expo-network";
import { Stack } from "expo-router";
import * as SplashScreen from "expo-splash-screen";

SplashScreen.preventAutoHideAsync();

export default function RootLayout() {
  const [loaded] = useFonts({
    SpaceMono: require("../assets/fonts/SpaceMono-Regular.ttf"),
  });

  const { isConnected } = useNetworkState();
  const colorScheme = useColorScheme();

  useEffect(() => {
    if (loaded) {
      SplashScreen.hideAsync();
    }
  }, [loaded]);

  if (!loaded) {
    return null;
  }

  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <ThemeProvider value={colorScheme === "dark" ? DarkTheme : DefaultTheme}>
        <SystemBars style={colorScheme === "dark" ? "light" : "dark"} />
        <StatusBar style={colorScheme === "dark" ? "light" : "dark"} />
        <AuthProvider>
          <WidgetProvider>
            <Stack initialRouteName="auth" screenOptions={{ headerShown: false }}>
              <Stack.Screen name="auth" options={{ headerShown: false }} />
              <Stack.Screen name="auth-popup" options={{ headerShown: false }} />
              <Stack.Screen name="auth-callback" options={{ headerShown: false }} />
              <Stack.Screen name="orders" options={{ headerShown: false }} />
              <Stack.Screen name="order-details" options={{ headerShown: false }} />
              <Stack.Screen name="create-order" options={{ headerShown: false }} />
              <Stack.Screen name="(tabs)" options={{ headerShown: false }} />
              <Stack.Screen name="+not-found" />
            </Stack>
          </WidgetProvider>
        </AuthProvider>
      </ThemeProvider>
    </GestureHandlerRootView>
  );
}

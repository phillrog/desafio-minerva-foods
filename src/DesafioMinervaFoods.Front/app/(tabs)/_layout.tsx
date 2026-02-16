// app/(tabs)/_layout.tsx
import React from "react";
import { Tabs } from "expo-router";
import FloatingTabBar from "@/components/FloatingTabBar";

export default function TabLayout() {
  return (
    <Tabs
    screenOptions={{
      headerShown: false,
      tabBarActiveTintColor: "#000", 
      tabBarInactiveTintColor: "#888", 
    }}
      tabBar={(props) => (
        <FloatingTabBar
          {...props}
          tabs={[
            { name: "Pedidos", route: "/(tabs)/(home)", icon: "shopping-cart" },
            { name: "Novo", route: "/(tabs)/create-order", icon: "add-circle-outline" },
            { name: "Perfil", route: "/(tabs)/profile", icon: "person-outline" },
          ]}
        />
      )}
      
      screenOptions={{ headerShown: false }}
    >
      <Tabs.Screen name="(home)" />
      <Tabs.Screen name="profile" />
      <Tabs.Screen name="create-order" />
      <Tabs.Screen name="order-details" options={{ href: null }} />
    </Tabs>
  );
}
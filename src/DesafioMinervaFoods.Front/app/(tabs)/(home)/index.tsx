// app/(tabs)/(home)/index.tsx
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { StyleSheet, View, Text, FlatList, RefreshControl, SafeAreaView, TextInput, TouchableOpacity } from "react-native";
import { useTheme } from "@react-navigation/native";
import { OrderCard } from "@/components/OrderCard";
import { Toast } from "@/components/Toast";
import { useOrders } from "@/hooks/useOrders";
import { OrderService } from "@/services/OrderService";
import { colors } from "@/styles/commonStyles";
import { useRouter, Stack } from "expo-router";
import { Package, UserIcon, ShoppingBag, Clock, DollarSign, CheckCircle, Delete, Search, Plus } from "lucide-react-native";

export default function HomeScreen() {
  
  const router = useRouter();
  const { orders, loading, fetchOrders, approveOrder } = useOrders();
  const [refreshing, setRefreshing] = useState(false);
  const [search, setSearch] = useState("");

  const [toastVisible, setToastVisible] = useState(false);
  const [toastMessage, setToastMessage] = useState("");

  useEffect(() => {
    console.log("Iniciando conexão SignalR...");
    OrderService.connectWebSocket();
    fetchOrders();
    const unsubscribe = OrderService.addNotificationListener((notification) => {
      setToastMessage(notification.message || "Atualização no pedido");
      setToastVisible(true);
      fetchOrders();
    });
    return () => unsubscribe();
  }, [fetchOrders]);


  const handleApproveFromList = useCallback(async (orderId: string) => {
    require('react-native').Alert.alert(
      "Aprovar Pedido",
      "Deseja confirmar a aprovação deste pedido?",
      [
        { text: "Cancelar", style: "cancel" },
        {
          text: "Sim, Aprovar",
          onPress: async () => {
            const result = await approveOrder(orderId);
            if (result.success) {
              setToastMessage("Pedido aprovado com sucesso!");
              setToastVisible(true);
              fetchOrders(); // Atualiza a lista para remover o botão do card aprovado
            } else {
              setToastMessage(result.message);
              setToastVisible(true);
            }
          },
        },
      ]
    );
  }, [approveOrder, fetchOrders]);

  const filteredOrders = useMemo(() => {
    const searchLower = search.toLowerCase().trim();

    return orders.filter((order) => {
      // 1. Busca por Nome do Cliente
      const customerMatch = order.customerName
        .toLowerCase()
        .includes(searchLower);

      // 2. Busca por ID (convertendo para string)
      const idMatch = String(order.id).toLowerCase().includes(searchLower);

      // 3. Busca por Valor (formatando o número para string para encontrar "196" em "1196.00")
      const valueMatch = String(order.totalAmount).includes(searchLower);

      // Retorna verdadeiro se QUALQUER um dos critérios bater
      return customerMatch || idMatch || valueMatch;
    });
  }, [orders, search]);

  // Lógica de Contagem por Status
  const stats = useMemo(() => {
    return {
      total: orders.length,
      amount: orders.reduce((sum, o) => sum + o.totalAmount, 0),
      criado: orders.filter((o) => o.status === "Criado").length,
      processando: orders.filter((o) => o.status === "Processando").length,
      pago: orders.filter((o) => o.status === "Pago").length,
      cancelado: orders.filter((o) => o.status === "Cancelado").length,
    };
  }, [orders]);
  
  const handleRefresh = async () => {
    setRefreshing(true);
    await fetchOrders();
    setRefreshing(false);
  };

  const formatCurrency = (value) => {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(value);
  };

  const renderEmptyList = () => (
    <View style={styles.emptyContainer}>
      <Package size={48} color="#ccc" style={{ marginBottom: 10 }} />
      <Text style={styles.emptyText}>Nenhum pedido encontrado</Text>
      {search.length > 0 && (
        <Text style={styles.emptySubText}>Tente ajustar seus filtros de busca</Text>
      )}
    </View>
  );

  return (
    <SafeAreaView style={styles.container} edges={["top"]}>
      <Stack.Screen options={{ headerShown: false }} />

      {/* HEADER LOGO */}
      <View style={styles.header}>
        <View style={styles.brandContainer}>
          <Text style={styles.brandDesafio}>desafio</Text>
          <Text style={styles.brandMinerva}>minerva</Text>
          <View style={styles.rowFooter}>
            <Text style={styles.brandFoods}>foods</Text>
            <View style={styles.miniAuthorContainer}>
              <Text style={styles.miniAuthor}>PHILLIPE ROGER SOUZA</Text>
            </View>
          </View>
        </View>
        
      </View>

      {/* KPIs PRINCIPAIS */}

      <View style={styles.statsRow}>
        {/* Card Pedidos */}
        <View style={[styles.statCard, { backgroundColor: "#2b5373" }]}>
          <Text style={styles.statLabel}>PEDIDOS GERAL</Text>

          <ShoppingBag size={48} color="rgba(255,255,255,0.25)" />
          <View style={styles.statMainContent}>
            <View style={styles.statValueContainer}>
              <View style={styles.miniBadge}>
                <Clock size={10} color="#FFD700" />
                <Text style={styles.miniBadgeText}>hoje </Text>
                <Text style={styles.statValue}>{stats.total}</Text>
              </View>
            </View>
          </View>
        </View>

        {/* Card Receita */}
        <View style={[styles.statCard, { backgroundColor: "#2e7d32" }]}>
          <Text style={styles.statLabel}>TOTAL GERAL</Text>

          <DollarSign size={48} color="rgba(255,255,255,0.25)" />
          <View style={styles.statMainContent}>
            <View style={styles.statValueContainer}>
              <Text style={styles.statValue}>
                {formatCurrency(stats.amount)}
              </Text>
            </View>
          </View>
        </View>
      </View>

      {/* SUB-CONTADORES (STATUS PILLS) */}
      <View style={styles.pillsRow}>
        <View style={styles.pill}>
          <Clock size={10} color="#f59e0b" />
          <Text style={styles.pillText}>{stats.criado} Criados</Text>
        </View>
        <View style={styles.pill}>
          <Package size={10} color="#3b82f6" />
          <Text style={styles.pillText}>{stats.processando} Em proc.</Text>
        </View>
        <View style={styles.pill}>
          <CheckCircle size={10} color="#10b981" />
          <Text style={styles.pillText}>{stats.pago} Pagos</Text>
        </View>
        <View style={styles.pill}>
          <Delete size={10} color="red" />
          <Text style={styles.pillText}>{stats.cancelado} Canc.</Text>
        </View>
      </View>

      {/* BUSCA */}
      <View style={styles.searchContainer}>
        <Search
          size={20}
          color={colors.textSecondary}
          style={styles.searchIcon}
        />
        <TextInput
          style={styles.searchInput}
          placeholder="Filtrar pedidos..."
          value={search}
          onChangeText={setSearch}
        />
      </View>

      {/* LISTA */}
      <FlatList
        data={filteredOrders}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => (
          <OrderCard
            order={item}
            onPress={() => router.push(`/order-details?id=${item.id}`)}
            onApprove={handleApproveFromList}
          />
        )}
        ListEmptyComponent={!loading ? renderEmptyList : null}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl
            refreshing={refreshing}
            onRefresh={handleRefresh}
            tintColor={colors.primary}
          />
        }
      />
      
      <View style={styles.footerContainer}>
        <Text style={styles.footerText}>
          P H I L L I P E R O G E R S O U Z A
        </Text>
        <Text style={styles.footerYear}>@ 2 0 2 6</Text>
      </View>

      <Toast
        message={toastMessage}
        visible={toastVisible}
        onHide={() => setToastVisible(false)}
      />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: "#f8f9fa" },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    padding: 20,
  },
  brandContainer: { flexDirection: "column" },
  brandDesafio: {
    fontSize: 24,
    fontWeight: "700",
    color: "#ececec",
    lineHeight: 22,
  },
  brandMinerva: {
    fontSize: 22,
    fontWeight: "700",
    color: "#2b5373",
    lineHeight: 22,
  },
  brandFoods: { fontSize: 20, fontWeight: "700", color: "#e84c53" },
  miniAuthor: {
    fontSize: 6,
    color: "#bbb",
    letterSpacing: 2,
    fontWeight: "600",
  },

  logoutButton: {
    flexDirection: "row",
    alignItems: "center",
    gap: 5,
    backgroundColor: "#fff",
    padding: 8,
    borderRadius: 8,
    elevation: 1,
  },
  logoutText: { color: colors.error, fontWeight: "700", fontSize: 12 },

  // KPIs
  statCard: {
    flex: 1,
    padding: 15,
    borderRadius: 20,
    minHeight: 140, // Aumentei um pouco para caber a estrutura
    alignItems: "center", // Separa o label (topo) do conteúdo (meio/fim)
    justifyContent: "space-between",
    elevation: 5,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.2,
    shadowRadius: 4,
  },
  statLabel: {
    fontSize: 10,
    fontWeight: "900",
    color: "rgba(255,255,255,0.7)",
    letterSpacing: 1,
    textAlign: "left",
  },
  statMainContent: {
    flexDirection: "row", // Ícone ao lado do valor
    alignItems: "center",
    justifyContent: "space-between",
    marginTop: 10,
  },
  statValueContainer: {
    alignItems: "flex-end", // Alinha o número à direita
  },
  statValue: {
    fontSize: 12,
    fontWeight: "bold",
    color: "#ffffff",
  },
  miniBadge: {
    flexDirection: "row",
    alignItems: "center",
    gap: 4,
    backgroundColor: "rgba(255,255,255,0.1)",
    paddingHorizontal: 6,
    paddingVertical: 2,
    borderRadius: 8,
    marginTop: 4,
  },
  miniBadgeText: {
    color: "#FFD700", // Dourado/Amarelo para combinar com o Clock
    fontSize: 9,
    fontWeight: "700",
    textTransform: "uppercase",
  },
  statsRow: {
    flexDirection: "row",
    gap: 12,
    paddingHorizontal: 20,
    marginBottom: 15,
  },

  cardIcon: { position: "absolute", right: -10, bottom: -10 },

  // Pills
  pillsRow: {
    flexDirection: "row",
    gap: 8,
    paddingHorizontal: 8,
    marginBottom: 20,
  },
  pill: {
    flexDirection: "row",
    alignItems: "center",
    gap: 4,
    backgroundColor: "#fff",
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 20,
    borderWidth: 1,
    borderColor: "#eee",
  },
  pillText: { fontSize: 11, fontWeight: "600", color: "#666" },

  // Busca
  searchContainer: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: "#fff",
    marginHorizontal: 20,
    paddingHorizontal: 15,
    borderRadius: 12,
    marginBottom: 15,
    height: 50,
    borderWidth: 1,
    borderColor: "#efefef",
  },
  searchInput: { flex: 1, color: "#333" },
  searchIcon: { marginRight: 10 },

  listContent: { paddingHorizontal: 20, paddingBottom: 100 },
  fab: {
    position: "absolute",
    bottom: 40, // Distância da borda inferior
    left: 0, // Estica o container da esquerda...
    right: 0, // ...até a direita
    alignItems: "center", // Centraliza o botão dentro desse espaço
    justifyContent: "center",
    zIndex: 10, // Garante que fique acima de tudo
  },
  // Crie um estilo específico para o círculo do botão agora
  fabCircle: {
    backgroundColor: "#2b5373",
    width: 65,
    height: 65,
    borderRadius: 32.5,
    justifyContent: "center",
    alignItems: "center",
    elevation: 8, // Sombra no Android
    shadowColor: "#000", // Sombra no iOS
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 6,
  },
  footerContainer: { paddingVertical: 10, backgroundColor: "#f8f9fa" },
  footerText: {
    fontSize: 8,
    color: "#bbb",
    letterSpacing: 4,
    textAlign: "center",
  },
  footerYear: { fontSize: 8, color: "#ddd", textAlign: "center", marginTop: 2 },
  emptyContainer: {
    paddingVertical: 50,
    alignItems: "center",
    justifyContent: "center",
  },
  emptyText: {
    fontSize: 16,
    fontWeight: "600",
    color: "#999",
  },
  emptySubText: {
    fontSize: 14,
    color: "#bbb",
    marginTop: 5,
  },
  profileButton: {
    flexDirection: "row",
    alignItems: "center",
    gap: 6,
    backgroundColor: "#fff",
    paddingHorizontal: 12,
    paddingVertical: 8,
    borderRadius: 20, // Mais arredondado para parecer um chip de perfil
    elevation: 2,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.1,
    shadowRadius: 2,
    borderWidth: 1,
    borderColor: "#f0f0f0",
  },
  profileButtonText: { 
    color: colors.primary, 
    fontWeight: "700", 
    fontSize: 13 
  },
});

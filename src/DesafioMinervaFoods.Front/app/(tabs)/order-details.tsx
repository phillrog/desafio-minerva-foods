import React, { useState, useEffect } from "react";
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  ActivityIndicator,
} from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { colors } from "@/styles/commonStyles";
import { ConfirmModal } from "@/components/ConfirmModal";
import type { Order } from "@/types/order.types";
import { getStatusDisplay } from "@/types/order.types";
import { Toast } from "@/components/Toast";
import { useOrders } from "@/hooks/useOrders";
import { Stack, useRouter, useLocalSearchParams } from "expo-router";
import {
  Trash2,
  AlertCircle,
  ThumbsUp,
  DollarSign,
  Truck,
  User,
  ChevronLeft,
  Package, // <-- Novo ícone importado
} from "lucide-react-native";

export default function OrderDetailsScreen() {
  const [isApproving, setIsApproving] = useState(false);
  const router = useRouter();
  const { id: orderId } = useLocalSearchParams<{ id: string }>();
  const { orders, loading, approveOrder, deleteOrder } = useOrders();

  const [order, setOrder] = useState<Order | null>(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [toast, setToast] = useState({
    visible: false,
    message: "",
    type: "info" as "success" | "error" | "info",
  });

  const formatCurrency = (value: number) => {
    return value.toLocaleString("pt-BR", {
      style: "currency",
      currency: "BRL",
    });
  };

  useEffect(() => {
    const foundOrder = orders.find((o) => o.id === orderId);
    if (foundOrder) setOrder(foundOrder);
  }, [orders, orderId]);

  const handleApprove = async () => {
    if (!order || isApproving) return;
    setIsApproving(true);
    const result = await approveOrder(order.id);
    if (result.success) {
      
      setToast({
        visible: true,
        message: result.message || "Pedido aprovado com sucesso!",
        type: "success",
      });
      setTimeout(() => router.back(), 1500);
    } else {
      setToast({ visible: true, message: result.message, type: "error" });
      setIsApproving(false);
    }
  };

  const handleDelete = async () => {
    if (!order) return;
    setShowDeleteModal(false);
    const result = await deleteOrder(order.id);
    if (result.success) {
      setToast({
        visible: true,
        message: "Pedido excluído com sucesso!",
        type: "success",
      });
      setTimeout(() => router.back(), 1500);
    } else {
      setToast({ visible: true, message: result.message, type: "error" });
    }
  };

  if (loading || !order) {
    return (
      <SafeAreaView style={styles.container}>
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color={colors.primary} />
        </View>
      </SafeAreaView>
    );
  }

  const { text: statusText, color: statusColor } = getStatusDisplay(
    order.status
  );
  const deliveryDateFormatted = new Date(
    order.estimatedDeliveryDate
  ).toLocaleDateString("pt-BR");
  const canApprove = order.status === "Criado";

  return (
    <SafeAreaView style={styles.container} edges={["top"]}>
      <Stack.Screen
        options={{
          title: `Pedido #${order.id.substring(0, 6)}`,
          headerShown: true,
          headerStyle: { backgroundColor: colors.primary },
          headerTintColor: "#ffffff",
          headerLeft: () => (
            <TouchableOpacity
              onPress={() => router.back()}
              style={{ marginLeft: 5 }}
            >
              <ChevronLeft size={28} color="#ffffff" />
            </TouchableOpacity>
          ),
          headerRight: () => (
            <TouchableOpacity
              onPress={() => setShowDeleteModal(true)}
              style={{ marginRight: 10 }}
            >
              <Trash2 size={22} color="#ffffff" />
            </TouchableOpacity>
          ),
        }}
      />

      <ScrollView
        style={styles.content}
        contentContainerStyle={styles.scrollContent}
      >
        {/* CARD 1: STATUS E VALOR */}
        <View style={styles.statusCard}>
          <View style={styles.statusHeader}>
            <View style={[styles.badge, { backgroundColor: statusColor }]}>
              <Text style={styles.badgeText}>{statusText.toUpperCase()}</Text>
            </View>
            {order.requiresManualApproval && (
              <View style={styles.alertContainer}>
                <AlertCircle size={16} color="#FF9800" />
                <Text style={styles.alertText}>Requer Aprovação</Text>
              </View>
            )}
          </View>
          <Text style={styles.amountLabel}>VALOR TOTAL</Text>
          <Text style={styles.amount}>{formatCurrency(order.totalAmount)}</Text>
        </View>

        {/* CARD 2: INFORMAÇÕES GERAIS (AGRUPADO) */}
        <View style={styles.infoGroupCard}>
          <View style={styles.infoRow}>
            <View style={styles.iconCircle}>
              <User size={18} color={colors.primary} />
            </View>
            <View style={styles.infoTextGroup}>
              <Text style={styles.infoLabel}>Cliente</Text>
              <Text style={styles.infoValue}>{order.customerName}</Text>
            </View>
          </View>
          <View style={styles.divider} />
          <View style={styles.infoRow}>
            <View style={styles.iconCircle}>
              <DollarSign size={18} color={colors.primary} />
            </View>
            <View style={styles.infoTextGroup}>
              <Text style={styles.infoLabel}>Pagamento</Text>
              <Text style={styles.infoValue}>{order.paymentDescription}</Text>
              <Text style={styles.infoSubValue}>
                {order.numberOfInstallments}x parcelas
              </Text>
            </View>
          </View>
          <View style={styles.divider} />
          <View style={styles.infoRow}>
            <View style={styles.iconCircle}>
              <Truck size={18} color={colors.primary} />
            </View>
            <View style={styles.infoTextGroup}>
              <Text style={styles.infoLabel}>Previsão de Entrega</Text>
              <Text style={styles.infoValue}>{deliveryDateFormatted}</Text>
              <Text style={styles.infoSubValue}>
                {order.deliveryDays} dias úteis
              </Text>
            </View>
          </View>
        </View>

        {/* CARD 3: ITENS DO PEDIDO (COM ÍCONE) */}
        <View style={styles.itemsSectionCard}>
          <View style={styles.itemsHeaderRow}>
            <Package size={20} color={colors.textSecondary} />
            <Text style={styles.itemsTitle}>ITENS DO PEDIDO</Text>
          </View>

          {order.items.map((item, index) => (
            <View key={index} style={styles.itemRow}>
              <View style={styles.itemMainInfo}>
                <Text style={styles.itemNameText}>{item.productName}</Text>
                <Text style={styles.itemTotalText}>
                  {formatCurrency(item.totalPrice)}
                </Text>
              </View>
              <Text style={styles.itemSubDetailText}>
                {item.quantity}x • {formatCurrency(item.unitPrice)} cada
              </Text>
              {index < order.items.length - 1 && (
                <View style={styles.itemDivider} />
              )}
            </View>
          ))}
        </View>

        {/* BOTÃO DE APROVAÇÃO */}
        <TouchableOpacity
          style={[styles.approveButton, !canApprove && styles.disabledButton]}
          onPress={canApprove ? handleApprove : null}
          disabled={!canApprove || isApproving}
          activeOpacity={0.8}
        >
          {isApproving ? (
            <ActivityIndicator color="#ffffff" />
          ) : (
            <View style={styles.approveButtonContent}>
              <ThumbsUp
                size={22}
                color="#ffffff"
                fill={canApprove ? "#ffffff" : "transparent"}
              />
              <Text style={styles.approveButtonText}>
                {canApprove ? "APROVAR PEDIDO AGORA" : "PEDIDO JÁ PROCESSADO"}
              </Text>
            </View>
          )}
        </TouchableOpacity>

        <View style={{ height: 140 }} />
      </ScrollView>

      <ConfirmModal
        visible={showDeleteModal}
        title="Excluir Pedido"
        message="Deseja realmente excluir este pedido?"
        onConfirm={handleDelete}
        onCancel={() => setShowDeleteModal(false)}
      />

      <Toast
        visible={toast.visible}
        message={toast.message}
        type={toast.type}
        onHide={() => setToast({ ...toast, visible: false })}
      />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: colors.backgroundSecondary },
  loadingContainer: { flex: 1, justifyContent: "center", alignItems: "center" },
  content: { flex: 1 },
  scrollContent: { padding: 16 },

  statusCard: {
    backgroundColor: colors.card,
    borderRadius: 16,
    padding: 20,
    marginBottom: 16,
    alignItems: "center",
    elevation: 4,
    borderTopWidth: 4,
    borderTopColor: colors.primary,
  },
  statusHeader: {
    flexDirection: "row",
    justifyContent: "center",
    alignItems: "center",
    gap: 10,
    marginBottom: 12,
  },
  badge: { paddingHorizontal: 12, paddingVertical: 4, borderRadius: 20 },
  badgeText: { fontSize: 11, fontWeight: "900", color: "#ffffff" },
  amountLabel: {
    fontSize: 12,
    color: colors.textSecondary,
    fontWeight: "700",
    letterSpacing: 1,
  },
  amount: { fontSize: 34, fontWeight: "900", color: colors.text, marginTop: 4 },

  infoGroupCard: {
    backgroundColor: colors.card,
    borderRadius: 16,
    padding: 16,
    marginBottom: 16,
    elevation: 2,
  },
  infoRow: {
    flexDirection: "row",
    alignItems: "center",
    gap: 14,
    paddingVertical: 10,
  },
  iconCircle: {
    width: 36,
    height: 36,
    borderRadius: 18,
    backgroundColor: colors.backgroundSecondary,
    justifyContent: "center",
    alignItems: "center",
  },
  infoTextGroup: { flex: 1 },
  infoLabel: {
    fontSize: 11,
    color: colors.textSecondary,
    fontWeight: "bold",
    textTransform: "uppercase",
  },
  infoValue: { fontSize: 16, color: colors.text, fontWeight: "600" },
  infoSubValue: { fontSize: 13, color: colors.textSecondary, marginTop: 1 },
  divider: {
    height: 1,
    backgroundColor: colors.border,
    marginLeft: 50,
    marginVertical: 4,
  },

  // Estilos da seção de itens atualizados
  itemsSectionCard: {
    backgroundColor: colors.card,
    borderRadius: 16,
    padding: 16,
    marginBottom: 20,
    elevation: 2,
  },
  itemsHeaderRow: {
    flexDirection: "row",
    alignItems: "center",
    gap: 8,
    marginBottom: 16,
  },
  itemsTitle: {
    fontSize: 13,
    fontWeight: "800",
    color: colors.textSecondary,
    letterSpacing: 0.5,
  },
  itemRow: { marginBottom: 12 },
  itemMainInfo: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  itemNameText: {
    fontSize: 15,
    fontWeight: "600",
    color: colors.text,
    flex: 1,
  },
  itemTotalText: { fontSize: 15, fontWeight: "700", color: colors.primary },
  itemSubDetailText: {
    fontSize: 13,
    color: colors.textSecondary,
    marginTop: 2,
  },
  itemDivider: { height: 1, backgroundColor: "#f5f5f5", marginTop: 12 },

  approveButton: {
    backgroundColor: "#2e7d32",
    borderRadius: 14,
    padding: 18,
    elevation: 4,
  },
  disabledButton: { backgroundColor: "#b0bec5", elevation: 0 },
  approveButtonContent: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 12,
  },
  approveButtonText: { color: "#ffffff", fontSize: 15, fontWeight: "800" },
  alertContainer: { flexDirection: "row", alignItems: "center", gap: 4 },
  alertText: { fontSize: 12, color: "#FF9800", fontWeight: "700" },
});

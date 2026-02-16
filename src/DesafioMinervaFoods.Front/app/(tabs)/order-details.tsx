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
import React, { useState, useEffect } from "react";
import {
  ArrowLeft,
  CheckCircle,
  Trash2,
  AlertCircle,
  ThumbsUp,
} from "lucide-react-native";

export default function OrderDetailsScreen() {
  const [isApproving, setIsApproving] = useState(false);

  const router = useRouter();
  const { id: orderId } = useLocalSearchParams<{ id: string }>();
  const { orders, loading, approveOrder, deleteOrder } = useOrders();

  const [order, setOrder] = useState<Order | null>(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [toastVisible, setToastVisible] = useState(false);
  const [toastMessage, setToastMessage] = useState("");

  // formatar valores no padrão PT-BR
  const formatCurrency = (value: number) => {
    return value.toLocaleString("pt-BR", {
      style: "currency",
      currency: "BRL",
    });
  };
  
  useEffect(() => {
    console.log("OrderDetailsScreen: Finding order", orderId);
    const foundOrder = orders.find((o) => o.id === orderId);
    if (foundOrder) {
      setOrder(foundOrder);
      console.log("OrderDetailsScreen: Order found", foundOrder);
    }
  }, [orders, orderId]);

  const handleApprove = async () => {
    if (!order || isApproving) return;

    setIsApproving(true);
    const result = await approveOrder(order.id);

    if (result.success) {
      setToastMessage("Pedido aprovado com sucesso!");
      setToastVisible(true);
      setTimeout(() => router.back(), 1500);
    } else {
      setToastMessage(result.message);
      setToastVisible(true);
      setIsApproving(false); // Para o loading se der erro
    }
  };

  const handleDelete = async () => {
    if (!order) return;

    console.log("OrderDetailsScreen: User confirmed delete");
    setShowDeleteModal(false);

    const result = await deleteOrder(order.id);

    if (result.success) {
      setToastMessage("Pedido excluído com sucesso!");
      setToastVisible(true);
      setTimeout(() => router.back(), 1500);
    } else {
      setToastMessage(result.message);
      setToastVisible(true);
    }
  };

  if (loading || !order) {
    return (
      <SafeAreaView style={styles.container} edges={["top"]}>
        <Stack.Screen options={{ headerShown: false }} />
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color={colors.primary} />
          <Text style={styles.loadingText}>Carregando detalhes...</Text>
        </View>
      </SafeAreaView>
    );
  }

  const statusInfo = getStatusDisplay(order.status);
  const statusText = statusInfo.text;
  const statusColor = statusInfo.color;
  const deliveryDateFormatted = new Date(
    order.estimatedDeliveryDate
  ).toLocaleDateString("pt-BR");
  const totalAmountFormatted = `${formatCurrency(order.totalAmount)}`;
  const canApprove = order.status === "Criado";
  const showAlert = order.requiresManualApproval;

  return (
    <SafeAreaView style={styles.container} edges={["top"]}>
      <Stack.Screen options={{ headerShown: false }} />

      <View style={styles.header}>
        <TouchableOpacity
          onPress={() => {
            if (router.canGoBack()) {
              router.back();
            } else {
              router.replace("/orders");
            }
          }}
          style={styles.backButton}
        >
          <ArrowLeft size={24} color={colors.text} />
        </TouchableOpacity>
        <Text style={styles.title}>Detalhes do Pedido</Text>
        <TouchableOpacity
          onPress={() => setShowDeleteModal(true)}
          style={styles.deleteButton}
        >
          <Trash2 size={24} color={colors.error} />
        </TouchableOpacity>
      </View>

      <ScrollView
        style={styles.content}
        contentContainerStyle={styles.scrollContent}
      >
        <View style={styles.statusCard}>
          <View style={styles.statusHeader}>
            <View style={[styles.badge, { backgroundColor: statusColor }]}>
              <Text style={styles.badgeText}>{statusText}</Text>
            </View>
            {showAlert && (
              <View style={styles.alertContainer}>
                <AlertCircle size={18} color="#FF9800" />
                <Text style={styles.alertText}>Requer aprovação manual</Text>
              </View>
            )}
          </View>
          <Text style={styles.amount}>{totalAmountFormatted}</Text>
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Cliente</Text>
          <Text style={styles.sectionValue}>{order.customerName}</Text>
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Pagamento</Text>
          <Text style={styles.sectionValue}>{order.paymentDescription}</Text>
          <Text style={styles.sectionSubValue}>
            {order.numberOfInstallments}x parcelas
          </Text>
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Entrega</Text>
          <Text style={styles.sectionValue}>
            Previsão: {deliveryDateFormatted}
          </Text>
          <Text style={styles.sectionSubValue}>
            {order.deliveryDays} dias úteis
          </Text>
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Itens do Pedido</Text>
          {order.items.map((item, index) => {
            const itemTotal = `${formatCurrency(item.totalPrice)}`;
            const itemUnit = `${formatCurrency(item.unitPrice)}`;
            const itemQty = `${item.quantity}x`;

            return (
              <View key={index} style={styles.itemCard}>
                <View style={styles.itemHeader}>
                  <Text style={styles.itemName}>{item.productName}</Text>
                  <Text style={styles.itemTotal}>{itemTotal}</Text>
                </View>
                <View style={styles.itemDetails}>
                  <Text style={styles.itemDetail}>{itemQty}</Text>
                  <Text style={styles.itemDetail}>•</Text>
                  <Text style={styles.itemDetail}>{itemUnit}</Text>
                  <Text style={styles.itemDetail}>cada</Text>
                </View>
              </View>
            );
          })}
        </View>

        {/* BOTÃO DE APROVAÇÃO (JOINHA) */}
        <TouchableOpacity
          style={[styles.approveButton, !canApprove && styles.disabledButton]}
          onPress={canApprove ? handleApprove : null}
          activeOpacity={canApprove ? 0.7 : 1}
        >
          <ThumbsUp
            size={22}
            color={canApprove ? "#ffffff" : "rgba(255,255,255,0.5)"}
            fill={canApprove ? "#ffffff" : "transparent"} // Preenchimento se estiver ativo
          />
          <Text
            style={[
              styles.approveButtonText,
              !canApprove && { color: "rgba(255,255,255,0.5)" },
            ]}
          >
            {canApprove ? "Aprovar este Pedido" : "Aprovação não disponível"}
          </Text>
        </TouchableOpacity>
      </ScrollView>

      <ConfirmModal
        visible={showDeleteModal}
        title="Excluir Pedido"
        message="Tem certeza que deseja excluir este pedido? Esta ação não pode ser desfeita."
        confirmText="Excluir"
        cancelText="Cancelar"
        onConfirm={handleDelete}
        onCancel={() => setShowDeleteModal(false)}
      />

      {toastVisible && (
        <Toast
          message={toastMessage}
          visible={toastVisible}
          onHide={() => setToastVisible(false)}
        />
      )}

      <View style={{ height: 140 }} />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.background,
  },
  header: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    paddingHorizontal: 20,
    paddingVertical: 16,
    borderBottomWidth: 1,
    borderBottomColor: colors.border,
  },
  
  backButton: {
    padding: 8,
  },
  title: {
    fontSize: 18,
    fontWeight: "600",
    color: colors.text,
    flex: 1,
    textAlign: "center",
  },
  deleteButton: {
    padding: 8,
  },
  content: {
    flex: 1,
  },
  scrollContent: {
    padding: 20,
  },
  statusCard: {
    backgroundColor: colors.card,
    borderRadius: 12,
    padding: 20,
    marginBottom: 20,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.08,
    shadowRadius: 8,
    elevation: 3,
  },
  statusHeader: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 12,
    gap: 12,
  },
  badge: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 16,
  },
  badgeText: {
    fontSize: 12,
    fontWeight: "600",
    color: "#ffffff",
  },
  alertContainer: {
    flexDirection: "row",
    alignItems: "center",
    gap: 6,
  },
  alertText: {
    fontSize: 12,
    color: "#FF9800",
    fontWeight: "500",
  },
  amount: {
    fontSize: 32,
    fontWeight: "bold",
    color: colors.text,
  },
  section: {
    marginBottom: 24,
  },
  sectionTitle: {
    fontSize: 14,
    fontWeight: "600",
    color: colors.textSecondary,
    marginBottom: 8,
    textTransform: "uppercase",
    letterSpacing: 0.5,
  },
  sectionValue: {
    fontSize: 18,
    color: colors.text,
    fontWeight: "500",
  },
  sectionSubValue: {
    fontSize: 14,
    color: colors.textSecondary,
    marginTop: 4,
  },
  itemCard: {
    backgroundColor: colors.backgroundSecondary,
    borderRadius: 8,
    padding: 12,
    marginTop: 8,
  },
  itemHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 6,
  },
  itemName: {
    fontSize: 16,
    fontWeight: "600",
    color: colors.text,
    flex: 1,
  },
  itemTotal: {
    fontSize: 16,
    fontWeight: "bold",
    color: colors.text,
  },
  itemDetails: {
    flexDirection: "row",
    gap: 6,
  },
  itemDetail: {
    fontSize: 14,
    color: colors.textSecondary,
  },
  approveButton: {
    backgroundColor: colors.success, // #4CAF50 ou similar
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 12,
    padding: 18,
    borderRadius: 12,
    marginTop: 20,
    elevation: 4,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.2,
    shadowRadius: 4,
  },
  disabledButton: {
    backgroundColor: "#cccccc", // Cinza para indicar inativo
    elevation: 0,
    shadowOpacity: 0,
  },
  approveButtonText: {
    color: "#ffffff",
    fontSize: 16,
    fontWeight: "700",
  },
});

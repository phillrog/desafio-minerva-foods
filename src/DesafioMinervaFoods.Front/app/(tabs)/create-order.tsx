import React, { useState, useMemo, useEffect } from "react";
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TextInput,
  TouchableOpacity,
  ActivityIndicator,
} from "react-native";
import { Stack, useRouter } from "expo-router";
import { SafeAreaView } from "react-native-safe-area-context";
import { colors } from "@/styles/commonStyles";
import { useOrders } from "@/hooks/useOrders";
import { usePaymentMethods } from "@/hooks/usePaymentMethods";
import { useCustomers } from "@/hooks/useCustomers";
import { Toast } from "@/components/Toast";
import { IconSymbol } from "@/components/IconSymbol";
import type {
  CreateOrderItem,
  CustomerResponse,
  PaymentMethodsResponse,
} from "@/types/order.types";
import {
  DollarSign,
  ThumbsUp,
  Package,
  User,
  CreditCard,
} from "lucide-react-native";

interface OrderItemForm extends CreateOrderItem {
  tempId: string;
}

export default function CreateOrderScreen() {
  const router = useRouter();
  const { createOrder } = useOrders();
  const { paymentMethods, loading: loadingPayments } = usePaymentMethods();
  const { customers, loading: loadingCustomers } = useCustomers();

  const [selectedCustomer, setSelectedCustomer] =
    useState<CustomerResponse | null>(null);
  const [showCustomerPicker, setShowCustomerPicker] = useState(false);
  const [selectedPayment, setSelectedPayment] =
    useState<PaymentMethodsResponse | null>(null);
  const [numberOfInstallments, setNumberOfInstallments] = useState("");
  const [showPaymentPicker, setShowPaymentPicker] = useState(false);

  const [items, setItems] = useState<OrderItemForm[]>([
    { tempId: "1", productName: "", quantity: "1,00", unitPrice: "0,000" },
  ]);
  const [loading, setLoading] = useState(false);
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

  const resetForm = () => {
    setSelectedCustomer(null);
    setSelectedPayment(null);
    setNumberOfInstallments("");
    setShowCustomerPicker(false);
    setShowPaymentPicker(false);
    setItems([
      { tempId: "1", productName: "", quantity: "1,00", unitPrice: "0,000" },
    ]);
  };

  useEffect(() => {
    if (selectedPayment) {
      setNumberOfInstallments(selectedPayment.numberOfInstallments.toString());
    }
  }, [selectedPayment]);

  const totalAmount = useMemo(() => {
    return items.reduce((sum, item) => {
      const qty = parseFloat(item.quantity.toString().replace(",", ".")) || 0;
      const price =
        parseFloat(item.unitPrice.toString().replace(",", ".")) || 0;
      return sum + qty * price;
    }, 0);
  }, [items]);

  const installmentValue = useMemo(() => {
    const installments = parseInt(numberOfInstallments) || 1;
    return totalAmount / installments;
  }, [totalAmount, numberOfInstallments]);

  const addItem = () => {
    setItems([
      ...items,
      {
        tempId: Date.now().toString(),
        productName: "",
        quantity: "1,00",
        unitPrice: "0,000",
      },
    ]);
  };

  const removeItem = (tempId: string) => {
    setItems(items.filter((item) => item.tempId !== tempId));
  };

  const updateItem = (
    tempId: string,
    field: keyof CreateOrderItem,
    value: string
  ) => {
    setItems(
      items.map((item) => {
        if (item.tempId === tempId) {
          if (field === "productName")
            return { ...item, [field]: value.toUpperCase() };
          let normalized = value.replace(",", ".");
          if (normalized !== "" && !/^\d*\.?\d*$/.test(normalized)) return item;
          return { ...item, [field]: normalized };
        }
        return item;
      })
    );
  };

  const handleSubmit = async () => {
    if (!selectedCustomer || !selectedPayment) {
      setToast({
        visible: true,
        message: "Preencha cliente e pagamento",
        type: "error",
      });
      return;
    }
    if (
      items.some(
        (i) => !i.productName.trim() || parseFloat(i.quantity.toString()) <= 0
      )
    ) {
      setToast({
        visible: true,
        message: "Revise os itens do pedido",
        type: "error",
      });
      return;
    }

    setLoading(true);
    const orderData = {
      customerId: selectedCustomer.id,
      paymentConditionId: selectedPayment.id,
      numberOfInstallments: parseInt(numberOfInstallments) || 1,
      items: items.map(({ tempId, ...item }) => ({
        productName: item.productName,
        quantity: parseFloat(item.quantity.toString().replace(",", ".")) || 0,
        unitPrice: parseFloat(item.unitPrice.toString().replace(",", ".")) || 0,
      })),
    };

    const result = await createOrder(orderData);
    setLoading(false);
    if (result.success) {
      setToast({
        visible: true,
        message: result.message || "Pedido criado com sucesso!",
        type: "success",
      });

      resetForm();
      
      setTimeout(() => router.back(), 1500);
    } else {
      setToast({ visible: true, message: result.message, type: "error" });
    }
  };

  return (
    <SafeAreaView style={styles.container} edges={["top"]}>
      <Stack.Screen
        options={{
          title: "Novo Pedido",
          headerShown: true,
          headerStyle: { backgroundColor: colors.primary },
          headerTintColor: "#ffffff",
        }}
      />

      <ScrollView
        style={styles.content}
        contentContainerStyle={styles.scrollContent}
      >
        {/* CARD AGRUPADO: CLIENTE E PAGAMENTO */}
        <View style={styles.infoGroupCard}>
          <View style={styles.infoRow}>
            <View style={styles.iconCircle}>
              <User size={18} color={colors.primary} />
            </View>
            <View style={styles.infoTextGroup}>
              <Text style={styles.infoLabel}>Cliente</Text>
              <TouchableOpacity
                onPress={() => setShowCustomerPicker(!showCustomerPicker)}
              >
                <Text
                  style={
                    selectedCustomer
                      ? styles.pickerTextSelected
                      : styles.pickerTextPlaceholder
                  }
                >
                  {selectedCustomer
                    ? selectedCustomer.name
                    : "Toque para selecionar..."}
                </Text>
              </TouchableOpacity>
            </View>
          </View>

          {showCustomerPicker && (
            <View style={styles.pickerOptions}>
              {customers.map((c) => (
                <TouchableOpacity
                  key={c.id}
                  style={styles.pickerOption}
                  onPress={() => {
                    setSelectedCustomer(c);
                    setShowCustomerPicker(false);
                  }}
                >
                  <Text style={styles.pickerOptionText}>{c.name}</Text>
                </TouchableOpacity>
              ))}
            </View>
          )}

          <View style={styles.divider} />

          <View style={styles.infoRow}>
            <View style={styles.iconCircle}>
              <CreditCard size={18} color={colors.primary} />
            </View>
            <View style={styles.infoTextGroup}>
              <Text style={styles.infoLabel}>Forma de Pagamento</Text>
              <TouchableOpacity
                onPress={() => setShowPaymentPicker(!showPaymentPicker)}
              >
                <Text
                  style={
                    selectedPayment
                      ? styles.pickerTextSelected
                      : styles.pickerTextPlaceholder
                  }
                >
                  {selectedPayment
                    ? selectedPayment.description
                    : "Toque para selecionar..."}
                </Text>
              </TouchableOpacity>
            </View>
          </View>

          {showPaymentPicker && (
            <View style={styles.pickerOptions}>
              {paymentMethods.map((m) => (
                <TouchableOpacity
                  key={m.id}
                  style={styles.pickerOption}
                  onPress={() => {
                    setSelectedPayment(m);
                    setShowPaymentPicker(false);
                  }}
                >
                  <Text style={styles.pickerOptionText}>{m.description}</Text>
                  <Text style={styles.pickerOptionSubtext}>
                    {m.numberOfInstallments}x
                  </Text>
                </TouchableOpacity>
              ))}
            </View>
          )}
        </View>

        {/* SEÇÃO DE ITENS */}
        <View style={styles.sectionHeader}>
          <View style={{ flexDirection: "row", alignItems: "center", gap: 8 }}>
            <Package size={20} color={colors.textSecondary} />
            <Text style={styles.sectionTitle}>ITENS DO PEDIDO</Text>
          </View>
          <TouchableOpacity onPress={addItem}>
            <IconSymbol
              ios_icon_name="plus.circle.fill"
              android_material_icon_name="add-circle"
              size={28}
              color={colors.primary}
            />
          </TouchableOpacity>
        </View>

        {items.map((item, index) => (
          <View key={item.tempId} style={styles.itemCard}>
            <View style={styles.itemHeader}>
              <Text style={styles.itemNumber}>Produto {index + 1}</Text>
              {items.length > 1 && (
                <TouchableOpacity onPress={() => removeItem(item.tempId)}>
                  <IconSymbol
                    ios_icon_name="trash"
                    android_material_icon_name="delete"
                    size={20}
                    color={colors.error}
                  />
                </TouchableOpacity>
              )}
            </View>
            <TextInput
              style={styles.input}
              placeholder="Nome do produto"
              autoCapitalize="characters"
              value={item.productName}
              onChangeText={(v) => updateItem(item.tempId, "productName", v)}
            />
            <View style={styles.row}>
              <View style={styles.halfInput}>
                <Text style={styles.label}>Qtd</Text>
                <TextInput
                  style={styles.input}
                  keyboardType="numeric"
                  value={item.quantity.toString().replace(".", ",")}
                  onChangeText={(v) => updateItem(item.tempId, "quantity", v)}
                />
              </View>
              <View style={styles.halfInput}>
                <Text style={styles.label}>Preço Unit.</Text>
                <TextInput
                  style={styles.input}
                  keyboardType="decimal-pad"
                  value={item.unitPrice.toString().replace(".", ",")}
                  onChangeText={(v) => updateItem(item.tempId, "unitPrice", v)}
                />
              </View>
            </View>
          </View>
        ))}

        {/* RESUMO DE VALORES (CARD FINAL) */}
        <View style={styles.summaryCard}>
          <View style={styles.totalInfo}>
            <View>
              <Text style={styles.summaryLabel}>TOTAL DO PEDIDO</Text>
              <Text style={styles.summaryAmount}>
                {formatCurrency(totalAmount)}
              </Text>
              {Number(numberOfInstallments) > 1 && (
                <Text style={styles.summarySubtext}>
                  {numberOfInstallments}x de {formatCurrency(installmentValue)}
                </Text>
              )}
            </View>
            <DollarSign size={32} color={colors.primary} opacity={0.3} />
          </View>
        </View>

        {/* BOTÃO FINALIZAR (FORA DO CARD DE VALORES) */}
        <TouchableOpacity
          style={[styles.submitButton, loading && styles.submitButtonDisabled]}
          onPress={handleSubmit}
          disabled={loading}
          activeOpacity={0.8}
        >
          {loading ? (
            <ActivityIndicator color="#ffffff" />
          ) : (
            <View style={styles.submitButtonContent}>
              <ThumbsUp size={22} color="#ffffff" fill="#ffffff" />
              <Text style={styles.submitButtonText}>
                FINALIZAR E CRIAR PEDIDO
              </Text>
            </View>
          )}
        </TouchableOpacity>

        <View style={{ height: 100 }} />
      </ScrollView>

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
  content: { flex: 1 },
  scrollContent: { padding: 16 },

  // Card Agrupado (Cliente/Pgto)
  infoGroupCard: {
    backgroundColor: colors.card,
    borderRadius: 16,
    padding: 16,
    marginBottom: 20,
    elevation: 2,
  },
  infoRow: {
    flexDirection: "row",
    alignItems: "center",
    gap: 14,
    paddingVertical: 8,
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
  divider: {
    height: 1,
    backgroundColor: colors.border,
    marginLeft: 50,
    marginVertical: 8,
  },

  // Itens
  sectionHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 12,
    paddingHorizontal: 4,
  },
  sectionTitle: {
    fontSize: 13,
    fontWeight: "800",
    color: colors.textSecondary,
    letterSpacing: 0.5,
  },
  itemCard: {
    backgroundColor: colors.card,
    borderRadius: 16,
    padding: 16,
    marginBottom: 12,
    elevation: 1,
  },
  itemHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 12,
  },
  itemNumber: { fontSize: 14, fontWeight: "bold", color: colors.primary },

  input: {
    backgroundColor: colors.backgroundSecondary,
    borderRadius: 10,
    padding: 12,
    fontSize: 16,
    color: colors.text,
    borderWidth: 1,
    borderColor: colors.border,
    marginBottom: 10,
  },
  label: {
    fontSize: 12,
    color: colors.textSecondary,
    marginBottom: 4,
    fontWeight: "600",
  },
  row: { flexDirection: "row", gap: 12 },
  halfInput: { flex: 1 },

  // Picker Styles
  pickerTextPlaceholder: { fontSize: 16, color: colors.textLight },
  pickerTextSelected: { fontSize: 16, color: colors.text, fontWeight: "600" },
  pickerOptions: {
    backgroundColor: colors.backgroundSecondary,
    borderRadius: 8,
    marginTop: 8,
    overflow: "hidden",
    borderWidth: 1,
    borderColor: colors.border,
  },
  pickerOption: {
    padding: 15,
    borderBottomWidth: 1,
    borderBottomColor: colors.border,
    flexDirection: "row",
    justifyContent: "space-between",
  },
  pickerOptionText: { fontSize: 15 },
  pickerOptionSubtext: {
    fontSize: 12,
    color: colors.primary,
    fontWeight: "bold",
  },

  // Card de Resumo (Sem o botão)
  summaryCard: {
    backgroundColor: colors.card,
    borderRadius: 16,
    padding: 20,
    marginBottom: 16,
    elevation: 3,
    borderLeftWidth: 5,
    borderLeftColor: colors.primary,
  },
  totalInfo: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  summaryLabel: {
    fontSize: 12,
    fontWeight: "bold",
    color: colors.textSecondary,
    letterSpacing: 1,
  },
  summaryAmount: { fontSize: 30, fontWeight: "900", color: colors.text },
  summarySubtext: { fontSize: 14, color: colors.textSecondary, marginTop: 2 },

  // Botão Estilo "Detalhes"
  submitButton: {
    backgroundColor: "#2e7d32",
    borderRadius: 14,
    padding: 18,
    elevation: 4,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.2,
    shadowRadius: 4,
  },
  submitButtonDisabled: { opacity: 0.6, backgroundColor: "#ccc" },
  submitButtonContent: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 12,
  },
  submitButtonText: {
    color: "#ffffff",
    fontSize: 15,
    fontWeight: "800",
    letterSpacing: 0.5,
  },
});

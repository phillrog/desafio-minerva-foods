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
import { DollarSign } from "lucide-react-native";

interface OrderItemForm extends CreateOrderItem {
  tempId: string;
}

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

  const [items, setItems] = useState<OrderItemForm[]>([
    { tempId: "1", productName: "", quantity: "1,00", unitPrice: "0,000" },
  ]);
  const [loading, setLoading] = useState(false);
  const [toast, setToast] = useState({
    visible: false,
    message: "",
    type: "info" as "success" | "error" | "info",
  });
  const [showPaymentPicker, setShowPaymentPicker] = useState(false);

  // formatar valores no padrão PT-BR
  const formatCurrency = (value: number) => {
    return value.toLocaleString("pt-BR", {
      style: "currency",
      currency: "BRL",
    });
  };

  useEffect(() => {
    if (selectedPayment) {
      setNumberOfInstallments(selectedPayment.numberOfInstallments.toString());
    }
  }, [selectedPayment]);

  const totalAmount = useMemo(() => {
    return items.reduce((sum, item) => {
      const qty = parseFloat(item.quantity) || 0;
      const price = parseFloat(item.unitPrice) || 0;
      return sum + (qty * price);
    }, 0);
  }, [items]);

  const installmentValue = useMemo(() => {
    const installments = parseInt(numberOfInstallments) || 1;
    return totalAmount / installments;
  }, [totalAmount, numberOfInstallments]);

  const addItem = () => {
    const newItem: OrderItemForm = {
      tempId: Date.now().toString(),
      productName: "",
      quantity: 1,
      unitPrice: 0,
    };
    setItems([...items, newItem]);
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
          // Se for o nome, apenas salva o texto
          if (field === "productName") {
            return { ...item, [field]: value.toUpperCase() };
          }
    
          // Para QTD e Valor:
          // 1. Troca a vírgula por ponto para o JavaScript entender
          let normalized = value.replace(",", ".");
    
          // 2. Permite apenas números e um único ponto (para não quebrar o decimal)
          if (normalized !== "" && !/^\d*\.?\d*$/.test(normalized)) {
            return item;
          }
    
          return { ...item, [field]: normalized };
        }
        return item;
      })
    );
  };

  const handleSubmit = async () => {
    if (!selectedCustomer) {
      setToast({
        visible: true,
        message: "Selecione um cliente",
        type: "error",
      });
      return;
    }

    if (!selectedPayment) {
      setToast({
        visible: true,
        message: "Selecione uma forma de pagamento",
        type: "error",
      });
      return;
    }

    if (items.length === 0 || items.some((item) => !item.productName.trim())) {
      setToast({
        visible: true,
        message: "Adicione pelo menos um item válido",
        type: "error",
      });
      return;
    }

    const hasInvalidItem = items.some((item) => {
      // Converte para número antes de validar
      const qty = parseFloat(item.quantity.toString().replace(',', '.')) || 0;
      const price = parseFloat(item.unitPrice.toString().replace(',', '.')) || 0;
      
      return price <= 0 || qty <= 0;
    });
    
    if (hasInvalidItem) {
      setToast({
        visible: true,
        message: "Quantidade e valor unitário devem ser maiores que zero",
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
        quantity: parseFloat(item.quantity.toString().replace(',', '.')) || 0,
        unitPrice: parseFloat(item.unitPrice.toString().replace(',', '.')) || 0,
      })),
    };

    const result = await createOrder(orderData);

    if (result.success) {
      setToast({ visible: true, message: result.message, type: "success" });
      setTimeout(() => router.back(), 1500);
    } else {
      setToast({ visible: true, message: result.message, type: "error" });
    }

    setLoading(false);
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
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Cliente</Text>
          <TouchableOpacity
            style={styles.picker}
            onPress={() => setShowCustomerPicker(!showCustomerPicker)}
            activeOpacity={0.7}
          >
            {loadingCustomers ? (
              <ActivityIndicator size="small" color={colors.primary} />
            ) : (
              <Text
                style={
                  selectedCustomer
                    ? styles.pickerTextSelected
                    : styles.pickerTextPlaceholder
                }
              >
                {selectedCustomer
                  ? selectedCustomer.name
                  : "Selecione o cliente"}
              </Text>
            )}
            <IconSymbol
              ios_icon_name="chevron.down"
              android_material_icon_name="arrow-drop-down"
              size={24}
              color={colors.textSecondary}
            />
          </TouchableOpacity>

          {showCustomerPicker && (
            <View style={styles.pickerOptions}>
              {customers.map((customer) => (
                <TouchableOpacity
                  key={customer.id}
                  style={styles.pickerOption}
                  onPress={() => {
                    setSelectedCustomer(customer);
                    setShowCustomerPicker(false);
                  }}
                >
                  <Text style={styles.pickerOptionText}>{customer.name}</Text>
                </TouchableOpacity>
              ))}
            </View>
          )}
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Forma de Pagamento</Text>

          <TouchableOpacity
            style={styles.picker}
            onPress={() => setShowPaymentPicker(!showPaymentPicker)}
            activeOpacity={0.7}
          >
            {loadingPayments ? (
              <ActivityIndicator size="small" color={colors.primary} />
            ) : (
              <Text
                style={
                  selectedPayment
                    ? styles.pickerTextSelected
                    : styles.pickerTextPlaceholder
                }
              >
                {selectedPayment
                  ? selectedPayment.description
                  : "Selecione a forma de pagamento"}
              </Text>
            )}
            <IconSymbol
              ios_icon_name="chevron.down"
              android_material_icon_name="arrow-drop-down"
              size={24}
              color={colors.textSecondary}
            />
          </TouchableOpacity>

          {showPaymentPicker && (
            <View style={styles.pickerOptions}>
              {paymentMethods.length > 0 ? (
                paymentMethods.map((method) => (
                  <TouchableOpacity
                    key={method.id}
                    style={styles.pickerOption}
                    onPress={() => {
                      setSelectedPayment(method);
                      setShowPaymentPicker(false);
                    }}
                    activeOpacity={0.7}
                  >
                    <Text style={styles.pickerOptionText}>
                      {method.description}
                    </Text>
                    <Text style={styles.pickerOptionSubtext}>
                      Máx: {method.numberOfInstallments.toString()}x
                    </Text>
                  </TouchableOpacity>
                ))
              ) : (
                <View style={styles.pickerOption}>
                  <Text style={styles.pickerTextPlaceholder}>
                    Nenhuma forma disponível
                  </Text>
                </View>
              )}
            </View>
          )}

          <View style={styles.row}>
            <View style={styles.halfInput}>
              <Text style={styles.label}>Parcelas</Text>
              <TextInput
                style={[
                  styles.input,
                  !selectedPayment && {
                    opacity: 0.5,
                    backgroundColor: "#f0f0f0",
                  },
                  selectedPayment && { backgroundColor: "#e9ecef" },
                ]}
                placeholder="Aguardando pagamento..."
                placeholderTextColor={colors.textLight}
                value={numberOfInstallments}
                editable={false}
                keyboardType="number-pad"
              />
            </View>
          </View>
        </View>

        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Text style={styles.sectionTitle}>Itens do Pedido</Text>
            <TouchableOpacity onPress={addItem} activeOpacity={0.7}>
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
                <Text style={styles.itemNumber}>Item {index + 1}</Text>
                {items.length > 1 && (
                  <TouchableOpacity
                    onPress={() => removeItem(item.tempId)}
                    activeOpacity={0.7}
                  >
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
                placeholderTextColor={colors.textLight}
                autoCapitalize="characters"
                value={item.productName}
                onChangeText={(value) =>
                  updateItem(item.tempId, "productName", value)
                }
              />

              <View style={styles.row}>
                {/* Campo Quantidade */}
                <View style={styles.halfInput}>
                  <Text style={styles.label}>Qtd</Text>
                  <TextInput
                    style={styles.input}
                    keyboardType="numeric" // Força o teclado de números
                    value={item.quantity.toString().replace(".", ",")} // Mostra vírgula para o usuário
                    onChangeText={(text) =>
                      updateItem(item.tempId, "quantity", text)
                    }
                  />
                </View>

                {/* Campo Preço Unitário */}
                <View style={styles.halfInput}>
                  <Text style={styles.label}>Preço Unit.</Text>
                  <TextInput
                    style={styles.input}
                    keyboardType="decimal-pad" // Teclado com ponto/vírgula
                    value={item.unitPrice.toString().replace(".", ",")} // Mostra vírgula para o usuário
                    onChangeText={(text) =>
                      updateItem(item.tempId, "unitPrice", text)
                    }
                  />
                </View>
              </View>
            </View>
          ))}
        </View>

        

        <View style={styles.footerCard}>
          <View style={styles.totalInfo}>
            <View>
              <Text style={styles.footerTotalLabel}>TOTAL GERAL</Text>
              <Text style={styles.footerTotalAmount}>{formatCurrency(totalAmount)}</Text>
              {Number(numberOfInstallments) > 1 && (
                <Text style={styles.footerInstallmentText}>
                  {numberOfInstallments}x de {formatCurrency(installmentValue)}
                </Text>
              )}
            </View>
            <DollarSign size={32} color={colors.primary} opacity={0.5} />
          </View>

          <TouchableOpacity
            style={[styles.submitButton, loading && styles.submitButtonDisabled]}
            onPress={handleSubmit}
            disabled={loading}
          >
            {loading ? (
              <ActivityIndicator color="#ffffff" />
            ) : (
              <View style={styles.submitButtonContent}>
                <IconSymbol ios_icon_name="checkmark.circle.fill" android_material_icon_name="check-circle" size={22} color="#ffffff" />
                <Text style={styles.submitButtonText}>FINALIZAR PEDIDO</Text>
              </View>
            )}
          </TouchableOpacity>
        </View>
      </ScrollView>

      <Toast
        visible={toast.visible}
        message={toast.message}
        type={toast.type}
        onHide={() => setToast({ ...toast, visible: false })}
      />

    <View style={{ height: 140 }} />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: colors.backgroundSecondary },
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
  content: { flex: 1 },
  scrollContent: { padding: 16 },
  section: {
    backgroundColor: colors.card,
    borderRadius: 12,
    padding: 16,
    marginBottom: 16,
    elevation: 3,
  },
  sectionHeader: { flexDirection: "row", justifyContent: "space-between", alignItems: "center", marginBottom: 12 },
  sectionTitle: { fontSize: 16, fontWeight: "bold", color: colors.text },
  input: {
    backgroundColor: colors.backgroundSecondary,
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
    color: colors.text,
    borderWidth: 1,
    borderColor: colors.border,
    marginBottom: 12,
  },
  picker: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    backgroundColor: colors.backgroundSecondary,
    borderRadius: 8,
    padding: 12,
    borderWidth: 1,
    borderColor: colors.border,
    marginBottom: 12,
  },
  pickerTextPlaceholder: { fontSize: 16, color: colors.textLight },
  pickerTextSelected: { fontSize: 16, color: colors.text },
  pickerOptions: {
    backgroundColor: colors.backgroundSecondary,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: colors.border,
    marginBottom: 12,
    overflow: "hidden",
  },
  pickerOption: {
    padding: 12,
    borderBottomWidth: 1,
    borderBottomColor: colors.border,
    flexDirection: "row",
    justifyContent: "space-between",
  },
  pickerOptionText: { fontSize: 16, color: colors.text },
  pickerOptionSubtext: { fontSize: 12, color: colors.textSecondary },
  row: { flexDirection: "row", gap: 12 },
  halfInput: { flex: 1 },
  label: { fontSize: 14, color: colors.textSecondary, marginBottom: 4 },
  itemCard: {
    backgroundColor: colors.backgroundSecondary,
    borderRadius: 8,
    padding: 12,
    marginBottom: 12,
    borderWidth: 1,
    borderColor: colors.border,
  },
  itemHeader: { flexDirection: "row", justifyContent: "space-between", alignItems: "center", marginBottom: 12 },
  itemNumber: { fontSize: 14, fontWeight: "600", color: colors.text },
  footerCard: {
    backgroundColor: colors.card,
    borderRadius: 16,
    padding: 20,
    elevation: 4,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: -2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    borderWidth: 1,
    borderColor: colors.border,
  },
  totalInfo: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 },
  footerTotalLabel: { fontSize: 12, fontWeight: 'bold', color: colors.textSecondary, letterSpacing: 1 },
  footerTotalAmount: { fontSize: 28, fontWeight: '900', color: colors.primary },
  footerInstallmentText: { fontSize: 14, color: colors.textSecondary, marginTop: 2 },
  submitButton: { backgroundColor: '#2e7d32', borderRadius: 12, padding: 18, elevation: 2 },
  submitButtonDisabled: { opacity: 0.6 },
  submitButtonContent: { flexDirection: 'row', alignItems: 'center', justifyContent: 'center' },
  submitButtonText: { color: '#ffffff', fontSize: 16, fontWeight: 'bold', marginLeft: 10 },
});

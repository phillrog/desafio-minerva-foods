import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity } from 'react-native';
import { ThumbsUp, ChevronRight } from 'lucide-react-native';
import { colors } from '@/styles/commonStyles';
import { Order, getStatusDisplay } from '@/types/order.types';

interface OrderCardProps {
  order: Order;
  onPress: () => void;
  onApprove: (id: string) => void;
}

export function OrderCard({ order, onPress, onApprove }: OrderCardProps) {
  const statusInfo = getStatusDisplay(order.status);
  
  // Normaliza a string para evitar erros de espaço ou maiúsculas
  const currentStatus = order.status?.toString().trim() || "";
  const canApprove = currentStatus === "Criado";

  // Debug para você ver no terminal porque o botão não aparece
  // console.log(`Pedido ${order.id} | Status Real: "${currentStatus}" | Aparece botão? ${canApprove}`);

  const handleQuickApprove = (e: any) => {
    e.stopPropagation(); 
    console.log("BOTÃO CLICADO NO CARD - ID:", order.id); // <--- ADICIONE ISSO
    onApprove(order.id);
  };

  const formatCurrency = (value) => {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(value);
  };


  return (
    <TouchableOpacity style={styles.container} onPress={onPress} activeOpacity={0.7}>
      <View style={styles.content}>
        <View style={styles.header}>
          <Text style={styles.orderId}>#{String(order.id).slice(-6).toUpperCase()}</Text>
          <View style={[styles.statusBadge, { backgroundColor: statusInfo.color }]}>
            <Text style={styles.statusText}>{statusInfo.text}</Text>
          </View>
        </View>

        <Text style={styles.customerName} numberOfLines={1}>
          {order.customerName}
        </Text>

        <View style={styles.footer}>
          <View>
            <Text style={styles.label}>Valor Total</Text>
            <Text style={styles.amount}>{formatCurrency(Number(order.totalAmount))}</Text>
          </View>

          {/* BOTÃO DE APROVAÇÃO RÁPIDA */}
          {canApprove ? (
            <TouchableOpacity 
              style={styles.quickApproveButton} 
              onPress={handleQuickApprove}
            >
              <ThumbsUp size={16} color="#ffffff" fill="#ffffff" />
              <Text style={styles.quickApproveText}>Aprovar</Text>
            </TouchableOpacity>
          ) : (
            <View style={styles.sideInfo}>
               <ChevronRight size={20} color={colors.textSecondary} />
            </View>
          )}
        </View>
      </View>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  container: {
    backgroundColor: '#ffffff',
    borderRadius: 12,
    marginBottom: 12,
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.1,
    shadowRadius: 3,
    borderLeftWidth: 4,
    borderLeftColor: colors.primary,
  },
  content: {
    padding: 16,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  orderId: {
    fontSize: 12,
    fontWeight: '700',
    color: colors.textSecondary,
  },
  statusBadge: {
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 6,
  },
  statusText: {
    color: '#ffffff',
    fontSize: 10,
    fontWeight: 'bold',
  },
  customerName: {
    fontSize: 16,
    fontWeight: '600',
    color: colors.text,
    marginBottom: 12,
  },
  footer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-end',
  },
  label: {
    fontSize: 10,
    color: colors.textSecondary,
    textTransform: 'uppercase',
  },
  amount: {
    fontSize: 18,
    fontWeight: '700',
    color: colors.text,
  },
  quickApproveButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#2e7d32', // Verde forte
    paddingHorizontal: 14,
    paddingVertical: 10,
    borderRadius: 10,
    gap: 8,
    // Garante que o botão seja clicável em uma área boa
    minWidth: 100,
    justifyContent: 'center',
  },
  quickApproveText: {
    color: '#ffffff',
    fontSize: 13,
    fontWeight: '800',
  },
  sideInfo: {
    height: 40,
    justifyContent: 'center',
  }
});

// Order Types following Clean Architecture

export enum OrderStatus {
  Processing = 0,
  Created = 1,
  Paid = 2,
  Cancelled = 3,
  Error = 9,
}

export interface OrderItem {
  id: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface Order {
  id: string;
  totalAmount: number;
  status: OrderStatus;
  requiresManualApproval: boolean;
  estimatedDeliveryDate: string;
  deliveryDays: number;
  items: OrderItem[];
  customerName: string;
  paymentDescription: string;
  numberOfInstallments: number;
  createdAt?: string;
}

export interface CreateOrderItem {
  productName: string;
  quantity: any;
  unitPrice: any;
}

export interface CreateOrderRequest {
  customerId: string;
  paymentConditionId: string;
  items: CreateOrderItem[];
}

export interface ResultSuccess<T> {
  data: T;
  isSuccess: true;
  errors: [];
}

export interface ResultError {
  isSuccess: false;
  errors: string[];
}

export type Result<T> = ResultSuccess<T> | ResultError;


export interface OrdersResponse {
  orders: Order[];
}

export interface OrderResponse {
  message: string;
  order: Order;
}

export interface PaymentMethodsResponse {
  id: string;
  description: string;
  numberOfInstallments: Number;
}

export interface CustomerResponse {
  id: string;
  name: string;
}

type StatusInfo = { text: string; color: string };


const STATUS_MAP: Record<string, StatusInfo> = {
  "Processando": { text: "Processando", color: "#FF9800" },
  "Criado":      { text: "Criado",      color: "#2196F3" },
  "Pago":        { text: "Pago",        color: "#4CAF50" },
  "Cancelado":   { text: "Cancelado",   color: "#F44336" },
  "Erro":        { text: "Erro",        color: "#9E9E9E" },
};


export function getStatusDisplay(status: string): StatusInfo {
  return STATUS_MAP[status] || { text: "Desconhecido", color: "#757575" };
}

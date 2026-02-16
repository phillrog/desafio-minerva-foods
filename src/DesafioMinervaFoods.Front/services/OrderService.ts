
// Service Layer - Separated from UI Components (Clean Architecture)
import { authenticatedGet, authenticatedPost, authenticatedPut, authenticatedDelete, BACKEND_URL, getBearerToken } from '@/utils/api';
import type { 
  Result, 
  OrdersResponse, 
  OrderResponse, 
  PaymentMethodsResponse,
  CreateOrderRequest, 
  CustomerResponse
} from '@/types/order.types';
import * as signalR from "@microsoft/signalr";

export class OrderService {
  private static readonly BASE_URL = '/api';
  private static connection: signalR.HubConnection | null = null;
  private static reconnectTimeout: NodeJS.Timeout | null = null;
  private static listeners: Set<(notification: any) => void> = new Set();

  // GET /api/Orders
  static async getOrders(filters?: { id?: string; status?: string }): Promise<Result<any>> {
    try {
      console.log('OrderService: Fetching orders with filters', filters);
      const params = new URLSearchParams();
      if (filters?.id) params.append('id', filters.id);
      if (filters?.status) params.append('status', filters.status);
      
      const queryString = params.toString();
      const url = queryString ? `${this.BASE_URL}/Orders?${queryString}` : `${this.BASE_URL}/Orders`;
      
      const response = await authenticatedGet<Result<any>>(url);
      console.log('OrderService: Orders fetched successfully', response);
      return response;
    } catch (error) {
      console.error('OrderService: Error fetching orders', error);
      return {
        isSuccess: false,
        errors: ['Erro ao buscar pedidos']
      };
    }
  }

  // GET /api/Orders/{id}
  static async getOrderById(orderId: string): Promise<Result<OrderResponse>> {
    try {
      console.log('OrderService: Fetching order by ID', orderId);
      const response = await authenticatedGet<Result<OrderResponse>>(`${this.BASE_URL}/Orders/${orderId}`);
      console.log('OrderService: Order fetched successfully', response);
      return response;
    } catch (error) {
      console.error('OrderService: Error fetching order', error);
      return {
        isSuccess: false,
        errors: ['Erro ao buscar pedido']
      };
    }
  }

  // POST /api/Orders
  static async createOrder(orderData: CreateOrderRequest): Promise<Result<OrderResponse>> {
    try {
      console.log('OrderService: Creating order', orderData);
      const response = await authenticatedPost<Result<OrderResponse>>(`${this.BASE_URL}/Orders`, orderData);
      console.log('OrderService: Order created successfully', response);
      return response;
    } catch (error) {
      console.error('OrderService: Error creating order', error);
      return {
        isSuccess: false,
        errors: ['Erro ao criar pedido']
      };
    }
  }

  // PUT /api/Orders/{id}/approve
  static async approveOrder(orderId: string): Promise<Result<OrderResponse>> {
    try {
      console.log('OrderService: Approving order', orderId);
      const response = await authenticatedPut<Result<OrderResponse>>(`${this.BASE_URL}/Orders/${orderId}/approve`, {});
      console.log('OrderService: Order approved successfully', response);
      return response;
    } catch (error) {
      console.error('OrderService: Error approving order', error);
      return {
        isSuccess: false,
        errors: ['Erro ao aprovar pedido']
      };
    }
  }

  // DELETE /api/Orders/{id}
  static async deleteOrder(orderId: string): Promise<Result<{ message: string }>> {
    try {
      console.log('OrderService: Deleting order', orderId);
      const response = await authenticatedDelete<Result<{ message: string }>>(`${this.BASE_URL}/Orders/${orderId}`);
      console.log('OrderService: Order deleted successfully', response);
      return response;
    } catch (error) {
      console.error('OrderService: Error deleting order', error);
      return {
        isSuccess: false,
        errors: ['Erro ao excluir pedido']
      };
    }
  }

  // WebSocket Notifications (SignalR Hub)
  static async connectWebSocket() {
    console.log('1. [SignalR] Iniciando connectWebSocket');
  
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      console.log('2. [SignalR] Já está conectado, abortando.');
      return;
    }
  
    try {
      const token = await getBearerToken();
      console.log('3. [SignalR] Token obtido:', token ? "Sim" : "Não (Problema aqui!)");
      
      if (!token) return;
  
      console.log('4. [SignalR] Construindo HubConnectionBuilder...');
      
      // Tente simplificar a construção para testar
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(`${BACKEND_URL}/orderHub`, {
          accessTokenFactory: () => token,
          transport: signalR.HttpTransportType.WebSockets,
          skipNegotiation: true
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();
  
      console.log('5. [SignalR] Objeto connection criado:', !!this.connection);
  
      // Dentro do connectWebSocket, antes do start:
      this.connection.onclose((err) => console.log("A CONEXÃO CAIU!", err));

      // Ouvinte para qualquer coisa que venha do servidor
      this.connection.on("OrderNotification", (data) => {
          console.log("!!! FINALMENTE CHEGOU ALGO !!!", data);
      });
      
      const start = async () => {
        console.log('6. [SignalR] Tentando executar .start()...');
        try {
          if (this.connection) {
            await this.connection.start();
            console.log('7. [SignalR] SUCESSO: Conectado!');

            this.connection.on("OrderNotification", (notification) => {
              
              console.log('--- MENSAGEM RECEBIDA DO SERVER ---', notification);
              
              if (this.listeners.size === 0) {
                console.warn('Alerta: Chegou notificação, mas não há ouvintes (listeners) registrados!');
              }
            
              this.listeners.forEach(listener => {
                console.log('Repassando para um listener...');
                listener(notification);
              });
            });
          } else {
            console.log('7. [SignalR] FALHA: this.connection ficou nulo estranhamente');
          }
        } catch (err) {
          console.log('7. [SignalR] ERRO no start, tentando em 5s...', err);
          setTimeout(start, 5000);
        }
      };
  
      await start();
  
    } catch (error) {
      console.error('X. [SignalR] Erro fatal na função connect:', error);
    }
  }

  static async disconnectWebSocket() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      console.log('OrderService: SignalR desconectado');
    }
  }

  static addNotificationListener(listener: (notification: any) => void) {
    this.listeners.add(listener);
    return () => this.listeners.delete(listener);
  }


  
  // GET /api/PaymentCondition
  static async getPaymentCondition(): Promise<Result<PaymentMethodsResponse[]>> {
    try {
      console.log('OrderService: Fetching payment methods');
      const response = await authenticatedGet<Result<PaymentMethodsResponse[]>>(`${this.BASE_URL}/PaymentCondition`);
      console.log('OrderService: Payment methods fetched successfully', response);
      return response;
    } catch (error) {
      console.error('OrderService: Error fetching payment methods', error);
      return {
        isSuccess: false,
        errors: ['Erro ao buscar formas de pagamento']
      };
    }
  }

  
  // GET /api/Customer
  static async getCustomers(): Promise<Result<CustomerResponse[]>> {
    try {
      console.log('OrderService: Fetching payment methods');
      const response = await authenticatedGet<Result<CustomerResponse[]>>(`${this.BASE_URL}/Customer`);
      console.log('OrderService: Customer fetched successfully', response);
      return response;
    } catch (error) {
      console.error('OrderService: Error fetching customers', error);
      return {
        isSuccess: false,
        errors: ['Erro ao clientes']
      };
    }
  }
}

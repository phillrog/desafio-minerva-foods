
// Custom Hook for Order Management (Clean Architecture)
import { useState, useEffect, useCallback } from 'react';
import { OrderService } from '@/services/OrderService';
import type { Order, CreateOrderRequest } from '@/types/order.types';

export function useOrders() {
  const [orders, setOrders] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchOrders = useCallback(async (filters?: { id?: string; status?: string }) => {
    console.log('useOrders: Fetching orders');
    setLoading(true);
    setError(null);
    
    const result = await OrderService.getOrders(filters);
    
    if (result.isSuccess) {
      setOrders(result.data);
      console.log('useOrders: Orders loaded', result.data.length);
    } else {
      setError(result.errors.join(', '));
      console.error('useOrders: Error loading orders', result.errors);
    }
    
    setLoading(false);
  }, []);

  const createOrder = useCallback(async (orderData: CreateOrderRequest) => {
    console.log('useOrders: Creating order');
    setLoading(true);
    setError(null);
    
    const result = await OrderService.createOrder(orderData);
    
    if (result.isSuccess) {
      console.log('useOrders: Order created successfully');
      await fetchOrders();
      return { success: true, message: result.data.message };
    } else {
      setError(result.errors.join(', '));
      console.error('useOrders: Error creating order', result.errors);
      return { success: false, message: result.errors.join(', ') };
    }
  }, [fetchOrders]);

  const approveOrder = useCallback(async (orderId: string) => {
    console.log('useOrders: Approving order', orderId);
    setLoading(true);
    setError(null);
    
    const result = await OrderService.approveOrder(orderId);
    
    if (result.isSuccess) {
      console.log('useOrders: Order approved successfully');
      await fetchOrders();
      return { success: true, message: result.data.message };
    } else {
      setError(result.errors.join(', '));
      console.error('useOrders: Error approving order', result.errors);
      return { success: false, message: result.errors.join(', ') };
    }
  }, [fetchOrders]);

  const deleteOrder = useCallback(async (orderId: string) => {
    console.log('useOrders: Deleting order', orderId);
    setLoading(true);
    setError(null);
    
    const result = await OrderService.deleteOrder(orderId);
    
    if (result.isSuccess) {
      console.log('useOrders: Order deleted successfully');
      await fetchOrders();
      return { success: true, message: result.data.message };
    } else {
      setError(result.errors.join(', '));
      console.error('useOrders: Error deleting order', result.errors);
      return { success: false, message: result.errors.join(', ') };
    }
  }, [fetchOrders]);

  useEffect(() => {
    fetchOrders();
  }, [fetchOrders]);

  return {
    orders,
    loading,
    error,
    fetchOrders,
    createOrder,
    approveOrder,
    deleteOrder,
  };
}

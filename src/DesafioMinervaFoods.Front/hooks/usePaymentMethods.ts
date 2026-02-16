
// Custom Hook for Payment Methods
import { useState, useEffect } from 'react';
import { OrderService } from '@/services/OrderService';
import { PaymentMethodsResponse } from '@/types/order.types';

export function usePaymentMethods() {
  const [paymentMethods, setPaymentMethods] = useState<PaymentMethodsResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchPaymentMethods = async () => {
      console.log('usePaymentMethods: Fetching payment methods');
      setLoading(true);
      setError(null);
      
      const result = await OrderService.getPaymentCondition();
      
      if (result.isSuccess) {
        
        setPaymentMethods(result.data);
        console.log('usePaymentMethods: Payment methods loaded', result.data);
      } else {
        setError(result.errors.join(', '));
        console.error('usePaymentMethods: Error loading payment methods', result.errors);
      }
      
      setLoading(false);
    };

    fetchPaymentMethods();
  }, []);

  return { paymentMethods, loading, error };
}

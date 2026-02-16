
// Custom Hook for Payment Methods
import { useState, useEffect } from 'react';
import { OrderService } from '@/services/OrderService';
import { CustomerResponse } from '@/types/order.types';

export function useCustomers() {
  const [customers, setCustomers] = useState<CustomerResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCustomers = async () => {
      console.log('useCustomers: Fetching payment methods');
      setLoading(true);
      setError(null);
      
      const result = await OrderService.getCustomers();
      
      if (result.isSuccess) {
        setCustomers(result.data);
        console.log('useCustomers: Customers loaded', result.data);
      } else {
        setError(result.errors.join(', '));
        console.error('useCustomers: Error loading customers', result.errors);
      }
      
      setLoading(false);
    };

    fetchCustomers();
  }, []);

  return { customers, loading, error };
}

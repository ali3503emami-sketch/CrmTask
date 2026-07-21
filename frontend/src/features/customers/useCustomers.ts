import { useQuery } from '@tanstack/react-query'
import { customersApi } from './customersApi'

export function useCustomers() {
  return useQuery({
    queryKey: ['customers'],
    queryFn: customersApi.getAll,
  })
}

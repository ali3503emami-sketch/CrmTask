import { useQuery } from '@tanstack/react-query'
import { contractsApi } from './contractsApi'

export function useContracts(customerId: string, enabled: boolean) {
  return useQuery({
    queryKey: ['contracts', customerId],
    queryFn: () => contractsApi.getByCustomer(customerId),
    enabled,
  })
}

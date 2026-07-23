import { useQuery } from '@tanstack/react-query'
import { contractsApi } from './contractsApi'

export function useAllContracts() {
  return useQuery({
    queryKey: ['contracts', 'all'],
    queryFn: contractsApi.getAll,
  })
}

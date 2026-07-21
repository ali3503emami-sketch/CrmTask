import { useMutation, useQueryClient } from '@tanstack/react-query'
import { contractsApi } from './contractsApi'
import type { CreateContractInput } from './types'

export function useCreateContract(customerId: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: CreateContractInput) => contractsApi.create(customerId, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contracts', customerId] })
    },
  })
}

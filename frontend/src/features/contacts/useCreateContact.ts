import { useMutation, useQueryClient } from '@tanstack/react-query'
import { contactsApi } from './contactsApi'
import type { CreateContactInput } from './types'

export function useCreateContact(customerId: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: CreateContactInput) => contactsApi.create(customerId, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contacts', customerId] })
    },
  })
}

import { useQuery } from '@tanstack/react-query'
import { contactsApi } from './contactsApi'

export function useContacts(customerId: string, enabled: boolean) {
  return useQuery({
    queryKey: ['contacts', customerId],
    queryFn: () => contactsApi.getByCustomer(customerId),
    enabled,
  })
}

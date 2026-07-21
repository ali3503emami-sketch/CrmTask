import { httpClient } from '../../shared/api/httpClient'
import type { Contact, CreateContactInput } from './types'

export const contactsApi = {
  getByCustomer: (customerId: string) => httpClient.get<Contact[]>(`/api/customers/${customerId}/contacts`),
  create: (customerId: string, input: CreateContactInput) =>
    httpClient.post<Contact>(`/api/customers/${customerId}/contacts`, input),
}

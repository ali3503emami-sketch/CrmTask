import { httpClient } from '../../shared/api/httpClient'
import type { Customer, CreateCustomerInput, UpdateCustomerInput } from './types'

export const customersApi = {
  getAll: () => httpClient.get<Customer[]>('/api/customers'),
  create: (input: CreateCustomerInput) => httpClient.post<Customer>('/api/customers', input),
  update: (id: string, input: UpdateCustomerInput) => httpClient.put<Customer>(`/api/customers/${id}`, input),
}

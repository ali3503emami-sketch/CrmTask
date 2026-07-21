import { httpClient } from '../../shared/api/httpClient'
import type { CreateCustomerInput, Customer } from './types'

export const customersApi = {
  getAll: () => httpClient.get<Customer[]>('/api/customers'),
  create: (input: CreateCustomerInput) => httpClient.post<Customer>('/api/customers', input),
}

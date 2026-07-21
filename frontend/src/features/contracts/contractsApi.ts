import { httpClient } from '../../shared/api/httpClient'
import type { Contract, CreateContractInput } from './types'

export const contractsApi = {
  getByCustomer: (customerId: string) => httpClient.get<Contract[]>(`/api/customers/${customerId}/contracts`),
  create: (customerId: string, input: CreateContractInput) =>
    httpClient.post<Contract>(`/api/customers/${customerId}/contracts`, input),
}

export type ContractStatus = 'Active' | 'ExpiringSoon' | 'Ended'

export interface Contract {
  id: string
  customerId: string
  title: string
  amount: number
  startDate: string
  startDateShamsi: string
  endDate: string
  endDateShamsi: string
  status: ContractStatus
}

export interface CreateContractInput {
  title: string
  amount: number
  startDate: string
  endDate: string
}

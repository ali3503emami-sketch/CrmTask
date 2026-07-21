export type CustomerCategory = 'Individual' | 'Legal'

export interface Customer {
  id: string
  name: string
  category: CustomerCategory
  phone: string
  createdAt: string
}

export interface CreateCustomerInput {
  name: string
  category: CustomerCategory
  phone: string
}

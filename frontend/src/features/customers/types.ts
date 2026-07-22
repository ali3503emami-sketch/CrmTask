export type CustomerCategory = 'Individual' | 'Legal'

export interface CustomerPersonnel {
  id: string
  fullName: string
  position: string | null
  phone: string | null
  mobile: string | null
  email: string | null
}

export interface CustomerPersonnelInput {
  fullName: string
  position: string | null
  phone: string | null
  mobile: string | null
  email: string | null
}

export interface Customer {
  id: string
  name: string
  category: CustomerCategory
  phone: string
  createdAt: string
  createdAtShamsi: string
  managerName: string | null
  managerBirthDate: string | null
  managerBirthDateShamsi: string | null
  address: string | null
  fax: string | null
  notes: string | null
  nationalId: string | null
  personnel: CustomerPersonnel[]
}

export interface CreateCustomerInput {
  name: string
  category: CustomerCategory
  phone: string
}

export interface UpdateCustomerInput {
  name: string
  category: CustomerCategory
  phone: string
  managerName: string | null
  managerBirthDate: string | null
  address: string | null
  fax: string | null
  notes: string | null
  nationalId: string | null
  personnel: CustomerPersonnelInput[]
}

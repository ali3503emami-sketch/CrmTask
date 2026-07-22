export type ContactDirection = 'Inbound' | 'Outbound'

export interface Contact {
  id: string
  customerId: string
  direction: ContactDirection
  summary: string
  contactedAt: string
  contactedAtShamsi: string
  nextFollowUpAt: string | null
  nextFollowUpAtShamsi: string | null
}

export interface CreateContactInput {
  direction: ContactDirection
  summary: string
  contactedAt: string
  nextFollowUpAt: string | null
}

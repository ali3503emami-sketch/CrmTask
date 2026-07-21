export type ContactDirection = 'Inbound' | 'Outbound'

export interface Contact {
  id: string
  customerId: string
  direction: ContactDirection
  summary: string
  contactedAt: string
  nextFollowUpAt: string | null
}

export interface CreateContactInput {
  direction: ContactDirection
  summary: string
  contactedAt: string
  nextFollowUpAt: string | null
}

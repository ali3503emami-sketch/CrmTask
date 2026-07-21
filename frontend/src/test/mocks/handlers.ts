import { http, HttpResponse } from 'msw'
import type { Customer } from '../../features/customers/types'
import type { Contact } from '../../features/contacts/types'
import type { Contract, ContractStatus } from '../../features/contracts/types'

// The "happy path" most tests can rely on without extra setup — see
// docs/testing-strategy.md for the convention (per-test server.use() overrides
// only for error/edge cases).
const initialCustomers: Customer[] = [
  {
    id: '11111111-1111-1111-1111-111111111111',
    name: 'شرکت فناوران البرز',
    category: 'Legal',
    phone: '02112345678',
    createdAt: '2026-07-01T00:00:00Z',
  },
]

const initialContacts: Contact[] = []
const initialContracts: Contract[] = []

// Mutable copies so a POST followed by a GET behaves like a real backend within
// a test; resetMockData() (called in test/setup.ts's afterEach) undoes any
// mutation so tests don't leak state into one another.
export let sampleCustomers: Customer[] = [...initialCustomers]
export let sampleContacts: Contact[] = [...initialContacts]
export let sampleContracts: Contract[] = [...initialContracts]

export function resetMockCustomers() {
  sampleCustomers = [...initialCustomers]
  sampleContacts = [...initialContacts]
  sampleContracts = [...initialContracts]
}

function computeContractStatus(endDate: string): ContractStatus {
  const today = new Date()
  const end = new Date(endDate)
  const daysUntilEnd = (end.getTime() - today.getTime()) / (1000 * 60 * 60 * 24)
  if (daysUntilEnd < 0) return 'Ended'
  if (daysUntilEnd <= 30) return 'ExpiringSoon'
  return 'Active'
}

export const handlers = [
  http.get('*/api/customers', () => HttpResponse.json(sampleCustomers)),
  http.post('*/api/customers', async ({ request }) => {
    const body = (await request.json()) as Omit<Customer, 'id' | 'createdAt'>
    const created: Customer = {
      id: crypto.randomUUID(),
      createdAt: new Date().toISOString(),
      ...body,
    }
    sampleCustomers.push(created)
    return HttpResponse.json(created, { status: 201 })
  }),
  http.get('*/api/customers/:customerId/contacts', ({ params }) =>
    HttpResponse.json(sampleContacts.filter((c) => c.customerId === params.customerId)),
  ),
  http.post('*/api/customers/:customerId/contacts', async ({ request, params }) => {
    const body = (await request.json()) as Omit<Contact, 'id' | 'customerId'>
    const created: Contact = {
      id: crypto.randomUUID(),
      customerId: params.customerId as string,
      ...body,
    }
    sampleContacts.push(created)
    return HttpResponse.json(created, { status: 201 })
  }),
  http.get('*/api/customers/:customerId/contracts', ({ params }) =>
    HttpResponse.json(sampleContracts.filter((c) => c.customerId === params.customerId)),
  ),
  http.post('*/api/customers/:customerId/contracts', async ({ request, params }) => {
    const body = (await request.json()) as Omit<Contract, 'id' | 'customerId' | 'status'>
    const created: Contract = {
      id: crypto.randomUUID(),
      customerId: params.customerId as string,
      status: computeContractStatus(body.endDate),
      ...body,
    }
    sampleContracts.push(created)
    return HttpResponse.json(created, { status: 201 })
  }),
]

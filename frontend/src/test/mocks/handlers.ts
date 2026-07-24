import { http, HttpResponse } from 'msw'
import type { Customer, CustomerPersonnelInput, UpdateCustomerInput } from '../../features/customers/types'
import type { Contact } from '../../features/contacts/types'
import type { Contract, ContractStatus } from '../../features/contracts/types'
import type { StaffMember } from '../../features/staff/types'
import type { TaskItem } from '../../features/tasks/types'
import type { ReferenceListItem } from '../../shared/referenceData/types'
import { toShamsi } from './toShamsi'

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
    createdAtShamsi: toShamsi('2026-07-01T00:00:00Z'),
    managerName: null,
    managerBirthDate: null,
    managerBirthDateShamsi: null,
    address: null,
    fax: null,
    notes: null,
    nationalId: null,
    categoryTitle: null,
    activityField: null,
    personnel: [],
  },
]

function toPersonnel(input: CustomerPersonnelInput[]): Customer['personnel'] {
  return input.map((p) => ({ id: crypto.randomUUID(), ...p }))
}

const initialContacts: Contact[] = []
const initialContracts: Contract[] = []
const initialStaff: StaffMember[] = [
  {
    id: '22222222-2222-2222-2222-222222222222',
    fullName: 'سارا محمدی',
    phoneNumber: '09121112233',
    position: null,
    isActive: true,
  },
]
const initialTasks: TaskItem[] = []
const initialPositions: ReferenceListItem[] = []
const initialCustomerCategories: ReferenceListItem[] = []
const initialActivityFields: ReferenceListItem[] = []

// Mutable copies so a POST followed by a GET behaves like a real backend within
// a test; resetMockData() (called in test/setup.ts's afterEach) undoes any
// mutation so tests don't leak state into one another.
export let sampleCustomers: Customer[] = [...initialCustomers]
export let sampleContacts: Contact[] = [...initialContacts]
export let sampleContracts: Contract[] = [...initialContracts]
export let sampleStaff: StaffMember[] = [...initialStaff]
export let sampleTasks: TaskItem[] = [...initialTasks]
export let samplePositions: ReferenceListItem[] = [...initialPositions]
export let sampleCustomerCategories: ReferenceListItem[] = [...initialCustomerCategories]
export let sampleActivityFields: ReferenceListItem[] = [...initialActivityFields]

export function resetMockCustomers() {
  sampleCustomers = [...initialCustomers]
  sampleContacts = [...initialContacts]
  sampleContracts = [...initialContracts]
  sampleStaff = [...initialStaff]
  sampleTasks = [...initialTasks]
  samplePositions = [...initialPositions]
  sampleCustomerCategories = [...initialCustomerCategories]
  sampleActivityFields = [...initialActivityFields]
}

function referenceListHandlers(route: string, items: () => ReferenceListItem[], setItems: (items: ReferenceListItem[]) => void) {
  return [
    http.get(`*${route}`, () => HttpResponse.json(items())),
    http.post(`*${route}`, async ({ request }) => {
      const body = (await request.json()) as { title: string }
      const created: ReferenceListItem = { id: crypto.randomUUID(), title: body.title }
      setItems([...items(), created])
      return HttpResponse.json(created, { status: 201 })
    }),
    http.put(`*${route}/:itemId`, async ({ request, params }) => {
      const item = items().find((i) => i.id === params.itemId)
      if (!item) return new HttpResponse(null, { status: 404 })
      const body = (await request.json()) as { title: string }
      item.title = body.title
      return HttpResponse.json(item)
    }),
  ]
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
    const body = (await request.json()) as { name: string; category: Customer['category']; phone: string }
    const createdAt = new Date().toISOString()
    const created: Customer = {
      id: crypto.randomUUID(),
      createdAt,
      createdAtShamsi: toShamsi(createdAt),
      managerName: null,
      managerBirthDate: null,
      managerBirthDateShamsi: null,
      address: null,
      fax: null,
      notes: null,
      nationalId: null,
      categoryTitle: null,
      activityField: null,
      personnel: [],
      ...body,
    }
    sampleCustomers.push(created)
    return HttpResponse.json(created, { status: 201 })
  }),
  http.put('*/api/customers/:customerId', async ({ request, params }) => {
    const customer = sampleCustomers.find((c) => c.id === params.customerId)
    if (!customer) return new HttpResponse(null, { status: 404 })
    const body = (await request.json()) as UpdateCustomerInput
    customer.name = body.name
    customer.category = body.category
    customer.phone = body.phone
    customer.managerName = body.managerName
    customer.managerBirthDate = body.managerBirthDate
    customer.managerBirthDateShamsi = body.managerBirthDate ? toShamsi(body.managerBirthDate) : null
    customer.address = body.address
    customer.fax = body.fax
    customer.notes = body.notes
    customer.nationalId = body.nationalId
    customer.categoryTitle = body.categoryTitle
    customer.activityField = body.activityField
    customer.personnel = toPersonnel(body.personnel)
    return HttpResponse.json(customer)
  }),
  http.get('*/api/customers/:customerId/contacts', ({ params }) =>
    HttpResponse.json(sampleContacts.filter((c) => c.customerId === params.customerId)),
  ),
  http.post('*/api/customers/:customerId/contacts', async ({ request, params }) => {
    const body = (await request.json()) as Omit<Contact, 'id' | 'customerId' | 'contactedAtShamsi' | 'nextFollowUpAtShamsi'>
    const created: Contact = {
      id: crypto.randomUUID(),
      customerId: params.customerId as string,
      contactedAtShamsi: toShamsi(body.contactedAt),
      nextFollowUpAtShamsi: body.nextFollowUpAt ? toShamsi(body.nextFollowUpAt) : null,
      ...body,
    }
    sampleContacts.push(created)
    return HttpResponse.json(created, { status: 201 })
  }),
  http.get('*/api/customers/:customerId/contracts', ({ params }) =>
    HttpResponse.json(sampleContracts.filter((c) => c.customerId === params.customerId)),
  ),
  http.post('*/api/customers/:customerId/contracts', async ({ request, params }) => {
    const body = (await request.json()) as Omit<Contract, 'id' | 'customerId' | 'status' | 'startDateShamsi' | 'endDateShamsi'>
    const created: Contract = {
      id: crypto.randomUUID(),
      customerId: params.customerId as string,
      status: computeContractStatus(body.endDate),
      startDateShamsi: toShamsi(body.startDate),
      endDateShamsi: toShamsi(body.endDate),
      ...body,
    }
    sampleContracts.push(created)
    return HttpResponse.json(created, { status: 201 })
  }),
  http.get('*/api/contracts', () => HttpResponse.json(sampleContracts)),
  http.get('*/api/staff', () => HttpResponse.json(sampleStaff)),
  http.post('*/api/staff', async ({ request }) => {
    const body = (await request.json()) as Omit<StaffMember, 'id' | 'isActive'>
    const created: StaffMember = { id: crypto.randomUUID(), isActive: true, ...body }
    sampleStaff.push(created)
    return HttpResponse.json(created, { status: 201 })
  }),
  http.put('*/api/staff/:staffId', async ({ request, params }) => {
    const staffMember = sampleStaff.find((s) => s.id === params.staffId)
    if (!staffMember) return new HttpResponse(null, { status: 404 })
    const body = (await request.json()) as Omit<StaffMember, 'id' | 'isActive'>
    staffMember.fullName = body.fullName
    staffMember.phoneNumber = body.phoneNumber
    staffMember.position = body.position
    return HttpResponse.json(staffMember)
  }),
  http.get('*/api/tasks', () => HttpResponse.json(sampleTasks)),
  http.post('*/api/tasks', async ({ request }) => {
    const body = (await request.json()) as {
      title: string
      description: string
      dueAt: string
      customerId: string | null
      assignedToStaffId: string
      createdByStaffId: string
      checklistFields: { label: string; fieldType: TaskItem['checklistItems'][number]['fieldType']; options: string[] | null }[]
    }
    const created: TaskItem = {
      id: crypto.randomUUID(),
      title: body.title,
      description: body.description,
      dueAt: body.dueAt,
      dueAtShamsi: toShamsi(body.dueAt),
      customerId: body.customerId,
      assignedToStaffId: body.assignedToStaffId,
      createdByStaffId: body.createdByStaffId,
      status: 'Open',
      checklistItems: body.checklistFields.map((f) => ({
        id: crypto.randomUUID(),
        label: f.label,
        fieldType: f.fieldType,
        options: f.options ?? [],
        value: null,
      })),
    }
    sampleTasks.push(created)
    return HttpResponse.json(created, { status: 201 })
  }),
  http.put('*/api/tasks/:taskId', async ({ request, params }) => {
    const task = sampleTasks.find((t) => t.id === params.taskId)
    if (!task) return new HttpResponse(null, { status: 404 })
    if (task.status === 'Done') return new HttpResponse('Task is already done and cannot be edited.', { status: 409 })
    const body = (await request.json()) as { title: string; description: string; dueAt: string; customerId: string | null }
    task.title = body.title
    task.description = body.description
    task.dueAt = body.dueAt
    task.dueAtShamsi = toShamsi(body.dueAt)
    task.customerId = body.customerId
    return HttpResponse.json(task)
  }),
  http.post('*/api/tasks/:taskId/mark-done', ({ params }) => {
    const task = sampleTasks.find((t) => t.id === params.taskId)
    if (!task) return new HttpResponse(null, { status: 404 })
    task.status = 'Done'
    return HttpResponse.json(task)
  }),
  http.put('*/api/tasks/:taskId/checklist-items/:checklistItemId', async ({ params, request }) => {
    const task = sampleTasks.find((t) => t.id === params.taskId)
    if (!task) return new HttpResponse(null, { status: 404 })
    const body = (await request.json()) as { value: string | null }
    const item = task.checklistItems.find((i) => i.id === params.checklistItemId)
    if (item) item.value = body.value
    return HttpResponse.json(task)
  }),
  ...referenceListHandlers(
    '/api/positions',
    () => samplePositions,
    (items) => (samplePositions = items),
  ),
  ...referenceListHandlers(
    '/api/customer-categories',
    () => sampleCustomerCategories,
    (items) => (sampleCustomerCategories = items),
  ),
  ...referenceListHandlers(
    '/api/activity-fields',
    () => sampleActivityFields,
    (items) => (sampleActivityFields = items),
  ),
]
